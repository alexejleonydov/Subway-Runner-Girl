using UnityEngine;

public class ShopBackgroundScreen : UIBaseScreen
{
	[SerializeField]
	private UITexture background1;

	[SerializeField]
	private UITexture background2;

	public static bool one = true;

	public override void Hide()
	{
		HideBackground();
		base.Hide();
	}

	public void HideBackground()
	{
		background1.enabled = false;
		background2.enabled = false;
	}

	public override void Show()
	{
		base.Show();
		ShowBackground();
	}

	public void ShowBackground()
	{
		background1.enabled = one;
		background2.enabled = !one;
	}
}
