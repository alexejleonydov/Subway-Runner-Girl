using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Condition/Keepsake")]
public class Keepsake : ScriptableObject
{
	public string sakeName;

	public string iconName;

	public string city;

	public ICondition condition;

	public string tip;

	public bool IfMeet()
	{
		return condition.IfMeet();
	}
}
