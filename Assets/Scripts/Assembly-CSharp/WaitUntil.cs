public class WaitUntil : Point
{
	private UIScreenController uiscreen;

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		uiscreen = UIScreenController.Instance;
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if ("FrontUI".Equals(uiscreen.GetTopScreenName()) && uiscreen.IsPopupQueueEmpty())
		{
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}
}
