using System;
using System.Collections.Generic;
using System.IO;
using MiniJSONs;
using UnityEngine;

public class TasksData
{
	public static Dictionary<TaskType, TaskTemplate> taskTemplates;

	public static Task[][] repeatableTasks;

	public static Task[][] singleuseTasks;

	public static void LoadFiles()
	{
		LoadRepeatableTasks();
		LoadStorylineTasks();
		LoadTaskTemplates();
	}

	private static void SaveDyadicArrayFile(Task[][] taskss, string filename)
	{
		List<object> list = new List<object>();
		List<object> list2 = new List<object>();
		foreach (Task[] array in taskss)
		{
			list2.Add(Task.ToJson(array[0]));
			list2.Add(Task.ToJson(array[1]));
			list2.Add(Task.ToJson(array[2]));
			list.Add(Json.Serialize(list2));
			list2.Clear();
		}
		string value = Json.Serialize(list);
		string path = Application.dataPath + "/Resources/Text/Task" + Path.DirectorySeparatorChar + filename;
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	public static void SaveRepeatableTasks()
	{
		SaveDyadicArrayFile(repeatableTasks, "repeatableTasks.txt");
	}

	public static void SaveStorylineTasks()
	{
		SaveDyadicArrayFile(singleuseTasks, "storylineTasks.txt");
	}

	public static void SaveTaskTemplates()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<TaskType, TaskTemplate> taskTemplate in taskTemplates)
		{
			dictionary.Add(taskTemplate.Key.ToString(), TaskTemplate.ToJson(taskTemplate.Value));
		}
		string value = Json.Serialize(dictionary);
		string path = Application.dataPath + "/Resources/Text/Task/taskTemplates.txt";
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			streamWriter.WriteLine(value);
			streamWriter.Close();
		}
	}

	private static void LoadDyadicArrayFile(out Task[][] taskss, string path)
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/" + path);
		if (textAsset == null)
		{
			taskss = new Task[0][];
			Debug.LogError("The file is not exist.");
			return;
		}
		List<object> list = Json.Deserialize(textAsset.text) as List<object>;
		taskss = new Task[list.Count][];
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			List<object> list2 = Json.Deserialize((string)list[i]) as List<object>;
			taskss[i] = new Task[3]
			{
				Task.Parse((string)list2[0]),
				Task.Parse((string)list2[1]),
				Task.Parse((string)list2[2])
			};
		}
	}

	public static void LoadRepeatableTasks()
	{
		LoadDyadicArrayFile(out repeatableTasks, "repeatableTasks");
	}

	public static void LoadStorylineTasks()
	{
		LoadDyadicArrayFile(out singleuseTasks, "storylineTasks");
	}

	public static void LoadTaskTemplates()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Text/Task/taskTemplates");
		if (textAsset == null)
		{
			Debug.LogError("The file is not exist.");
			return;
		}
		IDictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as IDictionary<string, object>;
		taskTemplates = new Dictionary<TaskType, TaskTemplate>();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			taskTemplates.Add((TaskType)Enum.Parse(typeof(TaskType), item.Key), TaskTemplate.Parse((string)item.Value));
		}
	}
}
