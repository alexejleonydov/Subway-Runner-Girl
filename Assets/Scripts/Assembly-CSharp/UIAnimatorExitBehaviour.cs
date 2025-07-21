using System;
using UnityEngine;

public class UIAnimatorExitBehaviour : StateMachineBehaviour
{
	public Action onStateUpdate;

	public Action onStateExit;

	public float normalizedTime;

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

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		if (onStateExit != null)
		{
			onStateExit();
		}
	}
}
