using UnityEngine;

public class Sample : Point
{
	[SerializeField]
	private AnimationClip idelClip;

	[SerializeField]
	private int frame;

	[SerializeField]
	private bool removeClipAtLast;

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
	}

	public override void OnInit(PointsManager manager)
	{
		if (idelClip != null)
		{
			manager.TargetAnim.AddClip(idelClip);
		}
	}

	public override void OnStart(PointsManager manager)
	{
		if (idelClip != null)
		{
			string clip = idelClip.name;
			AnimationState animationState = manager.TargetAnim[clip];
			animationState.enabled = true;
			animationState.speed = 0f;
			animationState.normalizedTime = (float)frame / (idelClip.frameRate * idelClip.length);
			manager.TargetAnim.anim.Sample();
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.GoToNextPoint();
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast && idelClip != null)
		{
			manager.TargetAnim.RemoveClip(idelClip);
		}
	}
}
