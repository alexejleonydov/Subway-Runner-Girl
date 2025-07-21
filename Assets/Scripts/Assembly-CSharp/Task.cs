using System;
using System.Collections.Generic;
using MiniJSONs;

public class Task
{
	public TaskType type;

	public int aim;

	public Task()
	{
	}

	public Task(TaskType type, int aim)
	{
		this.type = type;
		this.aim = aim;
	}

	public static string ToJson(Task task)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("type", task.type.ToString());
		dictionary.Add("goal", task.aim);
		return Json.Serialize(dictionary);
	}

	public static Task Parse(string json)
	{
		Task task = new Task();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary.ContainsKey("type"))
		{
			task.type = (TaskType)Enum.Parse(typeof(TaskType), (string)dictionary["type"]);
		}
		if (dictionary.ContainsKey("goal"))
		{
			task.aim = (int)(long)dictionary["goal"];
		}
		return task;
	}
}
