using System;
using UnityEngine;

public class UIAnimatorEnterBehaviour : StateMachineBehaviour
{
	public Action onStateUpdate;

	public float normalizedTime = 0.6f;

	private bool update;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (!update && stateInfo.normalizedTime >= normalizedTime)
		{
			if (onStateUpdate != null)
			{
				onStateUpdate();
			}
			onStateUpdate = null;
			update = true;
		}
	}
}
