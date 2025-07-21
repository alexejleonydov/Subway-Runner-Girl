using UnityEngine;

public class BuyButtonIngame : MonoBehaviour, IPurchaseHandler
{
	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UILabel freelbl;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel upgradeLbl;

	[SerializeField]
	private GameObject showGo;

	[SerializeField]
	private GameObject freeGo;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject hideGo;

	[SerializeField]
	private BoxCollider btnCollider;

	private bool _purchaseInProgress;

	private PropType _type;

	private bool inbuy;

	public void initBuyButton(PropType type)
	{
		_type = type;
	}

	public void Reload(bool forceBuy)
	{
		freelbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_BUTTON_VIDEO);
		upgradeLbl.text = Strings.Get(LanguageKey.UPGRADE_BTN_LAB);
		Upgrade upgrade = Upgrades.upgrades[_type];
		if (PlayerInfo.Instance.GetCurrentTier(_type) >= upgrade.numberOfTiers - 1)
		{
			hideGo.SetActive(true);
			showGo.SetActive(false);
			btnCollider.enabled = false;
			return;
		}
		hideGo.SetActive(false);
		showGo.SetActive(true);
		btnCollider.enabled = true;
		if (!forceBuy && PlayerInfo.Instance.CheckIfFreeUpgrade() && RiseSdk.Instance.HasRewardAd())
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_upgrades", 0);
			buyGo.SetActive(false);
			freeGo.SetActive(true);
			fill.spriteName = "btn_green";
			inbuy = false;
		}
		else
		{
			buyGo.SetActive(true);
			freeGo.SetActive(false);
			fill.spriteName = "btn_yellow";
			priceLbl.text = string.Empty + upgrade.getPrice(PlayerInfo.Instance.GetCurrentTier(_type) + 1);
			inbuy = true;
		}
	}

	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			if (inbuy)
			{
				PurchaseHandler.Instance.PurchaseUpgrade(_type, false, this);
			}
			else
			{
				FreeRewardClick();
			}
		}
	}

	private void FreeRewardClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_upgrades", 0);
		RiseSdk.Instance.TrackEvent("click_video_upgrades", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 8;
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

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		Upgrade upgrade = Upgrades.upgrades[_type];
		priceLbl.text = string.Empty + upgrade.getPrice(PlayerInfo.Instance.GetCurrentTier(_type) + 1);
		_purchaseInProgress = false;
	}
}
