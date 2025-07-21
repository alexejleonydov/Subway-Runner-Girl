public class PlayerAvatarAnimation : Point
{
	private CharacterModel characterModel;

	private void Awake()
	{
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	public override void OnEnd(PointsManager manager)
	{
		if (characterModel == null)
		{
			characterModel = Character.Instance.characterModel;
		}
		characterModel.StopIdleAnimations();
	}

	public override void OnStart(PointsManager manager)
	{
		if (characterModel == null)
		{
			characterModel = Character.Instance.characterModel;
		}
		characterModel.StartIdleAnimations();
	}

	public override bool OnUpdate(PointsManager manager)
	{
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
	}
}
