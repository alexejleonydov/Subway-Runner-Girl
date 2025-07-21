using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Condition/AllKeepsakeCollected")]
public class AllKeepsakeCollected : ICondition
{
	public string city;

	public override bool IfMeet()
	{
		return false;
	}
}
