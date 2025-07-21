using System.Collections.Generic;
using UnityEngine;

public class RandomizerHold : Randomizer
{
	[SerializeField]
	private GameObject[] children;

	private static int startIndex = 0;

	private static float distance = 3000f;

	private static int[] randomIndices = new int[21]
	{
		0, 1, 2, 3, 0, 4, 5, 1, 0, 2,
		4, 1, 3, 2, 0, 5, 1, 0, 3, 1,
		3
	};

	public static void Initialize()
	{
		startIndex = Random.Range(0, randomIndices.Length);
	}

	public override void PerformRandomizer(List<GameObject> objects)
	{
		int num = Mathf.FloorToInt(base.transform.position.z / distance) + startIndex;
		int num2 = randomIndices[(startIndex + num) % randomIndices.Length];
		for (int i = 0; i < children.Length; i++)
		{
			if (i == num2)
			{
				objects.Add(children[i]);
			}
			else
			{
				children[i].SetActive(false);
			}
		}
	}
}
