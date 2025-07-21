using System;
using UnityEngine;

public class TrialPopup : UIBaseScreen, IPurchaseHandler
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel tryLbl;

	[SerializeField]
	private UILabel messageLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private UILabel skillLbl;

	[SerializeField]
	private UILabel skill1Lbl;

	[SerializeField]
	private UILabel skill2Lbl;

	[SerializeField]
	private UILabel tipTitleLbl;

	[SerializeField]
	private UILabel tipOneLbl;

	[SerializeField]
	private UILabel tipTwoLbl;

	[SerializeField]
	private UILabel tipThreeLbl;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel sliderLbl;

	[SerializeField]
	private UISprite buyIcon;

	[SerializeField]
	private UISprite tryFill;

	[SerializeField]
	private UISprite tryIcon;

	[SerializeField]
	private UISprite skill1Icon;

	[SerializeField]
	private UISprite skill2Icon;

	[SerializeField]
	private GameObject tryGo;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject tipGo;

	[SerializeField]
	private UISprite[] slots;

	private TrialInfo _trialInfo;

	private Vector3 _tryLocalPos;

	private Vector3 _buyLocalPos;

	private Color _tryLblColor;

	private bool _purchaseInProgress;

	private int _tryState;

	public static bool startNewGame;

	public override void Init()
	{
		UIEventListener uIEventListener = UIEventListener.Get(tryGo);
		uIEventListener.onClick = OnTryClick;
		_tryLocalPos = tryGo.transform.localPosition;
		_buyLocalPos = buyGo.transform.localPosition;
		_tryLblColor = tryLbl.color;
		base.Init();
	}

	public override void Show()
	{
		base.Show();
		if (startNewGame)
		{
			tryGo.transform.localPosition = new Vector3(0f, _tryLocalPos.y, 0f);
			buyGo.SetActive(false);
		}
		else
		{
			tryGo.transform.localPosition = _tryLocalPos;
			buyGo.transform.localPosition = _buyLocalPos;
			buyGo.SetActive(true);
		}
		_trialInfo = TrialManager.Instance.currentTrialInfo;
		if (_trialInfo.type == TrialType.Character)
		{
			Characters.Model model = Characters.characterData[_trialInfo.characterType];
			CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(_trialInfo.characterType, _trialInfo.characterThemeId);
			Characters.UnlockType unlockType = model.unlockType;
			int price = model.Price;
			if (themeForCharacter != null)
			{
				unlockType = themeForCharacter.unlockType;
				price = themeForCharacter.price;
			}
			if (unlockType == Characters.UnlockType.coins)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			if (unlockType == Characters.UnlockType.keys)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
			}
			priceLbl.text = price.ToString();
		}
		else if (_trialInfo.type == TrialType.Helmet)
		{
			Helmets.Helm helm = Helmets.helmData[_trialInfo.helmetType];
			if (helm.unlockType == Helmets.UnlockType.coins)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			if (helm.unlockType == Helmets.UnlockType.keys)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
			}
			priceLbl.text = helm.price.ToString();
		}
		ReloadUI();
		ReloadTime();
	}

	protected override void AfterShow()
	{
		if (_trialInfo.type == TrialType.Character)
		{
			UIModelController.Instance.ActivateTrailRoleModel(_trialInfo.characterType, _trialInfo.characterThemeId);
		}
		else if (_trialInfo.type == TrialType.Helmet)
		{
			UIModelController.Instance.ActivateTrailHelmetModel(_trialInfo.helmetType);
		}
	}

	private void ReloadUI()
	{
		if (_trialInfo.type == TrialType.Character)
		{
			titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TITLE_R);
			skillLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_R);
			Characters.Model model = Characters.characterData[_trialInfo.characterType];
			if (model.freeReviveCount > 0)
			{
				skill1Icon.enabled = true;
				skill1Icon.spriteName = "Try_r_skill_icon1";
				skill1Lbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_R), model.freeReviveCount);
			}
			else
			{
				skill1Icon.enabled = false;
				skill1Lbl.text = string.Empty;
			}
			skill2Lbl.text = string.Empty;
			skill2Icon.enabled = false;
			tipTitleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_TITLE);
			tipOneLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT1);
			tipTwoLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT2);
			tipThreeLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_R_CONTENT3);
		}
		else if (_trialInfo.type == TrialType.Helmet)
		{
			titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TITLE_H);
			skillLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_H);
			skill1Lbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_H);
			skill2Lbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL2_H);
			skill1Icon.enabled = true;
			skill2Icon.enabled = true;
			skill1Icon.spriteName = "Try_h_skill_icon1";
			skill2Icon.spriteName = "Try_h_skill_icon2";
			tipTitleLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_TITLE);
			tipOneLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT1);
			tipTwoLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT2);
			tipThreeLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FILL_H_CONTENT3);
		}
		tryLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_FREE);
		messageLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_BOTTOM_INFO), _trialInfo.aim);
		RefreshSlots(PlayerInfo.Instance.CurrentTrialInfoLevel(), _trialInfo.aim);
		if (RiseSdk.Instance.HasRewardAd())
		{
			_tryState = 1;
			tryFill.color = Color.white;
			tryIcon.color = Color.white;
			tryLbl.color = _tryLblColor;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_Try_Popup", 0);
		}
		else
		{
			_tryState = 2;
			tryFill.color = Color.cyan;
			tryIcon.color = Color.cyan;
			tryLbl.color = Color.cyan;
		}
	}

	private void RefreshSlots(int cur, int aim)
	{
		int i = 0;
		for (int num = slots.Length; i < num; i++)
		{
			if (i < cur)
			{
				slots[i].enabled = true;
				slots[i].spriteName = "Try_h_h_slot2";
			}
			else if (i < aim)
			{
				slots[i].enabled = true;
				slots[i].spriteName = "Try_h_h_slot1";
			}
			else
			{
				slots[i].enabled = false;
			}
		}
		sliderLbl.text = cur + "/" + aim;
	}

	private void ReloadTime()
	{
		TimeSpan timeSpan = TrialManager.Instance.begainDateTime.AddDays(PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow;
		if (timeSpan.Ticks < 0)
		{
			timeLbl.text = "----";
			return;
		}
		timeLbl.text = timeSpan.Days + Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TIME_D) + " " + timeSpan.Hours + Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_TIME_H);
	}

	protected override void PrepareHide()
	{
		UIModelController.Instance.ClearModels();
	}

	public override void Hide()
	{
		if (startNewGame)
		{
			SaveMeManager.ResetSaveMeForNewRun();
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
		}
		startNewGame = false;
		base.Hide();
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

	public void OnBuyClick()
	{
		if (_trialInfo.type == TrialType.Character)
		{
			PurchaseHandler.Instance.PurchaseCharacter(_trialInfo.characterType, _trialInfo.characterThemeId, true, delegate
			{
				switch (Characters.characterOrder.IndexOf(_trialInfo.characterType))
				{
				case 0:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles1st", 0);
					break;
				case 1:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles2nd", 0);
					break;
				case 2:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles3rd", 0);
					break;
				case 3:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles4th", 0);
					break;
				case 4:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles5th", 0);
					break;
				case 5:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles6th", 0);
					break;
				case 6:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles7th", 0);
					break;
				case 7:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles8th", 0);
					break;
				case 8:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles9th", 0);
					break;
				case 9:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles10th", 0);
					break;
				case 10:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles11th", 0);
					break;
				case 11:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles12th", 0);
					break;
				case 12:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles13th", 0);
					break;
				}
				UIScreenController.Instance.ClosePopup(null);
				UIScreenController.Instance.ShowUnlockAnimationForCharacter(_trialInfo.characterType, _trialInfo.characterThemeId);
				UIModelController.Instance.SelectCharacterForPlay(_trialInfo.characterType, _trialInfo.characterThemeId);
				TrialManager.Instance.currentTrialInfo = null;
			});
		}
		else if (_trialInfo.type == TrialType.Helmet)
		{
			PurchaseHandler.Instance.PurchaseHelmet(_trialInfo.helmetType, this);
		}
	}

	public void OnTryClick(GameObject go)
	{
		if (_tryState == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			RiseSdk.Instance.TrackEvent("click_video_Try_Popup", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_Try_Popup", 0);
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 11;
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
		else if (_tryState == 2)
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
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 11)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_Try_Popup", 0);
			TrialManager.Instance.Begin();
			startNewGame = false;
			UIScreenController.Instance.ClosePopup(null);
			Game.Instance.StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
	}

	public void OnQuestionClick()
	{
		UIModelController.Instance.ActivateTutorialPopup(false);
		if (!tipGo.activeInHierarchy)
		{
			tipGo.SetActive(true);
		}
	}

	public void OnQuestionClose()
	{
		UIModelController.Instance.ActivateTutorialPopup(true);
		if (tipGo.activeInHierarchy)
		{
			tipGo.SetActive(false);
		}
	}

	public override void GainFocus()
	{
		base.GainFocus();
		UIModelController.Instance.ActivateTutorialPopup(true);
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		UIModelController.Instance.ActivateTutorialPopup(false);
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		switch (Helmets.helmOrder.IndexOf(_trialInfo.helmetType))
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
		UIScreenController.Instance.ClosePopup(null);
		UIScreenController.Instance.ShowUnlockAnimationForHelmet(_trialInfo.helmetType);
		PlayerInfo.Instance.currentHelmet = _trialInfo.helmetType;
		TrialManager.Instance.currentTrialInfo = null;
	}
}
