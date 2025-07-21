using System.Collections;
using UnityEngine;

public static class Utility
{
	public static int CompareVersions(string leftVersion, string rightVersion)
	{
		char[] separator = new char[1] { '.' };
		string[] array = leftVersion.Split(separator);
		char[] separator2 = new char[1] { '.' };
		string[] array2 = rightVersion.Split(separator2);
		for (int i = 0; i < array.Length || i < array2.Length; i++)
		{
			int num = ((i < array.Length) ? int.Parse(array[i]) : 0);
			int num2 = ((i < array2.Length) ? int.Parse(array2[i]) : 0);
			if (num != num2)
			{
				return num - num2;
			}
		}
		return 0;
	}

	public static int NumberOfDigits(int number)
	{
		int num = 0;
		if (number == 0)
		{
			return 1;
		}
		while (number != 0)
		{
			number /= 10;
			num++;
		}
		return num;
	}

	public static void SetLayerRecursively(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		IEnumerator enumerator = t.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Transform t2 = (Transform)enumerator.Current;
			SetLayerRecursively(t2, layer);
		}
	}
}
