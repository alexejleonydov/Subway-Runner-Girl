using UnityEngine;

public class WatchFreeViewPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel contentLbl;

	[SerializeField]
	private UILabel freeRewardLbl;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite freeSpr;

	[SerializeField]
	private BoxCollider btn_collider;

	[SerializeField]
	private TweenScale btn_tween;

	public static int rewardId;

	public override void Show()
	{
		base.Show();
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_TITLE);
		contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_REWARD);
		freeRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_WATCH_VIDEO_BUTTON_VIDEO);
		if (RiseSdk.Instance.HasRewardAd())
		{
			fillSpr.color = Color.white;
			freeSpr.color = Color.white;
			btn_collider.enabled = true;
			btn_tween.PlayForward();
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_box", 0);
		}
		else
		{
			fillSpr.color = Color.cyan;
			freeSpr.color = Color.cyan;
			btn_collider.enabled = false;
			btn_tween.enabled = false;
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

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type != RiseSdk.AdEventType.RewardAdShowFinished || id != 0)
		{
			return;
		}
		int num = ((Random.value < 0.5f) ? 1 : 2);
		if (num == 1)
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewcoins, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_menu_box", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward);
				UIScreenController.Instance.ClosePopup(null);
			});
		}
		else
		{
			FreeRewardManager.Instance.SetFreeViewReward(RewardType.viewkeys, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_menu_box", 0, UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward);
				UIScreenController.Instance.ClosePopup(null);
			});
		}
	}

	public void OnFreeViewClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_box", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_box", 0);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_box", 0);
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = rewardId;
				UIScreenController.Instance.PushPopup("VideoLoadingPopup");
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
				UIScreenController.Instance.ClosePopup(null);
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}
}
