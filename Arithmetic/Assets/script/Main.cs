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

        System.Random rand = new System.Random();
        compute.SetInt("e_seed", rand.Next(0, 2000000000));
        compute.SetInt("e_arraySize", arrSize);
        compute.SetInt("e_dictSize", dictSize);

        int k_init = compute.FindKernel("Init");

        b_culled.SetCounterValue(0);
        compute.SetBuffer(k_init, "TempBuffer", b_temp);
        compute.SetBuffer(k_init, "OriginalBuffer", b_original);
        compute.SetBuffer(k_init, "DictBuffer", b_dict);
        compute.SetBuffer(k_init, "CulledDict", b_culled);
        compute.Dispatch(k_init, 1, 1, 1);


    }

    void OnDisable()
    {
        b_original.Release();
        b_temp.Release();
        b_dict.Release();
        b_culled.Release();
    }
}
