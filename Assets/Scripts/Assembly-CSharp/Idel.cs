using UnityEngine;

public class Idel : Point
{
	[SerializeField]
	private AnimationClip idelClip;

	[SerializeField]
	private float minInterval;

	[SerializeField]
	private float maxInterval;

	[SerializeField]
	private bool removeClipAtLast;

	private float time;

	public override void OnInit(PointsManager manager)
	{
		if (idelClip != null)
		{
			manager.TargetAnim.AddClip(idelClip);
		}
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast && idelClip != null)
		{
			manager.TargetAnim.RemoveClip(idelClip);
		}
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		time = Random.Range(minInterval, maxInterval);
		if (idelClip != null)
		{
			manager.TargetAnim.Play(idelClip.name);
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (time > 0f)
		{
			time -= Time.deltaTime;
		}
		else
		{
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
	}
}
