using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Condition/RunLength")]
public class RunLength : ICondition
{
	public int target;

	public override bool IfMeet()
	{
		return false;
	}
}
