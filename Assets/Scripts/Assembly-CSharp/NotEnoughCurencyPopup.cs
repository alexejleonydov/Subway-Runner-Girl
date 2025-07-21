using UnityEngine;

public class NotEnoughCurencyPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel popupDescription;

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite viewIconSpr;

	[SerializeField]
	private BoxCollider freeCollider;

	[SerializeField]
	private UISprite iconSpr;

	[SerializeField]
	private UILabel amountLbl;

	[SerializeField]
	private GameObject buy;

	[SerializeField]
	private UILabel buyLbl;

	private InAppManagerPopupData _popupData;

	public override void Hide()
	{
		_popupData = null;
		base.Hide();
	}

	public void OnFreeReward(RiseSdk.AdEventType b, int type, string tag, int d)
	{
		if (b != RiseSdk.AdEventType.RewardAdShowFinished || type != 6)
		{
			return;
		}
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_not_enough", 0);
		if (_popupData.isCoins)
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewcoins, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_not_enough", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward);
				UIScreenController.Instance.ClosePopup(null);
			});
		}
		else
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewkeys, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_not_enough", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				UIScreenController.Instance.ClosePopup(null);
			});
		}
	}

	public void OnFreeViewClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_not_enough", 0);
		RiseSdk.Instance.TrackEvent("click_video_not_enough", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 6;
				UIScreenController.Instance.PushPopup("VideoLoadingPopup");
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	public void OnBuyClicked(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			RiseSdk.Instance.Pay(5);
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	public void OnCancelClicked()
	{
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()) && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
			UIScreenController.Instance.ClosePopup(null);
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public void OnOkClicked(GameObject go)
	{
		if (UIScreenController.isInstanced)
		{
			ShopManager.Instance.barIndex = (_popupData.isCoins ? 1 : 2);
			if ("CoinsUI_shop".Equals(UIScreenController.Instance.GetTopScreenName()))
			{
				(UIScreenController.Instance.GetScreenFromCache("CoinsUI_shop") as UIShopScreen).ResetScrollViewSmooth();
			}
			else
			{
				UIScreenController.Instance.SwitchScreen("CoinsUI_shop");
			}
			if (_popupData != null && _popupData.lastIsPopup)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public override void Show()
	{
		base.Show();
		_popupData = InAppManager.instance.GetPopupData();
		buyLbl.text = Strings.Get(LanguageKey.UI_POPUP_NOT_ENOUGH_CURENCY_BUTTON_BUY);
		if (_popupData != null)
		{
			popupTitle.text = _popupData.popupTitle;
			popupDescription.text = _popupData.popupDescription;
			freeLbl.text = Strings.Get(LanguageKey.UI_POPUP_NOT_ENOUGH_CURENCY_BUTTON_FREE);
			iconSpr.spriteName = ((!_popupData.isCoins) ? UIPosScalesAndNGUIAtlas.Instance.key : UIPosScalesAndNGUIAtlas.Instance.coin);
			amountLbl.text = ((!_popupData.isCoins) ? UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward : UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward).ToString();
			if (RiseSdk.Instance.HasRewardAd())
			{
				fillSpr.color = Color.white;
				viewIconSpr.color = Color.white;
				iconSpr.color = Color.white;
				freeCollider.enabled = true;
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_not_enough", 0);
			}
			else
			{
				fillSpr.color = Color.cyan;
				viewIconSpr.color = Color.cyan;
				iconSpr.color = Color.cyan;
				freeCollider.enabled = false;
			}
		}
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()) && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			UIEventListener uIEventListener = UIEventListener.Get(buy);
			uIEventListener.onClick = OnBuyClicked;
		}
		else
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(buy);
			uIEventListener2.onClick = OnOkClicked;
		}
	}
}
