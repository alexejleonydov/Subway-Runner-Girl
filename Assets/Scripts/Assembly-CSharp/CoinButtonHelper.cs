using UnityEngine;

public class CoinButtonHelper : MonoBehaviour
{
	[SerializeField]
	private int index;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UILabel price;

	[SerializeField]
	private GameObject vipTip;

	private void ShowNoDiscount(string backupDescription = "")
	{
		if (string.IsNullOrEmpty(InAppData.inAppData[index].description))
		{
			description.text = backupDescription;
		}
		else
		{
			description.text = InAppData.inAppData[index].description;
		}
		Common();
	}

	private void Common()
	{
		icon.spriteName = InAppData.inAppData[index].iconName;
		price.text = InAppData.inAppData[index].price;
		if (PlayerInfo.Instance.hasSubscribed && vipTip != null && InAppData.inAppData[index].type == InAppData.DataType.Key)
		{
			vipTip.SetActive(true);
		}
	}

	private void _Setup()
	{
		if (InAppData.inAppData[index].type == InAppData.DataType.Coin)
		{
			ShowNoDiscount(string.Format(Strings.Get(LanguageKey.COIN_BUTTON_HELPER_NUMBER_OF_COINS), InAppData.inAppData[index].amountOfCoins));
		}
		else
		{
			ShowNoDiscount(string.Format(Strings.Get(LanguageKey.COIN_BUTTON_HELPER_NUMBER_OF_KEYS), InAppData.inAppData[index].amountOfKeys));
		}
	}

	public void Init()
	{
		_Setup();
	}

	public void OnClick()
	{
		if (InAppData.inAppData[index].type == InAppData.DataType.Coin)
		{
			ShopManager.Instance.SetShopType(LanguageKey.ListTitle_label_03, InAppData.inAppData[index].iconName, InAppData.inAppData[index].amountOfCoins, 0, InAppData.inAppData[index].price, Pay);
		}
		else
		{
			ShopManager.Instance.SetShopType(LanguageKey.ListTitle_label_04, InAppData.inAppData[index].iconName, InAppData.inAppData[index].amountOfKeys, 0, InAppData.inAppData[index].price, Pay);
		}
	}

	private void Pay()
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			RiseSdk.Instance.Pay(index);
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}
}
