using UnityEngine;

public class IdelLoopRandomAnimation : Point
{
	[SerializeField]
	private AnimationClip[] randomClip;

	[SerializeField]
	private bool removeClipAtLast;

	private string[] randomClipName;

	private string clip;

	private float time;

	private void Awake()
	{
		randomClipName = new string[randomClip.Length];
		int i = 0;
		for (int num = randomClip.Length; i < num; i++)
		{
			randomClipName[i] = randomClip[i].name;
		}
	}

	public override void OnInit(PointsManager manager)
	{
		int i = 0;
		for (int num = randomClip.Length; i < num; i++)
		{
			manager.TargetAnim.AddClip(randomClip[i]);
		}
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast)
		{
			int i = 0;
			for (int num = randomClip.Length; i < num; i++)
			{
				manager.TargetAnim.RemoveClip(randomClip[i]);
			}
		}
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		CrossFadeRandomClip(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (time > 0f)
		{
			time -= Time.deltaTime;
		}
		else
		{
			CrossFadeRandomClip(manager);
		}
		return false;
	}

	private void CrossFadeRandomClip(PointsManager manager)
	{
		int num = Random.Range(0, randomClipName.Length);
		if (randomClipName[num].Equals(clip))
		{
			num = (num + 1) % randomClipName.Length;
		}
		clip = randomClipName[num];
		manager.TargetAnim.Play(clip);
		time = manager.TargetAnim[clip].length;
	}

	public override void OnImility(PointsManager manager)
	{
	}
}
