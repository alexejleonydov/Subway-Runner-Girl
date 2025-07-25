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

		/*string achievementDataText = "[\"{\\\"type\\\":\\\"EarnCoin\\\",\\\"goal\\\":5000}-{\\\"icon\\\":\\\"Achievement_icon_EarnCoin1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":2}\"," +
"\"{\\\"type\\\":\\\"EarnCoin\\\",\\\"goal\\\":50000}-{\\\"icon\\\":\\\"Achievement_icon_EarnCoin2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"EarnCoin\\\",\\\"goal\\\":100000}-{\\\"icon\\\":\\\"Achievement_icon_EarnCoin3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"TimeDeath\\\",\\\"goal\\\":3600}-{\\\"icon\\\":\\\"Achievement_icon_TimeDeath1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":2}\"," +
"\"{\\\"type\\\":\\\"TimeDeath\\\",\\\"goal\\\":18000}-{\\\"icon\\\":\\\"Achievement_icon_TimeDeath2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"TimeDeath\\\",\\\"goal\\\":72000}-{\\\"icon\\\":\\\"Achievement_icon_TimeDeath3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"HaveCharacters\\\",\\\"goal\\\":2}-{\\\"icon\\\":\\\"Achievement_icon_HaveRole1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"HaveCharacters\\\",\\\"goal\\\":6}-{\\\"icon\\\":\\\"Achievement_icon_HaveRole2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"HaveCharacters\\\",\\\"goal\\\":10}-{\\\"icon\\\":\\\"Achievement_icon_HaveRole3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":20}\"," +
"\"{\\\"type\\\":\\\"DailyQuests\\\",\\\"goal\\\":7}-{\\\"icon\\\":\\\"Achievement_icon_Daily1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"DailyQuests\\\",\\\"goal\\\":30}-{\\\"icon\\\":\\\"Achievement_icon_Daily2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"DailyQuests\\\",\\\"goal\\\":60}-{\\\"icon\\\":\\\"Achievement_icon_Daily3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":15}\"," +
"\"{\\\"type\\\":\\\"ScoreSingleRun\\\",\\\"goal\\\":100000}-{\\\"icon\\\":\\\"Achievement_icon_Score1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"ScoreSingleRun\\\",\\\"goal\\\":500000}-{\\\"icon\\\":\\\"Achievement_icon_Score2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"ScoreSingleRun\\\",\\\"goal\\\":1000000}-{\\\"icon\\\":\\\"Achievement_icon_Score3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":15}\"," +
"\"{\\\"type\\\":\\\"BeatOwnHighscore\\\",\\\"goal\\\":5}-{\\\"icon\\\":\\\"Achievement_icon_BeatScore1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":5}\"," +
"\"{\\\"type\\\":\\\"BeatOwnHighscore\\\",\\\"goal\\\":10}-{\\\"icon\\\":\\\"Achievement_icon_BeatScore2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"ScoreBooster\\\",\\\"goal\\\":10}-{\\\"icon\\\":\\\"Achievement_icon_Booster1\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":10}\"," +
"\"{\\\"type\\\":\\\"ScoreBooster\\\",\\\"goal\\\":20}-{\\\"icon\\\":\\\"Achievement_icon_Booster2\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":20}\"," +
"\"{\\\"type\\\":\\\"ScoreBooster\\\",\\\"goal\\\":30}-{\\\"icon\\\":\\\"Achievement_icon_Booster3\\\",\\\"rewardType\\\":2,\\\"rewardAmount\\\":30}\"]";*/


		string achievementDataText = PlayerPrefs.GetString("AchievementDataText", textAsset.text);

		// Log it first to verify it's valid JSON
		Debug.Log("Loaded from PlayerPrefs: " + achievementDataText);

		// Now parse it
		IList<object> list = Json.Deserialize(achievementDataText) as IList<object>;

		if (list == null || list.Count <= 0)
		{
			Debug.Log("Error, the file is empty.");
			return null;
		}

		foreach (var item in list)
		{
			Debug.Log(item); // each string like "{ \"type\": \"EarnCoin\", ... }-{...}"
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
		/*using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}*/

		PlayerPrefs.SetString("AchievementDataText", value);
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
