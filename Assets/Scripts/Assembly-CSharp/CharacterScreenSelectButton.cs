using System;
using UnityEngine;

public class CharacterScreenSelectButton : MonoBehaviour
{
	public enum ButtonStates
	{
		buy = 0,
		select = 1,
		lockedTheme = 2,
		lockedSymbol = 3,
		unselect = 4,
		exclusive = 5,
		level = 6
	}

	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UILabel selectedLabel;

	[SerializeField]
	private UILabel buyPriceLabel;

	[SerializeField]
	private UISprite buyKeySprite;

	[SerializeField]
	private UISprite buyCoinSprite;

	[SerializeField]
	private UISprite lockedThemeCoin;

	[SerializeField]
	private UISprite lockedThemeKey;

	[SerializeField]
	private UILabel lockedThemeAmount;

	[SerializeField]
	private UILabel lockedThemeFeedback;

	[SerializeField]
	private UISprite lockedSymbolToken;

	[SerializeField]
	private UILabel lockedSymbolProgress;

	[SerializeField]
	private UILabel exclusiveLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UILabel findLbl;

	[SerializeField]
	private UILabel inLbl;

	[SerializeField]
	private UILabel levelLbl;

	[SerializeField]
	private GameObject buy;

	[SerializeField]
	private GameObject select;

	[SerializeField]
	private GameObject exclusive;

	[SerializeField]
	private GameObject lockedTheme;

	[SerializeField]
	private GameObject lockedSymbol;

	[SerializeField]
	private GameObject tryButton;

	[SerializeField]
	private GameObject vip;

	[SerializeField]
	private GameObject Level;

	[SerializeField]
	private Color selectOutlineColor;

	[SerializeField]
	private Color selectedOutlineColor;

	[SerializeField]
	private UISprite[] freeTrySprs;

	private ButtonStates _activeState;

	private bool _hasInited;

	private CharacterScreenManager _managerInstance;

	private BoxCollider col;

	private Characters.CharacterType currentlyShownModelType;

	private int currentlyShownThemeIndex;

	private bool isCharacterOwned;

	private bool isThemeOwned;

	private Characters.Model modelData;

	private CharacterTheme modelTheme;

	private int unlockPrice;

	private Characters.UnlockType unlockType;

	private Color tryLblColor;

	private int tryState;

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
		PlayerInfo.Instance.OnSubscribed = (Action)Delegate.Combine(PlayerInfo.Instance.OnSubscribed, new Action(ReloadButton));
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		PlayerInfo.Instance.OnSubscribed = (Action)Delegate.Remove(PlayerInfo.Instance.OnSubscribed, new Action(ReloadButton));
	}

	public void InitButton()
	{
		_hasInited = true;
		_managerInstance = CharacterScreenManager.Instance;
		col = GetComponent<BoxCollider>();
		UIEventListener uIEventListener = UIEventListener.Get(tryButton);
		uIEventListener.onClick = OnTry;
		tryLblColor = tryLbl.color;
		ReloadButton();
	}

	public void OnClick()
	{
		if (_activeState == ButtonStates.buy)
		{
			if (PlayerInfo.Instance.tutorialStep == 1)
			{
				PlayerInfo.Instance.tutorialStep++;
				UIScreenController.Instance.HideTutorial();
			}
			PurchaseCharacter();
		}
		else if (_activeState == ButtonStates.exclusive)
		{
			UIScreenController.Instance.PushPopup("SubscribePopup");
		}
		else if (_activeState == ButtonStates.lockedSymbol)
		{
			UISliderInController.Instance.OnNeedEnoughSymbol(Strings.Get(modelData.symbolName));
		}
		else if (_activeState == ButtonStates.level)
		{
			UISliderInController.Instance.OnNeedEnoughLevel(modelData.Level);
		}
		else if (_activeState == ButtonStates.lockedTheme)
		{
			UISliderInController.Instance.OnErrorMessage(Strings.Get(modelTheme.unlockDescription));
		}
		else
		{
			_managerInstance.SelectCharacter(currentlyShownModelType, currentlyShownThemeIndex);
		}
	}

	public void OnTry(GameObject go)
	{
		if (tryState == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			RiseSdk.Instance.TrackEvent("click_video_try_role", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_role", 0);
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 5;
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

	private void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 5)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_role", 0);
			if (tryButton.activeSelf)
			{
				tryButton.SetActive(false);
			}
			Game.Instance.TestCharacter(currentlyShownModelType, currentlyShownThemeIndex);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	private void PurchaseCharacter()
	{
		_managerInstance.PurchaseCharacter(currentlyShownModelType, currentlyShownThemeIndex);
	}

	public void ReloadButton()
	{
		if (_hasInited)
		{
			CharacterScreenManager instance = CharacterScreenManager.Instance;
			currentlyShownModelType = instance.currenCharacterShown;
			currentlyShownThemeIndex = instance.currentCThemeShownIndex;
			modelData = Characters.characterData[currentlyShownModelType];
			modelTheme = CharacterThemes.GetThemeForCharacter(currentlyShownModelType, currentlyShownThemeIndex);
			unlockType = modelData.unlockType;
			isCharacterOwned = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModelType);
			unlockPrice = modelData.Price;
			if (modelTheme != null)
			{
				CharacterTheme characterTheme = modelTheme;
				unlockType = characterTheme.unlockType;
				isThemeOwned = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentlyShownModelType, currentlyShownThemeIndex);
				unlockPrice = characterTheme.price;
			}
			if (unlockType == Characters.UnlockType.free || (isCharacterOwned && isThemeOwned) || (isCharacterOwned && modelTheme == null))
			{
				SetButtonState(ButtonStates.select);
				UpdateUIForSelectState();
			}
			else if (unlockType == Characters.UnlockType.subscription)
			{
				SetButtonState(ButtonStates.exclusive);
				UpdateUIForExclusiveState();
			}
			else if (!isCharacterOwned && modelTheme != null)
			{
				SetButtonState(ButtonStates.lockedTheme);
				UpdateUIForLockedThemeState();
			}
			else if (unlockType == Characters.UnlockType.symbols)
			{
				SetButtonState(ButtonStates.lockedSymbol);
				UpdateUIForLockedSymbolState();
			}
			else if (PlayerInfo.Instance.amountOfLevel < modelData.Level)
			{
				SetButtonState(ButtonStates.level);
				UpdateUIForNeedLevel(modelData.Level);
			}
			else
			{
				SetButtonState(ButtonStates.buy);
				UpdateUIForBuyState();
			}
			UpdateTryButton();
			UpdateVIP();
		}
	}

	public void SetButtonState(ButtonStates state)
	{
		_activeState = state;
		switch (state)
		{
		case ButtonStates.buy:
			buy.SetActive(true);
			select.SetActive(false);
			lockedTheme.SetActive(false);
			lockedSymbol.SetActive(false);
			exclusive.SetActive(false);
			Level.SetActive(false);
			break;
		case ButtonStates.lockedSymbol:
			lockedSymbol.SetActive(true);
			buy.SetActive(false);
			select.SetActive(false);
			lockedTheme.SetActive(false);
			exclusive.SetActive(false);
			Level.SetActive(false);
			break;
		case ButtonStates.lockedTheme:
			lockedTheme.SetActive(true);
			buy.SetActive(false);
			select.SetActive(false);
			lockedSymbol.SetActive(false);
			exclusive.SetActive(false);
			Level.SetActive(false);
			break;
		case ButtonStates.select:
			select.SetActive(true);
			buy.SetActive(false);
			lockedTheme.SetActive(false);
			lockedSymbol.SetActive(false);
			exclusive.SetActive(false);
			Level.SetActive(false);
			break;
		case ButtonStates.exclusive:
			exclusive.SetActive(true);
			buy.SetActive(false);
			lockedTheme.SetActive(false);
			lockedSymbol.SetActive(false);
			select.SetActive(false);
			Level.SetActive(false);
			break;
		case ButtonStates.level:
			Level.SetActive(true);
			exclusive.SetActive(false);
			buy.SetActive(false);
			lockedTheme.SetActive(false);
			lockedSymbol.SetActive(false);
			select.SetActive(false);
			break;
		default:
			Debug.LogError("No handler for button state: " + state, null);
			break;
		}
	}

	private void ChangeButtonBgSprite(string color)
	{
		fill.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.fillSpriteNameFormat, color);
	}

	private void UpdateUIForBuyState()
	{
		ChangeButtonBgSprite("yellow");
		col.enabled = true;
		buyPriceLabel.text = unlockPrice.ToString();
		buyCoinSprite.gameObject.SetActive(unlockType == Characters.UnlockType.coins);
		buyKeySprite.gameObject.SetActive(unlockType == Characters.UnlockType.keys);
	}

	private void UpdateUIForNeedLevel(int level)
	{
		ChangeButtonBgSprite("gray");
		col.enabled = true;
		levelLbl.text = string.Format(Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_LEVELLOCK), level);
	}

	private void UpdateUIForExclusiveState()
	{
		ChangeButtonBgSprite("yellow");
		col.enabled = true;
		exclusiveLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_EXCLUSIVE);
	}

	private void UpdateUIForLockedSymbolState()
	{
		ChangeButtonBgSprite("gray");
		col.enabled = true;
		lockedSymbolProgress.text = PlayerInfo.Instance.GetCollectedSymbols(currentlyShownModelType).ToString() + "/" + unlockPrice;
		lockedSymbolToken.spriteName = modelData.symbolSprite2dName;
		findLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_FIND);
		inLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_IN);
	}

	private void UpdateUIForLockedThemeState()
	{
		ChangeButtonBgSprite("gray");
		col.enabled = true;
		CharacterTheme characterTheme = modelTheme;
		lockedThemeAmount.text = characterTheme.price.ToString();
		lockedThemeCoin.enabled = unlockType == Characters.UnlockType.coins;
		lockedThemeKey.enabled = unlockType == Characters.UnlockType.keys;
		string text = Strings.Get(characterTheme.unlockDescription);
		if (!string.IsNullOrEmpty(text))
		{
			lockedThemeFeedback.text = text;
		}
	}

	private void UpdateUIForSelectState()
	{
		if (PlayerInfo.Instance.currentCharacter == (int)currentlyShownModelType && PlayerInfo.Instance.currentThemeIndex == UIModelController.Instance.currentCThemeShownIndex)
		{
			ChangeButtonBgSprite("green");
			col.enabled = false;
			selectedLabel.text = Strings.Get(LanguageKey.UICHARACTER_SELECT_BUTTON_SELECTED);
			selectedLabel.effectColor = selectedOutlineColor;
		}
		else
		{
			ChangeButtonBgSprite("yellow");
			col.enabled = true;
			selectedLabel.text = Strings.Get(LanguageKey.UICHARACTER_SELECT_BUTTON_SELECT);
			selectedLabel.effectColor = selectOutlineColor;
		}
	}

	private void UpdateTryButton()
	{
		if (TrialManager.Instance.HasTrialCharacter(currentlyShownModelType) && ((!isCharacterOwned && modelTheme == null) || (modelTheme != null && !isThemeOwned)))
		{
			if (!tryButton.activeSelf)
			{
				tryButton.SetActive(true);
			}
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_try_role", 0);
				int i = 0;
				for (int num = freeTrySprs.Length; i < num; i++)
				{
					freeTrySprs[i].color = Color.white;
				}
				tryLbl.color = tryLblColor;
				tryState = 1;
			}
			else
			{
				int j = 0;
				for (int num2 = freeTrySprs.Length; j < num2; j++)
				{
					freeTrySprs[j].color = Color.cyan;
				}
				tryLbl.color = Color.cyan;
				tryState = 2;
			}
		}
		else
		{
			if (tryButton.activeSelf)
			{
				tryButton.SetActive(false);
			}
			tryState = 0;
		}
		watchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_WATCH);
		tryLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_TRY);
	}

	private void UpdateVIP()
	{
		if (currentlyShownModelType == CheckSubscription.subscriptionCharacterType)
		{
			if (!vip.activeSelf)
			{
				vip.SetActive(true);
			}
		}
		else if (vip.activeSelf)
		{
			vip.SetActive(false);
		}
	}
}
