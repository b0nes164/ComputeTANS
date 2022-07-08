using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DispatchTester : MonoBehaviour
{
    private float time;
    private int doper;
    private bool log;

    private int testSize;
    private int dispatchComparisons;
    private int[] dispatchInfo;
    private ComputeShader compute;

    private float[] timeTotals;
    private int[] kernels;
    private int[] kernelsTwo;
    private int[] kernelsThree;
    private int[] dispatchArgs;
    private ComputeBuffer[] argBuffers;
    private ComputeBuffer[] outBuffers;

    private bool dispatchError = true;
    private bool done;
    public void Initialize(int _testSize, int _doper, int _dispatchComparisons, ComputeShader _compute, bool _log)
    {
        testSize = _testSize;
        doper = _doper;
        dispatchComparisons = _dispatchComparisons;
        compute = _compute;
        log = _log;

        dispatchInfo = new int[_dispatchComparisons];
        timeTotals = new float[_dispatchComparisons];
        kernels = new int[_dispatchComparisons];
        kernelsTwo = new int[_dispatchComparisons];
        dispatchArgs = new int[_dispatchComparisons];
        argBuffers = new ComputeBuffer[_dispatchComparisons];
        outBuffers = new ComputeBuffer[_dispatchComparisons];

    }

    #region
    public void InitDispatch(int offset, int kernelOne, ComputeBuffer _args, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0 || dispatchInfo[offset] != 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 0;
        kernels[offset] = kernelOne;
        argBuffers[offset] = _args;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int kernelTwo, ComputeBuffer _args, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 1;
        kernels[offset] = kernelOne;
        kernelsTwo[offset] = kernelTwo;
        argBuffers[offset] = _args;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int _args, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 2;
        kernels[offset] = kernelOne;
        dispatchArgs[offset] = _args;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int kernelTwo, int _args, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 3;
        kernels[offset] = kernelOne;
        kernelsTwo[offset] = kernelTwo;
        dispatchArgs[offset] = _args;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int kernelTwo, int _args, ComputeBuffer _argsTwo, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 4;
        kernels[offset] = kernelOne;
        kernelsTwo[offset] = kernelTwo;
        dispatchArgs[offset] = _args;
        argBuffers[offset] = _argsTwo;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int kernelTwo, ComputeBuffer _args, int _argsTwo,  ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 5;
        kernels[offset] = kernelOne;
        kernelsTwo[offset] = kernelTwo;
        argBuffers[offset] = _args;
        dispatchArgs[offset] = _argsTwo;
        outBuffers[offset] = _out;
    }

    public void InitDispatch(int offset, int kernelOne, int kernelTwo, int kernelThree, int _args, ComputeBuffer _argsTwo, ComputeBuffer _out)
    {
        dispatchError = (offset > dispatchComparisons - 1 || offset < 0);
        if (dispatchError)
        {
            Debug.LogError("Invalid dispatch information");
            return;
        }

        dispatchInfo[offset] = 6;
        kernels[offset] = kernelOne;
        kernelsTwo[offset] = kernelTwo;
        kernelsThree[offset] = kernelThree;
        dispatchArgs[offset] = _args;
        argBuffers[offset] = _argsTwo;
        outBuffers[offset] = _out;
    }
    #endregion

    private IEnumerator TimingTest(int _offset)
    {
        for (int i = 0; i < testSize; i++)
        {
            switch (dispatchInfo[_offset])
            {
                case 0:
                    time = Time.realtimeSinceStartup;
                    compute.DispatchIndirect(kernels[_offset], argBuffers[_offset]);
                    break;
                case 1:
                    compute.DispatchIndirect(kernels[_offset], argBuffers[_offset]);
                    time = Time.realtimeSinceStartup;
                    compute.DispatchIndirect(kernelsTwo[_offset], argBuffers[_offset]);
                    break;
                case 2:
                    time = Time.realtimeSinceStartup;
                    compute.Dispatch(kernels[_offset], dispatchArgs[_offset], 1, 1);
                    break;
                case 3:
                    time = Time.realtimeSinceStartup;
                    compute.Dispatch(kernels[_offset], dispatchArgs[_offset], 1, 1);
                    compute.Dispatch(kernelsTwo[_offset], dispatchArgs[_offset], 1, 1);
                    break;
                case 4:
                    time = Time.realtimeSinceStartup;
                    compute.Dispatch(kernels[_offset], dispatchArgs[_offset], 1, 1);
                    compute.DispatchIndirect(kernelsTwo[_offset], argBuffers[_offset]);
                    break;
                case 5:
                    time = Time.realtimeSinceStartup;
                    compute.DispatchIndirect(kernels[_offset], argBuffers[_offset]);
                    compute.Dispatch(kernelsTwo[_offset], dispatchArgs[_offset], 1, 1);
                    break;
                case 6:
                    compute.DispatchIndirect(kernels[_offset], argBuffers[_offset]);
                    time = Time.realtimeSinceStartup;
                    compute.DispatchIndirect(kernelsTwo[_offset], argBuffers[_offset]);
                    compute.Dispatch(kernelsThree[_offset], dispatchArgs[_offset], 1, 1);
                    break;
                default:
                    break;
            }
            
            var request = AsyncGPUReadback.Request(outBuffers[_offset]);
            yield return new WaitUntil(() => request.done);
            float runTime = Time.realtimeSinceStartup - time;
            if (log)
            {
                Debug.Log("Time for    " + outBuffers[_offset].count + "   elements:    " + runTime + ".    " + (1.0f / runTime * outBuffers[_offset].count) + " elements/sec");
            }
            if (_offset == 0)
            {
                if (i >= doper)
                {
                    timeTotals[_offset] += runTime;
                }
            }
            else
            {
                timeTotals[_offset] += runTime;
            }
        }

        _offset++;
        if (_offset < dispatchComparisons)
        {
            StartCoroutine(TimingTest(_offset));
        }
        else
        {
            done = true;
        }
    }

    public IEnumerator TestMaster()
    {
        if (dispatchError)
        {
            Debug.LogError("Invalid testing setup, aborting test");
            yield break;
        }
        done = false;
        StartCoroutine(TimingTest(0));
        yield return new WaitUntil(() => done);
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        for (int i = 0; i < dispatchComparisons; i++)
        {
            if (i == 0)
            {
                Debug.Log("Average time for method " + i + "     " + (timeTotals[i] / (testSize - doper)) + "   seconds. Average speed is:   " + (testSize - doper) * 1.0f / timeTotals[i] * outBuffers[i].count);
            }
            else
            {
                Debug.Log("Average merge time method " + i + "     " + (timeTotals[i] / testSize) + "   seconds. Average speed is:   " + testSize * 1.0f / timeTotals[i] * outBuffers[i].count);
            }
        }
    }
}
