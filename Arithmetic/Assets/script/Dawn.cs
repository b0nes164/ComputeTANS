using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dawn : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute;


    private ComputeBuffer b_original;
    private ComputeBuffer b_temp;
    private ComputeBuffer b_freq;
    private ComputeBuffer b_symbolCount;

    private ComputeBuffer b_lNodes;
    private ComputeBuffer b_iNodes;
    private ComputeBuffer b_codeLengths;
    private ComputeBuffer b_flags;
    private ComputeBuffer b_copy;


    private int arrSize = 65536;
    //private int dictSize = 256;
    private int dictSize = 11;
    private int wiggleData = 3;

    private uint[] test;
    private uint[] test2;

    private uint[] clTest;
    private uint[] clTest2;
    private uint[] clTest3;
    private uint[] clTest4;
    private uint[] clTest5;

    void Start()
    {
        b_original = new ComputeBuffer(arrSize, sizeof(uint));
        b_temp = new ComputeBuffer(dictSize, sizeof(uint));
        //b_freq = new ComputeBuffer(arrSize, sizeof(uint));
        b_freq = new ComputeBuffer(dictSize, sizeof(uint));
        b_freq.SetData(new uint[11] { 9, 9, 11, 11, 13, 13, 13, 16, 18, 20, 38 });
        b_symbolCount = new ComputeBuffer(wiggleData, sizeof(uint));

        b_lNodes = new ComputeBuffer(dictSize, sizeof(uint) * 2);
        b_iNodes = new ComputeBuffer(dictSize, sizeof(uint) * 2);
        b_codeLengths = new ComputeBuffer(dictSize, sizeof(uint));
        b_copy = new ComputeBuffer(dictSize, sizeof(uint) * 3);
        b_flags = new ComputeBuffer(10, sizeof(uint));

        System.Random rand = new System.Random();
        compute.SetFloat("e_seed", (float)rand.NextDouble());
        compute.SetInt("e_arraySize", arrSize);
        compute.SetInt("e_dictSize", dictSize);

        int k_init = compute.FindKernel("Init");
        int k_cwl = compute.FindKernel("CodeWordLengths");

        compute.SetBuffer(k_init, "TempBuffer", b_temp);
        compute.SetBuffer(k_init, "OriginalBuffer", b_original);
        compute.SetBuffer(k_init, "FreqBuffer", b_freq);
        compute.SetBuffer(k_init, "SymbolCount", b_symbolCount);
        //compute.Dispatch(k_init, 1, 1, 1);

        compute.SetBuffer(k_cwl, "iNodesBuffer", b_iNodes);
        compute.SetBuffer(k_cwl, "lNodesBuffer", b_lNodes);
        compute.SetBuffer(k_cwl, "CLBuffer", b_codeLengths);
        compute.SetBuffer(k_cwl, "FreqBuffer", b_freq);
        compute.SetBuffer(k_cwl, "copyBuffer", b_copy);
        compute.SetBuffer(k_cwl, "flagBuffer", b_flags);
        compute.Dispatch(k_cwl, 1, 1, 1);

        clTest = new uint[b_freq.count];
        b_freq.GetData(clTest);
        clTest2 = new uint[b_lNodes.count * 2];
        b_lNodes.GetData(clTest2);
        clTest3 = new uint[b_iNodes.count * 2];
        b_iNodes.GetData(clTest3);
        clTest4 = new uint[b_copy.count * 3];
        b_copy.GetData(clTest4);
        clTest5 = new uint[b_flags.count];
        b_flags.GetData(clTest5);

        for (int i = 0; i < b_iNodes.count * 2; i += 2)
        {
            Debug.Log("Node: " + clTest3[i] + " Leader: " + clTest3[i + 1]);
        }

        Debug.Log("BREAK~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        for (int i = 0; i < b_lNodes.count * 2; i += 2)
        {
            Debug.Log("Node: " + clTest2[i] + " Leader: " + clTest2[i + 1]);
        }

        Debug.Log("Break~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        for (int i = 0; i < b_copy.count * 3; i += 3)
        {
            Debug.Log("Copy Node: " + clTest4[i] + " Leaf? : " + clTest4[i + 1] + " Index: " + clTest4[i + 2]);
        }

        Debug.Log("LCur: " + clTest5[0]);
        Debug.Log("MinFreq " + clTest5[3]);
        Debug.Log("Cur Leaves Number: " + clTest5[6]);
        /*
        test = new uint[b_freq.count];
        b_freq.GetData(test);

        test2 = new uint[b_symbolCount.count];
        b_symbolCount.GetData(test2);

        Debug.Log(test2[0]);

        uint total = 0;

        foreach (uint g in test)
        {
            if (g != 0)
            {
                Debug.Log(g);
                total += g;
            }
        }

        Debug.Log(total);
        */

    }

    void OnDisable()
    {
        b_original.Release();
        b_temp.Release();
        b_freq.Release();
        b_symbolCount.Release();

        b_iNodes.Release();
        b_lNodes.Release();
        b_codeLengths.Release();
        b_copy.Release();
        b_codeLengths.Release();
        b_flags.Release();
    }
}
