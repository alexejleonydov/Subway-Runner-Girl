using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class DailyLandingAwards
{
	public static DailyLandingAward[] awards;

	private const string path = "Text/dailyLanding";

	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/dailyLanding");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		string text = textAsset.text;
		IList<object> list = Json.Deserialize(text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		awards = new DailyLandingAward[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			awards[i] = new DailyLandingAward();
			awards[i].Parse((string)list[i]);
		}
		return true;
	}

	public static bool SaveFile()
	{
		if (awards == null || awards.Length <= 0)
		{
			return false;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < awards.Length; i++)
		{
			list.Add(awards[i].ToJson());
		}
		string value = Json.Serialize(list);
		string text = Application.dataPath + "/Resources/Text/dailyLanding.txt";
		using (StreamWriter streamWriter = File.CreateText(text))
		{
			streamWriter.Write(value);
			streamWriter.Close();
		}
		return true;
	}

	public static DailyLandingAward GetDailyLandingAwardByID(int dayId)
	{
		if (dayId <= 0 || dayId > awards.Length)
		{
			return null;
		}
		return awards[dayId - 1];
	}
}
