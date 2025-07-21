using System;
using System.Collections;
using UnityEngine;

public class GameOverScreen : UIBaseScreen
{
	[SerializeField]
	private UILabel scoreTitleLabel;

	[SerializeField]
	private NumberSprite multipleNumber;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel collectedCoinLabel;

	[SerializeField]
	private GameObject showGo;

	[SerializeField]
	private Animation show_anim;

	[SerializeField]
	private Animation vip_anim;

	[SerializeField]
	private UILabel vipLbl;

	[SerializeField]
	private GameOverNewUI selectUI;

	[SerializeField]
	private Animator coinEffectAnr;

	[SerializeField]
	private UISprite hightScoreTip;

	private bool hasNative;

	private int collectedCoinsFrom;

	private int collectedCoinsTo;

	private bool countingUpCoins;

	private bool hasBeenSetupAfterAGame;

	private ScoreCounterSoundPlayer scoreCounterSoundPlayer;

	private int scoreFrom;

	private int scoreTo;

	private Color yellow = new Color(1f, 0.8745098f, 0.04705882f);

	private bool showTryRole;

	public override void Init()
	{
		base.Init();
		scoreCounterSoundPlayer = GetComponent<ScoreCounterSoundPlayer>();
		selectUI.Init(this);
	}

	public void SetupBeforeChest()
	{
		hasBeenSetupAfterAGame = true;
		scoreLabel.text = string.Empty + GameStats.Instance.score;
		multipleNumber.SetLevelNumber(PlayerInfo.Instance.amountOfLevel);
		scoreFrom = GameStats.Instance.score;
	}

	public void SetupAfterChest()
	{
		if (hasBeenSetupAfterAGame)
		{
			TrialManager.Instance.End();
			Game.Instance.ResetTest();
			int coins = GameStats.Instance.coins;
			collectedCoinsFrom = 0;
			collectedCoinsTo = coins;
			scoreTo = scoreFrom + GameStats.CoinToScoreConversion(coins);
			if (coins != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.GetExactlyAmountOfCoins, coins);
			}
			showGo.SetActive(true);
			vip_anim.gameObject.SetActive(false);
			show_anim.Play();
			hasBeenSetupAfterAGame = false;
		}
	}

	public void StartCountUpCoins()
	{
		if (GameStats.Instance.coins != 0)
		{
			StartCoroutine("CountUpCoins");
		}
		else
		{
			selectUI.PlayFootAnim();
		}
	}

	private IEnumerator CountUpCoins()
	{
		float countFactor = 0f;
		float countTime = Mathf.Lerp(0.3f, 3f, (float)collectedCoinsTo / 200f);
		countingUpCoins = true;
		UpdateDoubleCoinLabels();
		collectedCoinLabel.text = collectedCoinsFrom.ToString();
		yield return new WaitForSeconds(0.2f);
		countFactor = 0f;
		scoreCounterSoundPlayer.PlayCoinSound(countFactor);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			scoreLabel.text = Mathf.Round(Mathf.SmoothStep(scoreFrom, scoreTo, countFactor)).ToString();
			collectedCoinLabel.text = Mathf.Round(Mathf.SmoothStep(collectedCoinsFrom, collectedCoinsTo, countFactor)).ToString();
			yield return null;
		}
		scoreCounterSoundPlayer.StopScoreSound();
		scoreLabel.text = scoreTo.ToString();
		collectedCoinLabel.text = collectedCoinsTo.ToString();
		countingUpCoins = false;
		CountUpCompleted();
		if (PlayerInfo.Instance.hasSubscribed)
		{
			StartCoroutine("CountUpVipCoins");
			yield break;
		}
		selectUI.ShowDownGo(showTryRole, collectedCoinsTo);
		yield return new WaitForSeconds(0.1f);
		ShowSubcribe();
		ShowPassLevelAD();
	}

	private IEnumerator CountUpVipCoins()
	{
		vip_anim.gameObject.SetActive(true);
		vip_anim.Play();
		coinEffectAnr.gameObject.SetActive(true);
		coinEffectAnr.Play("ui_jiesuan", 0, 0f);
		collectedCoinsFrom = collectedCoinsTo;
		collectedCoinsTo *= 2;
		float countFactor = 0f;
		float countTime = Mathf.Lerp(0.3f, 3f, (float)collectedCoinsTo / 200f);
		scoreCounterSoundPlayer.PlayCoinSound(countFactor);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			collectedCoinLabel.text = Mathf.Round(Mathf.SmoothStep(collectedCoinsFrom, collectedCoinsTo, countFactor)).ToString();
			yield return null;
		}
		scoreCounterSoundPlayer.StopScoreSound();
		selectUI.ShowDownGo(showTryRole, collectedCoinsTo);
		yield return new WaitForSeconds(0.1f);
		ShowSubcribe();
		ShowPassLevelAD();
	}

	private void ShowSubcribe()
	{
		if (PlayerInfo.Instance.numberOfRuns != 2 && GameStats.Instance.coins > 500 && (DateTime.UtcNow - PlayerInfo.Instance.firstInstallDate).Days > 1 && PlayerPrefs.GetInt("ShowSubscribeCount") < 1)
		{
			PlayerPrefs.SetInt("ShowSubscribeCount", PlayerPrefs.GetInt("ShowSubscribeCount") + 1);
			UIScreenController.Instance.QueuePopup("SubscribePopup");
		}
	}

	private void ShowPassLevelAD()
	{
		if (PlayerInfo.Instance.numberOfRuns != 2 && hasNative && !UIScreenController.Instance.isShowingPopup)
		{
			if (!UIScreenController.Instance.isShowingPopup)
			{
				float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
				if (Mathf.Abs(num - 0.5625f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
				}
				else if (Mathf.Abs(num - 0.6667f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
				}
				else if (Mathf.Abs(num - 0.75f) < 0.01f)
				{
					RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
				}
				else
				{
					RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
				}
			}
			else
			{
				base.LooseFocus();
			}
		}
		if (PlayerInfo.Instance.numberOfRuns == 2)
		{
			UIScreenController.Instance.PushPopup("RatingPopup");
		}
	}

	private void CountUpCompleted()
	{
	}

	public bool IsCountingUpCoins()
	{
		return countingUpCoins;
	}

	private void PushNewHighScoreCelebrationScreen(int newHighSchore)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.NewHighScore;
		celebrationReward.rewardType = CelebrationRewardType.highscore;
		celebrationReward.characterType = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		celebrationReward.characterThemeIndex = PlayerInfo.Instance.currentThemeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
	}

	private void OnEnable()
	{
		coinEffectAnr.gameObject.SetActive(false);
		//coinEffectAnr.Stop();
		coinEffectAnr.enabled = false;
		RiseSdkListener.OnAdEvent -= RewardAdSuc;
		RiseSdkListener.OnAdEvent += RewardAdSuc;
		if (!UIScreenController.Instance.CheckNetwork() || !RiseSdk.Instance.HasNativeAd("loading") || PlayerInfo.Instance.numberOfRuns == 2 || PlayerInfo.Instance.hasRemoveAd)
		{
			hasNative = false;
			showGo.transform.localPosition = new Vector3(0f, 655f, 0f);
		}
		else
		{
			hasNative = true;
			showGo.transform.localPosition = new Vector3(0f, 395f, 0f);
		}
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= RewardAdSuc;
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void RewardAdSuc(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished)
		{
			if (id == 4)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_double", 0);
				DoubleCoinsSet();
				selectUI.AfterDoubleCoins(GameStats.Instance.coins);
				PlayerInfo.Instance.gameOverDoubleCoinsShowCountLastDay++;
			}
			if (id == 10)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_try_endless", 0);
				selectUI.AfterTryCharacter();
				SaveMeManager.ResetSaveMeForNewRun();
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
			}
		}
	}

	public void DoubleCoinsSet()
	{
		int coins = GameStats.Instance.coins;
		int num = PlayerInfo.Instance.CheckGameOverDoubleCoinViewRate(coins);
		collectedCoinLabel.color = yellow;
		if (PlayerInfo.Instance.hasSubscribed)
		{
			collectedCoinLabel.text = string.Empty + coins * num * 2;
		}
		else
		{
			collectedCoinLabel.text = string.Empty + coins * num;
		}
	}

	public override void GainFocus()
	{
		base.GainFocus();
		if (hasNative)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.gameOverFullAdCount++;
		showGo.SetActive(false);
		selectUI.Show();
		scoreTitleLabel.text = Strings.Get(LanguageKey.UI_SCREEN_GAME_OVER_TITLE);
		vipLbl.text = Strings.Get(LanguageKey.GAMEOVER_VIPTIP);
		hightScoreTip.enabled = false;
		UISliderInController.Instance.Stop = true;
		int num = GameStats.Instance.score + GameStats.CoinToScoreConversion(GameStats.Instance.coins);
		bool flag = num > PlayerInfo.Instance.highestScore;
		PlayerInfo.Instance.highestScore = num;
		collectedCoinLabel.text = "0";
		if (flag)
		{
			hightScoreTip.enabled = true;
			HighestScoreSystem.Instance.End(num);
			PushNewHighScoreCelebrationScreen(num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.BeatOwnHighscore);
		}
		if (!TrialManager.Instance.IsInTest() && !Game.Instance.IsInTest() && Game.Instance.GetDuration() > 120f && PlayerInfo.Instance.CheckGameoverUITry())
		{
			showTryRole = true;
			PlayerInfo.Instance.ShowGameoverUITry();
		}
		else
		{
			showTryRole = false;
		}
		if (TrialManager.Instance.IsTestHelm)
		{
			PlayerInfo.Instance.IncreaseTrialProgress(GameStats.Instance.coinsWithHelmet);
		}
		else if (TrialManager.Instance.IsTestChar)
		{
			PlayerInfo.Instance.IncreaseTrialProgress((int)(Game.Instance.GetDuration() * 1000f));
		}
		RiseSdk.Instance.enableBackHomeAd(false, "custom");
		SetupBeforeChest();
		if (RewardManager.rewardsToUnlockCount > 0)
		{
			UIScreenController.Instance.QueuePopup("CelebrationPopup");
		}
		else if (GameStats.Instance.chestPickups > 0)
		{
			ShopManager.Instance.chestType = ChestType.Game;
			UIScreenController.Instance.QueuePopup("Box_Open");
		}
		else
		{
			SetupAfterChest();
		}
		TasksManager.Instance.CheckPlayerLevel();
		if (PlayerInfo.Instance.amountOfLevel / 6 != 0 && PlayerInfo.Instance.amountOfLevel % 6 == 0 && PlayerInfo.Instance.amountOfLevel > PlayerPrefs.GetInt("PreShowRatingLevel"))
		{
			PlayerPrefs.SetInt("PreShowRatingLevel", PlayerInfo.Instance.amountOfLevel);
			UIScreenController.Instance.QueuePopup("RatingPopup");
		}
	}

	public override void Hide()
	{
		UISliderInController.Instance.Stop = false;
		selectUI.Hide();
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom");
		}
		base.Hide();
	}

	private void UpdateDoubleCoinLabels()
	{
		collectedCoinLabel.color = Color.white;
	}
}
