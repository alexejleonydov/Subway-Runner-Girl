using System;
using System.Collections.Generic;
using MiniJSONs;
using UnityEngine;

public class TaskTemplate
{
	public LanguageKey description;

	public LanguageKey descriptionSingle;

	public LanguageKey ultraShortDescription;

	public LanguageKey ultraShortDescriptionSingle;

	public TaskTarget taskTarget;

	public bool singleRun;

	public bool completeIfLess;

	public bool completeIfEqual;

	public static string ToJson(TaskTemplate template)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("description", template.description.ToString());
		dictionary.Add("descriptionSingle", template.descriptionSingle.ToString());
		dictionary.Add("ultraShortDescription", template.ultraShortDescription.ToString());
		dictionary.Add("ultraShortDescriptionSingle", template.ultraShortDescriptionSingle.ToString());
		dictionary.Add("taskTarget", template.taskTarget.ToString());
		dictionary.Add("singleRun", template.singleRun);
		dictionary.Add("completeIfLess", template.completeIfLess);
		dictionary.Add("completeIfEqual", template.completeIfEqual);
		return Json.Serialize(dictionary);
	}

	public static TaskTemplate Parse(string json)
	{
		TaskTemplate taskTemplate = new TaskTemplate();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary.ContainsKey("description"))
		{
			taskTemplate.description = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["description"]);
		}
		if (dictionary.ContainsKey("descriptionSingle"))
		{
			taskTemplate.descriptionSingle = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["descriptionSingle"]);
		}
		if (dictionary.ContainsKey("ultraShortDescription"))
		{
			taskTemplate.ultraShortDescription = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["ultraShortDescription"]);
		}
		if (dictionary.ContainsKey("ultraShortDescriptionSingle"))
		{
			taskTemplate.ultraShortDescriptionSingle = (LanguageKey)Enum.Parse(typeof(LanguageKey), (string)dictionary["ultraShortDescriptionSingle"]);
		}
		if (dictionary.ContainsKey("taskTarget"))
		{
			taskTemplate.taskTarget = (TaskTarget)Enum.Parse(typeof(TaskTarget), (string)dictionary["taskTarget"]);
		}
		if (dictionary.ContainsKey("singleRun"))
		{
			taskTemplate.singleRun = (bool)dictionary["singleRun"];
		}
		if (dictionary.ContainsKey("completeIfLess"))
		{
			taskTemplate.completeIfLess = (bool)dictionary["completeIfLess"];
		}
		if (dictionary.ContainsKey("completeIfEqual"))
		{
			taskTemplate.completeIfEqual = (bool)dictionary["completeIfEqual"];
		}
		return taskTemplate;
	}
}
