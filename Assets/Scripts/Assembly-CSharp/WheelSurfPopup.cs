using System;
using System.Collections;
using UnityEngine;

public class WheelSurfPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel freeSpinLbl;

	[SerializeField]
	private UILabel spinLbl;

	[SerializeField]
	private UISprite luckySpr;

	[SerializeField]
	private UILabel coinLabel;

	[SerializeField]
	private UISprite mask;

	[SerializeField]
	private Transform pointer;

	[SerializeField]
	private Transform wheel;

	[SerializeField]
	private bool willInitUI;

	[SerializeField]
	private int[] rewardDisplayWeights;

	[SerializeField]
	private WheelReward[] rewardItems_FrontUI;

	[SerializeField]
	private WheelReward[] rewardItems_GameOverUI;

	[SerializeField]
	private RewardUI[] rewardUIs;

	[SerializeField]
	private int targetAngel;

	[SerializeField]
	private float targetVelocity;

	[SerializeField]
	private int pointerTargetAngel;

	[SerializeField]
	private int pointerFrequency;

	[SerializeField]
	private float acc;

	[SerializeField]
	private float dec;

	[SerializeField]
	private bool isClockWise;

	[SerializeField]
	private LotteryButtonHelp buttonHelp;

	[SerializeField]
	private int[] bestGameoverRewards = new int[2];

	public static ScreenUI screenUI;

	private WheelReward[] rewardItems_Selected;

	private float[] probability_FrontUI;

	private float[] probability_GameOverUI;

	private float[] probability;

	private int currentId;

	private const int needKeys = 20;

	private float pointerDeltaAngel;

	private int total;

	private System.Random randomGen = new System.Random();

	public override void GainFocus()
	{
		base.GainFocus();
		UpdateCoinUI();
	}

	public override void Init()
	{
		base.Init();
		LotteryButtonHelp lotteryButtonHelp = buttonHelp;
		lotteryButtonHelp.OnButtonClick = (Action)Delegate.Combine(lotteryButtonHelp.OnButtonClick, new Action(OnStartRoll));
		rewardItems_Selected = rewardItems_FrontUI;
		int i = 0;
		for (int num = rewardDisplayWeights.Length; i < num; i++)
		{
			total += rewardDisplayWeights[i];
		}
		probability_FrontUI = new float[rewardItems_FrontUI.Length];
		int j = 0;
		for (int num2 = rewardItems_FrontUI.Length; j < num2; j++)
		{
			probability_FrontUI[j] = rewardItems_FrontUI[j].probability;
		}
		probability_GameOverUI = new float[rewardItems_GameOverUI.Length];
		int k = 0;
		for (int num3 = rewardItems_GameOverUI.Length; k < num3; k++)
		{
			probability_GameOverUI[k] = rewardItems_GameOverUI[k].probability;
		}
		pointerDeltaAngel = 180f / (float)pointerFrequency;
		RefreshUI();
	}

	private void RefreshUI()
	{
		if (willInitUI)
		{
			int i = 0;
			for (int num = rewardUIs.Length; i < num; i++)
			{
				rewardUIs[i].icon.spriteName = rewardItems_Selected[i].icon;
				rewardUIs[i].count.text = "X" + rewardItems_Selected[i].count;
			}
		}
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_TITLE);
		freeSpinLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE);
		spinLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_NORMAL);
		luckySpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_LOTTERY_LUCK_SPRITE);
	}

	public override void Show()
	{
		base.Show();
		mask.enabled = false;
		UpdateCoinUI();
		if (screenUI == ScreenUI.GameOverUI)
		{
			rewardItems_Selected = rewardItems_GameOverUI;
			probability = probability_GameOverUI;
		}
		else
		{
			rewardItems_Selected = rewardItems_FrontUI;
			probability = probability_FrontUI;
		}
		RefreshUI();
		RefreshLabel();
		UpdateButton();
	}

	public override void Hide()
	{
		screenUI = ScreenUI.FrontUI;
		base.Hide();
	}

	private void UpdateButton()
	{
		buttonHelp.Reload(screenUI == ScreenUI.GameOverUI);
	}

	private void UpdateCoinUI()
	{
		coinLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
	}

	public void OnStartRoll()
	{
		UpdateCoinUI();
		GameStats.Instance.gameOverPlayLotteryCount++;
		OnStart();
		Roll();
	}

	private void OnStart()
	{
		mask.enabled = true;
	}

	private void OnEnd()
	{
		FreeRewardManager.Instance.SetFreeRewardType(rewardItems_Selected[currentId], delegate
		{
			if (screenUI == ScreenUI.GameOverUI && GameStats.Instance.gameOverPlayLotteryCount >= 2)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
			else
			{
				mask.enabled = false;
				UpdateButton();
				UpdateCoinUI();
			}
		});
	}

	private void Roll()
	{
		if (screenUI == ScreenUI.GameOverUI && !PlayerPrefs.HasKey("NewPlayerFirstClickGameoverLottery"))
		{
			int num = UnityEngine.Random.Range(0, 100);
			if (num > 50)
			{
				currentId = bestGameoverRewards[0];
			}
			else
			{
				currentId = bestGameoverRewards[0];
			}
			PlayerPrefs.SetInt("NewPlayerFirstClickGameoverLottery", 1);
		}
		else
		{
			currentId = Get(probability);
		}
		PayReward(rewardItems_Selected[currentId]);
		AudioPlayer.Instance.PlaySound("lottery_sprit", true);
		GoTo(currentId);
	}

	public int Get(float[] prob)
	{
		int result = 0;
		float[] array = new float[prob.Length];
		float num = 0f;
		for (int i = 0; i < prob.Length; i++)
		{
			num = (array[i] = num + prob[i]);
		}
		int num2 = randomGen.Next(0, (int)num + 1);
		for (int j = 0; j < array.Length; j++)
		{
			if ((float)num2 <= array[j])
			{
				result = j;
				break;
			}
		}
		return result;
	}

	private int Random(double[] probability)
	{
		AliasMethod aliasMethod = new AliasMethod(probability, new System.Random((int)DateTime.UtcNow.Ticks));
		return aliasMethod.next();
	}

	private void GoTo(int id)
	{
		int num = 0;
		for (int i = 0; i < id; i++)
		{
			num += rewardDisplayWeights[i];
		}
		int num2 = num + rewardDisplayWeights[id];
		int num3 = UnityEngine.Random.Range(num + 2, num2 - 2);
		Rotato(wheel, targetVelocity, (float)targetAngel - (float)(isClockWise ? 1 : (-1)) * ((float)num3 * 360f / (float)total), acc, dec, isClockWise);
	}

	public void Rotato(Transform target, float targetSpeed, float targetAngel, float acc, float dec, bool isClockWise)
	{
		StartCoroutine(Rotato_C(target, targetSpeed, targetAngel, acc, dec, isClockWise));
	}

	[ContextMenu("Check")]
	public void CheckDataIsAdapter()
	{
		float f = targetVelocity / acc;
		float f2 = targetVelocity / dec;
		float num = 0.5f * acc * Mathf.Pow(f, 2f);
		float num2 = 0.5f * dec * Mathf.Pow(f2, 2f);
		if ((float)targetAngel < num + num2 + 360f)
		{
			targetAngel = Mathf.CeilToInt((num + num2) / 360f) * 360 + 360;
		}
		else if (targetAngel % 360 != 0)
		{
			targetAngel = (targetAngel / 360 + 1) * 360;
		}
	}

	private IEnumerator Rotato_C(Transform target, float targetSpeed, float targetAngel, float acc, float dec, bool isClockWise)
	{
		targetAngel += target.transform.localEulerAngles.z;
		float accTime = targetSpeed / acc;
		float decTime = targetSpeed / dec;
		float accAngle = 0.5f * acc * Mathf.Pow(accTime, 2f);
		float decAngle = 0.5f * dec * Mathf.Pow(decTime, 2f);
		float constTime = (targetAngel - accAngle - decAngle) / targetSpeed;
		float totalTime = accTime + decTime + constTime;
		float total2 = 0f;
		float currentTime = 0f;
		float currentVectory = 0f;
		float deltaAngel3 = 0f;
		float pointerAngel2 = 0f;
		float movementTime2 = 0f;
		float currentPointerAngel2 = 0f;
		float lastVectory2 = 0f;
		Vector3 currentEuler3;
		while (currentTime < totalTime)
		{
			movementTime2 = Time.deltaTime;
			lastVectory2 = currentVectory;
			if (currentTime + movementTime2 <= accTime)
			{
				currentVectory = CalcSpeed(currentVectory, 0f, targetSpeed, acc, movementTime2);
				deltaAngel3 = Mathf.Abs((currentVectory + lastVectory2) * movementTime2 * 0.5f);
			}
			else if (currentTime >= accTime + constTime)
			{
				currentVectory = CalcSpeed(currentVectory, 0f, targetSpeed, 0f - dec, movementTime2);
				deltaAngel3 = Mathf.Abs((currentVectory + lastVectory2) * movementTime2 * 0.5f);
			}
			else
			{
				currentVectory = targetVelocity;
				deltaAngel3 = currentVectory * movementTime2;
				if (lastVectory2 < targetVelocity)
				{
					deltaAngel3 -= (targetVelocity - lastVectory2) * (accTime - currentTime) * 0.5f;
				}
				if (currentTime + movementTime2 > accTime + constTime)
				{
					currentVectory = CalcSpeed(currentVectory, 0f, targetSpeed, 0f - dec, movementTime2 + currentTime - accTime - constTime);
					deltaAngel3 -= (targetVelocity - currentVectory) * (movementTime2 + currentTime - accTime - constTime);
				}
			}
			total2 += deltaAngel3;
			currentEuler3 = target.localEulerAngles;
			if (isClockWise)
			{
				currentEuler3.z -= deltaAngel3;
			}
			else
			{
				currentEuler3.z += deltaAngel3;
			}
			target.localEulerAngles = currentEuler3;
			currentPointerAngel2 = Mathf.Abs(currentVectory / targetVelocity) * (float)pointerTargetAngel * (float)(isClockWise ? 1 : (-1));
			pointerAngel2 = ClampAngel(currentEuler3.z) / pointerDeltaAngel * currentPointerAngel2;
			currentEuler3 = pointer.localEulerAngles;
			currentEuler3.z = pointerAngel2;
			pointer.localEulerAngles = currentEuler3;
			currentTime += movementTime2;
			yield return null;
		}
		deltaAngel3 = currentVectory * (currentTime - totalTime) * 0.5f;
		total2 += deltaAngel3;
		currentEuler3 = target.localEulerAngles;
		if (isClockWise)
		{
			currentEuler3.z -= deltaAngel3;
		}
		else
		{
			currentEuler3.z += deltaAngel3;
		}
		target.localEulerAngles = currentEuler3;
		pointer.localEulerAngles = Vector3.zero;
		OnEnd();
	}

	private float CalcSpeed(float currentSpeed, float initSpeed, float maxSpeed, float acc, float time)
	{
		currentSpeed += acc * time;
		if (Mathf.Abs(currentSpeed - initSpeed) > Mathf.Abs(maxSpeed - initSpeed))
		{
			return maxSpeed;
		}
		return currentSpeed;
	}

	private float ClampAngel(float angel)
	{
		angel = Mathf.Repeat(angel + 360f, 360f);
		for (int i = 0; i < pointerFrequency; i++)
		{
			if (angel > (float)(2 * i) * pointerDeltaAngel && angel <= (float)(2 * i + 1) * pointerDeltaAngel)
			{
				return angel - (float)(2 * i) * pointerDeltaAngel;
			}
			if (angel > (float)(2 * i + 1) * pointerDeltaAngel && angel <= (float)(2 * i + 2) * pointerDeltaAngel)
			{
				return 0f - angel + (float)(2 * i + 2) * pointerDeltaAngel;
			}
		}
		return 0f;
	}

	private void PayReward(WheelReward wr)
	{
		switch (wr.type)
		{
		case WheelRewardType.Coin:
			PlayerInfo.Instance.amountOfCoins += wr.count;
			TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, wr.count);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, wr.count);
			if (screenUI == ScreenUI.FrontUI)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_menu_lottery", 0, wr.count);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_game_over_lottery", 0, wr.count);
			}
			break;
		case WheelRewardType.Key:
			PlayerInfo.Instance.amountOfKeys += wr.count;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, wr.count);
			if (screenUI == ScreenUI.FrontUI)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_menu_lottery", 0, wr.count);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_game_over_lottery", 0, wr.count);
			}
			break;
		case WheelRewardType.Headstart:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, wr.count);
			break;
		case WheelRewardType.Scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, wr.count);
			break;
		case WheelRewardType.LeeSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.lee, wr.count);
			break;
		case WheelRewardType.TurtlefokSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.turtlefok, wr.count);
			break;
		}
	}
}
