using System;
using System.Collections;
using UnityEngine;

public class MainScreen : UIBaseScreen
{
	[SerializeField]
	private GameObject goTween;

	[SerializeField]
	private UIButton goBtn;

	[SerializeField]
	private Animation goAnim;

	[SerializeField]
	private GameObject charTween;

	[SerializeField]
	private Transform charBtn;

	[SerializeField]
	private Animation charAnim;

	[SerializeField]
	private GameObject chestTween;

	[SerializeField]
	private Transform chestBtn;

	[SerializeField]
	private Animation chestAnim;

	[SerializeField]
	private MainScreenTopUI topUI;

	[SerializeField]
	private GameObject tutorialRoot;

	[SerializeField]
	private Transform finger;

	[SerializeField]
	private GameObject[] gameobjectsToTween;

	[SerializeField]
	private UILabel helmetNumLabel;

	[SerializeField]
	private UISprite boxTip;

	private int _lastGameoverAdCountForTrailRole;

	private bool autoShowPopup;

	[SerializeField]
	private TaskHelper taskTip;

	private InputActions inputActions;


	private void OnEnable()
	{
		inputActions = new InputActions();
		inputActions.Play.Enable();
		inputActions.Play.PlayGame.performed += OnStartPerformed;

		PlayerInfo.Instance.onPowerupAmountChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onPowerupAmountChanged, new Action(UpdatehelmLabel));
	}

	private void OnDisable()
	{
		inputActions.Play.PlayGame.performed -= OnStartPerformed;
		inputActions.Play.Disable();

		PlayerInfo.Instance.onPowerupAmountChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onPowerupAmountChanged, new Action(UpdatehelmLabel));
	}

	private void OnStartPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
	{
		Game.Instance.StartGame();
	}


	public void TapStartOnClick()
	{
		Game.Instance.StartGame();
	}

	public void TestClickAddExp()
	{
		PlayerInfo.Instance.amountOfExp += 50;
		TasksManager.Instance.CheckPlayerLevel();
	}

	private void RefreshLabel()
	{
		goBtn.normalSprite = Strings.Get(LanguageKey.ATLAS_UI_PLAY_BUTTON_SPRITE);
	}

	public override void Init()
	{
		base.Init();
		if (PlayerInfo.Instance.tutorialStep == 3)
		{
			PlayerInfo.Instance.tutorialCompleted = true;
		}
		if (!PlayerInfo.Instance.tutorialCompleted)
		{
			triggerTween(false);
		}
		else
		{
			triggerTween(true);
		}
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			UnityEngine.Object.Destroy(tutorialRoot);
		}
		_lastGameoverAdCountForTrailRole = PlayerInfo.Instance.gameOverFullAdCount;
		NotificationsObserver.Instance.RegisterNotificationAction(base.gameObject);
	}

	public override void Show()
	{
		base.Show();
		RefreshLabel();
		ShowDeliciousIcon();
		NotificationsObserver.Instance.NotifyNotificationDataChange();
		boxTip.enabled = ShopManager.Instance.IsCoolingDownOver();
		if (Characters.characterOrder.IndexOf((Characters.CharacterType)PlayerInfo.Instance.currentCharacter) == 8 && !PlayerInfo.Instance.hasSubscribed)
		{
			CharacterScreenManager.Instance.SelectCharacter(Characters.CharacterType.slick, 0);
		}
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			bool flag = false;
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow.Date - PlayerInfo.Instance.gameOverFullAdDate).Days != 0)
			{
				PlayerInfo.Instance.gameOverFullAdDate = utcNow.Date;
				PlayerInfo.Instance.showTrialPopupCount = 0;
				PlayerInfo.Instance.showWatchVideoPopupCount = 0;
				PlayerInfo.Instance.gameOverFullAdCount = 0;
				PlayerPrefs.SetInt("ShowSubscribeCount", 0);
				PlayerPrefs.SetInt("ShowDailyRewardsCount", 0);
				_lastGameoverAdCountForTrailRole = -1;
				flag = true;
			}
			bool Istoday = false;
			PlayerInfo.Instance.GetDailyLandingDaysInRow(out Istoday);
			if (PlayerPrefs.GetInt("PlayerHasLevel") == 2)
			{
				autoShowPopup = true;
				PlayerPrefs.SetInt("PlayerHasLevel", 3);
				UIScreenController.Instance.QueuePopup("NewLevelVersionTip");
			}
			if (!PlayerPrefs.HasKey("NewPlayerShowDailyReward") && (DateTime.UtcNow - PlayerInfo.Instance.firstInstallDate).Days < 1 && PlayerPrefs.GetInt("ShowDailyRewardsCount") < 1 && PlayerInfo.Instance.DailyLandingPayOut())
			{
				PlayerPrefs.SetInt("NewPlayBackMainTimes", PlayerPrefs.GetInt("NewPlayBackMainTimes") + 1);
				if (PlayerPrefs.GetInt("NewPlayBackMainTimes") >= 2)
				{
					autoShowPopup = true;
					PlayerPrefs.SetInt("ShowDailyRewardsCount", 1);
					UIScreenController.Instance.QueuePopup("DailyRewardsPopup");
					PlayerPrefs.SetInt("NewPlayerShowDailyReward", 1);
				}
			}
			if ((DateTime.UtcNow - PlayerInfo.Instance.firstInstallDate).Days > 0 && PlayerInfo.Instance.DailyLandingPayOut())
			{
				if (!UIScreenController.Instance.CheckNetwork())
				{
					autoShowPopup = true;
					PlayerPrefs.SetInt("ShowDailyRewardsCount", 1);
					UIScreenController.Instance.QueuePopup("DailyRewardsPopup");
				}
				else if (RiseSdk.Instance.HasRewardAd())
				{
					autoShowPopup = true;
					PlayerPrefs.SetInt("ShowDailyRewardsCount", 1);
					UIScreenController.Instance.QueuePopup("DailyRewardsPopup");
				}
			}
			if (!PlayerInfo.Instance.hasRemoveAd && Game.Instance.show20sAd && !autoShowPopup && Game.Instance.GetNextAdDuration() > 20f)
			{
				Game.Instance.showAdTime = Time.time;
				Game.Instance.closePopupOnAdEvent = false;
				Game.Instance.lastShowAd = "show_interstitial__start20s";
				if (RiseSdk.Instance.GetRemoteConfigInt("Show_video_inter_loading_config") == 1)
				{
					VideoLoadingPopup.adType = 1;
					VideoLoadingPopup.rewardId = 1;
					UIScreenController.Instance.PushPopup("VideoLoadingPopup");
				}
				else
				{
					RiseSdk.Instance.ShowAd("custom");
				}
				RiseSdk.Instance.TrackEvent("interstitial_start20s", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_start20s", 0);
				RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
				Game.Instance.show20sAd = false;
			}
			bool flag2 = false;
			if (PlayerInfo.Instance.ignoreSubscriptionPopup && utcNow > PlayerInfo.Instance.ignoreSubscriptionNextTime)
			{
				PlayerInfo.Instance.ignoreSubscriptionPopup = false;
			}
			if (!PlayerPrefs.HasKey("NewPlayerShowSubscribeFirst") && (DateTime.UtcNow - PlayerInfo.Instance.firstInstallDate).Days < 1)
			{
				autoShowPopup = true;
				UIScreenController.Instance.QueuePopup("SubscribePopup");
				PlayerPrefs.SetInt("NewPlayerShowSubscribeFirst", 1);
			}
			if ((DateTime.UtcNow - PlayerInfo.Instance.firstInstallDate).Days > 0 && PlayerInfo.Instance.gameOverFullAdCount >= 3 && PlayerPrefs.GetInt("ShowSubscribeCount") < 1)
			{
				PlayerPrefs.SetInt("ShowSubscribeCount", PlayerPrefs.GetInt("ShowSubscribeCount") + 1);
				autoShowPopup = true;
				UIScreenController.Instance.QueuePopup("SubscribePopup");
			}
			if (!PlayerPrefs.HasKey("OldPlayerShowUnlockNewScreenPopup") && !PlayerInfo.Instance.isNewPlayer)
			{
				City city = TrackController.Instance.LastTaskSetWithUnlockCity(PlayerInfo.Instance.amountOfLevel);
				if (city != null)
				{
					autoShowPopup = true;
					PlayerInfo.Instance.forceNextCityOrder = city.order;
					UIScreenController.Instance.QueuePopup("UnlockNewScreenPopup");
					PlayerPrefs.SetInt("OldPlayerShowUnlockNewScreenPopup", 1);
				}
			}
			bool flag3 = false;
			if (!TrialManager.Instance.nothingElse && TrialManager.Instance.currentTrialInfo != null && TrialManager.Instance.CheckOnMainScreen())
			{
				if (!PlayerInfo.Instance.isNewPlayer && flag)
				{
					flag2 = true;
					autoShowPopup = true;
					PlayerInfo.Instance.showTrialPopupCount++;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "Try_popup_auto", 0);
					UIScreenController.Instance.QueuePopup("TryHoverboardPopup");
				}
				else
				{
					flag3 = true;
				}
			}
			if (PlayerInfo.Instance.showTrialPopupCount <= 4 && _lastGameoverAdCountForTrailRole != PlayerInfo.Instance.gameOverFullAdCount && PlayerInfo.Instance.gameOverFullAdCount >= 4 && flag3)
			{
				flag2 = true;
				autoShowPopup = true;
				PlayerInfo.Instance.showTrialPopupCount++;
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "Try_popup_auto", 0);
				UIScreenController.Instance.QueuePopup("TryHoverboardPopup");
				PlayerInfo.Instance.gameOverFullAdCount = 0;
			}
			TrialManager.Instance.preUseTryRole = false;
			_lastGameoverAdCountForTrailRole = PlayerInfo.Instance.gameOverFullAdCount;
			if (!flag2 && PlayerInfo.Instance.shouldShowPlayerMenuPopup && !PlayerInfo.Instance.hasShownPlayerMenuPopup)
			{
				UIScreenController.Instance.PushScreen("CharacterScreen");
				PlayerInfo.Instance.hasShownPlayerMenuPopup = true;
				PlayerInfo.Instance.shouldShowPlayerMenuPopup = false;
				return;
			}
		}
		UpdatehelmLabel();
		StartCoroutine(CheckTutorial());
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private IEnumerator CheckTutorial()
	{
		goTween.SetActive(PlayerInfo.Instance.tutorialStep >= 0);
		charTween.SetActive(PlayerInfo.Instance.tutorialStep >= 1);
		topUI.gameObject.SetActive(PlayerInfo.Instance.tutorialStep >= 1);
		chestTween.SetActive(PlayerInfo.Instance.tutorialStep >= 2);
		if (tutorialRoot == null)
		{
			yield break;
		}
		if (PlayerInfo.Instance.tutorialStep == 3)
		{
			PlayerInfo.Instance.tutorialCompleted = true;
			UnityEngine.Object.Destroy(tutorialRoot);
			triggerTween(true);
			yield break;
		}
		yield return null;
		if (PlayerInfo.Instance.tutorialStep == 0)
		{
			finger.position = goBtn.transform.position + UIPosScalesAndNGUIAtlas.Instance.mainScreenPlayFingerOffset / UIScreenController.Instance.root.activeHeight * 2f;
			finger.localRotation = Quaternion.Euler(0f, 0f, UIPosScalesAndNGUIAtlas.Instance.mainScreenPlayFingerRotZ);
			goAnim.Play();
		}
		else if (PlayerInfo.Instance.tutorialStep == 1)
		{
			finger.position = charBtn.position + UIPosScalesAndNGUIAtlas.Instance.mainScreenCharFingerOffset / UIScreenController.Instance.root.activeHeight * 2f;
			finger.localRotation = Quaternion.Euler(0f, 0f, UIPosScalesAndNGUIAtlas.Instance.mainScreenCharFingerRotZ);
			charAnim.Play();
		}
		else if (PlayerInfo.Instance.tutorialStep == 2)
		{
			finger.position = chestBtn.position + UIPosScalesAndNGUIAtlas.Instance.mainScreenChestFingerOffset / UIScreenController.Instance.root.activeHeight * 2f;
			finger.localRotation = Quaternion.Euler(0f, 0f, UIPosScalesAndNGUIAtlas.Instance.mainScreenChestFingerRotZ);
			chestAnim.Play();
		}
	}

	public override void Hide()
	{
		RiseSdk.Instance.CloseDeliciousIconAd();
		base.Hide();
	}

	public void UpdatehelmLabel()
	{
		helmetNumLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet).ToString();
	}

	private void triggerTween(bool active)
	{
		int i = 0;
		for (int num = gameobjectsToTween.Length; i < num; i++)
		{
			gameobjectsToTween[i].SetActive(active);
		}
	}

	public void ShowDeliciousIcon()
	{
		if (!RiseSdk.Instance.HasDeliciousAd())
		{
			return;
		}
		float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
		if (num < 0.6f)
		{
			float num2 = (float)Screen.height / 1280f;
			if ((float)Screen.width > 800f)
			{
				RiseSdk.Instance.ShowDeliciousIconAd(20f * num2, 250f * num2, 130f * num2, 130f * num2, "delicious9-16");
			}
			else
			{
				RiseSdk.Instance.ShowDeliciousIconAd(20f * num2, 250f * num2, 130f * num2, 130f * num2, "delicious9-16");
			}
		}
		else if (num < 0.7f)
		{
			float num3 = (float)Screen.height / 1280f;
			RiseSdk.Instance.ShowDeliciousIconAd(20f * num3, 250f * num3, 130f * num3, 130f * num3, "delicious2-3");
		}
		else
		{
			float num4 = (float)Screen.height / 1280f;
			RiseSdk.Instance.ShowDeliciousIconAd(20f * num4, 250f * num4, 130f * num4, 130f * num4, "delicious3-4");
		}
	}

	public override void GainFocus()
	{
		base.GainFocus();
		if (Characters.characterOrder.IndexOf((Characters.CharacterType)PlayerInfo.Instance.currentCharacter) == 8 && !PlayerInfo.Instance.hasSubscribed)
		{
			CharacterScreenManager.Instance.SelectCharacter(Characters.CharacterType.slick, 0);
		}
		ShowDeliciousIcon();
		if (!PlayerInfo.Instance.hasRemoveAd && Game.Instance.show20sAd && PlayerInfo.Instance.tutorialCompleted && !autoShowPopup && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			Game.Instance.closePopupOnAdEvent = false;
			Game.Instance.lastShowAd = "show_interstitial__start20s";
			if (RiseSdk.Instance.GetRemoteConfigInt("Show_video_inter_loading_config") == 1)
			{
				VideoLoadingPopup.adType = 1;
				VideoLoadingPopup.rewardId = 1;
				UIScreenController.Instance.PushPopup("VideoLoadingPopup");
			}
			else
			{
				RiseSdk.Instance.ShowAd("custom");
			}
			RiseSdk.Instance.TrackEvent("interstitial_start20s", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_start20s", 0);
			RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
			Game.Instance.show20sAd = false;
		}
		autoShowPopup = false;
		taskTip.CheckTipShow();
	}

	public void OnCoinPlusClick()
	{
		ShopManager.Instance.barIndex = 1;
		if ("CoinsUI_shop".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("CoinsUI_shop") as UIShopScreen).ResetScrollViewSmooth();
		}
		else
		{
			UIScreenController.Instance.SwitchScreen("CoinsUI_shop");
		}
	}

	public void OnKeyPlusClick()
	{
		ShopManager.Instance.barIndex = 2;
		if ("CoinsUI_shop".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("CoinsUI_shop") as UIShopScreen).ResetScrollViewSmooth();
		}
		else
		{
			UIScreenController.Instance.SwitchScreen("CoinsUI_shop");
		}
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		RiseSdk.Instance.CloseDeliciousIconAd();
	}
}
