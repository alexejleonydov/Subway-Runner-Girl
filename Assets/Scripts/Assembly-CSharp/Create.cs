using UnityEngine;

public class Create : Point
{
	[SerializeField]
	private float minInterval;

	[SerializeField]
	private float maxInterval;

	private float time;

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
		Initialized(manager);
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		time = Random.Range(minInterval, maxInterval);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (time > 0f)
		{
			time -= Time.deltaTime;
		}
		else
		{
			Initialized(manager);
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	private void Initialized(PointsManager manager)
	{
		manager.InitializedModel(Vector3.zero, new Vector3(0f, 180f, 0f));
	}
}
