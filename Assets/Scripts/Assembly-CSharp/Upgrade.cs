using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;
using System.Globalization;
public class Upgrade
{
	public LanguageKey name;

	public LanguageKey nameTwoLines;

	public LanguageKey description;

	public LanguageKey mysteryBoxDescription;

	public int numberOfTiers;

	public float[] durations;

	public float speed;

	public float landSpeed;

	public int spawnProbability;

	public int minimumMeters;

	public int coinmagnetRange;

	public int[] pricesRaw;

	public int levelPriceMultiplyer;

	public string iconName;

	public int weight;

	public Upgrade()
	{
	}

	public Upgrade(Upgrade upgrade)
	{
		name = upgrade.name;
		nameTwoLines = upgrade.nameTwoLines;
		description = upgrade.description;
		mysteryBoxDescription = upgrade.mysteryBoxDescription;
		numberOfTiers = upgrade.numberOfTiers;
		durations = null;
		speed = upgrade.speed;
		landSpeed = upgrade.landSpeed;
		spawnProbability = upgrade.spawnProbability;
		minimumMeters = upgrade.minimumMeters;
		coinmagnetRange = upgrade.coinmagnetRange;
		pricesRaw = null;
		levelPriceMultiplyer = upgrade.levelPriceMultiplyer;
		iconName = upgrade.iconName;
		weight = upgrade.weight;
	}

	public LanguageKey GetName()
	{
		if (!string.IsNullOrEmpty(Strings.Get(nameTwoLines)))
		{
			return nameTwoLines;
		}
		return name;
	}

	public int getPrice(int tier)
	{
		if (pricesRaw == null || tier >= pricesRaw.Length)
		{
			return -1;
		}
		return pricesRaw[tier] + levelPriceMultiplyer * Mathf.Clamp(TasksManager.Instance.currentTaskSet, 0, TasksManager.Instance.taskSetStoryCount);
	}

	public static Upgrade Parse(string json)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		Upgrade upgrade = new Upgrade();
		if (dictionary == null)
		{
			return upgrade;
		}
		if (dictionary.ContainsKey("name"))
		{
			upgrade.name = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["name"]);
		}
		if (dictionary.ContainsKey("nameTwoLines"))
		{
			upgrade.nameTwoLines = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["nameTwoLines"]);
		}
		if (dictionary.ContainsKey("description"))
		{
			upgrade.description = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["description"]);
		}
		if (dictionary.ContainsKey("mysteryBoxDescription"))
		{
			upgrade.mysteryBoxDescription = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["mysteryBoxDescription"]);
		}
		if (dictionary.ContainsKey("numberOfTiers"))
		{
			upgrade.numberOfTiers = (int)(long)dictionary["numberOfTiers"];
		}
		if (dictionary.ContainsKey("durations"))
		{
			string text = (string)dictionary["durations"];
			if (string.IsNullOrEmpty(text))
			{
				upgrade.durations = null;
			}
			else
			{
				string[] array = text.Split('-');
				upgrade.durations = new float[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					upgrade.durations[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
				}
			}
		}
		if (dictionary.ContainsKey("speed"))
		{
			upgrade.speed = float.Parse((string)dictionary["speed"], CultureInfo.InvariantCulture);
		}
		if (dictionary.ContainsKey("landSpeed"))
		{
			upgrade.landSpeed = float.Parse((string)dictionary["landSpeed"], CultureInfo.InvariantCulture);
		}
		if (dictionary.ContainsKey("spawnProbability"))
		{
			upgrade.spawnProbability = (int)(long)dictionary["spawnProbability"];
		}
		if (dictionary.ContainsKey("minimumMeters"))
		{
			upgrade.minimumMeters = (int)(long)dictionary["minimumMeters"];
		}
		if (dictionary.ContainsKey("coinmagnetRange"))
		{
			upgrade.coinmagnetRange = (int)(long)dictionary["coinmagnetRange"];
		}
		if (dictionary.ContainsKey("pricesRaw"))
		{
			string text2 = (string)dictionary["pricesRaw"];
			if (string.IsNullOrEmpty(text2))
			{
				upgrade.pricesRaw = null;
			}
			else
			{
				string[] array2 = text2.Split('-');
				upgrade.pricesRaw = new int[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					upgrade.pricesRaw[j] = int.Parse(array2[j]);
				}
			}
		}
		if (dictionary.ContainsKey("levelPriceMultiplyer"))
		{
			upgrade.levelPriceMultiplyer = (int)(long)dictionary["levelPriceMultiplyer"];
		}
		if (dictionary.ContainsKey("iconName"))
		{
			upgrade.iconName = (string)dictionary["iconName"];
		}
		if (dictionary.ContainsKey("weight"))
		{
			upgrade.weight = (int)(long)dictionary["weight"];
		}
		return upgrade;
	}

	public static string ToJson(Upgrade upgrade)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("name", upgrade.name.ToString());
		dictionary.Add("nameTwoLines", upgrade.nameTwoLines.ToString());
		dictionary.Add("description", upgrade.description.ToString());
		dictionary.Add("mysteryBoxDescription", upgrade.mysteryBoxDescription.ToString());
		dictionary.Add("numberOfTiers", upgrade.numberOfTiers);
		string text = string.Empty;
		if (upgrade.durations != null && upgrade.durations.Length > 0)
		{
			for (int i = 0; i < upgrade.durations.Length; i++)
			{
				text = text + upgrade.durations[i] + ((i != upgrade.durations.Length - 1) ? "-" : string.Empty);
			}
		}
		dictionary.Add("durations", text);
		dictionary.Add("speed", upgrade.speed.ToString());
		dictionary.Add("landSpeed", upgrade.landSpeed.ToString());
		dictionary.Add("spawnProbability", upgrade.spawnProbability);
		dictionary.Add("minimumMeters", upgrade.minimumMeters);
		dictionary.Add("coinmagnetRange", upgrade.coinmagnetRange);
		text = string.Empty;
		if (upgrade.pricesRaw != null && upgrade.pricesRaw.Length > 0)
		{
			for (int j = 0; j < upgrade.pricesRaw.Length; j++)
			{
				text = text + upgrade.pricesRaw[j] + ((j != upgrade.pricesRaw.Length - 1) ? "-" : string.Empty);
			}
		}
		dictionary.Add("pricesRaw", text);
		dictionary.Add("levelPriceMultiplyer", upgrade.levelPriceMultiplyer);
		dictionary.Add("iconName", upgrade.iconName);
		dictionary.Add("weight", upgrade.weight);
		return Json.Serialize(dictionary);
	}
}
