using System;
using UnityEngine;

public class TriggerO : BaseO
{
	[SerializeField]
	protected OnTriggerObject onTrigger;

	[SerializeField]
	protected Animation anim;

	[SerializeField]
	protected string idleClip;

	[SerializeField]
	protected float interval;

	protected override void Awake()
	{
		if (onTrigger != null)
		{
			onTrigger.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(onTrigger.OnEnter, new OnTriggerObject.OnEnterDelegate(TriggerOnEnter));
		}
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		if (onTrigger != null)
		{
			float num = Game.Instance.currentSpeed;
			if (num <= 0f)
			{
				num = Game.Instance.speed.min;
			}
			onTrigger.transform.localPosition = new Vector3(0f, 0f, (0f - num) * interval);
			if (!string.IsNullOrEmpty(idleClip) && anim[idleClip] != null)
			{
				anim.enabled = true;
				anim.Play(idleClip);
			}
		}
	}

	public override void OnDeactivate()
	{
		if (onTrigger != null)
		{
			anim.Stop();
			anim.enabled = false;
		}
	}

	public virtual void TriggerOnEnter(Collider collider)
	{
	}
}
