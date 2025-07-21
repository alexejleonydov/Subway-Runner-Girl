using System;
using UnityEngine;

public class GameOverNewUI : MonoBehaviour
{
	[SerializeField]
	private UILabel luckyWheelLbl;

	[SerializeField]
	private UILabel tryItWatchLbl;

	[SerializeField]
	private UILabel claimLbl;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private GameObject downGo;

	[SerializeField]
	private GameObject upGo;

	[SerializeField]
	private float midSpringStrength;

	[SerializeField]
	private GameObject claimGo;

	[SerializeField]
	private ParticleSystem ps_claim;

	[SerializeField]
	private ParticleSystem ps_lotter_try;

	[SerializeField]
	private UILabel label_claim;

	[SerializeField]
	private GameObject doubleViewGo;

	[SerializeField]
	private UILabel label_double;

	[SerializeField]
	private UISprite sprite_doubleRate;

	[SerializeField]
	private UISprite[] doubleViewSpr;

	[SerializeField]
	private UILabel doubleAmountLbl;

	[SerializeField]
	private GameObject lotteryGo;

	[SerializeField]
	private GameObject tryCharacterGo;

	[SerializeField]
	private UISprite tryIconSpr;

	[SerializeField]
	private TweenScale ts_up;

	[SerializeField]
	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	[SerializeField]
	private Animation foot_anim;

	[SerializeField]
	private float footY;

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private Animation up_anim;

	private Color doubleAmountLblOriginColor;

	private int remoteValue;

	private bool showTryRole;

	private bool showConfirmPopup;

	private bool isAfterDoubleClick;

	private int gameOverDoubleCoinViewRate;

	private int doubleState;

	private TrialInfo trialInfo;

	private GameOverScreen gameOverScreen;

	public void Init(GameOverScreen screen)
	{
		gameOverScreen = screen;
		UIEventListener uIEventListener = UIEventListener.Get(claimGo);
		uIEventListener.onClick = OnClaimClick;
		uIEventListener = UIEventListener.Get(tryCharacterGo);
		uIEventListener.onClick = OnTryClick;
		uIEventListener = UIEventListener.Get(doubleViewGo);
		uIEventListener.onClick = OnDoubleClick;
		uIEventListener = UIEventListener.Get(lotteryGo);
		uIEventListener.onClick = OnLotteryClick;
		base.gameObject.SetActive(false);
		showTryRole = false;
		doubleAmountLblOriginColor = doubleAmountLbl.color;
	}

	private void OnEnable()
	{
		GameStats.Instance.OnGameOverPlayLotteryCountIncreased = (Action)Delegate.Combine(GameStats.Instance.OnGameOverPlayLotteryCountIncreased, new Action(OnGameOverPlayLotteryCountIncreased));
	}

	private void OnDisable()
	{
		GameStats.Instance.OnGameOverPlayLotteryCountIncreased = (Action)Delegate.Remove(GameStats.Instance.OnGameOverPlayLotteryCountIncreased, new Action(OnGameOverPlayLotteryCountIncreased));
	}

	private void RefreshLabel()
	{
		claimLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_CLAIM);
		adLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_AD_MORE_TIP);
		luckyWheelLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_LOTTERY);
		tryItWatchLbl.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_BUTTON_TRY);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		if (downGo.activeSelf)
		{
			downGo.SetActive(false);
		}
		if (upGo.activeSelf)
		{
			upGo.SetActive(false);
		}
		isAfterDoubleClick = false;
		foot_anim.transform.localPosition = Vector3.down * footY;
		GameStats.Instance.gameOverPlayLotteryCount = 0;
		showConfirmPopup = false;
		RefreshLabel();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void ShowDownGo(bool showTryRole, int coins)
	{
		downGo.SetActive(true);
		anim.Play();
		this.showTryRole = showTryRole;
		gameOverDoubleCoinViewRate = PlayerInfo.Instance.CheckGameOverDoubleCoinViewRate(coins);
		label_claim.text = coins.ToString();
		label_double.text = (coins * gameOverDoubleCoinViewRate).ToString();
		sprite_doubleRate.spriteName = "icon_X" + gameOverDoubleCoinViewRate;
		sprite_doubleRate.MakePixelPerfect();
		if (RiseSdk.Instance.HasRewardAd())
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_double", 0);
			doubleState = 1;
			int i = 0;
			for (int num = doubleViewSpr.Length; i < num; i++)
			{
				doubleViewSpr[i].color = Color.white;
			}
			doubleAmountLbl.color = doubleAmountLblOriginColor;
		}
		else
		{
			doubleState = 2;
			int j = 0;
			for (int num2 = doubleViewSpr.Length; j < num2; j++)
			{
				doubleViewSpr[j].color = Color.cyan;
			}
			doubleAmountLbl.color = Color.white;
		}
	}

	public void AfterTryCharacter()
	{
		if (trialInfo.type == TrialType.Character)
		{
			Game.Instance.TestCharacter(trialInfo.characterType, trialInfo.characterThemeId);
		}
		else if (trialInfo.type == TrialType.Helmet)
		{
			Game.Instance.TestHelmet(trialInfo.helmetType);
		}
		if (showConfirmPopup)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		if (upGo.activeInHierarchy)
		{
			upGo.SetActive(false);
		}
	}

	private void OnDoubleClick(GameObject go)
	{
		if (doubleState == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			RiseSdk.Instance.TrackEvent("click_video_double", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_double", 0);
			if (PlayerInfo.Instance.gameOverDoubleConfirmNoRemind)
			{
				if (UIScreenController.Instance.CheckNetwork())
				{
					if (RiseSdk.Instance.HasRewardAd())
					{
						VideoLoadingPopup.adType = 2;
						VideoLoadingPopup.rewardId = 4;
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
				showConfirmPopup = false;
			}
			else
			{
				showConfirmPopup = true;
				UIScreenController.Instance.PushPopup("ConfirmPopup");
			}
		}
		else if (doubleState == 2 && PlayerInfo.Instance.gameOverDoubleConfirmNoRemind)
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

	private void OnTryClick(GameObject go)
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_try_endless", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_try_endless", 0);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 10;
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

	public void AfterDoubleCoins(int coins)
	{
		isAfterDoubleClick = true;
		if (PlayerInfo.Instance.hasSubscribed)
		{
			if (coins > 0)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, coins * gameOverDoubleCoinViewRate * 2, HideDown);
			}
		}
		else if (coins > 0)
		{
			FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, coins * gameOverDoubleCoinViewRate, HideDown);
		}
		PlayerInfo.Instance.ResetGameOverDoubleCoinViewRate();
	}

	private void HideDown()
	{
		if (showConfirmPopup)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		downGo.SetActive(false);
		if (PlayerInfo.Instance.forceNextCityOrder > 0)
		{
			UIScreenController.Instance.PushPopup("UnlockNewScreenPopup");
		}
		foot_anim.Play();
		if (!isAfterDoubleClick)
		{
			ShowLotteryBtn(showTryRole);
		}
	}

	public void ShowLotteryBtn(bool showTryRole)
	{
		trialInfo = TrialManager.Instance.SelectValidlyTrialInfo();
		if (trialInfo == null)
		{
			showTryRole = false;
		}
		else
		{
			tryIconSpr.spriteName = trialInfo.icon;
		}
		if (RiseSdk.Instance.HasRewardAd())
		{
			if (!upGo.activeInHierarchy)
			{
				upGo.SetActive(true);
			}
			up_anim.Play();
			if (tryCharacterGo.activeInHierarchy != showTryRole)
			{
				tryCharacterGo.SetActive(showTryRole);
			}
			if (lotteryGo.activeInHierarchy == showTryRole)
			{
				lotteryGo.SetActive(!showTryRole);
			}
			if (showTryRole)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_try_endless", 0);
			}
			ps_lotter_try.Play();
			ts_up.PlayForward();
		}
	}

	private void OnClaimClick(GameObject go)
	{
		isAfterDoubleClick = false;
		int num = GameStats.Instance.coins * ((!PlayerInfo.Instance.hasSubscribed) ? 1 : 2);
		PlayerInfo.Instance.amountOfCoins += num;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, num);
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_in_game_run", 0, num);
		PlayerInfo.Instance.DonotClickDoubleCoinView();
		ps_claim.Play();
		HideDown();
	}

	private void OnLotteryClick(GameObject go)
	{
		WheelSurfPopup.screenUI = ScreenUI.GameOverUI;
		UIScreenController.Instance.PushPopup("LotteryPopup");
	}

	private void OnGameOverPlayLotteryCountIncreased()
	{
		if (upGo.activeInHierarchy)
		{
			upGo.SetActive(false);
		}
	}

	public void PlayFootAnim()
	{
		foot_anim.Play();
	}
}
