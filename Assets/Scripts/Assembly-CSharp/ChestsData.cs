using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ChestsData
{
	public static Dictionary<ChestType, Chest> chests;

	public static Dictionary<ChestType, ChestTemplate> chestTemplates;

	public static Dictionary<PrizeEntryType, PrizeEntryTemplate> prizeEntryTemplates;

	public static void LoadFile()
	{
		LoadChestFile();
		LoadChestTemplateFile();
		LoadPrizeEntryTemplateFile();
	}

	public static void SaveChestFile()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<ChestType, Chest> chest in chests)
		{
			dictionary.Add(chest.Key.ToString(), chest.Value.ToJson());
		}
		string value = RiseJson.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Chest/chest.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static void LoadChestFile()
	{
		if (chests == null)
		{
			chests = new Dictionary<ChestType, Chest>();
		}
		chests.Clear();
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Chest/chest");
		if (textAsset == null)
		{
			InitChestFile();
			Debug.LogError("The chest file is not exist at /chest.");
			return;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(textAsset.text) as IDictionary<string, object>;
		ChestType chestType = ChestType.None;
		int i = 0;
		for (int num = 6; i <= num; i++)
		{
			chestType = (ChestType)i;
			string key = chestType.ToString();
			if (dictionary.ContainsKey(key) && !chests.ContainsKey(chestType))
			{
				chests.Add(chestType, new Chest().Parse((string)dictionary[key]));
			}
		}
	}

	public static void InitChestFile()
	{
		int i = 0;
		for (int num = 6; i < num; i++)
		{
			ChestType chestType = (ChestType)i;
			Chest chest = new Chest();
			chest.type = chestType;
			chest.entryCount = 3;
			chest.PrizeEntryses = new PrizeEntryPool[3];
			for (int j = 0; j < chest.entryCount; j++)
			{
				chest.PrizeEntryses[j] = new PrizeEntryPool();
				chest.PrizeEntryses[j].prizeEntries = new PrizeEntry[10];
			}
			chests.Add(chestType, chest);
		}
		SaveChestFile();
	}

	public static void SaveChestTemplateFile()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<ChestType, ChestTemplate> chestTemplate in chestTemplates)
		{
			dictionary.Add(chestTemplate.Key.ToString(), chestTemplate.Value.ToJson());
		}
		string value = RiseJson.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Chest/chestTemplate.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static void LoadChestTemplateFile()
	{
		if (chestTemplates == null)
		{
			chestTemplates = new Dictionary<ChestType, ChestTemplate>();
		}
		chestTemplates.Clear();
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Chest/chestTemplate");
		if (textAsset == null)
		{
			InitChestTemplateFile();
			Debug.LogError("The chestTemplate file is not exist at /chestTemplate.");
			return;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(textAsset.text) as IDictionary<string, object>;
		ChestType chestType = ChestType.None;
		int i = 0;
		for (int num = 6; i <= num; i++)
		{
			chestType = (ChestType)i;
			string key = chestType.ToString();
			if (dictionary.ContainsKey(key) && !chestTemplates.ContainsKey(chestType))
			{
				chestTemplates.Add(chestType, new ChestTemplate().Parse((string)dictionary[key]));
			}
		}
	}

	public static void InitChestTemplateFile()
	{
		int i = 0;
		for (int num = 6; i < num; i++)
		{
			ChestType key = (ChestType)i;
			ChestTemplate value = new ChestTemplate();
			chestTemplates.Add(key, value);
		}
		SaveChestTemplateFile();
	}

	public static void SavePrizeEntryTemplateFile()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<PrizeEntryType, PrizeEntryTemplate> prizeEntryTemplate in prizeEntryTemplates)
		{
			dictionary.Add(prizeEntryTemplate.Key.ToString(), prizeEntryTemplate.Value.ToJson());
		}
		string value = RiseJson.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Chest/prizeEntryTemplate.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static void LoadPrizeEntryTemplateFile()
	{
		if (prizeEntryTemplates == null)
		{
			prizeEntryTemplates = new Dictionary<PrizeEntryType, PrizeEntryTemplate>();
		}
		prizeEntryTemplates.Clear();
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Chest/prizeEntryTemplate");
		if (textAsset == null)
		{
			InitPrizeEntryTemplateFile();
			Debug.LogError("The chest file is not exist at /prizeEntryTemplate.");
			return;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(textAsset.text) as IDictionary<string, object>;
		PrizeEntryType prizeEntryType = PrizeEntryType.None;
		int i = 0;
		for (int num = 10; i <= num; i++)
		{
			prizeEntryType = (PrizeEntryType)i;
			string key = prizeEntryType.ToString();
			if (dictionary.ContainsKey(key) && !prizeEntryTemplates.ContainsKey(prizeEntryType))
			{
				prizeEntryTemplates.Add(prizeEntryType, new PrizeEntryTemplate().Parse((string)dictionary[key]));
			}
		}
	}

	public static void InitPrizeEntryTemplateFile()
	{
		int i = 0;
		for (int num = 10; i < num; i++)
		{
			PrizeEntryType key = (PrizeEntryType)i;
			PrizeEntryTemplate value = new PrizeEntryTemplate();
			prizeEntryTemplates.Add(key, value);
		}
		SavePrizeEntryTemplateFile();
	}

	public static Chest GetChest(ChestType type)
	{
		if (chests == null || !chests.ContainsKey(type))
		{
			return null;
		}
		return chests[type];
	}

	public static ChestTemplate GetChestTemplate(ChestType type)
	{
		if (chestTemplates == null || !chestTemplates.ContainsKey(type))
		{
			return null;
		}
		return chestTemplates[type];
	}

	public static PrizeEntryTemplate GetPrizeEntryTemplate(PrizeEntryType type)
	{
		if (prizeEntryTemplates == null || !prizeEntryTemplates.ContainsKey(type))
		{
			return null;
		}
		return prizeEntryTemplates[type];
	}
}
