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


    private int arrSize = 65536;
    private int dictSize = 300;

    private float[] test;
    private uint[] test2;
    private uint[] test3;

    void Start()
    {
        b_original = new ComputeBuffer(arrSize, sizeof(uint));
        b_temp = new ComputeBuffer(dictSize, sizeof(uint));
        b_dict = new ComputeBuffer(arrSize, sizeof(uint));
        b_culled = new ComputeBuffer(arrSize, sizeof(uint) * 2, ComputeBufferType.Append);
        b_symbolCount = new ComputeBuffer(1, sizeof(uint));

        b_origProb = new ComputeBuffer(arrSize, sizeof(float));
        b_quantProb = new ComputeBuffer(arrSize, sizeof(float));
        b_quantRound = new ComputeBuffer(arrSize, sizeof(uint));
        b_errorCorr = new ComputeBuffer(arrSize, sizeof(uint) + sizeof(float));

        System.Random rand = new System.Random();
        compute.SetInt("e_seed", rand.Next(0, 2000000000));
        compute.SetInt("e_arraySize", arrSize);
        compute.SetInt("e_dictSize", dictSize);

        int k_init = compute.FindKernel("Init");
        int k_quant = compute.FindKernel("Quantize");

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

        test3 = new uint[1];
        b_symbolCount.GetData(test3);
        Debug.Log(test3[0]);

        test2 = new uint[b_quantRound.count];
        b_quantRound.GetData(test2);
        foreach (uint g in test2)
        {
            //Debug.Log(g);
        }

        test = new float[b_errorCorr.count * 2];

        for (int i = 1; i < test.Length; i+= 2)
        {
            Debug.Log(test[i]);
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
    }
}
