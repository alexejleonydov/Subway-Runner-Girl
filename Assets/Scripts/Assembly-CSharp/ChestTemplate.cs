using System;
using UnityEngine;

[Serializable]
public class ChestTemplate
{
	public LanguageKey description;

	public XY one;

	public XY two;

	public PrizeEntryType[] features;

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public ChestTemplate Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}
}
