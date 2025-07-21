using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelExpManager
{
	public class LevelExpData
	{
		public int minLevel;

		public int maxLevel;

		public int expCoefficient;

		public int levelAwardAmount;

		public LevelAwardStruct[] levelAwards;

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public LevelExpData Parse(string json)
		{
			JsonUtility.FromJsonOverwrite(json, this);
			return this;
		}
	}

	[Serializable]
	public struct LevelAwardStruct
	{
		public LevelAwardType levelAwardType;

		public int awardNum;
	}

	public enum LevelAwardType
	{
		Coin = 0,
		Exp = 1,
		Gem = 2
	}

	private static LevelExpManager _instance;

	private TimeCoolDown _freeCoolDown;

	public static Dictionary<int, LevelExpData> levelExpDatas = new Dictionary<int, LevelExpData>();

	public static LevelExpManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LevelExpManager();
			}
			return _instance;
		}
	}

	private LevelExpManager()
	{
		_freeCoolDown = new TimeCoolDown("GetNextTaskCoolingDown", 14400);
	}

	public bool IsCoolingDownOver()
	{
		return _freeCoolDown.IsCoolingDownOver();
	}

	public string GetCoolingDownTime()
	{
		return _freeCoolDown.GetCoolingDownTime3();
	}

	public void SetNewTime()
	{
		_freeCoolDown.SetFreeTime();
	}

	public void ForceCoolingDownOver()
	{
		_freeCoolDown.ForceCoolingDownOver();
	}

	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Characters/levelExp");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			InitlevelExpFile();
			Debug.LogError("The chest file is not exist at /chest.");
			return false;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		levelExpDatas = new Dictionary<int, LevelExpData>();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			levelExpDatas.Add(int.Parse(item.Key), new LevelExpData().Parse((string)dictionary[item.Key]));
		}
		return true;
	}

	private static void InitlevelExpFile()
	{
		int num = 2;
		int i = 0;
		for (int num2 = num; i < num2; i++)
		{
			num = i;
			LevelExpData levelExpData = new LevelExpData();
			levelExpData.minLevel = num + 1;
			levelExpData.maxLevel = 10 * (num + 1);
			levelExpData.expCoefficient = 29 + 10 * num;
			levelExpData.levelAwardAmount = 2;
			levelExpData.levelAwards = new LevelAwardStruct[2];
			for (int j = 0; j < levelExpData.levelAwardAmount; j++)
			{
				levelExpData.levelAwards[j] = default(LevelAwardStruct);
			}
			levelExpDatas.Add(num, levelExpData);
		}
		SaveFile();
	}

	public static bool SaveFile()
	{
		if (levelExpDatas == null || levelExpDatas.Count <= 0)
		{
			return false;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<int, LevelExpData> levelExpData in levelExpDatas)
		{
			Debug.Log(levelExpData.Value.levelAwards);
			dictionary.Add(levelExpData.Key.ToString(), levelExpData.Value.ToJson());
		}
		string value = RiseJson.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Characters/levelExp.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
		return true;
	}
}
