using System;
using System.Globalization;
using UnityEngine;

public static class Utils
{
	private static int randomNumber = -1;

	public static void Bar(string text, float ratio, int offset, Color color)
	{
		float num = 10f;
		float num2 = 20f;
		GUI.color = color;
		GUI.Button(new Rect(num, (float)Screen.height - num2 - num - (float)offset * num2, ((float)Screen.width - 2f * num) * ratio, num2), text);
	}

	public static int EncryptDecryptXORValue(int value)
	{
		return value ^ GetXorRandomValueForThisSession();
	}

	public static T FindComponentInParents<T>(this MonoBehaviour obj) where T : Component
	{
		return FindComponentInThisOrParents<T>(obj.transform.parent);
	}

	public static T FindComponentInThisOrParents<T>(Transform t) where T : Component
	{
		Transform transform = t;
		while (transform != null)
		{
			T component = t.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return (T)null;
	}

	public static T FindObject<T>() where T : class
	{
		T[] array = UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
		if (array == null || array.Length == 0)
		{
			Debug.LogWarning(string.Format("Could not find object of type {0}.", typeof(T).Name));
			return (T)null;
		}
		if (array.Length > 1)
		{
			Debug.LogWarning(string.Format("More than one instance found of type {0}.", typeof(T).Name));
		}
		return array[0];
	}

	public static T FindObject<T>(this MonoBehaviour obj) where T : class
	{
		T val = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
		if (val == null)
		{
			Debug.LogWarning(string.Format("Game object '{0}' could not find object of type {1}.", obj.gameObject.name, typeof(T).Name));
		}
		return val;
	}

	private static string GetIntBinaryString(int n)
	{
		char[] array = new char[32];
		int num = 31;
		for (int i = 0; i < 32; i++)
		{
			if ((n & (1 << i)) != 0)
			{
				array[num] = '1';
			}
			else
			{
				array[num] = '0';
			}
			num--;
		}
		return new string(array);
	}

	public static string GetLongName(Transform transform)
	{
		return (!(transform != null)) ? string.Empty : (GetLongName(transform.parent) + "/" + transform.name);
	}

	public static string GetLongNameList(Component[] components)
	{
		string[] array = new string[components.Length];
		for (int i = 0; i < components.Length; i++)
		{
			array[i] = GetLongName(components[i].transform);
		}
		return string.Join(", ", array);
	}

	private static int GetXorRandomValueForThisSession()
	{
		if (randomNumber == -1)
		{
			randomNumber = UnityEngine.Random.Range(1, 10000);
		}
		return randomNumber;
	}

	public static DateTime StringToDateTime(string value, DateTime defaultValue)
	{
		if (!string.IsNullOrEmpty(value))
		{
			DateTime result;
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
			{
				return result;
			}
			if (DateTime.TryParse(value, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static string toTitleCase(string value)
	{
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);
	}
}
