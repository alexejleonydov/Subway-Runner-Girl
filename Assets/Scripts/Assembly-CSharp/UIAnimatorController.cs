using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatorController : MonoBehaviour
{
	[SerializeField]
	private Animator self;

	[SerializeField]
	private List<Animator> animators = new List<Animator>();

	[SerializeField]
	private bool limit;

	[SerializeField]
	private int limitCount = 5;

	[SerializeField]
	private bool immediately;

	[SerializeField]
	[Range(0f, 1f)]
	private float interval = 0.5f;

	public void AddAnimator(Animator anim)
	{
		if (!(anim == null) && !animators.Contains(anim))
		{
			animators.Add(anim);
		}
	}

	public void OrderAnimators(int[] orders)
	{
		if (animators == null || animators.Count <= 0)
		{
			return;
		}
		Animator animator = null;
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			if (i != orders[i])
			{
				animator = animators[i];
				animators[i] = animators[orders[i]];
				animators[orders[i]] = animator;
			}
		}
	}

	public void WaitForCancel()
	{
		if (animators != null && animators.Count > 0)
		{
			int num = animators.Count;
			if (limit && immediately)
			{
				num = ((num <= limitCount) ? num : limitCount);
			}
			for (int i = 0; i < animators.Count; i++)
			{
				animators[i].enabled = false;
				animators[i].Update((i >= num) ? 1 : 0);
			}
		}
	}

	public void OnEnter(Action EnterOnEnter, Action EnterOnExit, Action ExitOnExit)
	{
		WaitForCancel();
		if (!(self != null))
		{
			return;
		}
		self.enabled = true;
		UIAnimatorEnterBehaviour behaviour = self.GetBehaviour<UIAnimatorEnterBehaviour>();
		behaviour.onStateUpdate = delegate
		{
			if (EnterOnEnter != null)
			{
				EnterOnEnter();
			}
			PlayAnimators();
		};
		UIAnimatorExitBehaviour behaviour2 = self.GetBehaviour<UIAnimatorExitBehaviour>();
		behaviour2.onStateUpdate = EnterOnExit;
		behaviour2.onStateExit = ExitOnExit;
	}

	private void PlayAnimators()
	{
		if (animators != null && animators.Count > 0)
		{
			if (immediately)
			{
				PlayAnimatorsImmediately();
			}
			else
			{
				PlayAnimatorsInterval();
			}
		}
	}

	private void PlayAnimatorsImmediately()
	{
		int num = animators.Count;
		if (limit)
		{
			num = ((num <= limitCount) ? num : limitCount);
		}
		for (int i = 0; i < animators.Count; i++)
		{
			animators[i].enabled = true;
		}
		for (int j = num; j < animators.Count; j++)
		{
			animators[j].Update(1f);
		}
	}

	private void PlayAnimatorsInterval()
	{
		StartCoroutine(PlayAnimatorsInterval_C());
	}

	private IEnumerator PlayAnimatorsInterval_C()
	{
		int index = 0;
		int max = animators.Count;
		if (limit)
		{
			max = ((max <= limitCount) ? max : limitCount);
		}
		float wait2 = interval;
		while (index < max)
		{
			animators[index].enabled = true;
			wait2 = animators[index].GetCurrentAnimatorStateInfo(0).length * interval;
			index++;
			yield return new WaitForSeconds(wait2);
		}
		for (int i = max; i < animators.Count; i++)
		{
			animators[i].enabled = true;
		}
	}

	public bool OnExit()
	{
		if (self != null)
		{
			self.SetTrigger("trigger");
			return true;
		}
		return false;
	}
}
