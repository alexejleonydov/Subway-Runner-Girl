using System;
using UnityEngine;

public class GlobalInit : MonoBehaviour
{
	private const string English = "english";

	private const string ChineseSimplified = "chinese";

	private const string ChineseTraditional = "chinese_traditional";

	private const string Japanese = "japanese";

	public bool debug;

	public string cityScenename;

	public string[] citiesScenename;

	public string languageKey = "english";

	public Strings.DocumentFormat documentFormat;

	public static GlobalInit Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		RiseSdk.Instance.Init();
		Application.targetFrameRate = 60;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Strings.Language = SelectLanguage();
		TasksData.LoadFiles();
		Upgrades.LoadFile();
		Characters.LoadFile();
		ChestsData.LoadFile();
		CharacterThemes.LoadFile();
		DailyLandingAwards.LoadFile();
		Achievements.LoadFile();
		Helmets.Load();
		LevelExpManager.LoadFile();
		//IvyApp.Instance.Statistics(string.Empty, string.Empty, "app_game_startup", 0);
		//RiseSdk.Instance.TrackEvent("app_game_startup", "default,default");
		AudioListener.volume = 0f;
		InAppManager.Init();
		Layers.Init();
		if (!PlayerPrefs.HasKey("FistInstallDate"))
		{
			if (!PlayerPrefs.HasKey("PlayerHasLevel"))
			{
				PlayerInfo.Instance.amountOfLevel = 1;
				PlayerPrefs.SetInt("PlayerHasLevel", 1);
			}
			PlayerPrefs.SetInt("FistInstallDate", 1);
			PlayerInfo.Instance.firstInstallDate = DateTime.UtcNow;
			PlayerInfo.Instance.isNewPlayer = true;
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters);
		}
		else
		{
			if (!PlayerPrefs.HasKey("PlayerHasLevel"))
			{
				PlayerInfo.Instance.amountOfLevel = PlayerInfo.Instance.currentTaskSet + 1;
				PlayerPrefs.SetInt("PlayerHasLevel", 2);
			}
			PlayerInfo.Instance.CheckOnlineNewDayOpen();
			if (PlayerInfo.Instance.tutorialCompleted)
			{
				PlayerInfo.Instance.tutorialStep = 3;
			}
		}
		if (PlayerInfo.Instance.currentCharacter == 10 && PlayerInfo.Instance.currentThemeIndex == 1)
		{
			PlayerInfo.Instance.currentCharacter = 0;
			PlayerInfo.Instance.currentThemeIndex = 0;
		}
		PlayerInfo.Instance.OpenGameAppAmount++;
		CheckSendSingleUserTime();
		switch ((DateTime.UtcNow.Date - PlayerInfo.Instance.lastPlayDate.Date).Days)
		{
		default:
			PlayerInfo.Instance.lastPlayDate = DateTime.UtcNow;
			PlayerInfo.Instance.playDailyTimes = 0;
			PlayerInfo.Instance.openAppCountDaily = 1;
			break;
		case 0:
			PlayerInfo.Instance.playDailyTimes += PlayerInfo.Instance.playOnceTimes;
			PlayerInfo.Instance.openAppCountDaily++;
			break;
		case 1:
			CheckSendDayEverageTime();
			CheckPlayerLevel();
			CheckGameOverDoubleVideo();
			PlayerInfo.Instance.lastPlayDate = DateTime.UtcNow;
			PlayerInfo.Instance.playDailyTimes = 0;
			break;
		}
	}

	private void Start()
	{
		//RiseSdkListener.OnResumeAdEvent -= onResumeAd;
		//RiseSdkListener.OnResumeAdEvent += onResumeAd;
		PlayerInfo.Instance.UpdateGameOverDoubleCoinsShowCount();
		PlayerInfo.Instance.playOnceTimes = 0;
	}

	private void CheckPlayerLevel()
	{
		if (PlayerInfo.Instance.playerLevel == 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0);
			return;
		}
		bool flag = false;
		int openAppCountDaily = PlayerInfo.Instance.openAppCountDaily;
		if (openAppCountDaily == 1 && PlayerInfo.Instance.playerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("loath_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0);
			PlayerInfo.Instance.playerLevel = 1;
			flag = true;
		}
		else if ((openAppCountDaily == 2 || openAppCountDaily == 3) && PlayerInfo.Instance.playerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("normal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0);
			PlayerInfo.Instance.playerLevel = 2;
			flag = true;
		}
		else if (openAppCountDaily > 3 && PlayerInfo.Instance.playerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0);
			PlayerInfo.Instance.playerLevel = 3;
			flag = true;
		}
		if (PlayerInfo.Instance.playerLevel == 3)
		{
			return;
		}
		float num = (float)PlayerInfo.Instance.playDailyTimes * 0.001f / (float)openAppCountDaily;
		if (num < 180f && PlayerInfo.Instance.playerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("loath_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0);
			PlayerInfo.Instance.playerLevel = 1;
			flag = true;
		}
		else if (num <= 360f && PlayerInfo.Instance.playerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("normal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0);
			PlayerInfo.Instance.playerLevel = 2;
			flag = true;
		}
		else if (PlayerInfo.Instance.playerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0);
			PlayerInfo.Instance.playerLevel = 3;
			flag = true;
		}
		if (!flag)
		{
			if (PlayerInfo.Instance.playerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("loath_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0);
			}
			else if (PlayerInfo.Instance.playerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("normal_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0);
			}
		}
	}

	private void CheckGameOverDoubleVideo()
	{
		if (PlayerInfo.Instance.watchDoublePlayerLevel == 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0);
			return;
		}
		int num = PlayerInfo.Instance.UpdateSpanDays();
		if (num == 0)
		{
			return;
		}
		bool flag = false;
		PlayerInfo.Instance.CheckGameOverDoubleCoinsSpanDays();
		int gameOverDoubleCoinsShowCountLastDay = PlayerInfo.Instance.gameOverDoubleCoinsShowCountLastDay;
		if (gameOverDoubleCoinsShowCountLastDay == 1 && PlayerInfo.Instance.watchDoublePlayerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 1;
			flag = true;
		}
		else if ((gameOverDoubleCoinsShowCountLastDay == 2 || gameOverDoubleCoinsShowCountLastDay == 3) && PlayerInfo.Instance.watchDoublePlayerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 2;
			flag = true;
		}
		else if (gameOverDoubleCoinsShowCountLastDay > 3 && PlayerInfo.Instance.watchDoublePlayerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 3;
			flag = true;
		}
		if (num <= 3 && !flag)
		{
			if (PlayerInfo.Instance.watchDoublePlayerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0);
			}
			else if (PlayerInfo.Instance.watchDoublePlayerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0);
			}
		}
		if (PlayerInfo.Instance.watchDoublePlayerLevel == 3 || num <= 3)
		{
			return;
		}
		gameOverDoubleCoinsShowCountLastDay = PlayerInfo.Instance.LastThreeDaysGameOverDoubleCoinsCount();
		if (gameOverDoubleCoinsShowCountLastDay <= 4 && gameOverDoubleCoinsShowCountLastDay >= 1 && PlayerInfo.Instance.watchDoublePlayerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 1;
			flag = true;
		}
		else if (gameOverDoubleCoinsShowCountLastDay <= 9 && PlayerInfo.Instance.watchDoublePlayerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 2;
			flag = true;
		}
		else if (PlayerInfo.Instance.watchDoublePlayerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0);
			PlayerInfo.Instance.watchDoublePlayerLevel = 3;
			flag = true;
		}
		if (!flag)
		{
			if (PlayerInfo.Instance.watchDoublePlayerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0);
			}
			else if (PlayerInfo.Instance.watchDoublePlayerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0);
			}
		}
	}

	private void CheckSendSingleUserTime()
	{
		float num = (float)PlayerInfo.Instance.playOnceTimes * 0.001f;
		if (num < 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_0_30s", 0);
		}
		else if (num < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_30_60s", 0);
		}
		else if (num < 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_1_2min", 0);
		}
		else if (num < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_2_3min", 0);
		}
		else if (num < 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_3_5min", 0);
		}
		else if (num < 360f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_5_6min", 0);
		}
		else if (num < 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_6_7min", 0);
		}
		else if (num >= 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_7_min", 0);
		}
	}

	private void CheckSendDayEverageTime()
	{
		float num = (float)PlayerInfo.Instance.playDailyTimes * 0.001f;
		if (num < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_0_1min", 0);
		}
		else if (num < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_1_3min", 0);
		}
		else if (num < 600f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_3_10min", 0);
		}
		else if (num < 1800f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_10_30min", 0);
		}
		else if (num > 1800f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_30_min", 0);
		}
	}

	private string SelectLanguage()
	{
		Debug.Log(Application.systemLanguage);
		Strings.documentFormat = documentFormat;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Japanese:
			return "japanese";
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
			return "chinese";
		case SystemLanguage.ChineseTraditional:
			return "chinese_traditional";
		default:
			return "english";
		}
	}

	private void onResumeAd()
	{
		Game.Instance.showAdTime = Time.time;
		RiseSdk.Instance.TrackEvent("interstitial_lockscreen", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_lockscreen", 0);
		RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
		Game.Instance.lastShowAd = "show_interstitial_lockscreen";
	}

	private void Update()
	{
		PlayerInfo.Instance.playOnceTimes += (int)(Time.deltaTime * 1000f);
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.CalcOnlineTotalSeconds();
	}
}
