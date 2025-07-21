using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMode : StageMenuSequence
{
	public AnimationClip[] idelRummages;

	private AnimationClip selectClip;

	private void Start()
	{
		if (anim == null)
		{
			anim = Character.Instance.characterModel.characterAnimation;
		}
	}

	public override void StartPlayIdleRummagesAnimation()
	{
		Character.Instance.transform.position = base.transform.position;
		Character.Instance.transform.rotation = base.transform.rotation;
		int i = 0;
		for (int num = idelRummages.Length; i < num; i++)
		{
			if (anim[idelRummages[i].name] == null)
			{
				anim.AddClip(idelRummages[i], idelRummages[i].name);
			}
		}
		selectClip = null;
		base.StartPlayIdleRummagesAnimation();
	}

	protected override IEnumerator Play()
	{
		isPlay = true;
		List<AnimationClip> temps = new List<AnimationClip>();
		while (isPlay)
		{
			int i = 0;
			for (int num = idelRummages.Length; i < num; i++)
			{
				if (idelRummages[i] != selectClip)
				{
					temps.Add(idelRummages[i]);
				}
			}
			int random = Random.Range(0, temps.Count);
			selectClip = temps[random];
			anim.Play(selectClip.name);
			animationTime = selectClip.length;
			while (animationTime > 0f)
			{
				animationTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		isPlay = false;
		selectClip = null;
		animationRoutine = null;
	}

	public override void StopPlayIdleRummagesAnimation()
	{
		int i = 0;
		for (int num = idelRummages.Length; i < num; i++)
		{
			if (anim[idelRummages[i].name] != null)
			{
				anim.RemoveClip(idelRummages[i]);
			}
		}
		base.StopPlayIdleRummagesAnimation();
	}
}
