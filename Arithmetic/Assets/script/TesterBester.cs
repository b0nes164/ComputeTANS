using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterBester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		uint[] l_hist = new uint[8];
		uint k = 0;

		System.Random rand = new System.Random();

		for (int j = 0; j < 100; j++)
		{
			for (int i = 0; i < 8; i++)
			{
				l_hist[i] = (uint)rand.Next();
			}
			uint l = l_hist[k];
			uint r = l_hist[k + 1];
			if (r > l)
			{
				l_hist[k] = r;
				l_hist[k + 1] = l;
			}

			l = l_hist[k + 2];
			r = l_hist[k + 3];
			if (r > l)
			{
				l_hist[k + 2] = r;
				l_hist[k + 3] = l;
			}

			l = l_hist[k + 4];
			r = l_hist[k + 5];
			if (r > l)
			{
				l_hist[k + 4] = r;
				l_hist[k + 5] = l;
			}

			l = l_hist[k + 6];
			r = l_hist[k + 7];
			if (r > l)
			{
				l_hist[k + 6] = r;
				l_hist[k + 7] = l;
			}

			//second row
			l = l_hist[k];
			r = l_hist[k + 2];
			if (r > l)
			{
				l_hist[k] = r;
				l_hist[k + 2] = l;
			}

			l = l_hist[k + 4];
			r = l_hist[k + 6];
			if (r > l)
			{
				l_hist[k + 4] = r;
				l_hist[k + 6] = l;
			}

			l = l_hist[k + 1];
			r = l_hist[k + 3];
			if (r > l)
			{
				l_hist[k + 1] = r;
				l_hist[k + 3] = l;
			}

			l = l_hist[k + 5];
			r = l_hist[k + 7];
			if (r > l)
			{
				l_hist[k + 5] = r;
				l_hist[k + 7] = l;
			}

			l = l_hist[k + 1];
			r = l_hist[k + 2];
			if (r > l)
			{
				l_hist[k + 1] = r;
				l_hist[k + 2] = l;
			}

			l = l_hist[k + 5];
			r = l_hist[k + 6];
			if (r > l)
			{
				l_hist[k + 5] = r;
				l_hist[k + 6] = l;
			}

			//thid row
			l = l_hist[k];
			r = l_hist[k + 4];
			if (r > l)
			{
				l_hist[k] = r;
				l_hist[k + 4] = l;
			}

			l = l_hist[k + 1];
			r = l_hist[k + 5];
			if (r > l)
			{
				l_hist[k + 1] = r;
				l_hist[k + 5] = l;
			}

			l = l_hist[k + 2];
			r = l_hist[k + 6];
			if (r > l)
			{
				l_hist[k + 2] = r;
				l_hist[k + 6] = l;
			}

			l = l_hist[k + 3];
			r = l_hist[k + 7];
			if (r > l)
			{
				l_hist[k + 3] = r;
				l_hist[k + 7] = l;
			}

			l = l_hist[k + 2];
			r = l_hist[k + 4];
			if (r > l)
			{
				l_hist[k + 2] = r;
				l_hist[k + 4] = l;
			}

			l = l_hist[k + 3];
			r = l_hist[k + 5];
			if (r > l)
			{
				l_hist[k + 3] = r;
				l_hist[k + 5] = l;
			}

			//last row
			l = l_hist[k + 1];
			r = l_hist[k + 2];
			if (r > l)
			{
				l_hist[k + 1] = r;
				l_hist[k + 2] = l;
			}

			l = l_hist[k + 3];
			r = l_hist[k + 4];
			if (r > l)
			{
				l_hist[k + 3] = r;
				l_hist[k + 4] = l;
			}

			l = l_hist[k + 5];
			r = l_hist[k + 6];
			if (r > l)
			{
				l_hist[k + 5] = r;
				l_hist[k + 6] = l;
			}

			for (int z = 0; z < 7; z++)
			{
				if (l_hist[z] < l_hist[z + 1])
				{
					Debug.Log("error");
				}
			}
		}
	}
}
