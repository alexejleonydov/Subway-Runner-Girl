using UnityEngine;
using UnityEngine.InputSystem;

public class DailyLandingPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UILabel tomorrowLbl;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private DailyLandingHelp[] helps;

	[SerializeField]
	private GameObject getGo;

	private InputActions inputActions;

	[SerializeField]
	private GameObject viewGo;

	public override void Init()
	{
		base.Init();
		for (int i = 0; i < helps.Length; i++)
		{
			helps[i].Init(i + 1);
		}
	}


	public override void Show()
	{
		base.Show();
		Refresh();
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_TITLE);
		getLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_BUTTON_GET);
		adLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_AD_DOUBLE_TIP);
		tomorrowLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_KEEP_CONTINUE);
	}

	private void Refresh()
	{
		bool Istoday;
		PlayerInfo.Instance.GetDailyLandingDaysInRow(out Istoday);
		for (int i = 0; i < helps.Length; i++)
		{
			helps[i].Refresh();
		}
		if (Istoday)
		{
			if (viewGo.activeInHierarchy)
			{
				viewGo.SetActive(false);
			}
			if (getGo.activeInHierarchy)
			{
				getGo.SetActive(false);
			}
			tomorrowLbl.enabled = true;
			return;
		}
		if (!getGo.activeInHierarchy)
		{
			getGo.SetActive(true);
		}
		tomorrowLbl.enabled = false;
		if (UIScreenController.Instance.CheckNetwork() && RiseSdk.Instance.HasRewardAd())
		{
			if (!viewGo.activeInHierarchy)
			{
				viewGo.SetActive(true);
			}
			getGo.transform.localPosition = Vector3.up * -540f;
			viewGo.transform.localPosition = Vector3.up * -405f;
		}
		else
		{
			if (viewGo.activeInHierarchy)
			{
				viewGo.SetActive(false);
			}
			getGo.transform.localPosition = Vector3.up * -405f;
		}
	}

	private void OnInteractPerformed(InputAction.CallbackContext context)
	{
		GameObject claimBtn = GameObject.Find("DailyRewardsPopup(Clone)");

		if (claimBtn != null || claimBtn.activeInHierarchy)
		{
			Debug.Log("Claim is pressed for Daily reward");
			OnReceiceClick();
		}
	}

	public void OnReceiceClick()
	{
		PlayerInfo.Instance.ReceiveDailyLandingPayout(1, CloseDailyRewardPopUp);
	}

	public void OnDoubleClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_double_daily", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_double_daily", 0);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 15;
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
		RiseSdkListener.OnAdEvent -= OnFreeView;
		RiseSdkListener.OnAdEvent += OnFreeView;

		inputActions = new InputActions();
		inputActions.Enable();
		inputActions.Play.PlayGame.performed += OnInteractPerformed;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeView;

		inputActions.Disable();
		inputActions.Play.PlayGame.performed -= OnInteractPerformed;
	}

	public void CloseDailyRewardPopUp()
	{
		Game.Instance.showAdTime = Time.time;
		UIScreenController.Instance.ClosePopup(null);
	}

	private void OnFreeView(RiseSdk.AdEventType aet, int id, string tag, int type)
	{
		if (aet == RiseSdk.AdEventType.RewardAdShowFinished && id == 15)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_double_daily", 0);
			PlayerInfo.Instance.ReceiveDailyLandingPayout(2, CloseDailyRewardPopUp);
		}
	}
}
