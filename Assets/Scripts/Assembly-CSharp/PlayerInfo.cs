using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Kiloo.Common;
using Network;
using UnityEngine;

public class PlayerInfo
{
	private enum Key
	{
		AmountOfCoins = 0,
		HighestScore = 1,
		HasSubscribed = 2,
		HasRemoveAd = 3,
		DailyLandingPayedOut = 6,
		CurrentCharacter = 7,
		CurrentMissionSet = 8,
		CurrentMissionSetProgress = 9,
		CollectedCharacterTokens = 10,
		TranscendByLeadborad = 11,
		TutorialCompleted = 12,
		TutorialStep = 13,
		DoubleCoins = 15,
		DailyLandingInRow = 17,
		MissionCompletedSum = 18,
		NumberOfRunsSinceLastGuideline = 19,
		AutoShowChangePlayerName = 20,
		HasFacebookLogin = 21,
		HasShownPlayerMenuPopup = 22,
		ShouldShownPlayerMenuPopup = 23,
		FirstGameOverNoRemind = 24,
		HasShownMission1Popup = 25,
		ShouldShowMission1Popup = 26,
		IgnoreTrailRolePopup = 27,
		DailyLandingLastPayoutDayOfYear = 29,
		AmountOfSuperMysteryBoxesOpened = 30,
		HasHoverboardsBeenSeen = 31,
		UnlockedHoverboardTypes = 32,
		CurrentHoverboard = 33,
		HistoryOfInapp = 36,
		HasCharacterBeenSeen = 37,
		AmountOfKeys = 39,
		HasFirstMBKey = 40,
		HasFirstSMBKey = 41,
		HasShownKeysPopup = 42,
		CharacterThemesUnlocked = 43,
		CharacterLastSelectedThemes = 44,
		CharacterThemesSeen = 45,
		PendingRewards = 46,
		GameOverDoubleConfirnNoRemind = 47,
		NumberOfRuns = 48,
		Stats = 49,
		FirstInstallDate = 51,
		ShowWatchVideoPopupCount = 52,
		AmountOfGameChestesOpened = 53,
		LotteryRemainCount = 54,
		LotteryLastDateTime = 55,
		ShowTrialPopupCount = 56,
		LastShowFreeUpgradeDate = 57,
		FreeUpgradeCount = 58,
		MenuSliderShow = 59,
		TopRunData = 60,
		AchievementProgress = 61,
		AchievementRewardPayedOut = 62,
		GameOverFullAdCount = 63,
		GameOverFullAdLastDate = 64,
		ForceNextCityOrder = 65,
		GameOverUITryCount = 66,
		GameOverUITryNextTime = 67,
		IgnoreSubscriptionPopup = 68,
		IgnoreSubscriptionNextTime = 69,
		LastPlayTime = 70,
		UpdateRewardIndex = 71,
		UpdateFromLastApp = 72,
		LastPushLocalNotificationDateTime = 73,
		WatchVideoSuccessNum = 74,
		AmountOfOpenGameApp = 75,
		LastOpenGameDateTime = 76,
		WallWalkTutorialCount = 77,
		GameOverDoubleCoinViewCycleCount = 78,
		DonotClickGameOverDoubleCoinViewCount = 79,
		IsNewPlayer = 80,
		LastGameOverDoubleCoinViewDate = 81,
		LastClearSingleUserDateTime = 82,
		LastSendSingerUserAverageTime = 83,
		LastSendDayAveragePlayTime = 84,
		OpenAppCountDaily = 85,
		PlayOnceTimes = 86,
		PlayDailyTimes = 87,
		GameOverDoubleCoinsShowCountOneDay = 88,
		GameOverDoubleCoinsShowCountTwoDay = 89,
		GameOverDoubleCoinsShowCountLastDay = 90,
		WatchDoublePlayerLevel = 91,
		PlayerLevel = 92,
		LastGameOverDoubleCoinsDateTime = 93,
		UpdateDateTime = 94,
		CurrentTrialProgress = 95,
		CurrentTrialIndex = 96,
		TotalTrialDays = 97,
		OnlineZonePayedOut = 98,
		OpenAppInNewDayStartDateTime = 99,
		OnlineTotalSeconds = 100,
		LotteryFreeCount = 101,
		LotteryFreeLastDateTime = 102,
		AmountOfLevel = 103,
		AmountOfExp = 104,
		LevelTaskComplete = 105,
		TaskRewardPayedOut = 106,
		GetTaskPlayerLeve = 107
	}

	public Action onCoinsChanged;

	public Action onKeysChanged;

	public Action onScoreMultiplierChanged;

	public Action onExpChanged;

	public Action onLevelChanged;

	public Action<Characters.CharacterType> OnCharacterOutfitUnlocked;

	public Action<Characters.CharacterType> OnSymbolCollected;

	public Action<Helmets.HelmType> OnHelmUnlocked;

	public Action OnSubscribed;

	public Action onPowerupAmountChanged;

	private int _amountOfGameChestesOpened;

	private DateTime _lastPushLocalNotificationDateTime;

	private bool _transcendByLeadborad;

	private int _amountOfSuperChestesOpened;

	private Dictionary<Characters.CharacterType, int[]> _characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();

	private Dictionary<Characters.CharacterType, int[]> _characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();

	private int[] _collectedCharacterTokens;

	private int _currentCharacter;

	private Helmets.HelmType _currentHelmet;

	private int[] _currentTaskProgress;

	private int _currentTaskSet = -1;

	private int[] _achievementProgress;

	private bool[] _achievementAwardPayedOut;

	private bool[] _taskRewardPayedOut;

	private int[] _currentTrialProgress;

	private int _currentTrialIndex;

	private int _totalTrialDays;

	private int _updateRewardIndex;

	private bool _updateFromLastApp;

	private int _watchVideoSuccessNum;

	private int _amountOfOpenGameApp;

	private int _dailyLandingInRow;

	private int _dailyLandingLastPayoutDayOfYear;

	private bool _dailyLandingPayedOut;

	private bool _dirty;

	private bool _doubleScore;

	private DateTime _openAppInNewDayStartDateTime;

	private int _onlineTotalSeconds;

	private bool[] _onlineRewardPayedOut;

	private int _openAppCountDaily;

	private int _playOnceTimes;

	private int _playDailyTimes;

	private DateTime _updateDateTime;

	private int _gameOverDoubleCoinsShowCountLastDay;

	private int _gameOverDoubleCoinsShowCountOneDay;

	private int _gameOverDoubleCoinsShowCountTwoDay;

	private DateTime _lastGameOverDoubleCoinsDateTime;

	private int _watchDoublePlayerLevel;

	private int _playerLevel;

	private DateTime _firstInstallDate;

	private DateTime _lastOpenGameDateTime;

	private DateTime _lastLotteryFreeViewDateTime;

	private DateTime _lastLotteryFreeDateTime;

	private int _showWatchVideoPopupCount;

	private DateTime _lastShowFreeUpgradeDate;

	private DateTime _lastGameOverDoubleCoinViewDate;

	private DateTime _lastPlayDate;

	private int _showTrialPopupCount;

	private Dictionary<Characters.CharacterType, bool> _hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();

	private bool _hasDoubleCoins;

	private Dictionary<Helmets.HelmType, bool> _hasHelmetBeenSeen = new Dictionary<Helmets.HelmType, bool>();

	private bool _hasReceivedFirstMBKey;

	private bool _hasReceivedFirstSMBKey;

	private bool _menuSliderShow;

	private bool _autoShowChangePlayerName;

	private bool _hasShownKeysPopup;

	private bool _hasShownTask1Popup;

	private bool _hasShownPlayerMenuPopup;

	private int _highestScore;

	private int _lotteryWatchViewRemainCount;

	private int _lotteryFreeRemainCount;

	private int _gameOverDoubleCoinViewCycleCount;

	private int _donotClickGameOverDoubleCoinViewCount;

	private int _freeUpgradeCount;

	private int _freeUpgradeInterval = 120;

	public Dictionary<Helmets.HelmType, bool> _helmetUnlockStatus = new Dictionary<Helmets.HelmType, bool>();

	private Dictionary<string, string> _inappHistory;

	private static PlayerInfo _instance;

	private Dictionary<Characters.CharacterType, int> _lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();

	private int _taskCompletedSum;

	private int _numberOfRuns;

	private int _numberOfRunsSinceLastGuideline;

	private bool _hasFacebookLogin;

	private List<CelebrationReward> _pendingRewards = new List<CelebrationReward>();

	private bool _gameOverDoubleConfirmNoRemind;

	private bool _shouldShownPlayerMenuPopup;

	private bool _firstGameOverNoRemind;

	private bool _shouldShowTask1Popup;

	private bool _shouldShowDailyLandingPopup;

	private bool _hasShowDailyLandingPopup;

	private bool _hasSubscribed;

	private bool _hasRemoveAd;

	private bool _tutorialCompleted;

	private int _tutorialStep;

	private Dictionary<PropType, int> _upgradeAmounts;

	private Dictionary<PropType, int> _upgradeTiers;

	private Dictionary<string, int> _wallWalkTutorialCount;

	private int _amountOfCoins;

	private int _amountOfKeys;

	private DateTime _gameOverFullAdLastDate;

	private int _gameOverFullAdCount;

	private bool _ignoreTrailRolePopup;

	private DateTime _gameoverUITryNextTime;

	private int _gameoverUITryCount;

	private DateTime _ignoreSubscriptionNextTime;

	private bool _ignoreSubscriptionPopup;

	private bool _isNewPlayer;

	private int _forceNextCityOrder;

	private int _amountOfLevel;

	public int _getTaskPlayerLevel;

	private bool _levelTaskComplete;

	private int _amountOfExp;

	public DateTime lastLotteryFreeViewDateTime
	{
		get
		{
			return _lastLotteryFreeViewDateTime;
		}
		set
		{
			_lastLotteryFreeViewDateTime = value;
			_dirty = true;
		}
	}

	public int lotteryFreeRemainCount
	{
		get
		{
			return _lotteryFreeRemainCount;
		}
		set
		{
			if (value != _lotteryFreeRemainCount)
			{
				_lotteryFreeRemainCount = value;
				_dirty = true;
			}
		}
	}

	public int donotClickGameOverDoubleCoinViewCount
	{
		get
		{
			return _donotClickGameOverDoubleCoinViewCount;
		}
		set
		{
			if (value != _donotClickGameOverDoubleCoinViewCount)
			{
				_donotClickGameOverDoubleCoinViewCount = value;
				_dirty = true;
			}
		}
	}

	public int currentTrialIndex
	{
		get
		{
			return _currentTrialIndex;
		}
		set
		{
			if (value != _currentTrialIndex)
			{
				_currentTrialIndex = value;
				_dirty = true;
			}
		}
	}

	public int totalTrialDays
	{
		get
		{
			return _totalTrialDays;
		}
		set
		{
			if (value != _totalTrialDays)
			{
				_totalTrialDays = value;
				_dirty = true;
			}
		}
	}

	public int WatchVideoSuccessNum
	{
		get
		{
			return _watchVideoSuccessNum;
		}
		set
		{
			_watchVideoSuccessNum = value;
			_dirty = true;
		}
	}

	public int OpenGameAppAmount
	{
		get
		{
			return _amountOfOpenGameApp;
		}
		set
		{
			_amountOfOpenGameApp = value;
			_dirty = true;
		}
	}

	public int showTrialPopupCount
	{
		get
		{
			return _showTrialPopupCount;
		}
		set
		{
			if (value != _showTrialPopupCount)
			{
				_showTrialPopupCount = value;
				_dirty = true;
			}
		}
	}

	public int lotteryWatchViewRemainCount
	{
		get
		{
			return _lotteryWatchViewRemainCount;
		}
		set
		{
			if (value != _lotteryWatchViewRemainCount)
			{
				_lotteryWatchViewRemainCount = value;
				_dirty = true;
			}
		}
	}

	public int amountOfCoins
	{
		get
		{
			return _amountOfCoins;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			_dirty = true;
			_amountOfCoins = value;
			Action action = onCoinsChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public int amountOfLevel
	{
		get
		{
			int num = ~(_amountOfLevel ^ 0x1CC5);
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		set
		{
			if (value < 1)
			{
				value = 1;
			}
			if (value > 999)
			{
				value = 999;
			}
			_dirty = true;
			_amountOfLevel = ~value ^ 0x1CC5;
			ServerManager.Instance.UploadPlayerLevel(value.ToString());
			Action action = onLevelChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public int amountOfExp
	{
		get
		{
			return _amountOfExp;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			_dirty = true;
			_amountOfExp = value;
			Action action = onExpChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public bool levelTaskComplete
	{
		get
		{
			return _levelTaskComplete;
		}
		set
		{
			_dirty = true;
			_levelTaskComplete = value;
		}
	}

	public int amountOfKeys
	{
		get
		{
			return _amountOfKeys;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			_amountOfKeys = value;
			_dirty = true;
			Action action = onKeysChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public int amountOfGameChestesOpened
	{
		get
		{
			return _amountOfGameChestesOpened;
		}
		set
		{
			if (_amountOfGameChestesOpened != value)
			{
				_amountOfGameChestesOpened = value;
				_dirty = true;
			}
		}
	}

	public bool transcendByLeadborad
	{
		get
		{
			return _transcendByLeadborad;
		}
		set
		{
			if (_transcendByLeadborad != value)
			{
				_transcendByLeadborad = value;
				_dirty = true;
			}
		}
	}

	public int amountOfSuperChestesOpened
	{
		get
		{
			return _amountOfSuperChestesOpened;
		}
		set
		{
			if (_amountOfSuperChestesOpened != value)
			{
				_amountOfSuperChestesOpened = value;
				_dirty = true;
			}
		}
	}

	public int currentCharacter
	{
		get
		{
			return _currentCharacter;
		}
		set
		{
			if (value != _currentCharacter)
			{
				_currentCharacter = value;
				_dirty = true;
			}
		}
	}

	public Helmets.HelmType currentHelmet
	{
		get
		{
			return _currentHelmet;
		}
		set
		{
			if (value != _currentHelmet)
			{
				_currentHelmet = value;
				_dirty = true;
			}
		}
	}

	public bool menuSliderShow
	{
		get
		{
			return _menuSliderShow;
		}
		set
		{
			if (_menuSliderShow != value)
			{
				_menuSliderShow = value;
				_dirty = true;
			}
		}
	}

	public int currentTaskSet
	{
		get
		{
			return _currentTaskSet;
		}
	}

	public int currentThemeIndex
	{
		get
		{
			return GetIndexForLastSelectedTheme((Characters.CharacterType)currentCharacter);
		}
		set
		{
			SetLastSelectedTheme((Characters.CharacterType)currentCharacter, value);
		}
	}

	public bool doubleScore
	{
		get
		{
			return _doubleScore;
		}
		set
		{
			if (value != _doubleScore)
			{
				_doubleScore = value;
				TriggerOnScoreMultiplierChanged();
			}
		}
	}

	public bool hasDoubleCoins
	{
		get
		{
			return _hasDoubleCoins;
		}
		set
		{
			if (_hasDoubleCoins != value)
			{
				_hasDoubleCoins = value;
				_dirty = true;
			}
		}
	}

	public DateTime firstInstallDate
	{
		get
		{
			return _firstInstallDate;
		}
		set
		{
			_firstInstallDate = value;
			_dirty = true;
		}
	}

	public bool hasReceivedFirstMBKey
	{
		get
		{
			return _hasReceivedFirstMBKey;
		}
		set
		{
			if (_hasReceivedFirstMBKey != value)
			{
				_hasReceivedFirstMBKey = value;
				_dirty = true;
			}
		}
	}

	public bool hasReceivedFirstSMBKey
	{
		get
		{
			return _hasReceivedFirstSMBKey;
		}
		set
		{
			if (_hasReceivedFirstSMBKey != value)
			{
				_hasReceivedFirstSMBKey = value;
				_dirty = true;
			}
		}
	}

	public bool autoShowChangePlayerName
	{
		get
		{
			return _autoShowChangePlayerName;
		}
		set
		{
			if (_autoShowChangePlayerName != value)
			{
				_autoShowChangePlayerName = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownKeysPopup
	{
		get
		{
			return _hasShownKeysPopup;
		}
		set
		{
			if (_hasShownKeysPopup != value)
			{
				_hasShownKeysPopup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShownTask1Popup
	{
		get
		{
			return _hasShownTask1Popup;
		}
		set
		{
			if (_hasShownTask1Popup != value)
			{
				_hasShownTask1Popup = value;
				_dirty = true;
			}
		}
	}

	public bool hasShowDailyLandingPopup
	{
		get
		{
			return _hasShowDailyLandingPopup;
		}
		set
		{
			if (_hasShowDailyLandingPopup != value)
			{
				_hasShowDailyLandingPopup = value;
			}
		}
	}

	public bool shouldShowDailyLandingPopup
	{
		get
		{
			return _shouldShowDailyLandingPopup;
		}
		set
		{
			if (_shouldShowDailyLandingPopup != value)
			{
				_shouldShowDailyLandingPopup = value;
			}
		}
	}

	public bool hasShownPlayerMenuPopup
	{
		get
		{
			return _hasShownPlayerMenuPopup;
		}
		set
		{
			if (_hasShownPlayerMenuPopup != value)
			{
				_hasShownPlayerMenuPopup = value;
				_dirty = true;
			}
		}
	}

	public int gameOverDoubleCoinsShowCountLastDay
	{
		get
		{
			return _gameOverDoubleCoinsShowCountLastDay;
		}
		set
		{
			if (value != _gameOverDoubleCoinsShowCountLastDay)
			{
				_gameOverDoubleCoinsShowCountLastDay = value;
				_dirty = true;
			}
		}
	}

	public int watchDoublePlayerLevel
	{
		get
		{
			return _watchDoublePlayerLevel;
		}
		set
		{
			if (value != _watchDoublePlayerLevel)
			{
				_watchDoublePlayerLevel = value;
				_dirty = true;
			}
		}
	}

	public int playerLevel
	{
		get
		{
			return _playerLevel;
		}
		set
		{
			if (value != _playerLevel)
			{
				if (value > 3)
				{
					_playerLevel = 3;
				}
				else
				{
					_playerLevel = value;
				}
				_dirty = true;
			}
		}
	}

	public int highestScore
	{
		get
		{
			return _highestScore;
		}
		set
		{
			if (value > _highestScore)
			{
				_highestScore = value;
				_dirty = true;
			}
			if (value <= 99999999)
			{
				ServerManager.Instance.UploadScore(value);
				ServerManager.Instance.UploadScoreGlobal(value);
				if (_hasSubscribed)
				{
					ServerManager.Instance.UploadScoreVip(value);
				}
			}
		}
	}

	public int showWatchVideoPopupCount
	{
		get
		{
			return _showWatchVideoPopupCount;
		}
		set
		{
			_showWatchVideoPopupCount = value;
			_dirty = true;
		}
	}

	public static PlayerInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new PlayerInfo();
			}
			return _instance;
		}
	}

	public CelebrationReward lastAddedReward { get; private set; }

	public int taskCompletedSum
	{
		get
		{
			if (_taskCompletedSum == 0)
			{
				_taskCompletedSum = currentTaskSet + 1;
			}
			return _taskCompletedSum;
		}
		set
		{
			_taskCompletedSum = value;
		}
	}

	public int numberOfRuns
	{
		get
		{
			return _numberOfRuns;
		}
		set
		{
			if (_numberOfRuns != value)
			{
				_numberOfRuns = value;
				_dirty = true;
			}
		}
	}

	public bool hasFacebookLogin
	{
		get
		{
			return _hasFacebookLogin;
		}
		set
		{
			_hasFacebookLogin = value;
			_dirty = true;
		}
	}

	public List<CelebrationReward> pendingRewards
	{
		get
		{
			return _pendingRewards;
		}
		set
		{
			_pendingRewards = value;
			_dirty = true;
		}
	}

	public TopRunData TopRunData { get; private set; }

	public int updateRewardIndex
	{
		get
		{
			return _updateRewardIndex;
		}
		set
		{
			if (value != _updateRewardIndex)
			{
				_updateRewardIndex = value;
				_dirty = true;
			}
		}
	}

	public bool updateFromLastApp
	{
		get
		{
			return _updateFromLastApp;
		}
		set
		{
			if (value != _updateFromLastApp)
			{
				_updateFromLastApp = value;
				_dirty = true;
			}
		}
	}

	public int rawMultiplier
	{
		get
		{
			return Mathf.Clamp(_currentTaskSet + 1, 1, 30);
		}
	}

	public bool gameOverDoubleConfirmNoRemind
	{
		get
		{
			return _gameOverDoubleConfirmNoRemind;
		}
		set
		{
			if (_gameOverDoubleConfirmNoRemind != value)
			{
				_gameOverDoubleConfirmNoRemind = value;
				_dirty = true;
			}
		}
	}

	public int scoreMultiplier
	{
		get
		{
			int num = Mathf.Clamp(amountOfLevel, 1, 999);
			if (GameStats.Instance.scoreBooster5Activated)
			{
				num += 5;
			}
			if (GameStats.Instance.scoreBooster10Activated)
			{
				num += 10;
			}
			if (doubleScore)
			{
				num *= 2;
			}
			return num;
		}
	}

	public bool firstGameOverNoRemind
	{
		get
		{
			return _firstGameOverNoRemind;
		}
		set
		{
			if (_firstGameOverNoRemind != value)
			{
				_firstGameOverNoRemind = value;
				_dirty = true;
			}
		}
	}

	public DateTime lastPlayDate
	{
		get
		{
			return _lastPlayDate;
		}
		set
		{
			_lastPlayDate = value;
			_dirty = true;
		}
	}

	public int playOnceTimes
	{
		get
		{
			return _playOnceTimes;
		}
		set
		{
			_playOnceTimes = value;
			_dirty = true;
		}
	}

	public int playDailyTimes
	{
		get
		{
			return _playDailyTimes;
		}
		set
		{
			_playDailyTimes = value;
			_dirty = true;
		}
	}

	public int openAppCountDaily
	{
		get
		{
			return _openAppCountDaily;
		}
		set
		{
			if (_openAppCountDaily != value)
			{
				_openAppCountDaily = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowTask1Popup
	{
		get
		{
			return _shouldShowTask1Popup;
		}
		set
		{
			if (_shouldShowTask1Popup != value)
			{
				_shouldShowTask1Popup = value;
				_dirty = true;
			}
		}
	}

	public bool shouldShowPlayerMenuPopup
	{
		get
		{
			return _shouldShownPlayerMenuPopup;
		}
		set
		{
			if (_shouldShownPlayerMenuPopup != value)
			{
				_shouldShownPlayerMenuPopup = value;
				_dirty = true;
			}
		}
	}

	public bool ignoreTrailRolePopup
	{
		get
		{
			return _ignoreTrailRolePopup;
		}
		set
		{
			if (_ignoreTrailRolePopup != value)
			{
				_ignoreTrailRolePopup = value;
				_dirty = true;
			}
		}
	}

	public bool ignoreSubscriptionPopup
	{
		get
		{
			return _ignoreSubscriptionPopup;
		}
		set
		{
			if (_ignoreSubscriptionPopup != value)
			{
				_ignoreSubscriptionPopup = value;
				_dirty = true;
			}
		}
	}

	public DateTime ignoreSubscriptionNextTime
	{
		get
		{
			return _ignoreSubscriptionNextTime;
		}
		set
		{
			if (_ignoreSubscriptionNextTime != value)
			{
				_ignoreSubscriptionNextTime = value;
				_dirty = true;
			}
		}
	}

	public int gameoverUITryCount
	{
		get
		{
			return _gameoverUITryCount;
		}
		set
		{
			if (_gameoverUITryCount != value)
			{
				_gameoverUITryCount = value;
				_dirty = true;
			}
		}
	}

	public DateTime gameoverUITryNextTime
	{
		get
		{
			return _gameoverUITryNextTime;
		}
		set
		{
			if (_gameoverUITryNextTime != value)
			{
				_gameoverUITryNextTime = value;
				_dirty = true;
			}
		}
	}

	public bool isNewPlayer
	{
		get
		{
			return _isNewPlayer;
		}
		set
		{
			if (_isNewPlayer != value)
			{
				_isNewPlayer = value;
				_dirty = true;
			}
		}
	}

	public Statistics stats { get; private set; }

	public int gameOverFullAdCount
	{
		get
		{
			return _gameOverFullAdCount;
		}
		set
		{
			if (_gameOverFullAdCount != value)
			{
				_gameOverFullAdCount = value;
				_dirty = true;
			}
		}
	}

	public DateTime gameOverFullAdDate
	{
		get
		{
			return _gameOverFullAdLastDate;
		}
		set
		{
			if (_gameOverFullAdLastDate != value)
			{
				_gameOverFullAdLastDate = value;
				_dirty = true;
			}
		}
	}

	public int forceNextCityOrder
	{
		get
		{
			return _forceNextCityOrder;
		}
		set
		{
			if (_forceNextCityOrder != value)
			{
				_forceNextCityOrder = value;
				_dirty = true;
			}
		}
	}

	public bool tutorialCompleted
	{
		get
		{
			return _tutorialCompleted;
		}
		set
		{
			if (_tutorialCompleted != value)
			{
				_tutorialCompleted = value;
				_dirty = true;
			}
		}
	}

	public int GetTaskPlayerLevel
	{
		get
		{
			return _getTaskPlayerLevel;
		}
		set
		{
			if (_getTaskPlayerLevel != value)
			{
				_getTaskPlayerLevel = value;
				_dirty = true;
			}
		}
	}

	public int tutorialStep
	{
		get
		{
			return _tutorialStep;
		}
		set
		{
			if (_tutorialStep != value)
			{
				_tutorialStep = value;
				_dirty = true;
			}
		}
	}

	public bool hasRemoveAd
	{
		get
		{
			return _hasRemoveAd;
		}
		set
		{
			if (_hasRemoveAd != value)
			{
				_hasRemoveAd = value;
				if (value)
				{
					RiseSdk.Instance.enableBackHomeAd(false, "custom");
				}
				_dirty = true;
			}
		}
	}

	public bool hasSubscribed
	{
		get
		{
			return _hasSubscribed;
		}
		set
		{
			if (_hasSubscribed != value)
			{
				_hasSubscribed = value;
				if (OnSubscribed != null)
				{
					OnSubscribed();
				}
				_dirty = true;
			}
		}
	}

	private PlayerInfo()
	{
		_upgradeAmounts = new Dictionary<PropType, int>
		{
			{
				PropType.helmet,
				0
			},
			{
				PropType.headstart500,
				0
			},
			{
				PropType.headstart2000,
				0
			},
			{
				PropType.chest,
				0
			},
			{
				PropType.scorebooster,
				0
			}
		};
		_upgradeTiers = new Dictionary<PropType, int>
		{
			{
				PropType.flypack,
				0
			},
			{
				PropType.supershoes,
				0
			},
			{
				PropType.coinmagnet,
				0
			},
			{
				PropType.doubleMultiplier,
				0
			}
		};
		_inappHistory = new Dictionary<string, string>();
		_gameOverDoubleConfirmNoRemind = false;
		_shouldShowDailyLandingPopup = false;
		_hasShowDailyLandingPopup = false;
		_collectedCharacterTokens = new int[Characters.characterData.Count];
		TopRunData = new TopRunData();
		Load();
	}

	public void AddPendingReward(CelebrationReward reward)
	{
		lastAddedReward = reward;
		bool flag = false;
		int num = -1;
		for (int i = 0; i < _pendingRewards.Count; i++)
		{
			if (_pendingRewards[i].Uid == reward.Uid)
			{
				flag = true;
			}
			if (_pendingRewards[i].CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore && num == -1)
			{
				num = i;
			}
		}
		if (flag)
		{
			return;
		}
		if (num > -1)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
			{
				_pendingRewards[num] = reward;
			}
			else
			{
				_pendingRewards.Insert(num, reward);
			}
		}
		else
		{
			_pendingRewards.Add(reward);
		}
		_dirty = true;
	}

	public void RemovePendingReward(CelebrationReward reward)
	{
		CelebrationReward celebrationReward = _pendingRewards.Find(reward.Find);
		if (celebrationReward == null)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest)
			{
				celebrationReward = _pendingRewards.Find(FindChest);
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest)
			{
				celebrationReward = _pendingRewards.Find(FindSuperChest);
			}
		}
		if (celebrationReward != null)
		{
			_pendingRewards.Remove(celebrationReward);
			if (lastAddedReward != null && celebrationReward.Uid == lastAddedReward.Uid)
			{
				lastAddedReward = null;
			}
			_dirty = true;
		}
	}

	private bool FindChest(CelebrationReward mbr)
	{
		return mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest;
	}

	private bool FindSuperChest(CelebrationReward mbr)
	{
		return mbr.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest;
	}

	public void AddSaveGemToUnlock()
	{
		amountOfKeys++;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, 1);
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_in_game_run", 0, 1);
	}

	public bool AddTransactionToHistory(string orderId, string itemId)
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WP8Player)
		{
			return true;
		}
		if (!_inappHistory.ContainsKey(orderId))
		{
			_inappHistory.Add(orderId, itemId);
			_dirty = true;
			return true;
		}
		return false;
	}

	public void BragCompleted()
	{
		ResetHighestScoreTo(highestScore);
	}

	public void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
	{
		bool Istoday;
		if (Instance.GetDailyLandingDaysInRow(out Istoday) == 0 && !Istoday)
		{
			TasksManager.Instance.RemoveProgressForThis(TaskTarget.DailyQuestInRow);
		}
	}

	public void CollectSymbol(Characters.CharacterType characterType, int amount)
	{
		_collectedCharacterTokens[(int)characterType] += amount;
		_dirty = true;
		Characters.Model model = Characters.characterData[characterType];
		if (model.unlockType == Characters.UnlockType.symbols)
		{
			int num = _collectedCharacterTokens[(int)characterType];
			if (num - amount < model.Price && num >= model.Price)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters);
			}
		}
		Action<Characters.CharacterType> onSymbolCollected = OnSymbolCollected;
		if (onSymbolCollected != null)
		{
			onSymbolCollected(characterType);
		}
	}

	public void DecreaseSymbol(Characters.CharacterType characterType, int amount)
	{
		_collectedCharacterTokens[(int)characterType] -= amount;
		if (_collectedCharacterTokens[(int)characterType] < 0)
		{
			_collectedCharacterTokens[(int)characterType] = 0;
		}
	}

	public int GetCollectedSymbols(Characters.CharacterType ModelType)
	{
		return _collectedCharacterTokens[(int)ModelType];
	}

	public int GetCurrentTaskProgress(int task)
	{
		if (task >= 3)
		{
			return _achievementProgress[task - 3];
		}
		if (_currentTaskProgress != null && task < _currentTaskProgress.Length)
		{
			return _currentTaskProgress[task];
		}
		return 0;
	}

	public int GetCurrentTier(PropType type)
	{
		if (!_upgradeTiers.ContainsKey(type))
		{
			return 0;
		}
		return _upgradeTiers[type];
	}

	public float GetHelmCoolDown()
	{
		return 5f;
	}

	public int GetIndexForLastSelectedTheme(Characters.CharacterType character)
	{
		int value;
		if (_lastSelectedThemes.TryGetValue(character, out value))
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && list.Count >= value)
			{
				return value;
			}
		}
		return 0;
	}

	public float GetPowerupDuration(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.durations[GetCurrentTier(type)];
	}

	public float GetPowerupLandSpeed(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.landSpeed;
	}

	public float GetPowerupSpeed(PropType type)
	{
		if (!Upgrades.upgrades.ContainsKey(type))
		{
			return 0f;
		}
		Upgrade upgrade = Upgrades.upgrades[type];
		return upgrade.speed;
	}

	private static List<CelebrationReward> GetRewardsFromString(string rewardsAsString)
	{
		List<CelebrationReward> list = new List<CelebrationReward>();
		if (!string.IsNullOrEmpty(rewardsAsString))
		{
			char[] separator = new char[1] { ';' };
			string[] array = rewardsAsString.Split(separator);
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.PopulateFromString(text);
					list.Add(celebrationReward);
				}
			}
		}
		return list;
	}

	private static string GetSavePath()
	{
		return Globals.GetUserDataPath() + "/playerdata";
	}

	private static string GetStringFromRewards(List<CelebrationReward> rewards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (CelebrationReward reward in rewards)
		{
			stringBuilder.Append(reward).Append(";");
		}
		int num = stringBuilder.ToString().LastIndexOf(';');
		if (num >= 0)
		{
			stringBuilder.Remove(num, 1);
		}
		return stringBuilder.ToString();
	}

	public int GetUpgradeAmount(PropType type)
	{
		return _upgradeAmounts[type];
	}

	public void SetUpgradeAmount(PropType type, int num)
	{
		_upgradeAmounts[type] = num;
	}

	public int GetUpgradeTierSum()
	{
		return GetCurrentTier(PropType.flypack) + GetCurrentTier(PropType.doubleMultiplier) + GetCurrentTier(PropType.coinmagnet) + GetCurrentTier(PropType.supershoes);
	}

	public bool HasHelmetBeenSeen(Helmets.HelmType helmType)
	{
		Helmets.Helm helm = Helmets.helmData[helmType];
		if (_hasHelmetBeenSeen.ContainsKey(helmType))
		{
			return _hasHelmetBeenSeen[helmType];
		}
		return false;
	}

	public void IncreasePowerupTier(PropType type)
	{
		if (_upgradeTiers.ContainsKey(type))
		{
			_upgradeTiers[type]++;
			_dirty = true;
		}
		else
		{
			LogError("Trying to increase tier for a non-tiered upgrade", null);
		}
	}

	public void IncreaseUpgradeAmount(PropType type, int amount = 1)
	{
		if (_upgradeAmounts.ContainsKey(type))
		{
			_upgradeAmounts[type] += amount;
			_dirty = true;
			Action action = onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
		}
		else
		{
			LogError("Trying to increase upgrade amount for a non-consumable", null);
		}
	}

	public bool IncrementCurrentTaskProgress(int task, int target)
	{
		if (_currentTaskProgress[task] < target)
		{
			_currentTaskProgress[task]++;
			_dirty = true;
			return _currentTaskProgress[task] == target;
		}
		return false;
	}

	public void InitCurrentTaskSet(int taskSet, int taskCount, bool resetProgress)
	{
		if (taskSet == _currentTaskSet)
		{
			return;
		}
		_currentTaskSet = taskSet;
		if (resetProgress)
		{
			_currentTaskProgress = new int[taskCount];
			for (int i = 0; i < taskCount; i++)
			{
				_currentTaskProgress[i] = 0;
			}
		}
		if (TrackController.Instance != null && _currentTaskSet > 0)
		{
			_forceNextCityOrder = TrackController.Instance.CurrentTaskSetChange(_currentTaskSet + 1);
		}
		_dirty = true;
		TriggerOnScoreMultiplierChanged();
	}

	private void InitDailyLanding()
	{
		_dailyLandingPayedOut = false;
	}

	public int GetDailyLandingDaysInRow(out bool Istoday)
	{
		int dayOfYear = DateTime.Now.DayOfYear;
		if (_dailyLandingLastPayoutDayOfYear == dayOfYear)
		{
			Istoday = true;
			return _dailyLandingInRow;
		}
		if (_dailyLandingLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && DateTime.Now.AddDays(-1.0).DayOfYear == _dailyLandingLastPayoutDayOfYear))
		{
			Istoday = false;
			InitDailyLanding();
			_dirty = true;
			return _dailyLandingInRow;
		}
		Istoday = false;
		_dailyLandingInRow = 0;
		InitDailyLanding();
		_dirty = true;
		return 0;
	}

	public void ReceiveDailyLandingPayout(int multiple, Action callback)
	{
		if (!_dailyLandingPayedOut)
		{
			DailyLandingAward dailyLandingAward = DailyLandingAwards.awards[_dailyLandingInRow];
			dailyLandingAward.Amount *= multiple;
			_dailyLandingLastPayoutDayOfYear = DateTime.Now.DayOfYear;
			_dailyLandingInRow = (_dailyLandingInRow + 1) % DailyLandingAwards.awards.Length;
			_dailyLandingPayedOut = true;
			FreeRewardManager.Instance.SetFreeRewardType(dailyLandingAward, callback);
			TasksManager.Instance.PlayerDidThis(TaskTarget.DailyQuests);
			TasksManager.Instance.PlayerDidThis(TaskTarget.DailyQuestInRow);
		}
	}

	public bool DailyLandingPayOut()
	{
		return !_dailyLandingPayedOut;
	}

	public void InitNew()
	{
		amountOfCoins = 0;
		amountOfLevel = 1;
		amountOfExp = 0;
		levelTaskComplete = false;
		_highestScore = 0;
		_hasFacebookLogin = false;
		_dailyLandingInRow = 0;
		_dailyLandingLastPayoutDayOfYear = 0;
		_dailyLandingPayedOut = false;
		_transcendByLeadborad = false;
		_amountOfGameChestesOpened = 0;
		_amountOfSuperChestesOpened = 0;
		_hasReceivedFirstMBKey = false;
		_hasReceivedFirstSMBKey = false;
		_lotteryWatchViewRemainCount = 2;
		_lotteryFreeRemainCount = 1;
		_lastLotteryFreeViewDateTime = DateTime.UtcNow;
		_lastLotteryFreeDateTime = DateTime.UtcNow;
		_lastPlayDate = DateTime.UtcNow;
		_menuSliderShow = true;
		amountOfKeys = 5;
		_currentCharacter = 0;
		_currentTaskSet = -1;
		_currentTaskProgress = null;
		_achievementProgress = new int[Achievements.NUMBER_OF_ACHIEVEMENTS];
		_achievementAwardPayedOut = new bool[Achievements.NUMBER_OF_ACHIEVEMENTS];
		_taskRewardPayedOut = new bool[3];
		_onlineRewardPayedOut = new bool[4];
		_onlineTotalSeconds = 0;
		_openAppInNewDayStartDateTime = DateTime.UtcNow;
		_tutorialCompleted = false;
		_tutorialStep = 0;
		_getTaskPlayerLevel = 1;
		_hasSubscribed = false;
		_hasRemoveAd = false;
		_hasDoubleCoins = false;
		_wallWalkTutorialCount = new Dictionary<string, int>();
		_numberOfRunsSinceLastGuideline = 0;
		_gameOverFullAdCount = 0;
		_gameOverFullAdLastDate = DateTime.Today;
		_forceNextCityOrder = 0;
		_autoShowChangePlayerName = true;
		_firstGameOverNoRemind = true;
		_shouldShownPlayerMenuPopup = false;
		_hasShownTask1Popup = false;
		_shouldShowTask1Popup = false;
		_hasShownPlayerMenuPopup = false;
		_ignoreTrailRolePopup = false;
		_ignoreSubscriptionPopup = false;
		_ignoreSubscriptionNextTime = DateTime.Today;
		_gameoverUITryCount = 3;
		_gameoverUITryNextTime = DateTime.UtcNow;
		_numberOfRuns = 0;
		_firstInstallDate = DateTime.UtcNow;
		_updateFromLastApp = false;
		_updateRewardIndex = 0;
		_playOnceTimes = 0;
		_playDailyTimes = 0;
		_updateDateTime = DateTime.UtcNow;
		_openAppCountDaily = 0;
		_lastPushLocalNotificationDateTime = DateTime.UtcNow;
		_lastOpenGameDateTime = DateTime.UtcNow;
		_watchVideoSuccessNum = 0;
		_amountOfOpenGameApp = 0;
		_isNewPlayer = false;
		_currentTrialProgress = new int[3];
		_currentTrialIndex = -1;
		_totalTrialDays = 0;
		for (int i = 0; i < _collectedCharacterTokens.Length; i++)
		{
			_collectedCharacterTokens[i] = 0;
		}
		Dictionary<PropType, int> dictionary = new Dictionary<PropType, int>(_upgradeAmounts.Count);
		foreach (PropType key in _upgradeAmounts.Keys)
		{
			if (key == PropType.helmet)
			{
				dictionary[key] = 3;
			}
			else
			{
				dictionary[key] = 0;
			}
		}
		_upgradeAmounts = dictionary;
		dictionary = new Dictionary<PropType, int>(_upgradeTiers.Count);
		foreach (PropType key2 in _upgradeTiers.Keys)
		{
			dictionary[key2] = 0;
		}
		_upgradeTiers = dictionary;
		_taskCompletedSum = 0;
		_hasHelmetBeenSeen = new Dictionary<Helmets.HelmType, bool>();
		_helmetUnlockStatus = new Dictionary<Helmets.HelmType, bool>();
		_inappHistory = new Dictionary<string, string>();
		_currentHelmet = Helmets.HelmType.normal;
		_hasCharacterBeenSeen = new Dictionary<Characters.CharacterType, bool>();
		_characterThemesUnlocked = new Dictionary<Characters.CharacterType, int[]>();
		_lastSelectedThemes = new Dictionary<Characters.CharacterType, int>();
		_characterThemesSeen = new Dictionary<Characters.CharacterType, int[]>();
		_pendingRewards = new List<CelebrationReward>();
		_showTrialPopupCount = 0;
		_gameOverDoubleConfirmNoRemind = false;
		_showWatchVideoPopupCount = 0;
		_freeUpgradeCount = 3;
		_lastShowFreeUpgradeDate = DateTime.UtcNow;
		_donotClickGameOverDoubleCoinViewCount = 0;
		_gameOverDoubleCoinsShowCountOneDay = 0;
		_gameOverDoubleCoinsShowCountTwoDay = 0;
		_gameOverDoubleCoinsShowCountLastDay = 0;
		_lastGameOverDoubleCoinsDateTime = DateTime.UtcNow;
		_watchDoublePlayerLevel = 0;
		_playerLevel = 0;
		_gameOverDoubleCoinViewCycleCount = 0;
		_lastGameOverDoubleCoinViewDate = DateTime.UtcNow;
		stats = new Statistics();
	}

	public bool isCharacterActive(Characters.CharacterType characterType)
	{
		return true;
	}

	public bool IsCollectionComplete(Characters.CharacterType characterType)
	{
		Characters.Model model = Characters.characterData[characterType];
		return model.unlockType == Characters.UnlockType.free || (model.unlockType == Characters.UnlockType.subscription && _hasSubscribed) || (model.unlockType != Characters.UnlockType.subscription && GetCollectedSymbols(characterType) >= model.Price);
	}

	public bool CanIncreasePowerup()
	{
		bool flag = false;
		Dictionary<PropType, int>.Enumerator enumerator = _upgradeTiers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value < 3)
			{
				flag = true;
				break;
			}
		}
		if (flag && CheckIfFreeUpgrade())
		{
			return true;
		}
		foreach (KeyValuePair<PropType, int> upgradeTier in _upgradeTiers)
		{
			if (Upgrades.upgrades[upgradeTier.Key].getPrice(upgradeTier.Value + 1) < _amountOfCoins && Upgrades.upgrades[upgradeTier.Key].getPrice(upgradeTier.Value + 1) > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanUnlockHelm()
	{
		int i = 1;
		for (int count = Helmets.helmData.Count; i < count; i++)
		{
			Helmets.HelmType helmType = (Helmets.HelmType)i;
			Helmets.Helm helm = Helmets.helmData[helmType];
			if (!HelmetManager.Instance.isHelmetUnlocked(helmType) && helm.level <= amountOfLevel)
			{
				if (helm.unlockType == Helmets.UnlockType.coins && helm.price <= _amountOfCoins)
				{
					return true;
				}
				if (helm.unlockType == Helmets.UnlockType.keys && helm.price <= _amountOfKeys)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanUnlockCharacter()
	{
		Characters.Model model = null;
		int i = 0;
		for (int count = Characters.characterData.Count; i < count; i++)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)i;
			model = Characters.characterData[characterType];
			if (model.unlockType != 0 && model.unlockType != Characters.UnlockType.subscription && GetCollectedSymbols(characterType) < model.Price && model.Level <= amountOfLevel)
			{
				if (model.unlockType == Characters.UnlockType.coins && model.Price <= _amountOfCoins)
				{
					return true;
				}
				if (model.unlockType == Characters.UnlockType.keys && model.Price <= _amountOfKeys)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanUnlockCharacterOrCTheme()
	{
		Characters.Model model = null;
		List<CharacterTheme> list = null;
		int[] array = null;
		int i = 0;
		for (int count = Characters.characterData.Count; i < count; i++)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)i;
			model = Characters.characterData[characterType];
			if (model.unlockType != 0 && model.unlockType != Characters.UnlockType.subscription && GetCollectedSymbols(characterType) < model.Price)
			{
				if (model.unlockType == Characters.UnlockType.coins && model.Price <= _amountOfCoins)
				{
					return true;
				}
				if (model.unlockType == Characters.UnlockType.keys && model.Price <= _amountOfKeys)
				{
					return true;
				}
			}
		}
		int j = 0;
		for (int count2 = Characters.characterData.Count; j < count2; j++)
		{
			Characters.CharacterType characterType = (Characters.CharacterType)j;
			list = CharacterThemes.characterCustomThemes[characterType];
			if (list == null || list.Count <= 0)
			{
				continue;
			}
			array = _characterThemesUnlocked[characterType];
			int k = 0;
			for (int count3 = list.Count; k < count3; k++)
			{
				bool flag = false;
				int l = 0;
				for (int num = array.Length; l < num; l++)
				{
					if (array[l] == k + 1)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (list[k].unlockType == Characters.UnlockType.coins && list[k].price <= _amountOfCoins)
					{
						return true;
					}
					if (list[k].unlockType == Characters.UnlockType.keys && list[k].price <= _amountOfKeys)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsNewUser()
	{
		return rawMultiplier <= 1;
	}

	public bool IsThemeSeenForCharacter(Characters.CharacterType character, int index)
	{
		if (index == 0)
		{
			return false;
		}
		int[] value;
		_characterThemesSeen.TryGetValue(character, out value);
		if (value == null)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (value[i] == index)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsThemeUnlockedForCharacter(Characters.CharacterType character, int index)
	{
		Characters.Model model = Characters.characterData[character];
		if (model.unlockType == Characters.UnlockType.subscription)
		{
			return _hasSubscribed;
		}
		if (index == 0)
		{
			return true;
		}
		int[] value;
		_characterThemesUnlocked.TryGetValue(character, out value);
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == index)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int HasThemeUnlockedForCharactersNum()
	{
		int num = 0;
		if (_characterThemesUnlocked.Count <= 0)
		{
			return num;
		}
		List<Characters.CharacterType> list = new List<Characters.CharacterType>(_characterThemesUnlocked.Keys);
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			Characters.CharacterType key = list[num2];
			int[] array = _characterThemesUnlocked[key];
			if (array != null)
			{
				num += array.Length;
			}
		}
		list = new List<Characters.CharacterType>(Characters.characterData.Keys);
		for (int num3 = list.Count - 1; num3 >= 0; num3--)
		{
			Characters.CharacterType characterType = list[num3];
			if (IsCollectionComplete(characterType))
			{
				num++;
			}
		}
		return num;
	}

	public bool IsSymbolUseful(Characters.CharacterType characterType)
	{
		Characters.Model model = Characters.characterData[characterType];
		return GetCollectedSymbols(characterType) < model.Price;
	}

	public void Load()
	{
		if (Application.isPlayer)
		{
			InitNew();
			return;
		}
		try
		{
			string path;
			string externalPath;
			TryGetLoadPaths(out path, out externalPath);
			MemoryStream memoryStream = new MemoryStream(FileUtil.Load(path, "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", externalPath, true));
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			binaryReader.ReadInt32();
			Dictionary<Key, string> dict = FileUtil.ReadEnumStringDictionary<Key>(binaryReader);
			amountOfCoins = LoadInt(dict, Key.AmountOfCoins, 0);
			amountOfExp = LoadInt(dict, Key.AmountOfExp, 0);
			levelTaskComplete = LoadBool(dict, Key.LevelTaskComplete, false);
			ResetHighestScoreTo(LoadInt(dict, Key.HighestScore, 0));
			_dailyLandingPayedOut = LoadBool(dict, Key.DailyLandingPayedOut, false);
			_currentCharacter = LoadInt(dict, Key.CurrentCharacter, 0);
			_currentTaskSet = LoadInt(dict, Key.CurrentMissionSet, -1);
			amountOfLevel = LoadInt(dict, Key.AmountOfLevel, 1);
			_transcendByLeadborad = LoadBool(dict, Key.TranscendByLeadborad, false);
			_amountOfSuperChestesOpened = LoadInt(dict, Key.AmountOfSuperMysteryBoxesOpened, 0);
			_hasReceivedFirstMBKey = LoadBool(dict, Key.HasFirstMBKey, false);
			_lotteryWatchViewRemainCount = LoadInt(dict, Key.LotteryRemainCount, 2);
			_lotteryFreeRemainCount = LoadInt(dict, Key.LotteryFreeCount, 1);
			_lastLotteryFreeViewDateTime = LoadDateTime(dict, Key.LotteryLastDateTime, DateTime.UtcNow);
			_lastLotteryFreeDateTime = LoadDateTime(dict, Key.LotteryFreeLastDateTime, DateTime.UtcNow);
			_lastPlayDate = LoadDateTime(dict, Key.LastPlayTime, DateTime.UtcNow);
			_hasReceivedFirstSMBKey = LoadBool(dict, Key.HasFirstSMBKey, false);
			_menuSliderShow = LoadBool(dict, Key.MenuSliderShow, true);
			_gameOverFullAdCount = LoadInt(dict, Key.GameOverFullAdCount, 0);
			_gameOverFullAdLastDate = LoadDateTime(dict, Key.GameOverFullAdLastDate, DateTime.Today);
			_forceNextCityOrder = LoadInt(dict, Key.ForceNextCityOrder, 0);
			_tutorialCompleted = LoadBool(dict, Key.TutorialCompleted, false);
			_tutorialStep = LoadInt(dict, Key.TutorialStep, 0);
			_getTaskPlayerLevel = LoadInt(dict, Key.GetTaskPlayerLeve, 1);
			_hasSubscribed = LoadBool(dict, Key.HasSubscribed, false);
			_hasRemoveAd = LoadBool(dict, Key.HasRemoveAd, false);
			_hasDoubleCoins = LoadBool(dict, Key.DoubleCoins, false);
			_numberOfRunsSinceLastGuideline = LoadInt(dict, Key.NumberOfRunsSinceLastGuideline, 0);
			_autoShowChangePlayerName = LoadBool(dict, Key.AutoShowChangePlayerName, true);
			_hasFacebookLogin = LoadBool(dict, Key.HasFacebookLogin, false);
			_firstGameOverNoRemind = LoadBool(dict, Key.FirstGameOverNoRemind, true);
			_hasShownTask1Popup = LoadBool(dict, Key.HasShownMission1Popup, false);
			_shouldShowTask1Popup = LoadBool(dict, Key.ShouldShowMission1Popup, false);
			_shouldShownPlayerMenuPopup = LoadBool(dict, Key.ShouldShownPlayerMenuPopup, false);
			_ignoreTrailRolePopup = LoadBool(dict, Key.IgnoreTrailRolePopup, false);
			_ignoreSubscriptionPopup = LoadBool(dict, Key.IgnoreSubscriptionPopup, false);
			_ignoreSubscriptionNextTime = LoadDateTime(dict, Key.IgnoreSubscriptionNextTime, DateTime.UtcNow);
			_gameoverUITryCount = LoadInt(dict, Key.GameOverUITryCount, 3);
			_gameoverUITryNextTime = LoadDateTime(dict, Key.GameOverUITryNextTime, DateTime.UtcNow);
			_gameOverDoubleCoinsShowCountOneDay = LoadInt(dict, Key.GameOverDoubleCoinsShowCountOneDay, 0);
			_gameOverDoubleCoinsShowCountTwoDay = LoadInt(dict, Key.GameOverDoubleCoinsShowCountTwoDay, 0);
			_gameOverDoubleCoinsShowCountLastDay = LoadInt(dict, Key.GameOverDoubleCoinsShowCountLastDay, 0);
			_watchDoublePlayerLevel = LoadInt(dict, Key.WatchDoublePlayerLevel, 0);
			_playerLevel = LoadInt(dict, Key.PlayerLevel, 0);
			_hasShownKeysPopup = LoadBool(dict, Key.HasShownKeysPopup, false);
			_hasShownPlayerMenuPopup = LoadBool(dict, Key.HasShownPlayerMenuPopup, false);
			amountOfKeys = LoadInt(dict, Key.AmountOfKeys, 5);
			_isNewPlayer = LoadBool(dict, Key.IsNewPlayer, false);
			_gameOverDoubleCoinViewCycleCount = LoadInt(dict, Key.AchievementProgress, 0);
			_donotClickGameOverDoubleCoinViewCount = LoadInt(dict, Key.GameOverDoubleCoinViewCycleCount, 0);
			TopRunData.Parse(LoadString(dict, Key.TopRunData, TopRunData.ToJson()));
			_lastGameOverDoubleCoinViewDate = LoadDateTime(dict, Key.LastGameOverDoubleCoinViewDate, DateTime.UtcNow);
			_pendingRewards = GetRewardsFromString(LoadString(dict, Key.PendingRewards, string.Empty));
			_gameOverDoubleConfirmNoRemind = LoadBool(dict, Key.GameOverDoubleConfirnNoRemind, false);
			_lastShowFreeUpgradeDate = LoadDateTime(dict, Key.LastShowFreeUpgradeDate, DateTime.UtcNow);
			_lastPushLocalNotificationDateTime = LoadDateTime(dict, Key.LastPushLocalNotificationDateTime, DateTime.UtcNow);
			_lastOpenGameDateTime = LoadDateTime(dict, Key.LastOpenGameDateTime, DateTime.UtcNow);
			_watchVideoSuccessNum = LoadInt(dict, Key.WatchVideoSuccessNum, 0);
			_amountOfOpenGameApp = LoadInt(dict, Key.AmountOfOpenGameApp, 0);
			_freeUpgradeCount = LoadInt(dict, Key.FreeUpgradeCount, 3);
			_showTrialPopupCount = LoadInt(dict, Key.ShowTrialPopupCount, 0);
			_firstInstallDate = LoadDateTime(dict, Key.FirstInstallDate, DateTime.UtcNow);
			_updateRewardIndex = LoadInt(dict, Key.UpdateRewardIndex, 0);
			_updateFromLastApp = LoadBool(dict, Key.UpdateFromLastApp, false);
			_playOnceTimes = LoadInt(dict, Key.PlayOnceTimes, 0);
			_playDailyTimes = LoadInt(dict, Key.PlayDailyTimes, 0);
			_updateDateTime = LoadDateTime(dict, Key.UpdateDateTime, DateTime.UtcNow);
			_openAppCountDaily = LoadInt(dict, Key.OpenAppCountDaily, 0);
			_showWatchVideoPopupCount = LoadInt(dict, Key.ShowWatchVideoPopupCount, 0);
			_numberOfRuns = LoadInt(dict, Key.NumberOfRuns, 0);
			stats = Statistics.Parse(LoadString(dict, Key.Stats, string.Empty));
			_currentTaskProgress = LoadIntArray(dict, Key.CurrentMissionSetProgress, null);
			_achievementProgress = LoadIntArray(dict, Key.AchievementProgress, new int[Achievements.NUMBER_OF_ACHIEVEMENTS]);
			_achievementAwardPayedOut = LoadBoolArray(dict, Key.AchievementRewardPayedOut, new bool[Achievements.NUMBER_OF_ACHIEVEMENTS]);
			_openAppInNewDayStartDateTime = LoadDateTime(dict, Key.OpenAppInNewDayStartDateTime, DateTime.UtcNow);
			_onlineTotalSeconds = LoadInt(dict, Key.OnlineTotalSeconds, 0);
			_onlineRewardPayedOut = LoadBoolArray(dict, Key.OnlineZonePayedOut, new bool[4]);
			_taskRewardPayedOut = LoadBoolArray(dict, Key.TaskRewardPayedOut, new bool[3]);
			_currentTrialProgress = LoadIntArray(dict, Key.CurrentTrialProgress, new int[3]);
			_currentTrialIndex = LoadInt(dict, Key.CurrentTrialIndex, -1);
			_totalTrialDays = LoadInt(dict, Key.TotalTrialDays, 0);
			_amountOfGameChestesOpened = LoadInt(dict, Key.AmountOfGameChestesOpened, 0);
			_dailyLandingInRow = LoadInt(dict, Key.DailyLandingInRow, 0);
			_dailyLandingLastPayoutDayOfYear = LoadInt(dict, Key.DailyLandingLastPayoutDayOfYear, 0);
			_taskCompletedSum = LoadInt(dict, Key.MissionCompletedSum, 0);
			int[] array = LoadIntArray(dict, Key.CollectedCharacterTokens, null);
			for (int i = 0; i < _collectedCharacterTokens.Length; i++)
			{
				_collectedCharacterTokens[i] = 0;
			}
			if (array != null)
			{
				int length = Mathf.Min(array.Length, _collectedCharacterTokens.Length);
				Array.Copy(array, _collectedCharacterTokens, length);
			}
			_hasHelmetBeenSeen = Globals.convertStringToEnumBoolDictionary<Helmets.HelmType>(LoadString(dict, Key.HasHoverboardsBeenSeen, string.Empty));
			string sourceString = LoadString(dict, Key.UnlockedHoverboardTypes, string.Empty);
			_helmetUnlockStatus = Globals.convertStringToEnumBoolDictionary<Helmets.HelmType>(sourceString);
			_inappHistory = Globals.convertStringToStringStringDictionary(LoadString(dict, Key.HistoryOfInapp, string.Empty));
			_currentHelmet = LoadEnum(dict, Key.CurrentHoverboard, Helmets.HelmType.normal);
			_hasCharacterBeenSeen = Globals.convertStringToEnumBoolDictionary<Characters.CharacterType>(LoadString(dict, Key.HasCharacterBeenSeen, string.Empty));
			_characterThemesUnlocked = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterThemesUnlocked, string.Empty));
			_lastSelectedThemes = Globals.convertStringToEnumIntDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterLastSelectedThemes, string.Empty));
			_characterThemesSeen = Globals.convertStringToEnumIntArrayDictionary<Characters.CharacterType>(LoadString(dict, Key.CharacterThemesSeen, string.Empty));
			_wallWalkTutorialCount = Globals.convertStringToStringIntDictionary(LoadString(dict, Key.WallWalkTutorialCount, string.Empty));
			foreach (KeyValuePair<PropType, int> item in FileUtil.ReadEnumIntDictionary<PropType>(binaryReader))
			{
				_upgradeAmounts[item.Key] = item.Value;
			}
			foreach (KeyValuePair<PropType, int> item2 in FileUtil.ReadEnumIntDictionary<PropType>(binaryReader))
			{
				if (_upgradeTiers.ContainsKey(item2.Key))
				{
					_upgradeTiers[item2.Key] = item2.Value;
				}
			}
			memoryStream.Close();
			_dirty = false;
		}
		catch
		{
			InitNew();
		}
	}

	private bool LoadBool(Dictionary<Key, string> dict, Key key, bool defaultValue)
	{
		string value;
		bool result;
		if (dict.TryGetValue(key, out value) && bool.TryParse(value, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private bool[] LoadBoolArray(Dictionary<Key, string> dict, Key key, bool[] defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
		{
			char[] separator = new char[1] { ',' };
			return Globals.convertAllStringToBool(value.Split(separator));
		}
		return defaultValue;
	}

	private DateTime LoadDateTime(Dictionary<Key, string> dict, Key key, DateTime defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value))
		{
			return Utils.StringToDateTime(value, defaultValue);
		}
		return defaultValue;
	}

	private T LoadEnum<T>(Dictionary<Key, string> dict, Key key, T defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value))
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value);
			}
			catch (Exception)
			{
			}
		}
		return defaultValue;
	}

	private int LoadInt(Dictionary<Key, string> dict, Key key, int defaultValue)
	{
		string value;
		int result;
		if (dict.TryGetValue(key, out value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private int[] LoadIntArray(Dictionary<Key, string> dict, Key key, int[] defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
		{
			char[] separator = new char[1] { ',' };
			return Globals.convertAllStringToInt(value.Split(separator));
		}
		return defaultValue;
	}

	private long LoadLong(Dictionary<Key, string> dict, Key key, long defaultValue)
	{
		string value;
		long result;
		if (dict.TryGetValue(key, out value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return defaultValue;
	}

	private string LoadString(Dictionary<Key, string> dict, Key key, string defaultValue)
	{
		string value;
		if (dict.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	public void LockHelm(Helmets.HelmType helmType)
	{
		if (_helmetUnlockStatus.ContainsKey(helmType))
		{
			_helmetUnlockStatus[helmType] = false;
			_dirty = true;
		}
	}

	private static void LogError(string msg, UnityEngine.Object context)
	{
		Debug.LogError(msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context)
	{
		Debug.LogWarning(msg, context);
	}

	public void MarkHelmetAsSeen(Helmets.HelmType helmType)
	{
		if (_hasHelmetBeenSeen.ContainsKey(helmType))
		{
			_hasHelmetBeenSeen[helmType] = true;
		}
		else
		{
			_hasHelmetBeenSeen.Add(helmType, true);
		}
		_dirty = true;
	}

	public void ResetHighestScoreTo(int score)
	{
		if (score != _highestScore)
		{
			_highestScore = score;
			_dirty = true;
		}
	}

	public void RunCompleted()
	{
		_dirty = true;
		_numberOfRunsSinceLastGuideline++;
		_numberOfRuns++;
		if (!_hasShowDailyLandingPopup)
		{
			bool Istoday;
			GetDailyLandingDaysInRow(out Istoday);
			if (Istoday)
			{
				_shouldShowDailyLandingPopup = false;
				_hasShowDailyLandingPopup = true;
			}
			else
			{
				_shouldShowDailyLandingPopup = true;
			}
		}
		if (_numberOfRunsSinceLastGuideline <= 4)
		{
			return;
		}
		_numberOfRunsSinceLastGuideline = 0;
		if (!_hasShownTask1Popup)
		{
			if (rawMultiplier < 5)
			{
				_shouldShowTask1Popup = true;
				_dirty = true;
				return;
			}
			_hasShownTask1Popup = true;
			_shouldShowTask1Popup = false;
			_dirty = true;
		}
		if (!_hasShownPlayerMenuPopup)
		{
			if (HasThemeUnlockedForCharactersNum() <= 1)
			{
				_shouldShownPlayerMenuPopup = true;
				_dirty = true;
			}
			else
			{
				_hasShownPlayerMenuPopup = true;
				_shouldShownPlayerMenuPopup = false;
				_dirty = true;
			}
		}
	}

	private void Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			Dictionary<Key, string> dict = new Dictionary<Key, string>();
			SaveInt(dict, Key.AmountOfCoins, amountOfCoins);
			SaveInt(dict, Key.AmountOfLevel, amountOfLevel);
			SaveInt(dict, Key.AmountOfExp, amountOfExp);
			SaveBool(dict, Key.LevelTaskComplete, levelTaskComplete);
			SaveInt(dict, Key.HighestScore, _highestScore);
			SaveBool(dict, Key.DailyLandingPayedOut, _dailyLandingPayedOut);
			SaveInt(dict, Key.CurrentCharacter, _currentCharacter);
			SaveInt(dict, Key.CurrentMissionSet, _currentTaskSet);
			SaveDateTime(dict, Key.LastShowFreeUpgradeDate, _lastShowFreeUpgradeDate);
			SaveInt(dict, Key.FreeUpgradeCount, _freeUpgradeCount);
			SaveBool(dict, Key.TranscendByLeadborad, _transcendByLeadborad);
			SaveInt(dict, Key.LotteryRemainCount, _lotteryWatchViewRemainCount);
			SaveDateTime(dict, Key.LotteryLastDateTime, _lastLotteryFreeViewDateTime);
			SaveInt(dict, Key.LotteryFreeCount, _lotteryFreeRemainCount);
			SaveDateTime(dict, Key.LotteryFreeLastDateTime, _lastLotteryFreeDateTime);
			SaveDateTime(dict, Key.LastPlayTime, _lastPlayDate);
			SaveInt(dict, Key.AmountOfSuperMysteryBoxesOpened, _amountOfSuperChestesOpened);
			SaveBool(dict, Key.HasFirstMBKey, _hasReceivedFirstMBKey);
			SaveBool(dict, Key.HasFirstSMBKey, _hasReceivedFirstSMBKey);
			SaveBool(dict, Key.TutorialCompleted, _tutorialCompleted);
			SaveInt(dict, Key.TutorialStep, _tutorialStep);
			SaveInt(dict, Key.GetTaskPlayerLeve, _getTaskPlayerLevel);
			SaveBool(dict, Key.HasSubscribed, _hasSubscribed);
			SaveBool(dict, Key.HasRemoveAd, _hasRemoveAd);
			SaveBool(dict, Key.DoubleCoins, _hasDoubleCoins);
			SaveDateTime(dict, Key.FirstInstallDate, _firstInstallDate);
			SaveInt(dict, Key.ShowWatchVideoPopupCount, _showWatchVideoPopupCount);
			SaveInt(dict, Key.ShowTrialPopupCount, _showTrialPopupCount);
			SaveInt(dict, Key.NumberOfRunsSinceLastGuideline, _numberOfRunsSinceLastGuideline);
			SaveBool(dict, Key.AutoShowChangePlayerName, _autoShowChangePlayerName);
			SaveBool(dict, Key.HasFacebookLogin, _hasFacebookLogin);
			SaveBool(dict, Key.MenuSliderShow, _menuSliderShow);
			SaveBool(dict, Key.FirstGameOverNoRemind, _firstGameOverNoRemind);
			SaveBool(dict, Key.HasShownMission1Popup, _hasShownTask1Popup);
			SaveBool(dict, Key.ShouldShowMission1Popup, _shouldShowTask1Popup);
			SaveBool(dict, Key.ShouldShownPlayerMenuPopup, _shouldShownPlayerMenuPopup);
			SaveInt(dict, Key.GameOverFullAdCount, _gameOverFullAdCount);
			SaveDateTime(dict, Key.GameOverFullAdLastDate, _gameOverFullAdLastDate);
			SaveInt(dict, Key.ForceNextCityOrder, _forceNextCityOrder);
			SaveBool(dict, Key.IgnoreSubscriptionPopup, _ignoreSubscriptionPopup);
			SaveDateTime(dict, Key.IgnoreSubscriptionNextTime, _ignoreSubscriptionNextTime);
			SaveInt(dict, Key.GameOverUITryCount, _gameoverUITryCount);
			SaveDateTime(dict, Key.GameOverUITryNextTime, _gameoverUITryNextTime);
			SaveBool(dict, Key.HasShownPlayerMenuPopup, _hasShownPlayerMenuPopup);
			SaveInt(dict, Key.GameOverDoubleCoinsShowCountOneDay, _gameOverDoubleCoinsShowCountOneDay);
			SaveInt(dict, Key.GameOverDoubleCoinsShowCountTwoDay, _gameOverDoubleCoinsShowCountTwoDay);
			SaveInt(dict, Key.GameOverDoubleCoinsShowCountLastDay, _gameOverDoubleCoinsShowCountLastDay);
			SaveInt(dict, Key.WatchDoublePlayerLevel, _watchDoublePlayerLevel);
			SaveInt(dict, Key.PlayOnceTimes, _playOnceTimes);
			SaveInt(dict, Key.PlayDailyTimes, _playDailyTimes);
			SaveDateTime(dict, Key.UpdateDateTime, _updateDateTime);
			SaveInt(dict, Key.OpenAppCountDaily, _openAppCountDaily);
			SaveDateTime(dict, Key.LastGameOverDoubleCoinsDateTime, _lastGameOverDoubleCoinsDateTime);
			SaveInt(dict, Key.PlayerLevel, _playerLevel);
			SaveBool(dict, Key.IgnoreTrailRolePopup, _ignoreTrailRolePopup);
			SaveBool(dict, Key.HasShownKeysPopup, _hasShownKeysPopup);
			SaveInt(dict, Key.UpdateRewardIndex, _updateRewardIndex);
			SaveBool(dict, Key.UpdateFromLastApp, _updateFromLastApp);
			SaveDateTime(dict, Key.LastPushLocalNotificationDateTime, _lastPushLocalNotificationDateTime);
			SaveDateTime(dict, Key.LastOpenGameDateTime, _lastOpenGameDateTime);
			SaveInt(dict, Key.WatchVideoSuccessNum, _watchVideoSuccessNum);
			SaveInt(dict, Key.AmountOfOpenGameApp, _amountOfOpenGameApp);
			SaveInt(dict, Key.GameOverDoubleCoinViewCycleCount, _gameOverDoubleCoinViewCycleCount);
			SaveInt(dict, Key.DonotClickGameOverDoubleCoinViewCount, _donotClickGameOverDoubleCoinViewCount);
			SaveBool(dict, Key.IsNewPlayer, _isNewPlayer);
			SaveDateTime(dict, Key.LastGameOverDoubleCoinViewDate, _lastGameOverDoubleCoinViewDate);
			SaveString(dict, Key.TopRunData, TopRunData.ToJson());
			if (_currentTaskSet >= 0)
			{
				SaveIntArray(dict, Key.CurrentMissionSetProgress, _currentTaskProgress);
			}
			SaveIntArray(dict, Key.AchievementProgress, _achievementProgress);
			SaveBoolArray(dict, Key.AchievementRewardPayedOut, _achievementAwardPayedOut);
			SaveBoolArray(dict, Key.OnlineZonePayedOut, _onlineRewardPayedOut);
			SaveBoolArray(dict, Key.TaskRewardPayedOut, _taskRewardPayedOut);
			SaveInt(dict, Key.OnlineTotalSeconds, _onlineTotalSeconds);
			SaveDateTime(dict, Key.OpenAppInNewDayStartDateTime, _openAppInNewDayStartDateTime);
			SaveIntArray(dict, Key.CurrentTrialProgress, _currentTrialProgress);
			SaveInt(dict, Key.CurrentTrialIndex, _currentTrialIndex);
			SaveInt(dict, Key.TotalTrialDays, _totalTrialDays);
			SaveIntArray(dict, Key.CollectedCharacterTokens, _collectedCharacterTokens);
			SaveInt(dict, Key.DailyLandingInRow, _dailyLandingInRow);
			SaveInt(dict, Key.DailyLandingLastPayoutDayOfYear, _dailyLandingLastPayoutDayOfYear);
			SaveInt(dict, Key.MissionCompletedSum, _taskCompletedSum);
			SaveString(dict, Key.HasHoverboardsBeenSeen, Globals.convertEnumBoolDictionaryToString(_hasHelmetBeenSeen));
			SaveString(dict, Key.UnlockedHoverboardTypes, Globals.convertEnumBoolDictionaryToString(_helmetUnlockStatus));
			SaveEnum(dict, Key.CurrentHoverboard, _currentHelmet);
			SaveString(dict, Key.HistoryOfInapp, Globals.convertStringStringDictionaryToString(_inappHistory));
			SaveString(dict, Key.HasCharacterBeenSeen, Globals.convertEnumBoolDictionaryToString(_hasCharacterBeenSeen));
			SaveInt(dict, Key.AmountOfKeys, amountOfKeys);
			SaveString(dict, Key.CharacterThemesUnlocked, Globals.convertEnumIntArrayDictionaryToString(_characterThemesUnlocked));
			SaveString(dict, Key.CharacterLastSelectedThemes, Globals.convertEnumIntDictionaryToString(_lastSelectedThemes));
			SaveString(dict, Key.CharacterThemesSeen, Globals.convertEnumIntArrayDictionaryToString(_characterThemesSeen));
			SaveString(dict, Key.WallWalkTutorialCount, Globals.convertStringIntDictionaryToString(_wallWalkTutorialCount));
			SaveString(dict, Key.PendingRewards, GetStringFromRewards(_pendingRewards));
			SaveBool(dict, Key.GameOverDoubleConfirnNoRemind, _gameOverDoubleConfirmNoRemind);
			SaveInt(dict, Key.NumberOfRuns, _numberOfRuns);
			SaveString(dict, Key.Stats, stats.ToString());
			SaveInt(dict, Key.AmountOfGameChestesOpened, _amountOfGameChestesOpened);
			FileUtil.WriteEnumStringDictionary(binaryWriter, dict);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeAmounts);
			FileUtil.WriteEnumIntDictionary(binaryWriter, _upgradeTiers);
			FileUtil.Save(GetSavePath(), memoryStream.GetBuffer(), "we12rtyuiklhgfdjerKJGHfvghyuhnjiokLJHl145rtyfghjvbn", 0, (int)memoryStream.Length, 1, 5, null);
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception ex)
		{
			LogError("Error saving player info: " + ex, null);
		}
	}

	private void SaveBool(Dictionary<Key, string> dict, Key key, bool value)
	{
		dict[key] = value.ToString();
	}

	private void SaveBoolArray(Dictionary<Key, string> dict, Key key, bool[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllBoolToString(value));
	}

	private void SaveDateTime(Dictionary<Key, string> dict, Key key, DateTime value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveEnum<T>(Dictionary<Key, string> dict, Key key, T value)
	{
		dict[key] = value.ToString();
	}

	public void SaveIfDirty()
	{
		if (_dirty || stats.dirty)
		{
			Save();
		}
	}

	private void SaveInt(Dictionary<Key, string> dict, Key key, int value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveIntArray(Dictionary<Key, string> dict, Key key, int[] value)
	{
		dict[key] = string.Join(",", Globals.convertAllIntToString(value));
	}

	private void SaveLong(Dictionary<Key, string> dict, Key key, long value)
	{
		dict[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	private void SaveString(Dictionary<Key, string> dict, Key key, string value)
	{
		dict[key] = value;
	}

	public void SetCurrentTaskProgress(int task, int progress)
	{
		if (task >= 3)
		{
			_achievementProgress[task - 3] = progress;
		}
		else if (_currentTaskProgress[task] != progress)
		{
			_currentTaskProgress[task] = progress;
		}
		_dirty = true;
	}

	public bool GetCurrentAchievementAward(int index)
	{
		if (index < 0 || index >= _achievementAwardPayedOut.Length)
		{
			return false;
		}
		return _achievementAwardPayedOut[index];
	}

	public bool GetAllAchievementAward()
	{
		for (int i = 0; i < Achievements.NUMBER_OF_ACHIEVEMENTS; i++)
		{
			if (TasksManager.Instance.GetTaskInfo(i + 3).complete && !_achievementAwardPayedOut[i])
			{
				return true;
			}
		}
		return false;
	}

	public void SetCurrentAchivementReward(int index, bool value)
	{
		if (index >= 0 && index < _achievementAwardPayedOut.Length)
		{
			_achievementAwardPayedOut[index] = value;
			NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.AchiementFinished);
			_dirty = true;
		}
	}

	public void CheckOnlineNewDayOpen()
	{
		if ((DateTime.UtcNow.Date - _openAppInNewDayStartDateTime.Date).Days == 0)
		{
			_openAppInNewDayStartDateTime = DateTime.UtcNow;
			return;
		}
		_openAppInNewDayStartDateTime = DateTime.UtcNow;
		_onlineTotalSeconds = 0;
		int i = 0;
		for (int num = _onlineRewardPayedOut.Length; i < num; i++)
		{
			_onlineRewardPayedOut[i] = false;
		}
	}

	public void SetCurrentTaskReward(int index, bool value)
	{
		if (index >= 0 && index < _taskRewardPayedOut.Length)
		{
			_taskRewardPayedOut[index] = value;
			_dirty = true;
		}
	}

	public bool GetIndexTaskRewardPayedOut(int index)
	{
		return _taskRewardPayedOut[index];
	}

	public void ResetTaskReward()
	{
		int i = 0;
		for (int num = _taskRewardPayedOut.Length; i < num; i++)
		{
			_taskRewardPayedOut[i] = false;
		}
		_dirty = false;
	}

	public bool TaskRewardAllPayed()
	{
		bool result = false;
		int i = 0;
		for (int num = _taskRewardPayedOut.Length; i < num; i++)
		{
			if (!_taskRewardPayedOut[i])
			{
				return false;
			}
			result = true;
		}
		return result;
	}

	public void CalcOnlineTotalSeconds()
	{
		int num = (int)(DateTime.UtcNow - _openAppInNewDayStartDateTime).TotalSeconds;
		if (num > 0)
		{
			_onlineTotalSeconds += num;
			_openAppInNewDayStartDateTime = _openAppInNewDayStartDateTime.AddSeconds(num);
			_dirty = true;
		}
	}

	public int GetOnlineTime()
	{
		CalcOnlineTotalSeconds();
		return _onlineTotalSeconds;
	}

	public bool GetOnlineZonePayedOut(int index)
	{
		if (index < 0 || index >= _onlineRewardPayedOut.Length)
		{
			return false;
		}
		return _onlineRewardPayedOut[index];
	}

	public void SetOnlineZonePayedOut(int index, bool value)
	{
		if (index >= 0 && index < _onlineRewardPayedOut.Length)
		{
			_onlineRewardPayedOut[index] = value;
		}
	}

	public bool AllOnlineZonePayedOut()
	{
		int i = 0;
		for (int num = _onlineRewardPayedOut.Length; i < num; i++)
		{
			if (!_onlineRewardPayedOut[i])
			{
				return false;
			}
		}
		return true;
	}

	public void SetLastSelectedTheme(Characters.CharacterType character, int themeIndex)
	{
		if (Application.isEditor)
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(character);
			if (list != null && (list.Count < themeIndex || themeIndex < 0))
			{
				Debug.LogError("The theme index is too great " + themeIndex + " for character: " + character.ToString());
			}
		}
		if (_lastSelectedThemes.ContainsKey(character))
		{
			_lastSelectedThemes.Remove(character);
		}
		if (themeIndex > 0)
		{
			_lastSelectedThemes.Add(character, themeIndex);
		}
		_dirty = true;
	}

	public bool CheckIfLotteryCanFree()
	{
		if (DateTime.UtcNow > _lastLotteryFreeDateTime)
		{
			_lotteryFreeRemainCount = 1;
		}
		return _lotteryFreeRemainCount > 0;
	}

	public bool CheckIfLotteryCanWatchFreeView()
	{
		if ((DateTime.UtcNow.Date - _lastLotteryFreeViewDateTime.Date).Days != 0)
		{
			_lastLotteryFreeViewDateTime = DateTime.UtcNow;
			_lotteryWatchViewRemainCount = 2;
		}
		if (_lotteryWatchViewRemainCount > 0)
		{
			return true;
		}
		return false;
	}

	public string LotteryFreeTimeSpan()
	{
		TimeSpan timeSpan = _lastLotteryFreeDateTime - DateTime.UtcNow;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public void UseLotteryFree()
	{
		_lotteryFreeRemainCount--;
		if (_lotteryFreeRemainCount <= 0)
		{
			_lastLotteryFreeDateTime = DateTime.UtcNow.AddHours(4.0);
		}
	}

	public void UseLotteryWatchView()
	{
		_lotteryWatchViewRemainCount--;
	}

	public bool CheckIfFreeUpgrade()
	{
		if ((DateTime.UtcNow.Date - _lastShowFreeUpgradeDate.Date).Days > 0)
		{
			_lastShowFreeUpgradeDate = DateTime.UtcNow;
			_freeUpgradeCount = 3;
		}
		if ((DateTime.UtcNow - _lastShowFreeUpgradeDate).TotalSeconds >= 0.0 && _freeUpgradeCount > 0)
		{
			return true;
		}
		return false;
	}

	public void UseFreeUpgrade()
	{
		_freeUpgradeCount--;
		_lastShowFreeUpgradeDate = DateTime.UtcNow.AddSeconds(_freeUpgradeInterval);
	}

	public void ThemeSeen(Characters.CharacterType character, int index)
	{
		int[] value;
		_characterThemesSeen.TryGetValue(character, out value);
		if (value == null)
		{
			value = new int[1] { index };
		}
		else
		{
			int[] array = new int[value.Length + 1];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = value[i];
			}
			array[value.Length] = index;
			value = array;
		}
		_characterThemesSeen.Remove(character);
		_characterThemesSeen.Add(character, value);
		_dirty = true;
	}

	public void TriggerOnScoreMultiplierChanged()
	{
		Action action = onScoreMultiplierChanged;
		if (action != null)
		{
			action();
		}
	}

	private static void TryGetLoadPaths(out string path, out string externalPath)
	{
		path = Application.persistentDataPath + "/playerdata";
		externalPath = null;
	}

	public void UnlockHelmet(Helmets.HelmType helmType)
	{
		if (_helmetUnlockStatus.ContainsKey(helmType))
		{
			_helmetUnlockStatus[helmType] = true;
		}
		else
		{
			_helmetUnlockStatus.Add(helmType, true);
		}
		Action<Helmets.HelmType> onHelmUnlocked = OnHelmUnlocked;
		if (onHelmUnlocked != null)
		{
			onHelmUnlocked(helmType);
		}
		_dirty = true;
	}

	public void UnlockTheme(Characters.CharacterType character, int index)
	{
		int[] value;
		_characterThemesUnlocked.TryGetValue(character, out value);
		if (value == null)
		{
			value = new int[1] { index };
		}
		else
		{
			int[] array = new int[value.Length + 1];
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == index)
				{
					return;
				}
				array[i] = value[i];
			}
			array[value.Length] = index;
			value = array;
		}
		_characterThemesUnlocked.Remove(character);
		_characterThemesUnlocked.Add(character, value);
		_dirty = true;
		Action<Characters.CharacterType> onCharacterOutfitUnlocked = OnCharacterOutfitUnlocked;
		if (onCharacterOutfitUnlocked != null)
		{
			onCharacterOutfitUnlocked(character);
		}
	}

	public void UseUpgrade(PropType type)
	{
		if (_upgradeAmounts.ContainsKey(type))
		{
			_upgradeAmounts[type]--;
			_dirty = true;
			Action action = onPowerupAmountChanged;
			if (action != null)
			{
				action();
			}
		}
	}

	public bool CheckGameoverUITry()
	{
		if ((DateTime.UtcNow.Date - _gameoverUITryNextTime.Date).Days > 0)
		{
			_gameoverUITryCount = 3;
			return true;
		}
		if (DateTime.UtcNow > _gameoverUITryNextTime && _gameoverUITryCount > 0)
		{
			return true;
		}
		return false;
	}

	public void ShowGameoverUITry()
	{
		_gameoverUITryNextTime = DateTime.UtcNow.AddMinutes(5.0);
		_gameoverUITryCount--;
		_dirty = true;
	}

	public int CheckGameOverDoubleCoinViewRate(int coins)
	{
		int result = 2;
		if ((DateTime.UtcNow.Date - _lastGameOverDoubleCoinViewDate.Date).Days == 0)
		{
			if (_gameOverDoubleCoinViewCycleCount >= 2)
			{
				return result;
			}
			if (_donotClickGameOverDoubleCoinViewCount == 1)
			{
				result = 3;
			}
			else if (_donotClickGameOverDoubleCoinViewCount == 2)
			{
				result = ((coins >= 500) ? 3 : 5);
			}
			else if (_donotClickGameOverDoubleCoinViewCount >= 3)
			{
				result = ((coins <= 500) ? 10 : ((coins > 1500) ? 3 : 5));
			}
			return result;
		}
		_lastGameOverDoubleCoinViewDate = DateTime.UtcNow;
		_gameOverDoubleCoinViewCycleCount = 0;
		_donotClickGameOverDoubleCoinViewCount = 0;
		return result;
	}

	public void ResetGameOverDoubleCoinViewRate()
	{
		_gameOverDoubleCoinViewCycleCount++;
		_donotClickGameOverDoubleCoinViewCount = 0;
	}

	public void DonotClickDoubleCoinView()
	{
		_donotClickGameOverDoubleCoinViewCount++;
	}

	public bool CheckWallWalkingTutorial(string cityName)
	{
		if (!_wallWalkTutorialCount.ContainsKey(cityName))
		{
			_wallWalkTutorialCount.Add(cityName, 0);
		}
		return _wallWalkTutorialCount[cityName] < 10;
	}

	public void AddWallWalkingTutorial(string cityName)
	{
		if (!_wallWalkTutorialCount.ContainsKey(cityName))
		{
			_wallWalkTutorialCount.Add(cityName, 0);
		}
		_wallWalkTutorialCount[cityName]++;
	}

	public int UpdateSpanDays()
	{
		return (DateTime.UtcNow.Date - _updateDateTime.Date).Days;
	}

	public void CheckGameOverDoubleCoinsSpanDays()
	{
		int days = (DateTime.UtcNow.Date - _lastGameOverDoubleCoinsDateTime.Date).Days;
		_lastGameOverDoubleCoinsDateTime = DateTime.UtcNow;
		if (days == 2)
		{
			_gameOverDoubleCoinsShowCountOneDay = _gameOverDoubleCoinsShowCountTwoDay;
			_gameOverDoubleCoinsShowCountTwoDay = _gameOverDoubleCoinsShowCountLastDay;
			_gameOverDoubleCoinsShowCountLastDay = 0;
		}
		else if (days == 3)
		{
			_gameOverDoubleCoinsShowCountOneDay = _gameOverDoubleCoinsShowCountTwoDay;
			_gameOverDoubleCoinsShowCountTwoDay = 0;
			_gameOverDoubleCoinsShowCountLastDay = 0;
		}
		else if (days > 3)
		{
			_gameOverDoubleCoinsShowCountOneDay = 0;
			_gameOverDoubleCoinsShowCountTwoDay = 0;
			_gameOverDoubleCoinsShowCountLastDay = 0;
		}
	}

	public int LastThreeDaysGameOverDoubleCoinsCount()
	{
		return _gameOverDoubleCoinsShowCountOneDay + _gameOverDoubleCoinsShowCountTwoDay + _gameOverDoubleCoinsShowCountLastDay;
	}

	public void UpdateGameOverDoubleCoinsShowCount()
	{
		_gameOverDoubleCoinsShowCountOneDay = _gameOverDoubleCoinsShowCountTwoDay;
		_gameOverDoubleCoinsShowCountTwoDay = _gameOverDoubleCoinsShowCountLastDay;
		_gameOverDoubleCoinsShowCountLastDay = 0;
	}

	public void NextTrialLevel()
	{
		int num = TrialManager.Instance.CurrentTrialIndex();
		int num2 = _currentTrialProgress[num] % 100 + 1;
		_currentTrialProgress[num] = num2;
		TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
		if (num2 < currentTrialInfo.aim)
		{
			return;
		}
		if (currentTrialInfo.type == TrialType.Character)
		{
			if (currentTrialInfo.characterThemeId == 0)
			{
				CollectSymbol(currentTrialInfo.characterType, int.MaxValue);
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters);
			}
			else
			{
				UnlockTheme(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			}
			NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
			UIScreenController.Instance.AddUnlockForCharacterToReward(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			UIModelController.Instance.SelectCharacterForPlay(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			SaveIfDirty();
		}
		else if (currentTrialInfo.type == TrialType.Helmet)
		{
			UnlockHelmet(currentTrialInfo.helmetType);
			UIScreenController.Instance.AddUnlockForHelmetToReward(currentTrialInfo.helmetType);
			currentHelmet = currentTrialInfo.helmetType;
		}
		TrialManager.Instance.currentTrialInfo = null;
	}

	public void IncreaseTrialProgress(int number)
	{
		int num = TrialManager.Instance.CurrentTrialIndex();
		_currentTrialProgress[num] += number * 100;
		int num2 = _currentTrialProgress[num] / 100;
		TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
		if (currentTrialInfo == null)
		{
			return;
		}
		int num3 = currentTrialInfo.taskAim;
		if (currentTrialInfo.type == TrialType.Character)
		{
			num3 *= 1000;
		}
		if (num2 < num3)
		{
			return;
		}
		int num4 = _currentTrialProgress[num] % 100 + 1;
		_currentTrialProgress[num] = num4;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "try_role_hoverboard_fill", 0);
		if (num4 < currentTrialInfo.aim)
		{
			return;
		}
		if (currentTrialInfo.type == TrialType.Character)
		{
			if (currentTrialInfo.characterThemeId == 0)
			{
				CollectSymbol(currentTrialInfo.characterType, int.MaxValue);
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters);
			}
			else
			{
				UnlockTheme(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			}
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_try_role_hoverboard", 0);
			NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
			switch (Characters.characterOrder.IndexOf(currentTrialInfo.characterType))
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
			UIScreenController.Instance.AddUnlockForCharacterToReward(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			UIModelController.Instance.SelectCharacterForPlay(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
		}
		else if (currentTrialInfo.type == TrialType.Helmet)
		{
			UnlockHelmet(currentTrialInfo.helmetType);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_try_role_hoverboard", 0);
			switch (Helmets.helmOrder.IndexOf(currentTrialInfo.helmetType))
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
			UIScreenController.Instance.AddUnlockForHelmetToReward(currentTrialInfo.helmetType);
			currentHelmet = currentTrialInfo.helmetType;
		}
		SaveIfDirty();
		TrialManager.Instance.currentTrialInfo = null;
	}

	public int CurrentTrialInfoProgress()
	{
		return _currentTrialProgress[TrialManager.Instance.CurrentTrialIndex()] / 100;
	}

	public int CurrentTrialInfoLevel()
	{
		return _currentTrialProgress[TrialManager.Instance.CurrentTrialIndex()] % 100;
	}

	public int NextTrial()
	{
		_currentTrialIndex++;
		if (_currentTrialIndex >= _currentTrialProgress.Length)
		{
			_currentTrialIndex = 0;
		}
		return _currentTrialIndex;
	}
}
