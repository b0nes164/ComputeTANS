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
    private ComputeBuffer b_quantize;
    private ComputeBuffer b_symbols;

    private int arrSize = 65536;
    private int dictSize = 300;

    private uint[] test;
    private uint[] test2;
    private uint[] test3;

    void Start()
    {
        b_original = new ComputeBuffer(arrSize, sizeof(uint));
        b_temp = new ComputeBuffer(dictSize, sizeof(uint));
        b_dict = new ComputeBuffer(arrSize, sizeof(uint));
        b_culled = new ComputeBuffer(arrSize, sizeof(uint) * 2, ComputeBufferType.Append);
        b_symbols = new ComputeBuffer(1, sizeof(uint));
        b_quantize = new ComputeBuffer(1024, sizeof(uint));

        System.Random rand = new System.Random();

        int k_init = compute.FindKernel("Init");
        int k_initTwo = compute.FindKernel("Init2");
        int k_initThree = compute.FindKernel("Init3");
        int k_genDict = compute.FindKernel("GenerateDict");
        int k_genCull = compute.FindKernel("GenerateCulledDict");
        int k_quantize = compute.FindKernel("Quantize");

        compute.SetInt("e_arraySize", arrSize);
        compute.SetInt("e_dictSize", dictSize);
        compute.SetInt("e_seed", rand.Next(0, 2000000000));

        compute.SetBuffer(k_init, "TempBuffer", b_temp);
        compute.Dispatch(k_init, Mathf.CeilToInt(b_temp.count / 1024f), 1, 1);

        compute.SetBuffer(k_initTwo, "OriginalBuffer", b_original);
        compute.SetBuffer(k_initTwo, "TempBuffer", b_temp);
        compute.Dispatch(k_initTwo, Mathf.CeilToInt(b_original.count / 1024f), 1, 1);

        compute.SetBuffer(k_initThree, "DictBuffer", b_dict);
        compute.Dispatch(k_initThree, Mathf.CeilToInt(b_dict.count / 1024f), 1, 1);

        compute.SetBuffer(k_genDict, "DictBuffer", b_dict);
        compute.SetBuffer(k_genDict, "OriginalBuffer", b_original);
        compute.Dispatch(k_genDict, Mathf.CeilToInt(b_dict.count /1024f), 1, 1);

        b_culled.SetCounterValue(0);
        b_symbols.SetData(new uint[1] { 0 } );
        compute.SetInt("g_numberOfSymbols", 0);
        compute.SetBuffer(k_genCull, "CulledDict", b_culled);
        compute.SetBuffer(k_genCull, "DictBuffer", b_dict);
        compute.SetBuffer(k_genCull, "NumberOfSymbols", b_symbols);
        compute.Dispatch(k_genCull, Mathf.CeilToInt(b_dict.count / 1024f), 1, 1);

        compute.

        compute.SetBuffer(k_quantize, "QuantizeTest", b_quantize);
        //compute.Dispatch(k_quantize, 1, 1, 1);


        test2 = new uint[1];
        b_symbols.GetData(test2);
        foreach (uint g in test2)
        {
            Debug.Log(g);
        }
 

        test = new uint[b_culled.count * 2];
        uint temp = 0;
        b_culled.GetData(test);
        for (int i = 1; i < test.Length; i += 2)
        {
            if (test[i] != 0)
            {
                temp += test[i];
            }
        }
        Debug.Log(temp + "\n");

        test3 = new uint[b_quantize.count];
        b_quantize.GetData(test3);
        foreach (uint g in test3)
        {
            Debug.Log(g);
        }
    }

    void OnDisable()
    {
        b_original.Release();
        b_temp.Release();
        b_dict.Release();
        b_culled.Release();
        b_quantize.Release();
        b_symbols.Dispose();
    }
}
