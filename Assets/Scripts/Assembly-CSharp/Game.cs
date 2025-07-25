using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
	public delegate void OnGameOverDelegate(GameStats gameStats);

	public delegate void OnIntroRunDelegate();

	public delegate void OnPauseChangeDelegate(bool pause);

	public delegate void OnSpeedChangedDelegate(float speed);

	public delegate void OnStageMenuSequenceDelegate();

	public delegate void OnTopMenuDelegate();

	public delegate void OnTurboHeadstartInputDelegate();

	[Serializable]
	public class SpeedInfo
	{
		public float min = 110f;

		public float max = 220f;

		public float rampUpDuration = 200f;
	}

	[Serializable]
	public class SwipeInfo
	{
		public float distanceMin = 0.1f;

		public float doubleTapDuration = 0.3f;
	}

	public bool isDead;

	public float currentSpeed;

	public float currentLevelSpeed = 30f;

	public float distancePerMeter = 8f;

	[SerializeField]
	private SwipeInfo swipe;

	public SpeedInfo speed;

	[SerializeField]
	private float backToCheckpointDelayTime = 0.7f;

	[SerializeField]
	private float backToCheckpointZoomTime = 1f;

	[SerializeField]
	private CharacterAttachmentCollection attachment;

	[HideInInspector]
	public Character character;

	[HideInInspector]
	public TrackController trackController;

	[SerializeField]
	private FollowZ followZ;

	private bool firstStart;

	public OnPauseChangeDelegate OnPauseChange;

	public OnStageMenuSequenceDelegate OnStageMenuSequence;

	public OnIntroRunDelegate OnIntroRun;

	public Variable<bool> IsInGame = new Variable<bool>(false);

	public Variable<bool> IsInTopMenu = new Variable<bool>(false);

	[HideInInspector]
	public bool awakeDone;

	[HideInInspector]
	public bool isReadyForSlideinPowerups;

	[HideInInspector]
	public bool wasButtonClicked;

	public TopMenuAnimations topMenu;

	public bool Paused;

	private bool _game_paused;

	private Animation characterAnimation;

	private CharacterCamera characterCamera;

	private static CharacterController characterController;

	private CharacterRendering characterRendering;

	private CharacterState characterState;

	private Swipe currentSwipe;

	private IEnumerator currentThread;

	private Boss boss;

	private bool goingBackToCheckpoint;

	public static bool HasLoaded;

	private static Game instance;

	private float internalStartTime;

	private float t;

	private int mRunSecond;

	private int mLastRunSecond;

	public Action OnGameEnded;

	public Action OnGameStarted;

	public Action OnMenuScreenShown;

	private Running running;

	private Flypack flypack;

	private SpringJump springJump;

	private BoundJump boundJump;

	private SpeedUp speedup;

	private TraversingCity traversingCity;

	private Raft raft;

	private Die die;

	private WallWalking wallWalking;

	private float startTime;

	private float startGameTime;

	private GameStats stats;

	public bool IsTestChar;

	public int preCharacter;

	public int preCharacterSkin;

	public bool IsTestHelm;

	public int preHelmet;

	[HideInInspector]
	public bool show20sAd = true;

	[HideInInspector]
	public float showAdTime = -20f;

	[HideInInspector]
	public bool closePopupOnAdEvent = true;

	private float expCoefficient;

	public int newPlayerSkipTaskNum = 3;

	private int touchCount;

	private float doubleTapDelay = 0.5f;

	private Vector2 delta = Vector2.zero;

	private Character.CriticalHitType type;

	public Character Character
	{
		get
		{
			return character;
		}
	}

	public static CharacterController Charactercontroller
	{
		get
		{
			if (characterController == null)
			{
				characterController = UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController;
			}
			return characterController;
		}
	}

	public Character.CriticalHitType HitType
	{
		get
		{
			return type;
		}
	}

	public bool HasSuperShoes
	{
		get
		{
			return attachment.SuperShoes.IsActive;
		}
	}

	public static Game Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Utils.FindObject<Game>();
			}
			return instance;
		}
	}

	public bool IsInFlypackMode
	{
		get
		{
			return characterState == flypack;
		}
	}

	public bool IsInSpringJumpMode
	{
		get
		{
			return characterState == springJump;
		}
	}

	public bool IsInBoundJumpMode
	{
		get
		{
			return characterState == boundJump;
		}
	}

	public bool isPaused
	{
		get
		{
			return _game_paused;
		}
	}

	public Flypack Jetpack
	{
		get
		{
			return flypack;
		}
	}

	public CharacterState CharacterState
	{
		get
		{
			return characterState;
		}
	}

	public Raft Raft
	{
		get
		{
			return raft;
		}
	}

	public CharacterAttachmentCollection Attachment
	{
		get
		{
			return attachment;
		}
	}

	public float NormalizedGameSpeed
	{
		get
		{
			return currentSpeed / speed.min;
		}
	}

	public float DefaultSpeedForAnimation
	{
		get
		{
			return 1f;
		}
	}

	public SpringJump SpringJump
	{
		get
		{
			return springJump;
		}
	}

	public Running Running
	{
		get
		{
			return running;
		}
	}

	public bool IsInRunningMode
	{
		get
		{
			return characterState == running;
		}
	}

	public bool GoingBackToCheckpoint
	{
		get
		{
			return goingBackToCheckpoint;
		}
	}

	public bool IsIntroEnd { get; private set; }

	public string lastShowAd { get; set; }

	public event OnSpeedChangedDelegate OnSpeedChanged;

	public event OnTurboHeadstartInputDelegate OnTurboHeadstartInput;

	private InputActions inputActions;

	private SwipeDir AnalyzeSwipe(Swipe swipe)
	{
		Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(swipe.start.x, swipe.start.y, 2f));
		if (Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(swipe.end.x, swipe.end.y, 2f)), b) < this.swipe.distanceMin)
		{
			return SwipeDir.None;
		}
		Vector3 lhs = swipe.end - swipe.start;
		SwipeDir result = SwipeDir.None;
		float num = 0f;
		float num2 = Vector3.Dot(lhs, Vector3.up);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Up;
		}
		num2 = Vector3.Dot(lhs, Vector3.down);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Down;
		}
		num2 = Vector3.Dot(lhs, Vector3.left);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Left;
		}
		num2 = Vector3.Dot(lhs, Vector3.right);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Right;
		}
		return result;
	}

	public void Awake()
	{
		character = Character.Instance;
		character.Initialize();
		characterRendering = CharacterRendering.Instance;
		characterAnimation = characterRendering.characterAnimation;
		trackController = TrackController.Instance;
		characterCamera = CharacterCamera.Instance;
		running = Running.Instance;
		flypack = Flypack.Instance;
		raft = Raft.Instance;
		die = Die.Instance;
		wallWalking = WallWalking.Instance;
		springJump = SpringJump.Instance;
		boundJump = BoundJump.Instance;
		speedup = SpeedUp.Instance;
		traversingCity = TraversingCity.Instance;
		boss = Boss.Instance;
		boss.Initialize();
		attachment = new CharacterAttachmentCollection();
		character.OnStumble += OnStumble;
		character.OnCriticalHit += OnCriticalHit;
		currentLevelSpeed = Speed(0f, speed);
		stats = GameStats.Instance;
		show20sAd = true;
		showAdTime = 0f;
		startGameTime = Time.time;
		//ImageManager.Instance.Load();
		if (PlayerInfo.Instance.hasFacebookLogin)
		{
			FacebookManger.Instance.LoginFacebook();
		}
		awakeDone = true;

		inputActions = new InputActions();


		inputActions.Play.Interact.performed += OnInteract;
		inputActions.Play.Up.performed += OnUp;
		inputActions.Play.Down.performed += OnDown;
		inputActions.Play.Left.performed += OnLeft;
		inputActions.Play.Right.performed += OnRight;

	}

	private void OnEnable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
		RiseSdkListener.OnPaymentEvent += PayResult;
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;

		inputActions.Enable();
	}

	private void OnDisable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		ResetTest(true);
		if (PlayerInfo.Instance != null)
		{
			PlayerInfo.Instance.SaveIfDirty();
		}

		inputActions.Disable();
	}



	public void ResetTest(bool quit = false)
	{
		if (IsTestChar)
		{
			IsTestChar = false;
			if (!quit && CharacterScreenManager.Instance != null)
			{
				CharacterScreenManager.Instance.SelectCharacter((Characters.CharacterType)preCharacter, preCharacterSkin);
			}
			else if (PlayerInfo.Instance != null)
			{
				PlayerInfo.Instance.currentCharacter = preCharacter;
				PlayerInfo.Instance.currentThemeIndex = preCharacterSkin;
			}
		}
		if (IsTestHelm)
		{
			IsTestHelm = false;
			if (PlayerInfo.Instance != null)
			{
				PlayerInfo.Instance.currentHelmet = (Helmets.HelmType)preHelmet;
			}
		}
	}

	public void TestCharacter(Characters.CharacterType characterType, int themeId)
	{
		IsTestChar = true;
		preCharacter = PlayerInfo.Instance.currentCharacter;
		preCharacterSkin = PlayerInfo.Instance.currentThemeIndex;
		CharacterScreenManager.Instance.SelectCharacter(characterType, themeId);
	}

	public void TestHelmet(Helmets.HelmType helmType)
	{
		IsTestHelm = true;
		preHelmet = (int)PlayerInfo.Instance.currentHelmet;
		PlayerInfo.Instance.currentHelmet = helmType;
	}

	public bool IsInTest()
	{
		return IsTestChar || IsTestHelm;
	}

	public void Start()
	{
		StartCoroutine(GameIntro());
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom");
		}
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		Debug.Log(string.Concat("TestSuoyin; ", type, ",Id:", id, ",AdType: ", eventType));
		if (eventType != 1 && eventType != 2)
		{
			return;
		}
		if (eventType == 2)
		{
			if (type == RiseSdk.AdEventType.RewardAdShowFinished)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_all_success", 0);
				PlayerInfo.Instance.WatchVideoSuccessNum++;
				if (closePopupOnAdEvent)
				{
					UIScreenController.Instance.ClosePopup(null);
				}
				closePopupOnAdEvent = false;
			}
			if (type == RiseSdk.AdEventType.RewardAdShowFailed)
			{
				if (closePopupOnAdEvent)
				{
					UIScreenController.Instance.ClosePopup(null);
				}
				closePopupOnAdEvent = false;
			}
		}
		if (eventType != 1)
		{
			return;
		}
		if (type == RiseSdk.AdEventType.FullAdShown)
		{
			RiseSdk.Instance.TrackEvent("show_interstitial_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_interstitial_all_success", 0);
			RiseSdk.Instance.TrackEvent(lastShowAd, "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, lastShowAd, 0);
		}
		if (type == RiseSdk.AdEventType.FullAdClosed)
		{
			if (closePopupOnAdEvent)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
			closePopupOnAdEvent = false;
		}
	}

	public void PayResult(RiseSdk.PaymentResult result, int billId)
	{
		if (result == RiseSdk.PaymentResult.Success)
		{
			int num = 0;
			switch (billId)
			{
				case 0:
					PlayerInfo.Instance.amountOfCoins += 7500;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, 7500);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, 7500);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 1:
					PlayerInfo.Instance.amountOfCoins += 18000;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, 18000);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, 18000);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 2:
					PlayerInfo.Instance.amountOfCoins += 30000;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, 30000);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, 30000);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 3:
					PlayerInfo.Instance.amountOfCoins += 45000;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, 45000);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, 45000);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 4:
					PlayerInfo.Instance.amountOfCoins += 100000;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, 10000);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, 10000);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 5:
					num = ((!PlayerInfo.Instance.hasSubscribed) ? 10 : 15);
					PlayerInfo.Instance.amountOfKeys += num;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, num);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_buy", 0, num);
					if (UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
					{
						SaveMeManager.SendReviveIfPurchaseSucceeded();
						UIScreenController.Instance.ClosePopup(null);
					}
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 6:
					num = ((!PlayerInfo.Instance.hasSubscribed) ? 25 : 38);
					PlayerInfo.Instance.amountOfKeys += num;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, num);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_buy", 0, num);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 7:
					num = ((!PlayerInfo.Instance.hasSubscribed) ? 80 : 120);
					PlayerInfo.Instance.amountOfKeys += num;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, num);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_buy", 0, num);
					UIScreenController.Instance.ClosePopup(null);
					break;
				case 8:
					num = ((!PlayerInfo.Instance.hasSubscribed) ? 300 : 450);
					PlayerInfo.Instance.amountOfKeys += num;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, num);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_buy", 0, num);
					UIScreenController.Instance.ClosePopup(null);
					break;
			}
		}
		else if (billId == 5 && UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME)
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
			UIScreenController.Instance.ClosePopup(null);
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	public void Pause(bool active)
	{
		Debug.Log("Suo Pause:" + active);
		if (active)
		{
			Paused = true;
			Time.timeScale = 0f;
			AudioListener.volume = 0f;
		}
		else
		{
			Paused = false;
			Time.timeScale = 1f;
			AudioListener.volume = (Settings.optionSound ? 1 : 0);
		}
	}

	public void ChangeCurrentSpeed(float timeOffset)
	{
		internalStartTime -= timeOffset;
	}

	public void ChangeState(CharacterState state)
	{
		characterState = state;
		if (state != null)
		{
			currentThread = state.Begin();
		}
	}

	public void GameStart()
	{
		IsIntroEnd = true;
		startGameTime = Time.time;
		UIScreenController.isGetBtnPressed = false;
		if (OnGameStarted != null)
		{
			OnGameStarted();
		}
	}

	public Dictionary<LevelExpManager.LevelAwardType, int> GetTaskLevelRewards(float level)
	{
		Dictionary<LevelExpManager.LevelAwardType, int> dictionary = new Dictionary<LevelExpManager.LevelAwardType, int>();
		foreach (KeyValuePair<int, LevelExpManager.LevelExpData> levelExpData in LevelExpManager.levelExpDatas)
		{
			if (level >= (float)levelExpData.Value.minLevel && level < (float)levelExpData.Value.maxLevel)
			{
				for (int i = 0; i < levelExpData.Value.levelAwards.Length; i++)
				{
					dictionary.Add(levelExpData.Value.levelAwards[i].levelAwardType, levelExpData.Value.levelAwards[i].awardNum);
				}
				return dictionary;
			}
			if (levelExpData.Key >= LevelExpManager.levelExpDatas.Count)
			{
				for (int j = 0; j < levelExpData.Value.levelAwards.Length; j++)
				{
					dictionary.Add(levelExpData.Value.levelAwards[j].levelAwardType, levelExpData.Value.levelAwards[j].awardNum);
				}
				return dictionary;
			}
		}
		return dictionary;
	}

	public float GetExpCoefficient(int level)
	{
		float result = 29f;
		foreach (KeyValuePair<int, LevelExpManager.LevelExpData> levelExpData in LevelExpManager.levelExpDatas)
		{
			if (level >= levelExpData.Value.minLevel && level < levelExpData.Value.maxLevel)
			{
				return levelExpData.Value.expCoefficient;
			}
			if (levelExpData.Key >= LevelExpManager.levelExpDatas.Count)
			{
				return levelExpData.Value.expCoefficient;
			}
		}
		return result;
	}

	public float GetExpCoefficient()
	{
		return GetExpCoefficient(PlayerInfo.Instance.amountOfLevel);
	}

	public float GetStartGameDuration()
	{
		return Time.time - startGameTime;
	}

	public float GetDuration()
	{
		return Time.time - startTime;
	}

	public float GetNextAdDuration()
	{
		return Time.time - showAdTime;
	}

	public void HandleControls()
	{
		if (doubleTapDelay <= 0f)
		{
			touchCount = 0;
			doubleTapDelay = 0f;
		}
		else
		{
			doubleTapDelay -= Time.deltaTime;
		}
		if (Paused || _game_paused || Input.touchCount <= 0)
		{
			return;
		}
		Touch touch = Input.touches[0];
		if (touch.phase == UnityEngine.TouchPhase.Began)
		{
			currentSwipe = new Swipe();
			currentSwipe.start = touch.position;
			currentSwipe.startTime = Time.time;
			if (touchCount == 0)
			{
				doubleTapDelay = 0.25f;
			}
			touchCount++;
			if (touchCount == 2)
			{
				characterState.HandleDoubleTap();
				touchCount = 0;
			}
		}
		if (touch.phase == UnityEngine.TouchPhase.Moved)
		{
			delta += touch.deltaPosition;
			if (Vector2.SqrMagnitude(delta) > 0.1f)
			{
				touchCount = 0;
			}
		}
		if ((touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled) && currentSwipe != null)
		{
			if (touch.phase == UnityEngine.TouchPhase.Ended)
			{
				delta = Vector2.zero;
			}
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			SwipeDir swipeDir = AnalyzeSwipe(currentSwipe);
			if (swipeDir != SwipeDir.None)
			{
				if (characterState != null)
				{
					characterState.HandleSwipe(swipeDir);
				}
				currentSwipe = null;
			}
		}
		if (touch.phase == UnityEngine.TouchPhase.Ended && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			if (AnalyzeSwipe(currentSwipe) == SwipeDir.None && characterState != null)
			{
				HandleTap();
			}
		}
	}

	private void HandleDebugControls()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.Chest);
			GameStats.Instance.chestPickups += 1 << TrackController.Instance.nextChestIndex - 1;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			PlayerInfo.Instance.NextTrialLevel();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Break();
		}
		if (Input.GetKeyUp(KeyCode.N))
		{
			Megaheadstart();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			GameStats.Instance.superShoesPickups++;
			attachment.Add(attachment.SuperShoes);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			GameStats.Instance.coinMagnetsPickups++;
			attachment.Add(attachment.CoinMagnet);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			attachment.Stop();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			PlayerInfo.Instance.amountOfCoins += 10000;
		}
		if (characterState != null)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				characterState.HandleSwipe(SwipeDir.Up);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				characterState.HandleSwipe(SwipeDir.Down);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				characterState.HandleSwipe(SwipeDir.Left);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				characterState.HandleSwipe(SwipeDir.Right);
			}
			if (Input.GetKeyDown(KeyCode.F12))
			{
				characterState.HandleDoubleTap();
			}
		}
	}

	private void OnUp(InputAction.CallbackContext context)
	{
		characterState.HandleSwipe(SwipeDir.Up);
	}

	private void OnDown(InputAction.CallbackContext context)
	{
		characterState.HandleSwipe(SwipeDir.Down);
	}

	private void OnLeft(InputAction.CallbackContext context)
	{
		characterState.HandleSwipe(SwipeDir.Left);
	}

	private void OnRight(InputAction.CallbackContext context)
	{
		characterState.HandleSwipe(SwipeDir.Right);
	}

	private void OnInteract(InputAction.CallbackContext context)
	{
		characterState.HandleDoubleTap();
	}



	private bool HandleTap()
	{
		bool result = false;
		wasButtonClicked = false;
		return result;
	}

	public void LayTrackChunks()
	{
		trackController.ChangeBackgroundMusic(character.z);
		trackController.LayTrackPieces(character.z);
	}

	private float Speed(float t, SpeedInfo speedInfo)
	{
		if (t < speedInfo.rampUpDuration)
		{
			return t * (speedInfo.max - speedInfo.min) / speedInfo.rampUpDuration + speedInfo.min;
		}
		return speedInfo.max;
	}

	public void ResetEnemy()
	{
		boss.enabled = true;
		boss.MuteProximityLoop();
		boss.ResetCatchUp();
		boss.ResetModelRootPosition();
		boss.ShowEnemies(false);
	}

	public void NewPlayerRunDuration()
	{
		if (GetDuration() <= 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_0_30s", 0);
		}
		else if (GetDuration() <= 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_30_60s", 0);
		}
		else if (GetDuration() <= 90f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_60_90s", 0);
		}
		else if (GetDuration() <= 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_90_120s", 0);
		}
		else if (GetDuration() <= 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_120_180s", 0);
		}
		else if (GetDuration() <= 240f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_180_240s", 0);
		}
		else if (GetDuration() <= 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_240_300s", 0);
		}
		else if (GetDuration() > 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "pass_300s_", 0);
		}
	}

	public void NormalPlayerRunDuration()
	{
		if (GetDuration() < 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time_0_30s", 0);
		}
		else if (GetDuration() < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time30_60s", 0);
		}
		else if (GetDuration() < 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time1_2min", 0);
		}
		else if (GetDuration() < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time2_3min", 0);
		}
		else if (GetDuration() < 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time3_5min", 0);
		}
		else if (GetDuration() < 360f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time5_6min", 0);
		}
		else if (GetDuration() < 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time6_7min", 0);
		}
		else if (GetDuration() >= 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "play_time7_min", 0);
		}
	}

	public void UM_OnRunDuration()
	{
	}

	public void TriggerPause(bool pauseGame)
	{
		_game_paused = pauseGame;
		if (pauseGame)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		if (OnPauseChange != null)
		{
			OnPauseChange(_game_paused);
		}
	}

	public void Update()
	{
		if (!SaveMeManager.IS_PURCHASE_RUNNING_INGAME && !trackController.IsRunningOnTutorialTrack)
		{
			t = Time.time - internalStartTime;
			currentLevelSpeed = Speed(t, speed);
			mRunSecond = (int)GetDuration();
			if (mRunSecond > mLastRunSecond)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.TimeDeath, mRunSecond - mLastRunSecond);
				mLastRunSecond = mRunSecond;
			}
		}
		else
		{
			startTime += Time.deltaTime;
			startGameTime += Time.deltaTime;
			internalStartTime += Time.deltaTime;
		}
		if (characterState != null && currentThread != null)
		{
			currentThread.MoveNext();
		}
		if (!Paused && !_game_paused && characterState != null)
		{
			attachment.Update();
		}
		GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
		{
			RiseSdk.Instance.OnExit();
		}


	}

	public void UpdateMeters()
	{
		stats.meters = Mathf.RoundToInt(character.z / distancePerMeter);
	}

	private IEnumerator TopMenu()
	{
		IsInGame.Value = false;
		IsInTopMenu.Value = true;
		AudioPlayer.Instance.PlayMusic("Start_station_bg", 1f);
		followZ.enabled = false;
		boss.MuteProximityLoop();
		trackController.DeactivateTrackPieces();
		attachment.Resume();
		attachment.StopWithNoEnding();
		attachment.Update();
		GameStats.Instance.ClearPowerups();
		flypack.coinsManager.ReleaseCoins();
		springJump.coinsManager.ReleaseCoins();
		StageMenuSequence();
		characterCamera.SetCameraToTopMenu();
		if (firstStart)
		{
			characterCamera.Animation.Play("introPan");
			characterCamera.Animation["introPan"].speed = 1f;
			float time = characterCamera.Animation["introPan"].length;
			while (time > 0f)
			{
				time -= Time.deltaTime;
				if (Input.GetMouseButtonDown(0))
				{
					characterCamera.Animation["introPan"].speed *= 2f;
					time /= 2f;
				}
				yield return null;
			}
			firstStart = false;
			if (UIScreenController.isInstanced)
			{
				UIScreenController.Instance.ShowMainMenu();
			}
		}
		if (OnMenuScreenShown != null)
		{
			OnMenuScreenShown();
		}
		yield return null;
	}

	private IEnumerator Intro()
	{
		stats.Reset();
		attachment.Stop();
		attachment.Update();
		IsIntroEnd = false;
		boss.MuteProximityLoop();
		isDead = false;
		character.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = PickupParticles.coinEfxOffset;
		boss.ShowEnemies(true);
		boss.PlayIntro();
		trackController.Restart();
		trackController.LayTrackPieces(0f);
		currentLevelSpeed = Speed(0f, speed);
		startTime = Time.time;
		Game game = this;
		Game game2 = this;
		int num = 0;
		game2.mRunSecond = 0;
		game.mLastRunSecond = num;
		internalStartTime = Time.time;
		character.Restart();
		SpawnUpgradeManager.Instance.Restart();
		characterCamera.Animation["startPan"].speed = 0.6f;
		characterCamera.Animation.Play("startPan");
		topMenu.OnNewGameStart();
		InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit();
		UIScreenController.isGetBtnPressed = true;
		if (OnIntroRun != null)
		{
			OnIntroRun();
		}
		float time = Time.time + characterCamera.Animation["startPan"].length / 0.6f;
		float fov_start = characterCamera.Camera.fieldOfView;
		float fov_end = characterCamera.CurrentCameraFollowPlayerConfig.NormalConfig.cameraFOV / characterCamera.Camera.aspect;
		while (time > Time.time)
		{
			characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_end, fov_start, (time - Time.time) / 0.6f);
			yield return null;
		}
		characterCamera.Camera.fieldOfView = fov_end;
		characterCamera.StartRun(character.transform.position, Quaternion.identity);
		boss.enabled = true;
		followZ.enabled = true;
		topMenu.Continue();
		if (trackController.IsRunningOnTutorialTrack)
		{
			boss.ResetCatchUp();
		}
		if (trackController.IsRunningOnTutorialTrack)
		{
			isReadyForSlideinPowerups = false;
		}
		else
		{
			isReadyForSlideinPowerups = true;
		}
		ChangeState(Running);
		yield return null;
	}

	private void StageMenuSequence()
	{
		boss.ShowEnemies(false);
		boss.enemies[0].localPosition = Vector3.zero;
		boss.StopAllCoroutines();
		boss.enabled = false;
		character.StopAllCoroutines();
		CharacterModel characterModel = character.characterModel;
		characterModel.meshBlobShadow.enabled = true;
		characterAnimation = characterModel.characterAnimation;
		topMenu.StartPlayIdleRummagesAnimation();
		characterCamera.enabled = false;
		characterCamera.Camera.fieldOfView = 43f / characterCamera.Camera.aspect;
		if (OnStageMenuSequence != null)
		{
			OnStageMenuSequence();
		}
		if (character.superShoes.IsActive)
		{
			character.superShoes.StopUse();
			character.IsJumpingHigher = false;
		}
		Helmet.Instance.HardReset();
		Jetpack.ResetTurboHeadstart();
		GameStats.Instance.ClearPowerups();
		GameStats.Instance.PAUSEPOWERUPS = false;
	}

	private IEnumerator GameIntro()
	{
		firstStart = true;
		ChangeState(null);
		StartCoroutine(TopMenu());
		yield return null;
	}

	public void StartTopMenu()
	{
		ChangeState(null);
		StartCoroutine(TopMenu());
	}

	public void StartGame()
	{
		if (Instance != null)
		{
			StartNewRun(false);
			UIScreenController.Instance.PushScreen("IngameUI");
			SaveMeManager.ResetSaveMeForNewRun();
		}
		else
		{
			UIScreenController.Instance.PushScreen("GameoverUI");
		}
	}

	public void StartNewRun(bool duel)
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
		IsInTopMenu.Value = false;
		IsInGame.Value = true;
		AudioPlayer.Instance.PlaySound("ChristmasGameStart", 0.5f, 1f, 1f);
		ChangeState(null);
		StartCoroutine(Intro());
		SendRunCharCount();
	}

	private void SendRunCharCount()
	{
		int num = 0;
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
		{
			if (PlayerInfo.Instance.IsCollectionComplete(characterDatum.Key))
			{
				num++;
			}
		}
		if (num >= 2)
		{
			switch (Characters.characterOrder.IndexOf(CharacterScreenManager.Instance.currenCharacterShown))
			{
				case 0:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles01", 0);
					break;
				case 1:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles02", 0);
					break;
				case 2:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles03", 0);
					break;
				case 3:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles04", 0);
					break;
				case 4:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles05", 0);
					break;
				case 5:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles06", 0);
					break;
				case 6:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles07", 0);
					break;
				case 7:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles08", 0);
					break;
				case 8:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles09", 0);
					break;
				case 9:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles10", 0);
					break;
				case 10:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles11", 0);
					break;
				case 11:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles12", 0);
					break;
				case 12:
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "roles_run_roles13", 0);
					break;
			}
		}
	}

	private IEnumerator BackToCheckPointSequence()
	{
		goingBackToCheckpoint = true;
		running.Pause = true;
		yield return new WaitForSeconds(backToCheckpointDelayTime);
		running.Pause = false;
		if (IsInGame.Value)
		{
			character.SetBackToCheckPoint(backToCheckpointZoomTime);
			yield return new WaitForSeconds(backToCheckpointZoomTime);
		}
		goingBackToCheckpoint = false;
	}

	public void WillDie()
	{
		if (attachment.IsActive(attachment.Helmet))
		{
			boss.Restart(true);
			attachment.Helmet.Stop = StopFlag.STOP;
			boss.MuteProximityLoop();
			boss.ResetCatchUp();
			if (character.IsStumbling)
			{
				character.StopStumble();
			}
			GameStats.Instance.RemoveHoverHelmPowerup();
			if (type == Character.CriticalHitType.FallIntoWater)
			{
				character.SetFrontToCheckPoint();
			}
			return;
		}
		if (trackController.IsRunningOnTutorialTrack)
		{
			if (!goingBackToCheckpoint)
			{
				StartCoroutine(BackToCheckPointSequence());
			}
			return;
		}
		GameStats.Instance.PAUSEPOWERUPS = true;
		MovingO.ActivateAutoPilot();
		boss.enabled = false;
		if (boss.isShowing)
		{
			if (characterAnimation["death_moving"].enabled)
			{
				boss.ShowEnemies(false);
			}
			else if (characterAnimation["intoWater"].enabled)
			{
				boss.FallIntoWater();
			}
			else
			{
				boss.CatchPlayer(characterAnimation);
			}
		}
		isDead = true;
		stats.duration = GetDuration();
		if (OnGameEnded != null)
		{
			OnGameEnded();
		}
		StopAllCoroutines();
		ChangeState(die);
	}

	public void Revive()
	{
		ResetEnemy();
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		character.ResetWalls();
		GameStats.Instance.PAUSEPOWERUPS = false;
		IsInGame.Value = true;
		StopAllCoroutines();
		characterCamera.Revive();
		if (type == Character.CriticalHitType.FallIntoWater)
		{
			character.SetFrontToCheckPoint();
		}
		ChangeState(Running);
		isDead = false;
	}

	public IEnumerator SkipRevive()
	{
		yield return new WaitForSeconds(0.1f);
		die.SkipRevive = true;
	}

	public void OnCriticalHit(Character.CriticalHitType type)
	{
		if (characterState != null)
		{
			this.type = type;
			AudioPlayer.Instance.PlaySound("leyou_Hr_death", true);
			characterState.HandleCriticalHit(type != Character.CriticalHitType.FallIntoWater);
		}
	}

	public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (character.IsStumbling && characterState != null)
		{
			StartCoroutine(StumbleDeathSequence());
		}
	}

	private IEnumerator StumbleDeathSequence()
	{
		currentSpeed = speed.min;
		yield return new WaitForSeconds(0.2f);
		if (IsInGame.Value && !IsInFlypackMode && !IsInSpringJumpMode && !IsInBoundJumpMode)
		{
			characterAnimation.CrossFade("death_bounce", 0.2f);
			if (characterState != null)
			{
				characterState.HandleCriticalHit();
			}
		}
	}

	public void OnStartWall(SwipeDir dir, Wall wall)
	{
		wallWalking.SetData(dir, wall.Height, wall.Bounds.max.z);
		ChangeState(wallWalking);
	}

	public void Megaheadstart()
	{
		if (!flypack.ActivateTurboHeadstart)
		{
			if (isDead)
			{
				return;
			}
			Jetpack.headStart = true;
			Jetpack.powerType = PropType.headstart;
			ChangeState(Jetpack);
		}
		if (this.OnTurboHeadstartInput != null)
		{
			this.OnTurboHeadstartInput();
		}
	}

	public void PickupFlypack()
	{
		Instance.StartFlypack();
		GameStats.Instance.flypackPickups++;
	}

	public void PickupPogostick(bool willShowPickup)
	{
		StartSpringJump(willShowPickup);
		GameStats.Instance.powerJumperPickups++;
	}

	public void PickupBound(float jumpHeight, float jumpDistance, float totalDistance, float startY)
	{
		StartBoundJump(jumpHeight, jumpDistance, totalDistance, new Vector3(character.transform.position.x, startY, character.transform.position.z));
		GameStats.Instance.powerJumperPickups++;
	}

	public void PickupTransition()
	{
		ChangeState(traversingCity);
	}

	public void StartFlypack()
	{
		flypack.headStart = false;
		flypack.powerType = PropType.flypack;
		ChangeState(Jetpack);
	}

	public void StartSpeedUp()
	{
		ChangeState(speedup);
	}

	public void StartBoundJump(float jumpHeight, float jumpDistance, float totalDistance, Vector3 position)
	{
		boundJump.SetBoundJumpData(jumpHeight, jumpDistance, totalDistance, position);
		ChangeState(boundJump);
	}

	public void StartSpringJump(bool willShowPickup)
	{
		SpringJump.powerType = PropType.springJump;
		SpringJump.WillShowPickup = willShowPickup;
		ChangeState(SpringJump);
	}
}
