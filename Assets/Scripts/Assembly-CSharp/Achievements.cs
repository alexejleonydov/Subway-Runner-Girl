using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Achievements
{
	private static Task[] _achievementArray;

	private static AchievementInfo[] _achievementInfos;

	private static Achievements _instance;

	public static int NUMBER_OF_ACHIEVEMENTS = 20;

	public static Task[] achievementArray
	{
		get
		{
			return _achievementArray;
		}
	}

	public static AchievementInfo[] achievementInfo
	{
		get
		{
			return _achievementInfos;
		}
	}

	public static Task[] LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/Achievements");
		if (textAsset == null)
		{
			Debug.Log("The file is not exist.");
			return null;
		}
		IList<object> list = Json.Deserialize(textAsset.text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			Debug.Log("Error, the file is empty.");
			return null;
		}
		_achievementArray = new Task[NUMBER_OF_ACHIEVEMENTS];
		_achievementInfos = new AchievementInfo[NUMBER_OF_ACHIEVEMENTS];
		int i = 0;
		for (int count = list.Count; i < count && i < NUMBER_OF_ACHIEVEMENTS; i++)
		{
			string text = (string)list[i];
			string[] array = text.Split('-');
			_achievementArray[i] = Task.Parse(array[0]);
			_achievementInfos[i] = new AchievementInfo().Parse(array[1]);
		}
		for (; i < NUMBER_OF_ACHIEVEMENTS; i++)
		{
			_achievementArray[i] = new Task();
			_achievementInfos[i] = new AchievementInfo();
		}
		return _achievementArray;
	}

	public static void SaveFile()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < _achievementArray.Length; i++)
		{
			list.Add(Task.ToJson(_achievementArray[i]) + "-" + _achievementInfos[i].ToJson());
		}
		string value = Json.Serialize(list);
		string path = Application.dataPath + "/Resources/Text/Task/Achievements.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static int getAchievementIndexInArray(TaskType type)
	{
		for (int i = 0; i < NUMBER_OF_ACHIEVEMENTS; i++)
		{
			if (achievementArray[i].type == type)
			{
				return i;
			}
		}
		return -1;
	}

	public static void Init()
	{
		_achievementArray = new Task[NUMBER_OF_ACHIEVEMENTS];
		_achievementInfos = new AchievementInfo[NUMBER_OF_ACHIEVEMENTS];
		for (int i = 0; i < NUMBER_OF_ACHIEVEMENTS; i++)
		{
			_achievementArray[i] = new Task();
			_achievementInfos[i] = new AchievementInfo();
		}
		SaveFile();
	}
}
