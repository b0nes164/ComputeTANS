using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridRadix : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute;

    private static int size = 100;
    private static int max = 32;
    private static int[] arr = new int[size];

    void Start()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(1, max);
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
        }

        for (int i = _arr.Length - 1; i >= 0; i--)
        {
            count[_arr[i]]--;
            output[count[arr[i]]] = _arr[i];
        }

        for (int i = 0; i < output.Length; i++)
        {
            Debug.Log(output[i]); 
        }
    }
}
