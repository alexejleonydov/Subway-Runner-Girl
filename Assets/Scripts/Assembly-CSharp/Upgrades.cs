using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Upgrades
{
	public static Dictionary<PropType, Upgrade> upgrades;

	public static void LoadFile()
	{
		Load();
	}

	public static Dictionary<PropType, Upgrade> Load()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Upgrades");
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		upgrades = new Dictionary<PropType, Upgrade>();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			upgrades.Add((PropType)Enum.Parse(typeof(PropType), item.Key), Upgrade.Parse((string)item.Value));
		}
		return upgrades;
	}

	public static void Save()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<PropType, Upgrade> upgrade in upgrades)
		{
			dictionary.Add(upgrade.Key.ToString(), Upgrade.ToJson(upgrade.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Upgrades.txt";
		try
		{
			using (StreamWriter streamWriter = File.CreateText(path))
			{
				streamWriter.WriteLine(value);
				streamWriter.Close();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}
}
