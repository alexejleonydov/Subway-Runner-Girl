using UnityEngine;

public class MoveAnimation : Point
{
	[SerializeField]
	private AnimationClip clip;

	[SerializeField]
	private float speed;

	[SerializeField]
	private bool animationDrive;

	[SerializeField]
	private bool front;

	[SerializeField]
	private float frame;

	[SerializeField]
	private bool removeClipAtLast;

	[SerializeField]
	private bool rotate;

	private float moveSpeed;

	private float moveTime;

	private float time;

	public override void OnInit(PointsManager manager)
	{
		manager.TargetAnim.AddClip(clip);
		move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast)
		{
			manager.TargetAnim.RemoveClip(clip);
		}
	}

	public override void OnStart(PointsManager manager)
	{
		manager.TargetAnim[clip.name].speed = speed;
		manager.TargetAnim.Play(clip.name);
		time = manager.TargetAnim[clip.name].length / speed;
		if (front)
		{
			moveTime = time - frame / clip.frameRate / speed;
			moveSpeed = manager.GetDistanceToTarget() / moveTime;
		}
		else
		{
			moveTime = frame / clip.frameRate / speed;
			moveSpeed = manager.GetDistanceToTarget() / moveTime;
		}
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (time > 0f)
		{
			time -= Time.deltaTime;
			if (animationDrive)
			{
				return false;
			}
			if ((front && time < moveTime) || (!front && time > moveTime))
			{
				manager.RotatoToPoint();
				manager.Move(moveSpeed * Time.deltaTime);
			}
		}
		else
		{
			manager.GoToNextPoint();
		}
		return true;
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
	}
}
