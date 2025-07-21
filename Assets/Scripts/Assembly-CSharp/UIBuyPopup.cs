using UnityEngine;

public class UIBuyPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UISprite iconSpr;

	[SerializeField]
	private UILabel amountLbl;

	[SerializeField]
	private UISprite buyIconSpr;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel payLbl;

	private ShopPopupData _popupData;

	public override void Show()
	{
		base.Show();
		_popupData = ShopManager.Instance.GetShopPopupData();
		if (_popupData != null)
		{
			titleLbl.text = Strings.Get(_popupData.title);
			amountLbl.text = _popupData.num.ToString();
			priceLbl.text = _popupData.price;
			payLbl.text = _popupData.price;
			iconSpr.spriteName = _popupData.icon;
			iconSpr.MakePixelPerfect();
			switch (_popupData.unlockType)
			{
			case 0:
				buyIconSpr.enabled = false;
				priceLbl.enabled = false;
				payLbl.enabled = true;
				break;
			case 1:
				buyIconSpr.enabled = true;
				buyIconSpr.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
				priceLbl.enabled = true;
				payLbl.enabled = false;
				break;
			case 2:
				buyIconSpr.enabled = true;
				buyIconSpr.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
				priceLbl.enabled = true;
				payLbl.enabled = false;
				break;
			}
		}
	}

	public void OnButtonClick()
	{
		if (_popupData.buyCallback != null)
		{
			_popupData.buyCallback();
		}
	}
}
