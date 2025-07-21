using System.Collections;
using UnityEngine;

public class OrderMode : StageMenuSequence
{
	public AnimationClip[] orders;

	private int selectIndex;

	public override void StartPlayIdleRummagesAnimation()
	{
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			if (anim[orders[i].name] == null)
			{
				anim.AddClip(orders[i], orders[i].name);
			}
		}
		selectIndex = -1;
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		isPlay = true;
		while (isPlay)
		{
			selectIndex = Mathf.Clamp(selectIndex + 1, 0, orders.Length);
			AnimationClip clip = orders[selectIndex];
			anim.Play(clip.name);
			animationTime = clip.length;
			while (animationTime > 0f)
			{
				animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		isPlay = false;
		selectIndex = -1;
		animationRoutine = null;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			if (anim[orders[i].name] != null)
			{
				anim.RemoveClip(orders[i]);
			}
		}
		base.StopPlayIdleRummagesAnimation();
	}
}
