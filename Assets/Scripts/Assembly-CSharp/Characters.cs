using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class Characters
{
	public enum CharacterType
	{
		slick = 0,
		clown = 1,
		strong = 2,
		spike = 3,
		yutani = 4,
		frank = 5,
		lee = 6,
		turtlefok = 7,
		nijia = 8,
		darcy = 9,
		venice = 10,
		explorer = 11,
		princess = 12,
		none = 13
	}

	public class Model
	{
		public LanguageKey name;

		public string modelName;

		public int Price;

		public UnlockType unlockType;

		public int Level;

		public LanguageKey symbolName;

		public string symbolSprite2dName;

		public TaskTarget taskTargetKey;

		public string buttonBgSpriteName;

		public string buttonIconSpriteName;

		public int freeReviveCount;

		public int order;

		public Model()
		{
		}

		public Model(Model model)
		{
			name = model.name;
			modelName = model.modelName;
			Price = model.Price;
			unlockType = model.unlockType;
			Level = model.Level;
			symbolName = model.symbolName;
			symbolSprite2dName = model.symbolSprite2dName;
			taskTargetKey = model.taskTargetKey;
			buttonBgSpriteName = model.buttonBgSpriteName;
			buttonIconSpriteName = model.buttonIconSpriteName;
			freeReviveCount = model.freeReviveCount;
			order = model.order;
		}

		public static string ToJson(Model model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("name", model.name.ToString());
			dictionary.Add("modelName", model.modelName);
			dictionary.Add("Price", model.Price);
			dictionary.Add("unlockType", model.unlockType.ToString());
			dictionary.Add("Level", model.Level);
			dictionary.Add("symbolName", model.symbolName.ToString());
			dictionary.Add("symbolSprite2dName", model.symbolSprite2dName);
			dictionary.Add("taskTargetKey", model.taskTargetKey.ToString());
			dictionary.Add("buttonBgSpriteName", model.buttonBgSpriteName);
			dictionary.Add("buttonIconSpriteName", model.buttonIconSpriteName);
			dictionary.Add("freeReviveCount", model.freeReviveCount);
			dictionary.Add("order", model.order);
			return Json.Serialize(dictionary);
		}

		public static Model Parse(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			IDictionary<string, object> dictionary = Json.Deserialize(json) as IDictionary<string, object>;
			if (dictionary == null || dictionary.Count <= 0)
			{
				return null;
			}
			Model model = new Model();
			if (dictionary.ContainsKey("name"))
			{
				model.name = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["name"]);
			}
			if (dictionary.ContainsKey("modelName"))
			{
				model.modelName = (string)dictionary["modelName"];
			}
			if (dictionary.ContainsKey("Price"))
			{
				model.Price = (int)(long)dictionary["Price"];
			}
			if (dictionary.ContainsKey("unlockType"))
			{
				model.unlockType = (UnlockType)Enum.Parse(typeof(UnlockType), (string)dictionary["unlockType"]);
			}
			if (dictionary.ContainsKey("Level"))
			{
				model.Level = (int)(long)dictionary["Level"];
			}
			if (dictionary.ContainsKey("symbolName"))
			{
				model.symbolName = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["symbolName"]);
			}
			if (dictionary.ContainsKey("symbolSprite2dName"))
			{
				model.symbolSprite2dName = (string)dictionary["symbolSprite2dName"];
			}
			if (dictionary.ContainsKey("taskTargetKey"))
			{
				model.taskTargetKey = (TaskTarget)Enum.Parse(typeof(TaskTarget), (string)dictionary["taskTargetKey"]);
			}
			if (dictionary.ContainsKey("buttonBgSpriteName"))
			{
				model.buttonBgSpriteName = (string)dictionary["buttonBgSpriteName"];
			}
			if (dictionary.ContainsKey("buttonIconSpriteName"))
			{
				model.buttonIconSpriteName = (string)dictionary["buttonIconSpriteName"];
			}
			if (dictionary.ContainsKey("freeReviveCount"))
			{
				model.freeReviveCount = (int)(long)dictionary["freeReviveCount"];
			}
			if (dictionary.ContainsKey("order"))
			{
				model.order = (int)(long)dictionary["order"];
			}
			return model;
		}
	}

	public enum UnlockType
	{
		free = 0,
		symbols = 1,
		coins = 2,
		keys = 3,
		subscription = 4
	}

	public static Dictionary<CharacterType, Model> characterData;

	public static List<CharacterType> characterOrder;

	public static bool LoadFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Characters/data");

		//string characterDataText = "{\"slick\":\"{\\\"name\\\":\\\"CHARACTERS_ELLEN\\\",\\\"modelName\\\":\\\"slick\\\",\\\"Price\\\":0,\\\"unlockType\\\":\\\"free\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":\\\"\\\",\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_slick01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":2}\",\"frank\":\"{\\\"name\\\":\\\"CHARACTERS_FRANK\\\",\\\"modelName\\\":\\\"frank\\\",\\\"Price\\\":30,\\\"unlockType\\\":\\\"keys\\\",\\\"Level\\\":5,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_frank01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":4}\",\"nijia\":\"{\\\"name\\\":\\\"CHARACTERS_NINJA\\\",\\\"modelName\\\":\\\"nijia\\\",\\\"Price\\\":30000,\\\"unlockType\\\":\\\"coins\\\",\\\"Level\\\":8,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_nijia01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":5}\",\"lee\":\"{\\\"name\\\":\\\"CHARACTERS_LEE\\\",\\\"modelName\\\":\\\"lee\\\",\\\"Price\\\":50,\\\"unlockType\\\":\\\"symbols\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"UI_MYSTERY_BOX_LEE_S\\\",\\\"symbolSprite2dName\\\":\\\"Unlock_l\\\",\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_lee01\\\",\\\"freeReviveCount\\\":2,\\\"order\\\":1}\",\"spike\":\"{\\\"name\\\":\\\"CHARACTERS_KELVIN\\\",\\\"modelName\\\":\\\"spike\\\",\\\"Price\\\":10000,\\\"unlockType\\\":\\\"coins\\\",\\\"Level\\\":3,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_spike01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":3}\",\"yutani\":\"{\\\"name\\\":\\\"CHARACTERS_TAG\\\",\\\"modelName\\\":\\\"yutani\\\",\\\"Price\\\":50,\\\"unlockType\\\":\\\"keys\\\",\\\"Level\\\":10,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_yutani01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":6}\",\"turtlefok\":\"{\\\"name\\\":\\\"CHARACTERS_TURTLEFOK\\\",\\\"modelName\\\":\\\"turtlefok\\\",\\\"Price\\\":100,\\\"unlockType\\\":\\\"symbols\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"UI_MYSTERY_BOX_TURTLEFOK_S\\\",\\\"symbolSprite2dName\\\":\\\"Unlock_t\\\",\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_turtlefok01\\\",\\\"freeReviveCount\\\":2,\\\"order\\\":0}\",\"clown\":\"{\\\"name\\\":\\\"CHARACTERS_CLOWN\\\",\\\"modelName\\\":\\\"clown\\\",\\\"Price\\\":80000,\\\"unlockType\\\":\\\"coins\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_clown01\\\",\\\"freeReviveCount\\\":1,\\\"order\\\":7}\",\"strong\":\"{\\\"name\\\":\\\"CHARACTERS_STRONG\\\",\\\"modelName\\\":\\\"strong\\\",\\\"Price\\\":100000,\\\"unlockType\\\":\\\"subscription\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_strong01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":8}\",\"darcy\":\"{\\\"name\\\":\\\"CHARACTERS_DARCY\\\",\\\"modelName\\\":\\\"darcy\\\",\\\"Price\\\":120000,\\\"unlockType\\\":\\\"coins\\\",\\\"Level\\\":15,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_darcy01\\\",\\\"freeReviveCount\\\":0,\\\"order\\\":9}\",\"venice\":\"{\\\"name\\\":\\\"CHARACTERS_VENICE\\\",\\\"modelName\\\":\\\"venice\\\",\\\"Price\\\":300,\\\"unlockType\\\":\\\"keys\\\",\\\"Level\\\":0,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_venice01\\\",\\\"freeReviveCount\\\":1,\\\"order\\\":10}\",\"explorer\":\"{\\\"name\\\":\\\"CHARACTERS_EXPLORER\\\",\\\"modelName\\\":\\\"explorer\\\",\\\"Price\\\":200000,\\\"unlockType\\\":\\\"coins\\\",\\\"Level\\\":20,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_explorer01\\\",\\\"freeReviveCount\\\":1,\\\"order\\\":11}\",\"princess\":\"{\\\"name\\\":\\\"CHARACTERS_PRINCESS\\\",\\\"modelName\\\":\\\"princess\\\",\\\"Price\\\":188,\\\"unlockType\\\":\\\"keys\\\",\\\"Level\\\":25,\\\"symbolName\\\":\\\"NULL\\\",\\\"symbolSprite2dName\\\":null,\\\"taskTargetKey\\\":\\\"none\\\",\\\"buttonBgSpriteName\\\":\\\"AX_clothBg\\\",\\\"buttonIconSpriteName\\\":\\\"icon_darcy01\\\",\\\"freeReviveCount\\\":1,\\\"order\\\":12}\"}";

		//TextAsset textAsset;

		string characterDataText = PlayerPrefs.GetString("CharactersDataText", textAsset.text);


		if (textAsset == null || string.IsNullOrEmpty(characterDataText))
		{
			return false;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(characterDataText) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0)
		{
			return false;
		}
		characterData = new Dictionary<CharacterType, Model>();
		characterOrder = new List<CharacterType>(characterData.Count);
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			characterData.Add((CharacterType)Enum.Parse(typeof(CharacterType), item.Key), Model.Parse((string)item.Value));
			characterOrder.Add(CharacterType.slick);
		}
		foreach (KeyValuePair<CharacterType, Model> characterDatum in characterData)
		{
			characterOrder[characterDatum.Value.order] = characterDatum.Key;
		}
		return true;
	}

	public static bool SaveFile()
	{
		if (characterData == null || characterData.Count <= 0)
		{
			return false;
		}
		if (characterData.ContainsKey(CharacterType.none))
		{
			return false;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<CharacterType, Model> characterDatum in characterData)
		{
			dictionary.Add(characterDatum.Key.ToString(), Model.ToJson(characterDatum.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Characters/data.txt";
		/*using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}*/

		PlayerPrefs.SetString("CharactersDataText", value);

		return true;
	}
}
