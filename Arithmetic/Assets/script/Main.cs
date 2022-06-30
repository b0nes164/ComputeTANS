using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute;

    private ComputeBuffer b_original;
    private ComputeBuffer b_temp;
    private ComputeBuffer b_dict;
    private ComputeBuffer b_culled;
    private ComputeBuffer b_symbolCount;

    private ComputeBuffer b_origProb;
    private ComputeBuffer b_quantProb;
    private ComputeBuffer b_quantRound;
    private ComputeBuffer b_errorCorr;

    private ComputeBuffer b_binTree;

    private int arrSize = 65536;
    private int dictSize = 256;

    private int binSize = 2901;

    private int wiggleData = 3;

    private float [] test;
    private uint[] test2;
    private float[] test3;
    private uint[] test4;
    private uint[] test5;
    private uint[] test6;

    private uint[] test7;


    void Start()
    {
        b_original = new ComputeBuffer(arrSize, sizeof(uint));
        b_temp = new ComputeBuffer(dictSize, sizeof(uint));
        b_dict = new ComputeBuffer(arrSize, sizeof(uint));
        b_culled = new ComputeBuffer(arrSize, sizeof(uint) * 2, ComputeBufferType.Append);
        b_symbolCount = new ComputeBuffer(wiggleData, sizeof(uint));

        b_origProb = new ComputeBuffer(arrSize, sizeof(float));
        b_quantProb = new ComputeBuffer(arrSize, sizeof(float));
        b_quantRound = new ComputeBuffer(arrSize, sizeof(uint));
        b_errorCorr = new ComputeBuffer(arrSize, sizeof(float) * 2);

        b_binTree = new ComputeBuffer(binSize, sizeof(uint) * 3);

        System.Random rand = new System.Random();
        compute.SetFloat("e_seed", (float)rand.NextDouble());
        compute.SetInt("e_arraySize", arrSize);
        compute.SetInt("e_dictSize", dictSize);
        compute.SetInt("e_binarySize", binSize);

        int k_init = compute.FindKernel("Init");
        int k_quant = compute.FindKernel("Quantize");
        int k_correctSingle = compute.FindKernel("CorrectSingle");
        int k_correctMulti = compute.FindKernel("CorrectMulti");
        int k_binTree = compute.FindKernel("BinaryTree");

        b_culled.SetCounterValue(0);
        compute.SetBuffer(k_init, "TempBuffer", b_temp);
        compute.SetBuffer(k_init, "OriginalBuffer", b_original);
        compute.SetBuffer(k_init, "DictBuffer", b_dict);
        compute.SetBuffer(k_init, "CulledDict", b_culled);
        compute.SetBuffer(k_init, "SymbolCount", b_symbolCount);
        compute.Dispatch(k_init, 1, 1, 1);

        compute.SetBuffer(k_quant, "SymbolCount", b_symbolCount);
        compute.SetBuffer(k_quant, "DictBuffer", b_dict);
        compute.SetBuffer(k_quant, "orig_prob", b_origProb);
        compute.SetBuffer(k_quant, "quant_prob", b_quantProb);
        compute.SetBuffer(k_quant, "quant_round", b_quantRound);
        compute.SetBuffer(k_quant, "ErrorCorr", b_errorCorr);
        compute.Dispatch(k_quant, 1, 1, 1);

        //symbolCount
        test6 = new uint[wiggleData];
        b_symbolCount.GetData(test6);

        Debug.Log("The symbol count is " + test6[0]);
        Debug.Log("The quantized total is " + test6[1]);


        compute.SetBuffer(k_correctSingle, "SymbolCount", b_symbolCount);
        compute.SetBuffer(k_correctSingle, "DictBuffer", b_dict);
        compute.SetBuffer(k_correctSingle, "orig_prob", b_origProb);
        compute.SetBuffer(k_correctSingle, "quant_prob", b_quantProb);
        compute.SetBuffer(k_correctSingle, "quant_round", b_quantRound);
        compute.SetBuffer(k_correctSingle, "ErrorCorr", b_errorCorr);
        //compute.Dispatch(k_correctSingle, 1, 1, 1);

        compute.SetBuffer(k_correctMulti, "SymbolCount", b_symbolCount);
        compute.SetBuffer(k_correctMulti, "DictBuffer", b_dict);
        compute.SetBuffer(k_correctMulti, "orig_prob", b_origProb);
        compute.SetBuffer(k_correctMulti, "quant_prob", b_quantProb);
        compute.SetBuffer(k_correctMulti, "quant_round", b_quantRound);
        compute.SetBuffer(k_correctMulti, "ErrorCorr", b_errorCorr);
        compute.Dispatch(k_correctMulti, 1, 1, 1);

        //Quantized Probability
        test3 = new float[b_quantProb.count];
        b_quantProb.GetData(test3);

        //Quantized Frequency
        test2 = new uint[b_quantRound.count];
        b_quantRound.GetData(test2);

        //error corrections floats
        test = new float[b_errorCorr.count * 2];
        b_errorCorr.GetData(test);

        //error correction uints
        test4 = new uint[b_errorCorr.count * 2];
        b_errorCorr.GetData(test4);

        //Frequency
        test5 = new uint[arrSize];
        b_dict.GetData(test5);

        uint total = 0;

        for (int i = 0; i < arrSize; i++)
        {
            if (test4[i * 2] != 0)
            {
                //Debug.Log("Symbol: " + test4[i * 2] + ", Frequency: " + test5[test4[i * 2]] + ", Quantized Frequency " + test2[test4[i * 2]] + ",  the corrected error is " + test[(i * 2) + 1]);
                total += test5[test4[i * 2]];
            }
        }

        //grab data again for validation
        b_symbolCount.GetData(test6);
        if (test6[1] == test6[2])
        {
            Debug.Log("Correction Validated");
        }
        else
        {
            Debug.Log("Correction Error");
        }

        if (total != arrSize)
        {
            Debug.Log("Frequency error: " + total);
        }

        compute.SetBuffer(k_binTree, "BinaryBuffer", b_binTree);
        compute.Dispatch(k_binTree, 1, 1, 1);

        test7 = new uint[binSize * 3];
        b_binTree.GetData(test7);

        for (int i = 2; i < test7.Length; i += 3)
        {
            Debug.Log(test7[i]);
        }
    }

    void OnDisable()
    {
        b_original.Release();
        b_temp.Release();
        b_dict.Release();
        b_culled.Release();
        b_symbolCount.Release();

        b_origProb.Release();
        b_quantProb.Release();
        b_quantRound.Release();
        b_errorCorr.Release();

        b_binTree.Release();
    }
}
