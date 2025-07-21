using System.Collections;
using UnityEngine;

public class LoopMode : StageMenuSequence
{
	public AnimationClip loopClip;

	public override void StartPlayIdleRummagesAnimation()
	{
		if (anim[loopClip.name] == null)
		{
			anim.AddClip(loopClip, loopClip.name);
		}
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		isPlay = true;
		while (isPlay)
		{
			anim.Play(loopClip.name);
			animationTime = loopClip.length;
			while (animationTime > 0f)
			{
				animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		isPlay = false;
		animationRoutine = null;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		if (anim[loopClip.name] != null)
		{
			anim.RemoveClip(loopClip);
		}
		base.StopPlayIdleRummagesAnimation();
	}
}
