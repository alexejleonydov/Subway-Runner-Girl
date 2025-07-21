using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Condition/ScoreMultiplier")]
public class ScoreMultiplier : ICondition
{
	public int target;

	public override bool IfMeet()
	{
		return false;
	}
}
