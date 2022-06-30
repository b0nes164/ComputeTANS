using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ParMerge : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute;

    [SerializeField]
    private bool timing;

    [SerializeField]
    private bool debug;

    [SerializeField]
    private bool sort_text;

    [SerializeField]
    private bool intersection_text;

    [SerializeField]
    private bool main;

    [SerializeField]
    private bool methodB;

    [SerializeField]
    private bool timingTest;

    private static int threadBlocks = 64;
    private float time;
    private float totalA;
    private float totalB;
    private bool done;

    private static int k_parMerge = 0;
    private static int k_parB = 1;
    private static int k_mergeB = 2;

    private static int lengthA = 4194304;
    private static int lengthB = 4194304;

    private ComputeBuffer b_mergeArgs;

    private ComputeBuffer b_mergeLengths;
    private ComputeBuffer b_mergeA;
    private ComputeBuffer b_mergeB;
    private ComputeBuffer b_groupA;
    private ComputeBuffer b_groupB;
    private ComputeBuffer b_mergeOut;

    private ComputeBuffer b_t_mergeA;
    private ComputeBuffer b_t_mergeB;
    private ComputeBuffer b_t_groupA;
    private ComputeBuffer b_t_groupB;
    private ComputeBuffer b_t_mergeOut;

    void Start()
    {
        Initialize();
        if(main)DispatchMain();
        if(methodB)DispatchB();

        if (timingTest)
        {
            if (!main && !methodB)
            {
                StartCoroutine(TestMaster(100, 10));
            }
            else
            {
                Debug.Log("Please disable other dispatches to allow test to run.");
            }
        }
    }

    private void Initialize()
    {
        b_mergeArgs = new ComputeBuffer(1, sizeof(uint) * 4, ComputeBufferType.IndirectArguments);

        b_mergeLengths = new ComputeBuffer(2, sizeof(uint));
        b_mergeA = new ComputeBuffer(lengthA, sizeof(uint));
        b_mergeB = new ComputeBuffer(lengthB, sizeof(uint));
        b_groupA = new ComputeBuffer(threadBlocks * 2, sizeof(uint));
        b_groupB = new ComputeBuffer(threadBlocks * 2, sizeof(uint));
        b_mergeOut = new ComputeBuffer(lengthA + lengthB, sizeof(uint));

        b_t_mergeA = new ComputeBuffer(lengthA, sizeof(int));
        b_t_mergeB = new ComputeBuffer(lengthB, sizeof(int));
        b_t_groupA = new ComputeBuffer(threadBlocks, sizeof(int));
        b_t_groupB = new ComputeBuffer(threadBlocks, sizeof(int));
        b_t_mergeOut = new ComputeBuffer(lengthA + lengthB, sizeof(int));

        b_mergeArgs.SetData(new uint[4] { (uint)threadBlocks, 1, 1, 0});

        uint[] arrA = new uint[lengthA];
        uint[] arrB = new uint[lengthB];
        int[] t_arrA = new int[lengthA];
        int[] t_arrB = new int[lengthB];

        for (int i = 0; i < lengthB; i++)
        {
            if (i < lengthA)
            {
                arrA[i] = (uint)i;
                t_arrA[i] = i;
            }

            arrB[i] = (uint)(i + 1);
            t_arrB[i] = i + 1;
        }

        b_mergeA.SetData(arrA);
        b_mergeB.SetData(arrB);
        b_t_mergeA.SetData(t_arrA);
        b_t_mergeB.SetData(t_arrB);
        compute.SetInt("sizeA", lengthA);
        compute.SetInt("sizeB", lengthB);
        b_mergeLengths.SetData(new uint[2] { (uint)lengthA, (uint)lengthB });

        compute.SetBuffer(k_parMerge, "b_mergeLengths", b_mergeLengths);
        compute.SetBuffer(k_parMerge, "b_mergeA", b_mergeA);
        compute.SetBuffer(k_parMerge, "b_mergeB", b_mergeB);
        //compute.SetBuffer(k_parMerge, "b_groupA", b_groupA);
        //compute.SetBuffer(k_parMerge, "b_groupB", b_groupB);
        compute.SetBuffer(k_parMerge, "b_mergeOut", b_mergeOut);

        compute.SetBuffer(k_parB, "t_mergeA", b_t_mergeA);
        compute.SetBuffer(k_parB, "t_mergeB", b_t_mergeB);
        compute.SetBuffer(k_parB, "t_groupA", b_t_groupA);
        compute.SetBuffer(k_parB, "t_groupB", b_t_groupB);

        compute.SetBuffer(k_mergeB, "t_mergeA", b_t_mergeA);
        compute.SetBuffer(k_mergeB, "t_mergeB", b_t_mergeB);
        compute.SetBuffer(k_mergeB, "t_groupA", b_t_groupA);
        compute.SetBuffer(k_mergeB, "t_groupB", b_t_groupB);
        compute.SetBuffer(k_mergeB, "t_mergeOut", b_t_mergeOut);
    }

    private void DispatchMain()
    {
        time = Time.realtimeSinceStartup;
        compute.DispatchIndirect(k_parMerge, b_mergeArgs);
        if (timing) StartCoroutine(Timing(b_mergeOut));
        if (debug) Debugger(b_mergeOut, b_groupA, b_groupB);
    }

    private void DispatchB()
    {
        time = Time.realtimeSinceStartup;
        compute.DispatchIndirect(k_parB, b_mergeArgs);
        compute.DispatchIndirect(k_mergeB, b_mergeArgs);
        if (timing) StartCoroutine(Timing(b_t_mergeOut));
        if (debug) Debugger(b_t_mergeOut, b_t_groupA, b_t_groupB);
    }

    private void Debugger(ComputeBuffer _merge, ComputeBuffer _aInt, ComputeBuffer _bInt)
    {
        uint[] mergeOut = new uint[lengthA + lengthB];
        uint[] aIntersections = new uint[_aInt.count];
        uint[] bIntersections = new uint[_bInt.count];
        _merge.GetData(mergeOut);
        _aInt.GetData(aIntersections);
        _bInt.GetData(bIntersections);

        bool er = false;
        for (int i = 0; i < mergeOut.Length - 1; i++)
        {
            if (sort_text)
            {
                Debug.Log(mergeOut[i]);
            }
            if (!er && mergeOut[i] > mergeOut[i + 1])
            {
                er = true;
            }
        }

        if (sort_text)
        {
            Debug.Log(mergeOut[mergeOut.Length - 1]);
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        if (er)
        {
            Debug.LogError("Merge sorting error");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        for (int i = 0; i < aIntersections.Length; i++)
        {
            if (intersection_text) Debug.Log("Count: " + i + " A: " + aIntersections[i] + " B: " + bIntersections[i]);
        }
    }

    private IEnumerator Timing(ComputeBuffer _merge)
    {
        var request = AsyncGPUReadback.Request(_merge);
        yield return new WaitUntil(() => request.done);
        Debug.Log("Merge time for    " + _merge.count + "   elements:    " + (Time.realtimeSinceStartup - time) + ".    " + ((1.0f / (Time.realtimeSinceStartup - time)) * _merge.count) + " elements/sec");
    }

    private IEnumerator TimingTest(int _doper, int _testSize, ComputeShader _compute, int _kernel, ComputeBuffer _merge, bool totalCase)
    {
        for (int i = 0; i < _testSize; i++)
        {
            time = Time.realtimeSinceStartup;
            _compute.DispatchIndirect(_kernel, b_mergeArgs);
            var request = AsyncGPUReadback.Request(_merge);
            yield return new WaitUntil(() => request.done);
            float runTime = Time.realtimeSinceStartup - time;
            Debug.Log("Merge time for    " + _merge.count + "   elements:    " + runTime + ".    " + (1.0f / runTime * _merge.count) + " elements/sec");
            if (totalCase)
            {
                if (i >= _doper)
                {
                    totalA += runTime;
                }
            }
            else
            {
                totalB += runTime;
            }
        }

        StartCoroutine(TimingTest(_testSize, compute, k_parB, k_mergeB, b_t_mergeOut, false));
    }

    private IEnumerator TimingTest(int _testSize, ComputeShader _compute, int _kernel, int _kernelTwo, ComputeBuffer _merge, bool totalCase)
    {
        for (int i = 0; i < _testSize; i++)
        {
            time = Time.realtimeSinceStartup;
            _compute.DispatchIndirect(_kernel, b_mergeArgs);
            _compute.DispatchIndirect(_kernelTwo, b_mergeArgs);
            var request = AsyncGPUReadback.Request(_merge);
            yield return new WaitUntil(() => request.done);
            float runTime = Time.realtimeSinceStartup - time;
            Debug.Log("Merge time for    " + _merge.count + "   elements:    " + runTime + ".    " + (1.0f /  runTime * _merge.count) + " elements/sec");
            if (totalCase)
            {
                totalA += runTime;
            }
            else
            {
                totalB += runTime;
            }
        }

        done = true; 
    }

    private IEnumerator TestMaster(int _testSize, int doper)
    {
        done = false;
        StartCoroutine(TimingTest(doper, _testSize, compute, k_parMerge, b_mergeOut, true));
        yield return new WaitUntil(() => done);
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Debug.Log("Average merge time for single dispatch method    " + (totalA / (_testSize - doper)) + "   seconds. Average speed is:   " + (_testSize - doper) * 1.0f / totalA * (lengthA + lengthB));
        Debug.Log("Average merge time for multi-dispatch method    " + (totalB / (_testSize)) + "   seconds. Average speed is:   " + _testSize * 1.0f / totalB * (lengthA + lengthB));
        Debug.Log("Timing ratio multi/single:   " + (totalB * (_testSize - doper)) / (totalA * _testSize));
    }

    private void OnDisable()
    {
        b_mergeArgs.Release();

        b_mergeLengths.Release();
        b_mergeA.Release();
        b_mergeB.Release();
        b_groupA.Release();
        b_groupB.Release();
        b_mergeOut.Release();

        b_t_mergeA.Release();
        b_t_mergeB.Release();
        b_t_groupA.Release();
        b_t_groupB.Release();
        b_t_mergeOut.Release();
    }
    /*
    private void SetWaveSize()
    {
        int k_waveSize = 0;
        ComputeBuffer b_waveSize = new ComputeBuffer(1, sizeof(uint));
        compute.SetBuffer(k_waveSize, "b_waveSize", b_waveSize);
        compute.Dispatch(k_waveSize, 1, 1, 1);
        uint[] waveSize = new uint[1];
        b_waveSize.GetData(waveSize);
        b_waveSize.Release();
        switch (waveSize[0])
        {
            case 16:
                compute.EnableKeyword("WAVE_SIZE_16");
                compute.DisableKeyword("WAVE_SIZE_32");
                compute.DisableKeyword("WAVE_SIZE_64");
                break;
            case 32:
                compute.DisableKeyword("WAVE_SIZE_16");
                compute.EnableKeyword("WAVE_SIZE_32");
                compute.DisableKeyword("WAVE_SIZE_64");
                break;
            case 64:
                compute.DisableKeyword("WAVE_SIZE_16");
                compute.DisableKeyword("WAVE_SIZE_32");
                compute.EnableKeyword("WAVE_SIZE_64");
                break;
            default:
                compute.DisableKeyword("WAVE_SIZE_16");
                compute.EnableKeyword("WAVE_SIZE_32");
                compute.DisableKeyword("WAVE_SIZE_64");
                break;
        }
    }
    */
}
