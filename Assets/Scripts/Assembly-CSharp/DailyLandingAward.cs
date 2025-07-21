using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;

public class DailyLandingAward
{
	public enum DailyLandingRewardType
	{
		Coins = 0,
		Chest = 1,
		Keys = 2
	}

	public DailyLandingRewardType type;

	public int Amount;

	public ChestType chestType;

	public DailyLandingAward()
	{
		type = DailyLandingRewardType.Coins;
		Amount = 1;
		chestType = ChestType.Normal;
	}

	public DailyLandingAward(DailyLandingAward award)
	{
		type = award.type;
		Amount = award.Amount;
		chestType = award.chestType;
	}

	public string ToJson()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("type", type.ToString());
		dictionary.Add("Amount", Amount);
		dictionary.Add("chestType", chestType.ToString());
		return Json.Serialize(dictionary);
	}

	public DailyLandingAward Parse(string json)
	{
		IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
		if (dictionary.ContainsKey("type"))
		{
			try
			{
				type = (DailyLandingRewardType)Enum.Parse(typeof(DailyLandingRewardType), (string)dictionary["type"]);
			}
			catch (ArgumentException)
			{
				Debug.Log(json + "Parse Error.");
			}
		}
		if (dictionary.ContainsKey("Amount"))
		{
			Amount = (int)(long)dictionary["Amount"];
		}
		if (dictionary.ContainsKey("chestType"))
		{
			try
			{
				chestType = (ChestType)Enum.Parse(typeof(ChestType), (string)dictionary["chestType"]);
			}
			catch (ArgumentException)
			{
				Debug.Log(json + "Parse Error.");
			}
		}
		return this;
	}
}
