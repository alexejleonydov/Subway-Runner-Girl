using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Helmets
{
	public struct Helm
	{
		public LanguageKey name;

		public string helmModelName;

		public int price;

		public UnlockType unlockType;

		public int level;

		public LanguageKey description;

		public bool useMagent;

		public bool useMutiplier;

		public static string ToJson(Helm helm)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("name", helm.name.ToString());
			dictionary.Add("helmModelName", helm.helmModelName);
			dictionary.Add("price", helm.price);
			dictionary.Add("unlockType", helm.unlockType);
			dictionary.Add("level", helm.level);
			dictionary.Add("description", helm.description.ToString());
			dictionary.Add("useMagent", helm.useMagent);
			dictionary.Add("useMutiplier", helm.useMutiplier);
			return Json.Serialize(dictionary);
		}

		public static Helm Parse(string json)
		{
			Helm result = default(Helm);
			IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
			if (dictionary.ContainsKey("name"))
			{
				try
				{
					result.name = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["name"]);
				}
				catch (ArgumentException)
				{
					Debug.LogError((string)dictionary["name"] + " is not one of StringID!");
				}
			}
			if (dictionary.ContainsKey("helmModelName"))
			{
				result.helmModelName = (string)dictionary["helmModelName"];
			}
			if (dictionary.ContainsKey("price"))
			{
				result.price = (int)(long)dictionary["price"];
			}
			if (dictionary.ContainsKey("unlockType"))
			{
				try
				{
					result.unlockType = (UnlockType)Enum.Parse(typeof(UnlockType), (string)dictionary["unlockType"]);
				}
				catch (ArgumentException)
				{
					Debug.LogError((string)dictionary["unlockType"] + " is not one of UnlockType!");
				}
			}
			if (dictionary.ContainsKey("level"))
			{
				result.level = (int)(long)dictionary["level"];
			}
			if (dictionary.ContainsKey("description"))
			{
				try
				{
					result.description = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["description"]);
				}
				catch (ArgumentException)
				{
					Debug.LogError((string)dictionary["description"] + " is not one of StringID!");
				}
			}
			if (dictionary.ContainsKey("useMagent"))
			{
				result.useMagent = (bool)dictionary["useMagent"];
			}
			if (dictionary.ContainsKey("useMutiplier"))
			{
				result.useMutiplier = (bool)dictionary["useMutiplier"];
			}
			return result;
		}
	}

	public enum HelmType
	{
		notset = 0,
		normal = 1,
		bouncer = 2,
		snowhelm = 3,
		miami = 4,
		monster = 5,
		rome = 6,
		star = 7
	}

	public enum UnlockType
	{
		alwaysUnlocked = 0,
		free = 1,
		coins = 2,
		hiddenUntillUnlocked = 3,
		keys = 4
	}

	private static string path = "Text/Helm";

	public static Dictionary<HelmType, Helm> helmData;

	public static List<HelmType> helmOrder;

	public static bool Save()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (helmData == null)
		{
			return false;
		}
		foreach (KeyValuePair<HelmType, Helm> helmDatum in helmData)
		{
			dictionary.Add(helmDatum.Key.ToString(), Helm.ToJson(helmDatum.Value));
		}
		string value = Json.Serialize(dictionary);
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		using (StreamWriter streamWriter = File.CreateText(Application.dataPath + "/Resources/" + path + ".txt"))
		{
			streamWriter.WriteLine(value);
		}
		List<object> list = new List<object>();
		foreach (HelmType item in helmOrder)
		{
			list.Add(item);
		}
		string value2 = Json.Serialize(list);
		if (string.IsNullOrEmpty(value2))
		{
			return false;
		}
		using (StreamWriter streamWriter2 = File.CreateText(Application.dataPath + "/Resources/" + path + "Order.txt"))
		{
			streamWriter2.WriteLine(value2);
		}
		return true;
	}

	public static bool Load()
	{
		helmData = new Dictionary<HelmType, Helm>();
		TextAsset textAsset = Resources.Load<TextAsset>(path);
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		foreach (KeyValuePair<string, object> item2 in dictionary)
		{
			try
			{
				HelmType key = (HelmType)Enum.Parse(typeof(HelmType), item2.Key);
				Helm value = Helm.Parse((string)item2.Value);
				if (helmData.ContainsKey(key))
				{
					Debug.LogError(item2.Key + " has exist!");
					return false;
				}
				helmData.Add(key, value);
			}
			catch (ArgumentException)
			{
				Debug.LogError(item2.Key + " is not a membor of HelmType!");
				return false;
			}
		}
		helmOrder = new List<HelmType>(helmData.Count);
		textAsset = Resources.Load<TextAsset>(path + "Order");
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			return false;
		}
		IList<object> list = Json.Deserialize(textAsset.text) as IList<object>;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		foreach (object item3 in list)
		{
			try
			{
				HelmType item = (HelmType)Enum.Parse(typeof(HelmType), (string)item3);
				if (helmOrder.Contains(item))
				{
					Debug.LogError((string)item3 + " has exist!");
					return false;
				}
				helmOrder.Add(item);
			}
			catch (ArgumentException)
			{
				Debug.LogError((string)item3 + " is not a membor of HelmType!");
				return false;
			}
		}
		return true;
	}
}
