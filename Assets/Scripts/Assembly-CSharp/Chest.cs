using System;
using UnityEngine;

[Serializable]
public class Chest
{
	public ChestType type;

	public UnlockType unlockType;

	public int price;

	public int entryCount;

	public PrizeEntryPool[] PrizeEntryses;

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public Chest Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}

	public PrizeEntry[] Roll()
	{
		PrizeEntry[] array = new PrizeEntry[entryCount];
		int num = 0;
		for (int i = 0; i < entryCount; i++)
		{
			array[i] = PrizeEntryses[i].Roll();
			num = UnityEngine.Random.Range(array[i].min, array[i].max);
			array[i].min = (array[i].max = num);
		}
		if (type == ChestType.Game && PlayerInfo.Instance.amountOfGameChestesOpened == 0)
		{
			PlayerInfo.Instance.amountOfGameChestesOpened++;
			PrizeEntry prizeEntry = default(PrizeEntry);
			prizeEntry.itemType = PrizeEntryType.SEB;
			prizeEntry.min = 1;
			prizeEntry.max = 1;
			prizeEntry.weight = 1f;
			array[0] = prizeEntry;
		}
		return array;
	}
}
