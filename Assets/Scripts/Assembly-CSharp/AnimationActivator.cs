using UnityEngine;

public class AnimationActivator : BaseO
{
	private Animation anim;

	protected override void Awake()
	{
		anim = GetComponent<Animation>();
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		anim.enabled = true;
		anim.Play();
	}

	public override void OnDeactivate()
	{
		anim.Stop();
		anim.enabled = false;
	}
}
