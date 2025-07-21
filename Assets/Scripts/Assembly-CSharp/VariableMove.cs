using UnityEngine;

public class VariableMove : Point
{
	[SerializeField]
	protected AnimationClip animClip;

	[SerializeField]
	protected float animSpeed;

	[SerializeField]
	private AudioClipInfo audioClip;

	[SerializeField]
	protected float initialSpeed;

	[SerializeField]
	protected float acc;

	[SerializeField]
	protected float maxSpeed;

	[SerializeField]
	private bool removeClipAtLast;

	private float speed;

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

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		speed = initialSpeed;
		if (animClip != null)
		{
			manager.TargetAnim.SetSpeed(animClip.name, animSpeed);
			manager.TargetAnim.Play(animClip.name);
		}
		manager.PlaySound(audioClip, true);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.RotatoToTarget();
		speed += acc * Time.deltaTime;
		if (Mathf.Abs(speed - initialSpeed) > Mathf.Abs(maxSpeed - initialSpeed))
		{
			speed = maxSpeed;
		}
		manager.TargetAnim.SetSpeed(animClip.name, animSpeed * speed / initialSpeed);
		manager.Move(speed * Time.deltaTime);
		manager.Check();
		return true;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformToTarget();
		manager.StopSound();
	}
}
