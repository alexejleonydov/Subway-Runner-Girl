using System.Collections.Generic;
using UnityEngine;

public class DebugShow : MonoBehaviour
{
	private static List<string> messages = new List<string>();

	private static bool willShowDebugInfoByGUI;

	[SerializeField]
	private Rect startRect;

	[SerializeField]
	private Rect offset;

	private Rect nextRect;

	public static void SetWillShowDebugInfoByGUI(bool value)
	{
		willShowDebugInfoByGUI = value;
	}

	public static void Log(string info)
	{
		if (willShowDebugInfoByGUI)
		{
			messages.Add(info);
		}
	}

	private void OnGUI()
	{
		if (!willShowDebugInfoByGUI)
		{
			return;
		}
		GUI.color = Color.red;
		GUI.Label(startRect, "The Messages Below:");
		nextRect = startRect;
		foreach (string message in messages)
		{
			nextRect = new Rect(nextRect.xMin + offset.xMin, nextRect.yMax + offset.yMax, nextRect.width + offset.width, nextRect.height + offset.height);
			GUI.Label(nextRect, "Log:" + message);
		}
		nextRect = new Rect(100f, Screen.height - 100, 100f, 100f);
		if (GUI.Button(nextRect, "Clear"))
		{
			messages.Clear();
		}
	}
}
