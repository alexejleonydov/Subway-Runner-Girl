using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class TaskList : MonoBehaviour
{
	[SerializeField]
	private UILabel[] taskDescriptions = new UILabel[3];

	[SerializeField]
	private GameObject[] CompleteGameObjs = new GameObject[3];

	[SerializeField]
	private UILabel[] completeLabel = new UILabel[3];

	[SerializeField]
	private UISlider[] taskProgressSilders = new UISlider[3];

	[SerializeField]
	private UILabel[] progressLabel = new UILabel[3];

	[SerializeField]
	private GameObject[] getRewardButtons = new GameObject[3];

	[SerializeField]
	private UILabel[] getRewardLabel = new UILabel[3];

	[SerializeField]
	private UILabel[] taskReward_Coin = new UILabel[3];

	[SerializeField]
	private UILabel[] taskReward_Exp = new UILabel[3];

	[SerializeField]
	private UILabel[] rewardTitleLabel = new UILabel[3];

	[SerializeField]
	private ParticleSystem[] getRewardPar = new ParticleSystem[3];

	[SerializeField]
	private UILabel waitNextTaskTime;

	[SerializeField]
	private UILabel waitNextTaskDescription;

	[SerializeField]
	private UILabel waitNextTaskBtnLabel;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private Fly fly;

	[SerializeField]
	private UITexture headTexture;

	[SerializeField]
	public UILabel levelLabel;

	[SerializeField]
	public UILabel expLabel;

	[SerializeField]
	public UISlider expSlider;

	private TaskInfo[] _cachedTaskInfo = new TaskInfo[3];

	private int _cachedTaskSet;

	private TaskInfo[] _currentTasks;

	private bool hasCached;

	private int rewardCoinsNum;

	private int rewardExpNum;

	private int oriExp;

	private int oriLevel;

	private BoxCollider[] getBtnColliders = new BoxCollider[3];

	private int oneOfEach
	{
		get
		{
			int num = 4;
			if (Game.Instance.IsInGame.Value)
			{
				if (GameStats.Instance.doubleMultiplierPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.coinMagnetsPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.superShoesPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.flypackPickups != 0)
				{
					num--;
				}
			}
			return num;
		}
	}

	public void Show()
	{
		if (UIScreenController.Instance.GetTopScreenName().Equals("FrontUI"))
		{
			for (int i = 0; i < getRewardButtons.Length; i++)
			{
				getBtnColliders[i] = getRewardButtons[i].GetComponent<BoxCollider>();
			}
			for (int j = 0; j < completeLabel.Length; j++)
			{
				completeLabel[j].text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_COMPLETED_GOTTENLABEL);
			}
			for (int k = 0; k < getRewardLabel.Length; k++)
			{
				getRewardLabel[k].text = Strings.Get(LanguageKey.UI_POPUP_DAILY_BUTTON_GET);
			}
			levelLabel.text = "LV. " + PlayerInfo.Instance.amountOfLevel;
			oriExp = PlayerInfo.Instance.amountOfExp;
			oriLevel = PlayerInfo.Instance.amountOfLevel;
			RefreshExp(oriLevel, oriExp);
			RefreshLevel(oriLevel);
			RefreshHeadUI();
		}
		InitGetRewardButton();
		TasksManager.Instance.CheckGetNextTask(RefreshContents);
	}

	private void InitGetRewardButton()
	{
		if (UIScreenController.Instance.GetTopScreenName().Equals("FrontUI"))
		{
			UIEventListener uIEventListener = UIEventListener.Get(getRewardButtons[0]);
			uIEventListener.onClick = OnClickGetReward;
			uIEventListener = UIEventListener.Get(getRewardButtons[1]);
			uIEventListener.onClick = OnClickGetReward;
			uIEventListener = UIEventListener.Get(getRewardButtons[2]);
			uIEventListener.onClick = OnClickGetReward;
		}
		for (int i = 0; i < rewardTitleLabel.Length; i++)
		{
			rewardTitleLabel[i].text = Strings.Get(LanguageKey.TASK_POPUP_REWARD);
		}
	}

	private void OnClickGetReward(GameObject go)
	{
		switch (go.name)
		{
		case "Claim1":
			PlayerInfo.Instance.SetCurrentTaskReward(0, true);
			getRewardPar[0].Play();
			fly.gameObject.transform.localPosition = new Vector3(185f, 40f, 250f);
			break;
		case "Claim2":
			PlayerInfo.Instance.SetCurrentTaskReward(1, true);
			getRewardPar[1].Play();
			fly.gameObject.transform.localPosition = new Vector3(185f, -140f, 250f);
			break;
		case "Claim3":
			PlayerInfo.Instance.SetCurrentTaskReward(2, true);
			getRewardPar[2].Play();
			fly.gameObject.transform.localPosition = new Vector3(185f, -340f, 250f);
			break;
		}
		PlayerInfo.Instance.amountOfCoins += rewardCoinsNum;
		if (rewardCoinsNum > 0)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_mission_reward", 0, rewardCoinsNum);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, rewardCoinsNum);
		}
		oriExp = PlayerInfo.Instance.amountOfExp;
		oriLevel = PlayerInfo.Instance.amountOfLevel;
		PlayerInfo.Instance.amountOfExp += rewardExpNum;
		GetRewardButtomEnable(false);
		StartCoroutine(WaitFlyStop(go, rewardExpNum));
		StartCoroutine(CountUp(rewardExpNum));
	}

	private IEnumerator CountUp(int amount)
	{
		yield return new WaitForSeconds(0.15f);
		float countFactor = 0f;
		float countTime = Mathf.Lerp(1.5f, 3f, (float)amount / 1000f);
		int from = 0;
		int count2 = 0;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			count2 = Mathf.RoundToInt(Mathf.SmoothStep(from, amount, countFactor));
			AddExps(count2);
			yield return null;
		}
		RefreshExp(PlayerInfo.Instance.amountOfLevel, PlayerInfo.Instance.amountOfExp);
	}

	private IEnumerator WaitFlyStop(GameObject gameObject, int amount)
	{
		yield return StartCoroutine(fly.Begain());
		CheckPlayerLevel();
		TasksManager.Instance.CheckGetNextTask(RefreshContents, RefreshTime);
		GetRewardButtomEnable(true);
	}

	private void GetRewardButtomEnable(bool enable)
	{
		for (int i = 0; i < getBtnColliders.Length; i++)
		{
			getBtnColliders[i].enabled = enable;
		}
	}

	public void AddExps(int add)
	{
		int num = oriLevel;
		int num2 = oriExp + add;
		float num3 = Game.Instance.GetExpCoefficient(num) * (float)(num - 1) + 8f;
		if ((float)num2 >= num3)
		{
			num2 -= (int)num3;
			AddLevel(1);
		}
		RefreshExp(oriLevel, num2);
	}

	private void AddLevel(int add)
	{
		RefreshLevel(oriLevel + add);
	}

	private void RefreshLevel(int amount)
	{
		levelLabel.text = "LV. " + amount;
	}

	private void RefreshExp(int level, int amount)
	{
		float num = Game.Instance.GetExpCoefficient(level) * (float)(level - 1) + 8f;
		expLabel.text = amount + "/" + num;
		expSlider.value = (float)amount / num;
	}

	private void CheckPlayerLevel()
	{
		float num = Game.Instance.GetExpCoefficient() * (float)(PlayerInfo.Instance.amountOfLevel - 1) + 8f;
		float num2 = 0f;
		num2 = PlayerInfo.Instance.amountOfExp;
		if (num2 >= num)
		{
			PlayerInfo.Instance.amountOfLevel++;
			PlayerInfo.Instance.amountOfExp = (int)(num2 - num);
			UIScreenController.Instance.PushPopup("LevelUpPopup");
		}
	}

	private void RefreshTime()
	{
		LevelExpManager.Instance.SetNewTime();
	}

	public void OnClickSkipWaitTime()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_mission_refresh", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_mission_refresh", 0);
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 17;
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
		waitNextTaskDescription.text = Strings.Get(LanguageKey.TASK_POPUP_NEXT_MISSION_IN);
		if (waitNextTaskBtnLabel != null)
		{
			waitNextTaskBtnLabel.text = Strings.Get(LanguageKey.TASK_POPUP_REFRESH_MISSION);
		}
		if (adLbl != null)
		{
			adLbl.text = Strings.Get(LanguageKey.UI_POPUP_TASK_AD_MORE_MISSION_TIP);
		}
		RiseSdkListener.OnAdEvent -= OnFreeView;
		RiseSdkListener.OnAdEvent += OnFreeView;
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeView;
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}

	private void OnFreeView(RiseSdk.AdEventType aet, int id, string tag, int type)
	{
		if (aet == RiseSdk.AdEventType.RewardAdShowFinished && id == 17)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_mission_refresh", 0);
			LevelExpManager.Instance.ForceCoolingDownOver();
			TasksManager.Instance.CheckGetNextTask(RefreshContents);
		}
	}

	private void RefreshContents(bool showWaitingTime)
	{
		_currentTasks = TasksManager.Instance.GetTaskInfo();
		hasCached = true;
		foreach (KeyValuePair<LevelExpManager.LevelAwardType, int> taskLevelReward in Game.Instance.GetTaskLevelRewards(PlayerInfo.Instance.GetTaskPlayerLevel))
		{
			if (taskLevelReward.Key == LevelExpManager.LevelAwardType.Coin)
			{
				rewardCoinsNum = taskLevelReward.Value;
			}
			if (taskLevelReward.Key == LevelExpManager.LevelAwardType.Exp)
			{
				rewardExpNum = taskLevelReward.Value;
			}
		}
		if (showWaitingTime || !LevelExpManager.Instance.IsCoolingDownOver())
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_mission_refresh", 0);
			waitNextTaskDescription.transform.parent.gameObject.SetActive(true);
			for (int i = 0; i < taskDescriptions.Length; i++)
			{
				taskDescriptions[i].transform.parent.gameObject.SetActive(false);
			}
			return;
		}
		waitNextTaskDescription.transform.parent.gameObject.SetActive(false);
		for (int j = 0; j < taskDescriptions.Length; j++)
		{
			taskDescriptions[j].transform.parent.gameObject.SetActive(true);
			LabelAndNumberUpdate(j, taskDescriptions[j], taskProgressSilders[j], progressLabel[j], getRewardButtons[j], CompleteGameObjs[j]);
			taskReward_Coin[j].text = rewardCoinsNum.ToString();
			taskReward_Exp[j].text = rewardExpNum.ToString();
		}
	}

	private void LabelAndNumberUpdate(int taskArrayNr, UILabel sendTaskLabel, UISlider progress, UILabel progressLabel, GameObject getRewardObj, GameObject completeObj)
	{
		if (_currentTasks[taskArrayNr].complete)
		{
			string format = Strings.Get(_currentTasks[taskArrayNr].template.ultraShortDescription);
			sendTaskLabel.text = string.Format(format, _currentTasks[taskArrayNr].task.aim);
			progress.gameObject.SetActive(false);
			if (PlayerInfo.Instance.GetIndexTaskRewardPayedOut(taskArrayNr))
			{
				completeObj.SetActive(true);
				if (UIScreenController.Instance.GetTopScreenName().Equals("FrontUI"))
				{
					getRewardObj.transform.parent.gameObject.SetActive(false);
				}
			}
			else if (UIScreenController.Instance.GetTopScreenName().Equals("FrontUI"))
			{
				getRewardObj.transform.parent.gameObject.SetActive(true);
				completeObj.SetActive(false);
			}
			else
			{
				progress.gameObject.SetActive(true);
				completeObj.SetActive(false);
				progress.value = (float)_currentTasks[taskArrayNr].progress / (float)_currentTasks[taskArrayNr].task.aim;
				progressLabel.text = _currentTasks[taskArrayNr].progress + "/" + _currentTasks[taskArrayNr].task.aim;
			}
		}
		else
		{
			progress.gameObject.SetActive(true);
			string format2 = Strings.Get(_currentTasks[taskArrayNr].template.description);
			if (_currentTasks[taskArrayNr].task.type == TaskType.OneOfEachPowerup)
			{
				sendTaskLabel.text = string.Format(format2, _currentTasks[taskArrayNr].task.aim);
				progress.value = (float)oneOfEach / (float)_currentTasks[taskArrayNr].task.aim;
				progressLabel.text = oneOfEach + "/" + _currentTasks[taskArrayNr].task.aim;
			}
			else
			{
				sendTaskLabel.text = string.Format(format2, _currentTasks[taskArrayNr].task.aim);
				progress.value = (float)_currentTasks[taskArrayNr].progress / (float)_currentTasks[taskArrayNr].task.aim;
				progressLabel.text = _currentTasks[taskArrayNr].progress + "/" + _currentTasks[taskArrayNr].task.aim;
			}
			completeObj.SetActive(false);
			if (UIScreenController.Instance.GetTopScreenName().Equals("FrontUI"))
			{
				getRewardObj.transform.parent.gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (waitNextTaskTime != null)
		{
			if (!LevelExpManager.Instance.IsCoolingDownOver())
			{
				waitNextTaskTime.text = LevelExpManager.Instance.GetCoolingDownTime();
			}
			else
			{
				waitNextTaskTime.text = string.Empty;
			}
		}
	}
}
