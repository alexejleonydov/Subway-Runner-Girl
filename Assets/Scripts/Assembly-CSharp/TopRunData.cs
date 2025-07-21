using System;
using UnityEngine;

[Serializable]
public class TopRunData
{
	public string playerName;

	public string country;

	public string weekstring;

	public int hasGoldMedal;

	public int hasSliverMedal;

	public int hasBronzeMedal;

	public TopRunData()
	{
		UnityEngine.Random.InitState(DateTime.UtcNow.Millisecond);
		playerName = "Player" + UnityEngine.Random.Range(1000, 9999) + UnityEngine.Random.Range(10, 99);
		country = "NOTSET";
		weekstring = "2013_201917_week";
		hasGoldMedal = 0;
		hasSliverMedal = 0;
		hasBronzeMedal = 0;
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public TopRunData Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}
}
