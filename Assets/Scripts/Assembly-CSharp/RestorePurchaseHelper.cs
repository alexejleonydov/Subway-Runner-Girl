using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel collectLbl;

	[SerializeField]
	private UILabel time;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite viewSpr;

	[SerializeField]
	private BoxCollider btnCollider;

	private TimeCoolDown coolDown;

	private void Awake()
	{
		coolDown = new TimeCoolDown("RestorePurchaseHelper", 180);
	}

	private void OnEnable()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_TITLE);
		freeLbl.text = Strings.Get(LanguageKey.SHOP_BOX_LABEL_FERR);
		collectLbl.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_GET);
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type != RiseSdk.AdEventType.RewardAdShowFinished || id != 1)
		{
			return;
		}
		int num = ((Random.value < 0.5f) ? 1 : 2);
		if (num == 1)
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewcoins, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_free", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				coolDown.SetFreeTime();
			});
		}
		else
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewkeys, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_free", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				coolDown.SetFreeTime();
			});
		}
	}

	public void OnFreeRewardClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_shop_gem", 0);
		RiseSdk.Instance.TrackEvent("click_video_shop_gem", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_shop_gem", 0);
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 1;
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

	private void Update()
	{
		if (!coolDown.IsCoolingDownOver())
		{
			time.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_CD) + coolDown.GetCoolingDownTime2();
			btnCollider.enabled = false;
			collectLbl.enabled = false;
			fillSpr.color = Color.cyan;
			viewSpr.enabled = false;
		}
		else
		{
			time.text = string.Empty;
			btnCollider.enabled = true;
			collectLbl.enabled = true;
			fillSpr.color = Color.white;
			viewSpr.enabled = true;
		}
	}
}
