using UnityEngine;

public class TryCharacterPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel descripeLbl;

	[SerializeField]
	private UILabel freeLbl;

	public override void Show()
	{
		base.Show();
		RefreshLable();
	}

	private void RefreshLable()
	{
	}

	public void OnFreeViewClick()
	{
	}
}
