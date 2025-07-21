using UnityEngine;

public class RedeemPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UISprite keyIcon;

	[SerializeField]
	private UILabel coinNum;

	[SerializeField]
	private UILabel gemNum;

	private int coinAmount;

	private int gemAmount;

	public override void Show()
	{
		base.Show();
		RefreshLabel();
		coinAmount = 0;
		gemAmount = 0;
		RecodeManager.Good good = null;
		int i = 0;
		for (int num = RecodeManager.Instance.GetRecodes().Length; i < num; i++)
		{
			good = RecodeManager.Instance.GetRecodes()[i];
			int result;
			if (int.TryParse(good._num, out result))
			{
				switch (good._id)
				{
				case "1":
					coinAmount += result;
					break;
				case "2":
					gemAmount += result;
					break;
				}
			}
		}
		coinNum.text = "X" + coinAmount;
		gemNum.text = "X" + gemAmount;
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_REDEEM_TITLE);
		getLbl.text = Strings.Get(LanguageKey.UI_POPUP_REDEEM_BUTTON_GET);
	}

	public void GetRecodeReward()
	{
		RecodeManager.Instance.GetRecodeGoods();
		UIScreenController.Instance.ClosePopup(null);
	}
}
