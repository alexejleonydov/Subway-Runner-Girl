using UnityEngine;

public class MoveRandomAnimation : Point
{
	[SerializeField]
	private AnimationClip[] clipNames;

	[SerializeField]
	private bool removeClipAtLast;

	private Vector3 initPos;

	private AnimationClip clip;

	private float time;

	public override void OnInit(PointsManager manager)
	{
		int i = 0;
		for (int num = clipNames.Length; i < num; i++)
		{
			manager.TargetAnim.AddClip(clipNames[i]);
		}
		move = true;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
		if (removeClipAtLast)
		{
			int i = 0;
			for (int num = clipNames.Length; i < num; i++)
			{
				manager.TargetAnim.RemoveClip(clipNames[i]);
			}
		}
	}

	public override void OnStart(PointsManager manager)
	{
		initPos = manager.transform.position;
		CrossFadeRandomClip(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.SetPositionAccordingRefrence(initPos);
		manager.RotatoToPoint();
		if (time > 0f)
		{
			time -= Time.deltaTime;
		}
		else
		{
			manager.GoToNextPoint();
		}
		return true;
	}

	public override void OnEnd(PointsManager manager)
	{
		manager.Refrence.localPosition = Vector3.zero;
	}

	private void CrossFadeRandomClip(PointsManager manager)
	{
		int num = Random.Range(0, clipNames.Length);
		clip = clipNames[num];
		manager.TargetAnim.CrossFade(clip.name, 0.2f);
		time = manager.TargetAnim[clip.name].length;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.Refrence.localPosition = Vector3.zero;
	}
}
