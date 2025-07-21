using System;
using UnityEngine;

[Serializable]
public class PrizeEntryPool
{
	public PrizeEntry[] prizeEntries;

	public PrizeEntry Roll()
	{
		float num = 0f;
		for (int i = 0; i < prizeEntries.Length; i++)
		{
			num += prizeEntries[i].weight;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		for (int j = 0; j < prizeEntries.Length; j++)
		{
			num3 += prizeEntries[j].weight;
			if (num2 <= num3)
			{
				return prizeEntries[j];
			}
		}
		return prizeEntries[0];
	}
}
