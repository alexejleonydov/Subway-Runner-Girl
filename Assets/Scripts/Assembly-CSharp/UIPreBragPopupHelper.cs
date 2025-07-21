public class UIPreBragPopupHelper : UIBaseScreen
{
	public UILabel description;

	private void BragClicked()
	{
		UIScreenController.Instance.QueuePopup("BragPopup");
		UIScreenController.Instance.ClosePopup(null);
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.ClosePopup(null);
	}

	public override void Init()
	{
		base.Init();
	}

	public override void Show()
	{
		base.Show();
	}
}
