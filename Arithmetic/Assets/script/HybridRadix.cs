using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class HybridRadix : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute;

    [SerializeField]
    private DispatchTester tester;

    private static int threadBlocks = 64;
    private static int groupSize = 32;
    private static int size = 2048;
    private static int max = 65536;
    private static int[] arr = new int[size];

    private static int k_init = 0;
    private static int k_dHist = 1;
    private static int k_gHist = 2;
    private static int k_wavePrefix = 3;
    private static int k_conPrefix = 4;

    private ComputeBuffer b_args;
    private ComputeBuffer b_input;
    private ComputeBuffer b_output;
    private ComputeBuffer b_globalHistogram;
    private ComputeBuffer b_tempHist;

    private ComputeBuffer b_prefixes;
    private ComputeBuffer b_check;

    void Start()
    {
        System.Random rand = new System.Random();

        b_args = new ComputeBuffer(1, sizeof(uint) * 4, ComputeBufferType.IndirectArguments);
        b_input = new ComputeBuffer(size, sizeof(uint));
        b_output = new ComputeBuffer(size, sizeof(uint));
        b_prefixes = new ComputeBuffer(size, sizeof(uint));
        b_globalHistogram = new ComputeBuffer(256, sizeof(uint));
        b_tempHist = new ComputeBuffer(threadBlocks * groupSize, sizeof(uint));
        b_check = new ComputeBuffer(size, sizeof(uint));

        b_args.SetData(new uint[] { (uint)threadBlocks, 1, 1, 0 });
        compute.SetFloat("e_seed", (float)rand.NextDouble());
        compute.SetInt("e_sortLength", size);

        compute.SetBuffer(k_init,"b_input", b_input);
        //compute.DispatchIndirect(k_init, b_args);

        compute.SetBuffer(k_dHist, "b_globalHistogram", b_globalHistogram);
        compute.SetBuffer(k_dHist, "b_input", b_input);
        compute.SetBuffer(k_dHist, "b_prefixes", b_prefixes);
        //compute.DispatchIndirect(k_dHist, b_args);

        compute.SetBuffer(k_gHist, "b_globalHistogram", b_globalHistogram);
        compute.SetBuffer(k_gHist, "b_input", b_input);
        compute.SetBuffer(k_gHist, "b_prefixes", b_prefixes);
        //compute.DispatchIndirect(k_gHist, b_args);

        compute.SetBuffer(k_wavePrefix, "b_globalHistogram", b_globalHistogram);
        //compute.Dispatch(k_wavePrefix, 1, 1, 1);

        compute.SetBuffer(k_conPrefix, "b_globalHistogram", b_globalHistogram);
        //compute.Dispatch(k_conPrefix, 1, 1, 1);

        uint[] test = new uint[b_globalHistogram.count];
        b_globalHistogram.GetData(test);

        foreach (uint g in test)
        {
            //Debug.Log(g);
        }

        Test();
    }

    private void Test()
    {
        tester.Initialize(500, 100, 2, compute, false);
        tester.InitDispatch(1, k_init, k_dHist, k_conPrefix, b_args, b_globalHistogram);
        tester.InitDispatch(0, k_init, k_gHist, b_args, b_globalHistogram);
        StartCoroutine(tester.TestMaster());
    }

    private void CountDriver()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, max);
        }

        CountSort(arr, max);
    }

    private void CountSort(int[] _arr, int _max)
    {
        int[] count = new int[_max + 1];
        int[] output = new int[_arr.Length];

        for (int i = 0; i < count.Length; i++)
        {
            count[i] = 0;
        }

        for (int i = 0; i < _arr.Length; i++)
        {
            count[_arr[i]]++;
        }

        for (int i = 1; i <= _max; i++)
        {
            count[i] += count[i - 1];
            Debug.Log("p: " + count[i]);
        }

        for (int i = _arr.Length - 1; i >= 0; i--)
        {
            count[_arr[i]]--;
            output[count[_arr[i]]] = _arr[i];
        }

        for (int i = 0; i < output.Length; i++)
        {
            Debug.Log(output[i]); 
        }
    }

    void OnDisable()
    {
        b_args.Release();
        b_input.Release();
        b_output.Release();
        b_globalHistogram.Release();
        b_tempHist.Release();
        b_prefixes.Release();
        b_check.Release();
    }
}
