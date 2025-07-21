using UnityEngine;

public class AnimationsActivator : BaseO
{
	private Animation[] animations;

	protected override void Awake()
	{
		animations = GetComponentsInChildren<Animation>();
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		int i = 0;
		for (int num = animations.Length; i < num; i++)
		{
			if (animations[i] != null)
			{
				animations[i].enabled = true;
				animations[i].Play();
			}
		}
	}

	public override void OnDeactivate()
	{
		int i = 0;
		for (int num = animations.Length; i < num; i++)
		{
			if (animations[i] != null)
			{
				animations[i].Stop();
				animations[i].enabled = false;
			}
		}
	}
}
