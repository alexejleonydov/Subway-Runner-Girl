using UnityEngine;

public class Move : Point
{
	[SerializeField]
	protected AnimationClip animClip;

	[SerializeField]
	protected float animSpeed;

	[SerializeField]
	protected float moveSpeed;

	[SerializeField]
	private bool removeClipAtLast;

	[SerializeField]
	private AudioClipInfo audioClip;

	private bool onStop;

	public override void OnInit(PointsManager manager)
	{
		if (animClip != null)
		{
			manager.TargetAnim.AddClip(animClip);
		}
		move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast && animClip != null)
		{
			manager.TargetAnim.RemoveClip(animClip);
		}
	}

	public override void OnStart(PointsManager manager)
	{
		if (animClip != null)
		{
			manager.TargetAnim.SetSpeed(animClip.name, animSpeed);
			manager.TargetAnim.Play(animClip.name);
		}
		manager.PlaySound(audioClip, false);
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.StopSound();
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.RotatoToTarget();
		manager.Move(moveSpeed * (float)((!onStop) ? 1 : 2) * Time.deltaTime);
		manager.Check();
		return true;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
		manager.StopSound();
	}
}
