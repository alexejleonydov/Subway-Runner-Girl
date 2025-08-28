using UnityEngine;
using UnityEngine.InputSystem;

public class HelmetSelectButton : MonoBehaviour, IPurchaseHandler
{
	private enum State
	{
		buy = 0,
		select = 1,
		level = 2
	}

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UISprite fillSprite;

	[SerializeField]
	private UISprite buycoinSprite;

	[SerializeField]
	private UISprite buykeySprite;

	[SerializeField]
	private UILabel buypriceSprite;

	[SerializeField]
	private UILabel selectLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject select;

	[SerializeField]
	private GameObject level;

	[SerializeField]
	private GameObject tryGo;

	[SerializeField]
	private string greenBtnSpriteName;

	[SerializeField]
	private string blueBtnSpriteName;

	[SerializeField]
	private string grayBtnSpriteName;

	[SerializeField]
	private UISprite[] freeTrySprs;

	[SerializeField]
	private Color selectOutlineColor;

	[SerializeField]
	private Color selectedOutlineColor;

	private BoxCollider col;

	private Helmets.Helm currentHelm;

	private Helmets.HelmType currentHelmtype;

	private bool helmunlocked;

	private Helmets.UnlockType unlocktype;

	private int price;

	private State activeState;

	private bool isInited;

	private int selectInt = -1;

	private bool _purchaseInProgress;

	private Color tryLblColor;

	private int tryState;

	private InputActions inputActions;

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;

		inputActions = new InputActions();
		inputActions.Enable();
		inputActions.Play.PlayGame.performed += OnCharacterPerformed;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;

		inputActions.Disable();
		inputActions.Play.PlayGame.performed -= OnCharacterPerformed;
	}

	private void OnCharacterPerformed(InputAction.CallbackContext context)
	{

		if (context.performed && gameObject.activeInHierarchy)
		{
			OnClick();
		}

	}

	private void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 13)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_helmet", 0);
			if (tryGo.activeSelf)
			{
				tryGo.SetActive(false);
			}
			Game.Instance.TestHelmet(currentHelmtype);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	public void OnClick()
	{
		if (!isInited)
		{
			return;
		}
		if (activeState == State.buy)
		{
			PurchaseHelmetTheme();
		}
		else if (activeState == State.select)
		{
			if (selectInt == 1)
			{
				PlayerInfo.Instance.currentHelmet = currentHelmtype;
				SetToSelectState();
			}
		}
		else if (activeState == State.level)
		{
			UISliderInController.Instance.OnNeedEnoughLevel(currentHelm.level);
		}
	}

	public void OnTryClick()
	{
		if (tryState == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			RiseSdk.Instance.TrackEvent("click_video_try_helmet", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_helmet", 0);
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 13;
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
		else if (tryState == 2)
		{
			if (UIScreenController.Instance.CheckNetwork())
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
			else
			{
				UIScreenController.Instance.PushPopup("NoNetworkPopup");
			}
		}
	}

	private void PurchaseHelmetTheme()
	{
		if (!_purchaseInProgress)
		{
			_purchaseInProgress = true;
			PurchaseHandler.Instance.PurchaseHelmetTheme(currentHelmtype, this);
		}
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		switch (Helmets.helmOrder.IndexOf(currentHelmtype))
		{
			case 1:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet2nd", 0);
				break;
			case 2:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet3rd", 0);
				break;
			case 3:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet4th", 0);
				break;
			case 4:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet5th", 0);
				break;
			case 5:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet6th", 0);
				break;
			case 6:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_helmet7th", 0);
				break;
		}
		_purchaseInProgress = false;
		if (TrialManager.Instance.IsCurrentHelmetTrial(currentHelmtype))
		{
			TrialManager.Instance.currentTrialInfo = null;
		}
		UIScreenController.Instance.ShowUnlockAnimationForHelmet(currentHelmtype);
	}

	public void InitButton(HelmScreen screen)
	{
		isInited = true;
		col = GetComponent<BoxCollider>();
		tryLblColor = tryLbl.color;
		UpdateSelectState(PlayerInfo.Instance.currentHelmet);
	}

	public void UpdateSelectState(Helmets.HelmType helmtype)
	{
		if (isInited)
		{
			helmunlocked = HelmetManager.Instance.isHelmetUnlocked(helmtype);
			currentHelm = Helmets.helmData[helmtype];
			currentHelmtype = helmtype;
			unlocktype = currentHelm.unlockType;
			price = currentHelm.price;
			if (helmunlocked)
			{
				SetState(State.select);
				SetToSelectState();
			}
			else if (PlayerInfo.Instance.amountOfLevel < currentHelm.level)
			{
				SetState(State.level);
				SetToLevelState(currentHelm.level);
			}
			else
			{
				SetState(State.buy);
				SetToBuyState();
			}
			UpdateTryButton();
		}
	}

	private void SetState(State newState)
	{
		activeState = newState;
		switch (newState)
		{
			case State.buy:
				buyGo.SetActive(true);
				select.SetActive(false);
				level.SetActive(false);
				break;
			case State.select:
				select.SetActive(true);
				buyGo.SetActive(false);
				level.SetActive(false);
				break;
			case State.level:
				level.SetActive(true);
				select.SetActive(false);
				buyGo.SetActive(false);
				break;
		}
	}

	private void SetToBuyState()
	{
		fillSprite.spriteName = greenBtnSpriteName;
		col.enabled = true;
		buycoinSprite.enabled = unlocktype == Helmets.UnlockType.coins;
		buykeySprite.enabled = unlocktype == Helmets.UnlockType.keys;
		buypriceSprite.text = price.ToString();
	}

	private void SetToLevelState(int level)
	{
		fillSprite.spriteName = grayBtnSpriteName;
		col.enabled = true;
		levelLabel.text = string.Format(Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_LEVELLOCK), level);
	}

	private void SetToSelectState()
	{
		if (currentHelmtype == PlayerInfo.Instance.currentHelmet)
		{
			selectInt = 0;
			fillSprite.spriteName = blueBtnSpriteName;
			col.enabled = false;
			selectLabel.text = Strings.Get(LanguageKey.HOVERBOARD_SELCTECT_BUTTON_SELECTED);
			selectLabel.effectColor = selectedOutlineColor;
		}
		else if (currentHelmtype != PlayerInfo.Instance.currentHelmet)
		{
			selectInt = 1;
			fillSprite.spriteName = greenBtnSpriteName;
			col.enabled = true;
			selectLabel.text = Strings.Get(LanguageKey.HOVERBOARD_SELCTECT_BUTTON_SELECT);
			selectLabel.effectColor = selectOutlineColor;
		}
	}

	private void UpdateTryButton()
	{
		if (TrialManager.Instance.HasHelmetTrial(currentHelmtype) && !helmunlocked)
		{
			if (!tryGo.activeSelf)
			{
				tryGo.SetActive(false);
			}
			if (RiseSdk.Instance.HasRewardAd())
			{
				tryState = 1;
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_try_helmet", 0);
				int i = 0;
				for (int num = freeTrySprs.Length; i < num; i++)
				{
					freeTrySprs[i].color = Color.white;
				}
				tryLbl.color = tryLblColor;
			}
			else
			{
				tryState = 2;
				int j = 0;
				for (int num2 = freeTrySprs.Length; j < num2; j++)
				{
					freeTrySprs[j].color = Color.cyan;
				}
				tryLbl.color = Color.cyan;
			}
		}
		else
		{
			tryState = 0;
			if (tryGo.activeSelf)
			{
				tryGo.SetActive(false);
			}
		}
		watchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_WATCH);
		tryLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_TRY);
	}
}
