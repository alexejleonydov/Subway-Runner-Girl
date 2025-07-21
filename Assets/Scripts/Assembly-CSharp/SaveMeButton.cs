using UnityEngine;

public class SaveMeButton : MonoBehaviour
{
	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private UILabel saveMePrice;

	public SaveMePopup saveMePopup;

	public BoxCollider cancleCollider;

	public void CenterUnit(Transform targetTransform, float aditionalOffset = 0f)
	{
		Vector3 localPosition = (targetTransform.localPosition = Vector3.zero);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(targetTransform);
		localPosition.x = localPosition.x - bounds.extents.x / 2f + aditionalOffset;
		targetTransform.localPosition = localPosition;
	}

	public void WatchVideoOnClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_saveme", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_saveme", 0);
		Game.Instance.wasButtonClicked = true;
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 3;
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

	public void OnClick()
	{
		Game.Instance.wasButtonClicked = true;
		cancleCollider.enabled = false;
		int numberOfKeysToSaveMe = SaveMeManager.GetNumberOfKeysToSaveMe();
		if (numberOfKeysToSaveMe <= PlayerInfo.Instance.amountOfKeys)
		{
			PlayerInfo.Instance.amountOfKeys -= numberOfKeysToSaveMe;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, numberOfKeysToSaveMe);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_int_game_revive", 0, numberOfKeysToSaveMe);
			ReviveGameSuc();
		}
		else
		{
			SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = true;
			SaveMeManager.IS_PURCHASE_RUNNING_INGAME = true;
			PurchaseHandler.Instance.PurchaseKeysIfNeeded(numberOfKeysToSaveMe);
		}
	}

	public void OnFreeReviveClick()
	{
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(true);
		}
		Revive.Instance.SendRevive();
		UIScreenController.Instance.ClosePopup(null);
		SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
	}

	public static void ReviveGameSuc()
	{
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(true);
		}
		Revive.Instance.SendRevive();
		SaveMeManager.IncrementNumberOfUsedKeys();
		UIScreenController.Instance.ClosePopup(null);
		SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
		TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, SaveMeManager.GetNumberOfKeysToSaveMe());
	}

	private void OnEnable()
	{
		watchLbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		freeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		adLbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_AD_REVIVE_TIP);
		saveMePrice.text = SaveMeManager.GetNumberOfKeysToSaveMe() + string.Empty;
		cancleCollider.enabled = true;
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 3)
		{
			UIScreenController.Instance.ClosePopup(null);
			ReviveGameSuc();
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_saveme", 0);
		}
	}
}
