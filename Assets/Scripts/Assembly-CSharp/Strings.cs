using System;
using UnityEngine;

public class Strings
{
	public enum DocumentFormat
	{
		TXT = 0,
		CSV = 1
	}

	private static string language;

	public static string LOCALE_FILES_FOLDER = "Text";

	public static string LOCALE_FILES_MANDATORY_ENDING = "_locale";

	public static string[] values;

	private static DocumentFormat _documentFormat;

	public static DocumentFormat documentFormat
	{
		get
		{
			return _documentFormat;
		}
		set
		{
			switch (value)
			{
			case DocumentFormat.TXT:
				LOCALE_FILES_FOLDER = "Text";
				LOCALE_FILES_MANDATORY_ENDING = "_locale";
				break;
			case DocumentFormat.CSV:
				LOCALE_FILES_FOLDER = "CSV";
				LOCALE_FILES_MANDATORY_ENDING = "_locale";
				break;
			}
			_documentFormat = value;
		}
	}

	public static string Language
	{
		get
		{
			return language;
		}
		set
		{
			if (_documentFormat == DocumentFormat.CSV)
			{
				LoadCSV(value);
			}
			else if (_documentFormat == DocumentFormat.TXT)
			{
				Load(value);
			}
		}
	}

	public static bool Exists(string keyString)
	{
		try
		{
			Get(keyString);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	public static string Get(string keyString)
	{
		return string.IsNullOrEmpty(keyString) ? null : Get((LanguageKey)(int)Enum.Parse(typeof(LanguageKey), keyString, true));
	}

	public static string Get(LanguageKey key)
	{
		if (language == null)
		{
			LogWarning("Strings not loaded. Loading default language. Tried to get string: " + key, null);
			Language = "english";
		}
		return values[(int)key];
	}

	private static void Load(string language)
	{
		if (!(Strings.language != language))
		{
			return;
		}
		Strings.language = language;
		if (values == null)
		{
			int[] array = (int[])Enum.GetValues(typeof(LanguageKey));
			values = new string[array[array.Length - 1] + 1];
		}
		else
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = null;
			}
		}
		TextAsset textAsset = (TextAsset)Resources.Load(LOCALE_FILES_FOLDER + "/" + language + LOCALE_FILES_MANDATORY_ENDING, typeof(TextAsset));
		string text = textAsset.text;
		int num = 0;
		string key;
		string value;
		while ((num = StringUtility.GetNextKeyValuePair(text, num, out key, out value)) >= 0)
		{
			int num2 = (int)Enum.Parse(typeof(LanguageKey), key, true);
			values[num2] = value;
			if (num == text.Length)
			{
				break;
			}
		}
		if (values[0] != null)
		{
			throw new Exception("Strings.Load: String set for " + Enum.GetName(typeof(LanguageKey), 0));
		}
		for (int j = 1; j < values.Length; j++)
		{
			if (values[j] == null && Enum.IsDefined(typeof(LanguageKey), j))
			{
				throw new Exception("Strings.Load: String not set for " + Enum.GetName(typeof(LanguageKey), j));
			}
		}
	}

	private static bool LoadCSV(string language)
	{
		if (Strings.language != language)
		{
			Strings.language = language;
			if (values == null)
			{
				int[] array = (int[])Enum.GetValues(typeof(LanguageKey));
				values = new string[array[array.Length - 1] + 1];
			}
			else
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = null;
				}
			}
			TextAsset textAsset = (TextAsset)Resources.Load(LOCALE_FILES_FOLDER + "/" + language + LOCALE_FILES_MANDATORY_ENDING, typeof(TextAsset));
			if (textAsset == null)
			{
				return false;
			}
			CSVReader cSVReader = new CSVReader(textAsset.bytes);
			while (true)
			{
				BetterList<string> betterList = cSVReader.ReadLine();
				if (betterList == null || betterList.size == 0)
				{
					break;
				}
				if (!string.IsNullOrEmpty(betterList[0]))
				{
					int num = (int)Enum.Parse(typeof(LanguageKey), betterList[0]);
					values[num] = betterList[1];
				}
			}
			return true;
		}
		return true;
	}

	public static void LogWarning(string msg, UnityEngine.Object context)
	{
		Debug.LogWarning(msg, context);
	}
}
