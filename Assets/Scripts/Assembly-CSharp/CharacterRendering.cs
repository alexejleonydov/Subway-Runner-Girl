using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRendering : MonoBehaviour
{
	public delegate void CharacterModelInitializedDelegate(GameObject helmetRoot);

	[Serializable]
	public class AnimationClipLists
	{
		public AnimationClip[] run;

		public AnimationClip[] superRun;

		public AnimationClip[] startWallRunLeft;

		public AnimationClip[] wallRunLeft;

		public AnimationClip[] endWallRunLeft;

		public AnimationClip[] startWallRunRight;

		public AnimationClip[] wallRunRight;

		public AnimationClip[] endWallRunRight;

		public AnimationClip[] jump;

		public AnimationClip[] hangtime;

		public AnimationClip[] landing;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;

		public AnimationClip[] roll;

		public AnimationClip[] hitMid;

		public AnimationClip[] hitUpper;

		public AnimationClip[] hitLower;

		public AnimationClip[] hitMoving;

		public AnimationClip[] fallWater;

		public AnimationClip[] pickChest;

		public AnimationClip[] stumble;

		public AnimationClip[] stumbleMix;

		public AnimationClip[] stumbleDeath;

		public AnimationClip[] stumbleLeftSide;

		public AnimationClip[] stumbleRightSide;

		public AnimationClip[] stumbleLeftCorner;

		public AnimationClip[] stumbleRightCorner;
	}

	[Serializable]
	public class Animations
	{
		public string[] RUN = new string[1] { "run" };

		public string[] SUPERRUN = new string[1] { "run" };

		public string[] START_WALLRUN_LEFT = new string[1] { "startWallRun" };

		public string[] WALLRUN_LEFT = new string[1] { "wallRun" };

		public string[] END_WALLRUN_LEFT = new string[1] { "endWallRun" };

		public string[] START_WALLRUN_RIGHT = new string[1] { "startWallRun" };

		public string[] WALLRUN_RIGHT = new string[1] { "wallRun" };

		public string[] END_WALLRUN_RIGHT = new string[1] { "endWallRun" };

		public string[] LAND = new string[1] { "landing" };

		public string[] JUMP = new string[3] { "jump", "jump2", "jump_salto" };

		public string[] HANGTIME = new string[1] { "hangtime" };

		public string[] ROLL = new string[1] { "roll" };

		public string[] DODGE_LEFT = new string[1] { "dodgeLeft" };

		public string[] DODGE_RIGHT = new string[1] { "dodgeRight" };

		public string[] TURBO_HEADSTART = new string[1] { "dodgeRight" };

		public string[] HIT_MID = new string[1] { "hitMid" };

		public string[] HIT_UPPER = new string[1] { "hitUpper" };

		public string[] HIT_LOWER = new string[1] { "hitLower" };

		public string[] HIT_MOVING = new string[1] { "hitMoving" };

		public string[] FALLWATER = new string[1] { "intoWater" };

		public string[] PICKCHEST = new string[1] { "pickChest" };

		public string[] STUMBLE_MIX = new string[1] { "stumble" };

		public string[] STUMBLE_LEFT_SIDE = new string[1] { "stumbleLeftSide" };

		public string[] STUMBLE_RIGHT_SIDE = new string[1] { "stumbleRightSide" };

		public string[] STUMBLE_LEFT_CORNER = new string[1] { "stumbleLeftCorner" };

		public string[] STUMBLE_RIGHT_CORNER = new string[1] { "stumbleRightCorner" };

		public string[] GRIND = new string[1] { "run" };

		public string[] GET_ON = new string[1] { "run" };

		public string[] STUMBLE = new string[1] { "stumble" };

		public string DEFAULT_ANIMATION;

		public string DodgeLeft
		{
			get
			{
				return GetRandomAnimationName(DODGE_LEFT);
			}
		}

		public string DodgeRight
		{
			get
			{
				return GetRandomAnimationName(DODGE_RIGHT);
			}
		}

		public string GetOn
		{
			get
			{
				return GetRandomAnimationName(GET_ON);
			}
		}

		public string Grind
		{
			get
			{
				return GetRandomAnimationName(GRIND);
			}
		}

		public string Hangtime
		{
			get
			{
				return GetRandomAnimationName(HANGTIME);
			}
		}

		public string HitLower
		{
			get
			{
				return GetRandomAnimationName(HIT_LOWER);
			}
		}

		public string HitMid
		{
			get
			{
				return GetRandomAnimationName(HIT_MID);
			}
		}

		public string HitMoving
		{
			get
			{
				return GetRandomAnimationName(HIT_MOVING);
			}
		}

		public string FallWater
		{
			get
			{
				return GetRandomAnimationName(FALLWATER);
			}
		}

		public string PickChest
		{
			get
			{
				return GetRandomAnimationName(PICKCHEST);
			}
		}

		public string HitUpper
		{
			get
			{
				return GetRandomAnimationName(HIT_UPPER);
			}
		}

		public string Jump
		{
			get
			{
				return GetRandomAnimationName(JUMP);
			}
		}

		public string Land
		{
			get
			{
				return GetRandomAnimationName(LAND);
			}
		}

		public string Roll
		{
			get
			{
				return GetRandomAnimationName(ROLL);
			}
		}

		public string Run
		{
			get
			{
				return GetRandomAnimationName(RUN);
			}
		}

		public string SuperRun
		{
			get
			{
				return GetRandomAnimationName(SUPERRUN);
			}
		}

		public string StartWallRunLeft
		{
			get
			{
				return GetRandomAnimationName(START_WALLRUN_LEFT);
			}
		}

		public string WallRunLeft
		{
			get
			{
				return GetRandomAnimationName(WALLRUN_LEFT);
			}
		}

		public string EndWallRunLeft
		{
			get
			{
				return GetRandomAnimationName(END_WALLRUN_LEFT);
			}
		}

		public string StartWallRunRight
		{
			get
			{
				return GetRandomAnimationName(START_WALLRUN_RIGHT);
			}
		}

		public string WallRunRight
		{
			get
			{
				return GetRandomAnimationName(WALLRUN_RIGHT);
			}
		}

		public string EndWallRunRight
		{
			get
			{
				return GetRandomAnimationName(END_WALLRUN_RIGHT);
			}
		}

		public string Stumble
		{
			get
			{
				return GetRandomAnimationName(STUMBLE);
			}
		}

		public string StumbleLeftCorner
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_LEFT_CORNER);
			}
		}

		public string StumbleLeftSide
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_LEFT_SIDE);
			}
		}

		public string StumbleMix
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_MIX);
			}
		}

		public string StumbleRightCorner
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_RIGHT_CORNER);
			}
		}

		public string StumbleRightSide
		{
			get
			{
				return GetRandomAnimationName(STUMBLE_RIGHT_SIDE);
			}
		}

		private string GetRandomAnimationName(string[] animationsNames)
		{
			int num = UnityEngine.Random.Range(0, animationsNames.Length);
			return animationsNames[num];
		}

		private string[] GetRandomHoverJumps(string[] helmetJump, string[] helmetHangtime)
		{
			if (helmetJump.Length != helmetHangtime.Length)
			{
				Debug.LogWarning("helmetJump.Length (" + helmetJump.Length + ") != (" + helmetHangtime.Length + ") helmetHangtime.Length");
			}
			int num = UnityEngine.Random.Range(0, Mathf.Min(helmetJump.Length, helmetHangtime.Length));
			return new string[2]
			{
				helmetJump[num],
				helmetHangtime[num]
			};
		}
	}

	[Serializable]
	public class JetpackClips
	{
		public AnimationClip[] run;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;
	}

	[Serializable]
	public class PogostickClips
	{
		public AnimationClip[] run;

		public AnimationClip[] hangtime;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;
	}

	[Serializable]
	public class SuperSneaksClips
	{
		public AnimationClip[] run;
	}

	[Serializable]
	public class WaterBoardClips
	{
		public AnimationClip[] run;
	}

	[SerializeField]
	private AnimationClipLists defaultAnimations;

	[SerializeField]
	private JetpackClips jetpackAnimations;

	[SerializeField]
	private SuperSneaksClips SuperShoesAnimations;

	[SerializeField]
	private PogostickClips pogostickClips;

	[SerializeField]
	private WaterBoardClips WaterBoardAnimations;

	[OptionalField]
	public Animation characterAnimation;

	[SerializeField]
	private AnimationCurve jetpackParticleOffsetCurve;

	public Animations animations;

	[SerializeField]
	private GameObject characterModelPrefab;

	[SerializeField]
	private GameObject characterRenderingEffectsPrefab;

	private List<AnimationClip> addedAnimClipsNames = new List<AnimationClip>();

	private AnimationState caught;

	private Character character;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRenderingEffects characterRenderingEffects;

	private GameObject currentHelmet;

	private HelmetRendering.AnimationList defaultHelmetAnimationList;

	private HelmetRendering defaultHelmetRendering;

	private RaftRendering reftRenderer;

	private Boss boss;

	private Game game;

	private string hangtimeAnimation;

	private Helmet helmet;

	private Raft raft;

	private Animation helmetAnimation;

	private HelmetRendering helmetRendering;

	private Animation raftAnimation;

	private RaftRendering raftRendering;

	private Vector3 effectInitRot = Vector3.zero;

	private Vector3 effectInitScale = Vector3.one;

	private static CharacterRendering instance;

	private bool isRolling;

	private Flypack flypack;

	private SpeedUp speedup;

	private string jumpAnimation;

	private SpringJump springJump;

	private BoundJump boundJump;

	private Revive revive;

	private SuperShoes superShoes;

	private WallWalking wallWalking;

	private bool[] waterBoards;

	private bool isOnWaterBoard;

	public CharacterModel CharacterModel
	{
		get
		{
			return characterModel;
		}
	}

	public static CharacterRendering Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Utils.FindObject<CharacterRendering>();
			}
			return instance;
		}
	}

	public event CharacterModelInitializedDelegate CharacterModelInitialized;

	private string GetHelmetAnimationName(string animationName)
	{
		if (helmetAnimation.GetClip(animationName) != null)
		{
			return animationName;
		}
		return animations.DEFAULT_ANIMATION;
	}

	private string GetRaftAnimationName(string animationName)
	{
		if (raftAnimation.GetClip(animationName) != null)
		{
			return animationName;
		}
		return animations.DEFAULT_ANIMATION;
	}

	private void SpringJumpOnFlyAheadStart()
	{
		effectInitRot = characterRenderingEffects.FlypackParticles.transform.rotation.eulerAngles;
		effectInitScale = characterRenderingEffects.FlypackParticles.transform.localScale;
	}

	public void Initialize()
	{
		InitializeCharacterModel();
		InitializeAnimations();
		waterBoards = new bool[3];
		isOnWaterBoard = false;
		game = Game.Instance;
		character = Character.Instance;
		character.characterModel = characterModel;
		characterController = Game.Charactercontroller;
		superShoes = SuperShoes.Instance;
		helmet = Helmet.Instance;
		raft = Raft.Instance;
		flypack = Flypack.Instance;
		springJump = SpringJump.Instance;
		boundJump = BoundJump.Instance;
		wallWalking = WallWalking.Instance;
		boss = Boss.Instance;
		revive = Revive.Instance;
		InitializeCharacterRenderingEffects();
		character.OnChangeTrack += OnChangeTrack;
		character.OnStumble += OnStumble;
		character.OnTutorialMoveBackToCheckPoint += OnTutorialMoveBackToCheckPoint;
		character.OnTutorialStartFromCheckPoint += OnTutorialStartFromCheckPoint;
		character.OnHitByTrain += OnHitByTrain;
		character.OnFallIntoWater += OnFallIntoWater;
		character.OnJump += OnJump;
		character.OnJumpIfHitByTrain += OnJump;
		character.OnRoll += OnRoll;
		character.OnLanding += OnLanding;
		character.OnJumpRoll += OnJumpRoll;
		character.OnHangtime += OnHangtime;
		character.IsGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(character.IsGrounded.OnChange, new Variable<bool>.OnChangeDelegate(OnChangeIsGrounded));
		character.OnHandleWaterBoard += OnHandleWaterBoard;
		WaterBoard.GetOnWaterBoard = (Action<int>)Delegate.Combine(WaterBoard.GetOnWaterBoard, new Action<int>(OnGetOnWaterBoard));
		WaterBoard.GetOffWaterBoard = (Action<int>)Delegate.Combine(WaterBoard.GetOffWaterBoard, new Action<int>(OnGetOffWaterBoard));
		IngameChestPickedHelper.OnPickedChest += OnPickedChest;
		speedup = SpeedUp.Instance;
		SpeedUp speedUp = speedup;
		speedUp.OnStart = (SpeedUp.OnStartDelegate)Delegate.Combine(speedUp.OnStart, new SpeedUp.OnStartDelegate(OnSpeedupStart));
		SpeedUp speedUp2 = speedup;
		speedUp2.OnStop = (SpeedUp.OnStopDelegate)Delegate.Combine(speedUp2.OnStop, new SpeedUp.OnStopDelegate(OnSpeedupStop));
		game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(IsInGame_OnChange));
		game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(OnStageMenuSequence));
		game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(game.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
		game.OnTurboHeadstartInput += HandleOnTurboHeadstart;
		game.OnGameStarted = (Action)Delegate.Combine(game.OnGameStarted, new Action(OnGameStart));
		helmet.OnSwitchToHelmet += OnSwitchToHelmet;
		helmet.OnEndHelmet += OnSwitchToRunning;
		helmet.OnJump += OnJump;
		helmet.OnRun += OnRun;
		raft.OnSwitchToRoft += OnSwitchToRaft;
		raft._OnEndRaft += OnEndRaft;
		flypack.OnStart = (Flypack.OnStartDelegate)Delegate.Combine(flypack.OnStart, new Flypack.OnStartDelegate(OnSwitchToFlypack));
		flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(flypack.OnStop, new Flypack.OnStopDelegate(FlypackOnStop));
		flypack.OnFlyAheadStart = (Flypack.OnFlyAheadStartDelegate)Delegate.Combine(flypack.OnFlyAheadStart, new Flypack.OnFlyAheadStartDelegate(SpringJumpOnFlyAheadStart));
		flypack.OnFlyAheadUpdate = (Flypack.OnFlyAheadUpdateDelegate)Delegate.Combine(flypack.OnFlyAheadUpdate, new Flypack.OnFlyAheadUpdateDelegate(FlypackOnFlyAheadUpdate));
		flypack.OnFlyAheadEnd = (Flypack.OnFlyAheadEndDelegate)Delegate.Combine(flypack.OnFlyAheadEnd, new Flypack.OnFlyAheadEndDelegate(FlypackOnFlyAheadEnd));
		flypack.OnHidTurboHeadstartButtons += OnHidTurboHeadstartButtons;
		superShoes.OnSwitchToSuperShoes += OnSwitchToSuperShoes;
		superShoes.SuperShoesOnStop += SuperShoesOnStop;
		springJump.OnStart = (SpringJump.OnStartDelegate)Delegate.Combine(springJump.OnStart, new SpringJump.OnStartDelegate(SpringJumpOnStart));
		springJump.OnHangtime = (SpringJump.OnStartDelegate)Delegate.Combine(springJump.OnHangtime, new SpringJump.OnStartDelegate(SpringJumpOnHangtime));
		springJump.OnStop = (SpringJump.OnStopDelegate)Delegate.Combine(springJump.OnStop, new SpringJump.OnStopDelegate(SpringJumpOnStop));
		wallWalking.OnJumpAheadStart = (WallWalking.OnJumpAheadStartDelegate)Delegate.Combine(wallWalking.OnJumpAheadStart, new WallWalking.OnJumpAheadStartDelegate(WallWalkingOnJumpAheadStart));
		wallWalking.OnJumpAheadEnd = (WallWalking.OnJumpAheadEndDelegate)Delegate.Combine(wallWalking.OnJumpAheadEnd, new WallWalking.OnJumpAheadEndDelegate(WallWalkingOnJumpAheadEnd));
		wallWalking.OnLeaveWall = (WallWalking.OnLeaveWallDelegate)Delegate.Combine(wallWalking.OnLeaveWall, new WallWalking.OnLeaveWallDelegate(OnLeaveWallRun));
		boundJump.OnStart = (BoundJump.OnStartDelegate)Delegate.Combine(boundJump.OnStart, new BoundJump.OnStartDelegate(SpringJumpOnStart));
		boundJump.OnStop = (BoundJump.OnStopDelegate)Delegate.Combine(boundJump.OnStop, new BoundJump.OnStopDelegate(SpringJumpOnStop));
		boss.OnCatchPlayer = (Boss.OnCatchPlayerDelegate)Delegate.Combine(boss.OnCatchPlayer, new Boss.OnCatchPlayerDelegate(OnCatchPlayer));
		revive.OnRevive += OnRevive;
		revive.OnSwitchToRunning += OnSwitchToRunning;
		defaultHelmetRendering = HelmetModelPreviewFactory.Instance.GetHelmetSelection(0).helmetPrefab.GetComponent<HelmetRendering>();
		defaultHelmetAnimationList = new HelmetRendering.AnimationList();
		defaultHelmetAnimationList.runAnimations = defaultHelmetRendering.runAnimations;
		defaultHelmetAnimationList.superRunAnimations = defaultHelmetRendering.superRunAnimations;
		defaultHelmetAnimationList.landAnimations = defaultHelmetRendering.landAnimations;
		defaultHelmetAnimationList.jumpAnimations = defaultHelmetRendering.jumpAnimations;
		defaultHelmetAnimationList.hangtimeAnimations = defaultHelmetRendering.hangtimeAnimations;
		defaultHelmetAnimationList.rollAnimations = defaultHelmetRendering.rollAnimations;
		defaultHelmetAnimationList.dodgeLeftAnimations = defaultHelmetRendering.dodgeLeftAnimations;
		defaultHelmetAnimationList.dodgeRightAnimations = defaultHelmetRendering.dodgeRightAnimations;
		defaultHelmetAnimationList.grindAnimations = defaultHelmetRendering.grindAnimations;
		defaultHelmetAnimationList.grindLandAnimations = defaultHelmetRendering.grindLandAnimations;
		defaultHelmetAnimationList.getOnHelmAnimations = defaultHelmetRendering.getOnHelmAnimations;
	}

	private void InitializeAnimations()
	{
		animations = new Animations();
		animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
		animations.HIT_UPPER = InitializeClips(defaultAnimations.hitUpper);
		animations.HIT_LOWER = InitializeClips(defaultAnimations.hitLower);
		animations.HIT_MOVING = InitializeClips(defaultAnimations.hitMoving);
		animations.STUMBLE = InitializeClips(defaultAnimations.stumble);
		animations.STUMBLE_MIX = InitializeClips(defaultAnimations.stumbleMix);
		animations.STUMBLE_LEFT_SIDE = InitializeClips(defaultAnimations.stumbleLeftSide);
		animations.STUMBLE_RIGHT_SIDE = InitializeClips(defaultAnimations.stumbleRightSide);
		animations.STUMBLE_LEFT_CORNER = InitializeClips(defaultAnimations.stumbleLeftCorner);
		animations.STUMBLE_RIGHT_CORNER = InitializeClips(defaultAnimations.stumbleRightCorner);
		animations.FALLWATER = InitializeClips(defaultAnimations.fallWater);
		animations.PICKCHEST = InitializeClips(defaultAnimations.pickChest);
		characterAnimation["caught"].layer = 4;
		characterAnimation["caught"].enabled = false;
		characterAnimation["stumble"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumble"].layer = 2;
		characterAnimation["stumble"].weight = 1f;
		characterAnimation["stumbleCornerLeft"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumbleCornerLeft"].layer = 2;
		characterAnimation["stumbleCornerLeft"].weight = 1f;
		characterAnimation["stumbleCornerRight"].AddMixingTransform(characterModel.spineTransform);
		characterAnimation["stumbleCornerRight"].layer = 2;
		characterAnimation["stumbleCornerRight"].weight = 1f;
	}

	public void InitializeCharacterModel()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		characterModel = gameObject.GetComponent<CharacterModel>();
		characterAnimation = characterModel.characterAnimation;
	}

	private void InitializeCharacterRenderingEffects()
	{
		characterRenderingEffects = UnityEngine.Object.Instantiate(characterRenderingEffectsPrefab).GetComponent<CharacterRenderingEffects>();
		characterRenderingEffects.Initialize(characterModel);
	}

	private string[] InitializeClips(AnimationClip[] clips)
	{
		string[] array = new string[clips.Length];
		for (int i = 0; i < clips.Length; i++)
		{
			AnimationClip animationClip = clips[i];
			if (!addedAnimClipsNames.Contains(animationClip))
			{
				addedAnimClipsNames.Add(animationClip);
				characterAnimation.AddClip(animationClip, animationClip.name);
			}
			array[i] = animationClip.name;
		}
		return array;
	}

	private void IsInGame_OnChange(bool isInGame)
	{
		if (!isInGame)
		{
			FlypackOnStop();
			SpringJumpOnStop();
		}
	}

	private void HandleOnTurboHeadstart()
	{
		if (isRolling)
		{
			EndRollAnim();
		}
		StopAllCoroutines();
		StartCoroutine(myTween.To(1f, delegate(float t)
		{
			FlypackOnFlyAheadUpdate(t);
		}));
	}

	private void FlypackOnFlyAheadUpdate(float ratio)
	{
		float num = Mathf.Lerp(0f, 1f, jetpackParticleOffsetCurve.Evaluate(ratio));
		characterRenderingEffects.FlypackParticles.transform.rotation = Quaternion.Euler(effectInitRot - new Vector3(num, 0f, 0f));
		characterRenderingEffects.FlypackParticles.transform.localScale = effectInitScale + new Vector3(0f, 0f, num * 2f);
	}

	private void FlypackOnFlyAheadEnd()
	{
		characterRenderingEffects.SetIngParticlesActive();
	}

	private void OnHidTurboHeadstartButtons()
	{
		characterRenderingEffects.SetEndParticlesActive();
	}

	private void FlypackOnStop()
	{
		characterRenderingEffects.FlypackParticles.transform.localScale = effectInitScale;
		characterRenderingEffects.FlypackParticles.SetActive(false);
		int i = 0;
		for (int num = characterModel.meshFlypack.Length; i < num; i++)
		{
			characterModel.meshFlypack[i].enabled = false;
		}
		characterModel.animFlypack.Stop();
		if (helmet.IsActive)
		{
			OnSwitchToHelmet(currentHelmet);
		}
		else if (superShoes.IsActive)
		{
			OnSwitchToSuperShoes();
			OnHangtime();
		}
		else
		{
			OnSwitchToRunning();
			OnHangtime();
		}
	}

	private void OnGameStart()
	{
		string run = animations.Run;
		characterAnimation[run].speed = game.DefaultSpeedForAnimation;
		characterAnimation.CrossFade(run, 0.1f);
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
	{
		caught = characterAnimation[currentCharacterCaught];
		StartCoroutine(BegainCatchPlayerAnim(caught, catchUpTime));
	}

	private IEnumerator BegainCatchPlayerAnim(AnimationState caught, float delay)
	{
		this.caught.wrapMode = WrapMode.Once;
		this.caught.weight = 0f;
		this.caught.normalizedTime = 0f;
		this.caught.enabled = true;
		yield return new WaitForSeconds(delay);
		StartCoroutine(myTween.To(0.2f, delegate(float t)
		{
			caught.weight = Mathf.Lerp(0f, 1f, t);
		}));
	}

	private void OnChangeIsGrounded(bool isGrounded)
	{
		characterModel.shadow.enabled = isGrounded;
	}

	private void WallWalkingOnJumpAheadStart(SwipeDir dir)
	{
		string animation = animations.StartWallRunLeft;
		string animation2 = animations.WallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = animations.StartWallRunRight;
			animation2 = animations.WallRunRight;
		}
		characterAnimation.CrossFade(animation, 0.1f);
		characterAnimation.CrossFadeQueued(animation2, 0.1f);
		if (helmet.IsActive && helmetAnimation != null)
		{
			helmetAnimation.CrossFade(animation, 0.1f);
			helmetAnimation.CrossFadeQueued(animation2, 0.1f);
		}

		
	}

	private void WallWalkingOnJumpAheadEnd(SwipeDir dir)
	{
	}

	private void OnStartWallRun(SwipeDir dir)
	{
		string jump = animations.Jump;
		string animation = animations.WallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = animations.WallRunRight;
		}
		characterAnimation.CrossFade(jump, 0.1f);
		characterAnimation.CrossFadeQueued(animation, 0.1f);
		if (helmet.IsActive && helmetAnimation != null)
		{
			helmetAnimation.CrossFade(jump, 0.1f);
			helmetAnimation.CrossFadeQueued(animation, 0.1f);
		}
	}

	private void OnLeaveWallRun(SwipeDir dir)
	{
		string animation = animations.EndWallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = animations.EndWallRunRight;
		}
		characterAnimation.CrossFade(animation, 0.1f);
		if (helmet.IsActive && helmetAnimation != null)
		{
			helmetAnimation.CrossFade(animation, 0.1f);
		}
	}

	private void OnGetOnWaterBoard(int x)
	{
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		waterBoards[x] = true;
		isOnWaterBoard = true;
		OnSwitchToWaterBoard();
	}

	private void OnHandleWaterBoard(int x)
	{
		if (waterBoards[x] && !isOnWaterBoard)
		{
			OnSwitchToWaterBoard();
			isOnWaterBoard = true;
		}
		else if (!waterBoards[x] && isOnWaterBoard)
		{
			if (helmet.IsActive)
			{
				OnSwitchToHelmet(currentHelmet);
				OnRun();
			}
			else if (superShoes.IsActive)
			{
				OnSwitchToSuperShoes();
				OnRun();
			}
			else
			{
				OnSwitchToRunning();
				OnRun();
			}
			isOnWaterBoard = false;
		}
	}

	private void OnGetOffWaterBoard(int x)
	{
		waterBoards[x] = false;
		isOnWaterBoard = false;
		if (!helmet.IsActive)
		{
			if (superShoes.IsActive)
			{
				OnSwitchToSuperShoes();
				OnRun();
			}
			else
			{
				OnSwitchToRunning();
				OnRun();
			}
		}
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (characterController.isGrounded && !SpringJump.Instance.isActive)
		{
			string text = ((direction == Character.OnChangeTrackDirection.Left) ? animations.DodgeLeft : animations.DodgeRight);
			characterAnimation[text].speed = game.DefaultSpeedForAnimation;
			characterAnimation.CrossFade(text, 0.02f);
			if (raft.IsActive && raftAnimation != null)
			{
				raftAnimation.CrossFade(GetRaftAnimationName(text), 0.02f);
			}
			else if (helmet.IsActive && helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(text), 0.02f);
			}
		}
		if (!character.IsJumping)
		{
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, game.Attachment.IsActive(game.Attachment.Helmet) ? 0.15f : 0.02f);
			if (raft.IsActive && raftAnimation != null)
			{
				raftAnimation.CrossFadeQueued(GetRaftAnimationName(run), 0.02f);
			}
			else if (helmet.IsActive && helmetAnimation != null)
			{
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(run), 0.02f);
			}
		}
	}

	private void OnHangtime()
	{
		if (!character.IsRolling)
		{
			if (!helmet.IsActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.Hangtime;
			}
			characterAnimation.CrossFade(hangtimeAnimation, 0.3f);
			if (raftAnimation != null)
			{
				raftAnimation.CrossFade(GetRaftAnimationName(hangtimeAnimation), 0.3f);
			}
			else if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(hangtimeAnimation), 0.3f);
			}
		}
	}

	private void OnPickedChest()
	{
		if (game.IsInRunningMode && !character.IsJumping && !character.IsFalling)
		{
			string pickChest = animations.PickChest;
			characterAnimation.CrossFade(pickChest, 0.2f);
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, 0.3f);
			if (helmet.IsActive && helmetAnimation != null)
			{
				helmetAnimation.CrossFade(pickChest, 0.1f);
				helmetAnimation.CrossFadeQueued(run, 0.1f);
			}
		}
	}

	private void OnHitByTrain()
	{
		characterAnimation.Play(animations.HitMoving);
		Vector3 currentPos = character.transform.position;
		Vector3 camPos = character.characterCamera.transform.position;
		StartCoroutine(myTween.To(0.5f, delegate(float t)
		{
			character.transform.position = Vector3.Lerp(currentPos, new Vector3(camPos.x, camPos.y - 33f, currentPos.z), t);
		}));
	}

	private void OnFallIntoWater()
	{
		characterAnimation.Play(animations.FallWater);
	}

	private void OnIntroRun()
	{
		waterBoards[0] = false;
		waterBoards[1] = false;
		waterBoards[2] = false;
		isOnWaterBoard = false;
		characterModel.transform.localPosition = Vector3.zero;
		characterModel.PlayBall.gameObject.SetActive(false);
		characterModel.Flower.gameObject.SetActive(false);
		characterModel.Heart.gameObject.SetActive(false);
		OnSwitchToRunning();
	}

	private void OnJump()
	{
		jumpAnimation = animations.Jump;
		hangtimeAnimation = animations.Hangtime;
		characterAnimation.CrossFade(jumpAnimation, 0.15f);
		if (raft.IsActive && raftAnimation != null)
		{
			raftAnimation.CrossFade(GetRaftAnimationName(jumpAnimation), 0.15f);
		}
		else if (helmet.IsActive && helmetAnimation != null)
		{
			helmetAnimation.CrossFade(GetHelmetAnimationName(jumpAnimation), 0.15f);
		}
	}

	private void OnJumpRoll()
	{
		characterRenderingEffects.SetJumpRool();
	}

	private void OnLanding(Transform characterTransform)
	{
		string text = animations.Run;
		characterAnimation[text].speed = game.DefaultSpeedForAnimation;
		if (character.IsRolling)
		{
			return;
		}
		if (helmet.IsActive || raft.IsActive)
		{
			string text2;
			if (character.IsAboveGround)
			{
				text = animations.Grind;
				text2 = text + "_land";
			}
			else
			{
				text2 = animations.Land;
			}
			characterAnimation.CrossFade(text2, 0.1f);
			characterAnimation.CrossFadeQueued(text, 0.2f);
			if (raftAnimation != null)
			{
				helmetAnimation.CrossFade(GetRaftAnimationName(text2), 0.1f);
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(text), 0.2f);
			}
			else if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(text2), 0.1f);
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(text), 0.2f);
			}
		}
		else
		{
			string land = animations.Land;
			characterAnimation.CrossFade(land, 0.05f);
			characterAnimation.CrossFadeQueued(text, 0.1f);
		}
	}

	private void OnRevive()
	{
		StopAllCoroutines();
		waterBoards[0] = false;
		waterBoards[1] = false;
		waterBoards[2] = false;
		isOnWaterBoard = false;
		if (caught != null)
		{
			caught.enabled = false;
		}
		OnJump();
		if (CoinMagnet.Instance.IsActive)
		{
			characterAnimation["hold_magnet"].enabled = true;
			characterAnimation.Play("hold_magnet");
		}
	}

	private void OnRoll()
	{
		StartCoroutine(OnRollPlayAnimation());
	}

	private IEnumerator OnRollPlayAnimation()
	{
		isRolling = true;
		string rollAnimation = animations.Roll;
		string runAnimation = animations.Run;
		characterAnimation[runAnimation].speed = game.DefaultSpeedForAnimation;
		characterAnimation.CrossFade(rollAnimation, 0.1f);
		if (raft.IsActive)
		{
			characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
			if (raftAnimation != null)
			{
				raftAnimation.CrossFade(GetRaftAnimationName(rollAnimation), 0.1f);
				raftAnimation.CrossFadeQueued(GetRaftAnimationName(runAnimation), raft.IsActive ? 0.2f : 0f);
			}
		}
		else if (helmet.IsActive)
		{
			characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(rollAnimation), 0.1f);
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(runAnimation), helmet.IsActive ? 0.2f : 0f);
			}
		}
		else
		{
			characterAnimation.CrossFadeQueued(runAnimation, 0.1f);
		}
		float endTime = Time.time + characterAnimation[rollAnimation].length;
		while (Time.time < endTime && characterAnimation[rollAnimation].enabled && isRolling)
		{
			yield return null;
		}
		EndRollAnim();
	}

	private void EndRollAnim()
	{
		isRolling = false;
		character.EndRoll();
	}

	private void OnRun()
	{
		if (character.IsFalling || character.IsJumping)
		{
			return;
		}
		string run = animations.Run;
		if (characterController.isGrounded)
		{
			characterAnimation[run].speed = game.DefaultSpeedForAnimation;
			characterAnimation.CrossFade(run);
			if (raftAnimation != null)
			{
				raftAnimation.CrossFade(GetRaftAnimationName(run));
			}
			else if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(run));
			}
		}
	}

	private void OnStageMenuSequence()
	{
		if (characterAnimation != null)
		{
			if (caught != null)
			{
				caught.enabled = false;
			}
			characterAnimation.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (character.IsStumbling && game.CharacterState != null && game.IsInRunningMode)
		{
			characterAnimation.Play(animations.HitLower);
			

			
			return;
		}
		if (stumbleType == Character.StumbleType.Bush || colliderName == "lightSignal" || colliderName == "powerbox")
		{
			characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
			characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
			return;
		}
		if (stumbleType == Character.StumbleType.Side)
		{
			if (!game.Attachment.IsActive(game.Attachment.Helmet) && !game.IsInFlypackMode && !game.IsInSpringJumpMode && !game.IsInBoundJumpMode)
			{
				if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
				{
					characterAnimation.CrossFade(animations.StumbleLeftSide, 0.2f);
				}
				if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
				{
					characterAnimation.CrossFade(animations.StumbleRightSide, 0.2f);
				}
			}
			if (!character.IsJumping)
			{
				characterAnimation.CrossFadeQueued(animations.Run, (!game.Attachment.IsActive(game.Attachment.Helmet)) ? 0.02f : 0.4f);
			}
			return;
		}
		switch (horizontalHit)
		{
		case Character.StumbleHorizontalHit.Center:
			switch (verticalHit)
			{
			case Character.StumbleVerticalHit.Lower:
				if (game.Attachment.IsActive(game.Attachment.Helmet))
				{
					characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
				}
				else
				{
					characterAnimation.CrossFade(animations.Stumble, 0.05f);
				}
				characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
				break;
			case Character.StumbleVerticalHit.Middle:
				characterAnimation.CrossFade(animations.HitMid, 0.07f);
				break;
			case Character.StumbleVerticalHit.Upper:
				characterAnimation.CrossFade(animations.HitUpper, 0.07f);
				break;
			}
			return;
		case Character.StumbleHorizontalHit.Left:
			characterAnimation.Play(game.Attachment.IsActive(game.Attachment.Helmet) ? animations.StumbleLeftCorner : animations.StumbleLeftSide);
			break;
		case Character.StumbleHorizontalHit.LeftCorner:
			characterAnimation.Play(animations.StumbleLeftCorner);
			break;
		case Character.StumbleHorizontalHit.Right:
			characterAnimation.Play(game.Attachment.IsActive(game.Attachment.Helmet) ? animations.StumbleRightCorner : animations.StumbleRightSide);
			break;
		case Character.StumbleHorizontalHit.RightCorner:
			characterAnimation.Play(animations.StumbleRightCorner);
			break;
		}
		characterAnimation.PlayQueued(animations.Run);
	}

	private void OnSwitchToRaft(GameObject raft)
	{
		ToggleRaft(raft);
		string getOn = animations.GetOn;
		characterAnimation.CrossFade(getOn, 0.2f);
		string run = animations.Run;
		characterAnimation.CrossFadeQueued(run, 0.2f);
		if (raftAnimation != null)
		{
			raftAnimation.CrossFade(run, 0.2f);
		}
	}

	private void OnEndRaft()
	{
		if (helmet.IsActive)
		{
			OnSwitchToHelmet(currentHelmet);
		}
		else if (superShoes.IsActive)
		{
			OnSwitchToSuperShoes();
			OnJump();
		}
		else
		{
			OnSwitchToRunning();
			OnJump();
		}
	}

	private void OnSwitchToHelmet(GameObject helmet)
	{
		ToggleCustomHelmet(helmet);
		string getOn = animations.GetOn;
		characterAnimation.CrossFade(getOn, 0.1f);
		if (helmetAnimation != null)
		{
			helmetAnimation.CrossFade(GetHelmetAnimationName(getOn), 0.1f);
		}
		hangtimeAnimation = animations.Hangtime;
		if (!character.IsFalling && !character.IsJumping)
		{
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, 0.2f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(run), 0.2f);
			}
		}
		else
		{
			characterAnimation.CrossFade(hangtimeAnimation, 0.2f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(hangtimeAnimation), 0.2f);
			}
		}
	}

	private void OnSwitchToPogostickJump()
	{
		animations.RUN = InitializeClips(pogostickClips.run);
		animations.DODGE_LEFT = InitializeClips(pogostickClips.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(pogostickClips.dodgeRight);
		animations.HANGTIME = InitializeClips(pogostickClips.hangtime);
		characterAnimation.CrossFade(animations.Run);
	}

	private void OnSwitchToFlypack(bool isHeadStart)
	{
		animations.RUN = InitializeClips(jetpackAnimations.run);
		animations.DODGE_LEFT = InitializeClips(jetpackAnimations.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(jetpackAnimations.dodgeRight);
		int i = 0;
		for (int num = characterModel.meshFlypack.Length; i < num; i++)
		{
			characterModel.meshFlypack[i].enabled = true;
		}
		characterModel.animFlypack.Play();
		characterRenderingEffects.FlypackParticles.SetActive(true);
		characterRenderingEffects.SetStartParticlesActive();
		characterAnimation.CrossFade(animations.Run);
	}

	public void OnSwitchToRunning()
	{
		if (superShoes.IsActive)
		{
			animations.RUN = InitializeClips(SuperShoesAnimations.run);
		}
		else if (isOnWaterBoard)
		{
			animations.RUN = InitializeClips(WaterBoardAnimations.run);
		}
		else
		{
			animations.RUN = InitializeClips(defaultAnimations.run);
		}
		animations.START_WALLRUN_LEFT = InitializeClips(defaultAnimations.startWallRunLeft);
		animations.WALLRUN_LEFT = InitializeClips(defaultAnimations.wallRunLeft);
		animations.END_WALLRUN_LEFT = InitializeClips(defaultAnimations.endWallRunLeft);
		animations.START_WALLRUN_RIGHT = InitializeClips(defaultAnimations.startWallRunRight);
		animations.WALLRUN_RIGHT = InitializeClips(defaultAnimations.wallRunRight);
		animations.END_WALLRUN_RIGHT = InitializeClips(defaultAnimations.endWallRunRight);
		animations.LAND = InitializeClips(defaultAnimations.landing);
		animations.JUMP = InitializeClips(defaultAnimations.jump);
		animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
		animations.ROLL = InitializeClips(defaultAnimations.roll);
		animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
		if (currentHelmet != null)
		{
			ToggleCustomHelmet(null);
		}
	}

	private void OnSwitchToSuperShoes()
	{
		if (helmetRendering == null)
		{
			OnSwitchToRunning();
		}
	}

	private void OnSwitchToWaterBoard()
	{
		if (helmetRendering == null)
		{
			animations.RUN = InitializeClips(WaterBoardAnimations.run);
			characterAnimation.CrossFade(animations.Run, 0.1f);
		}
	}

	public void ActivateSnow(bool activate)
	{
		if (activate)
		{
			characterRenderingEffects.ActivateSnow();
		}
		else
		{
			characterRenderingEffects.DeactivateSnow();
		}
	}

	private void OnTutorialMoveBackToCheckPoint(float duration)
	{
		characterAnimation.CrossFade(animations.Run, duration);
	}

	private void OnTutorialStartFromCheckPoint()
	{
		characterAnimation.Play(animations.Run);
	}

	private void OnSpeedupStart()
	{
		characterRenderingEffects.SetSpeedupActive();
		string superRun = animations.SuperRun;
		if (helmet.IsActive)
		{
			characterAnimation.CrossFade(superRun, 0.1f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(superRun), 0.1f);
			}
		}
		else
		{
			characterAnimation.CrossFade(superRun, 0.1f);
		}
	}

	private void OnSpeedupStop()
	{
		characterRenderingEffects.SetSpeedupDeactive();
		string run = animations.Run;
		if (helmet.IsActive)
		{
			characterAnimation.CrossFade(run, 0.2f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFade(GetHelmetAnimationName(run), 0.2f);
			}
		}
		else
		{
			characterAnimation.CrossFade(run, 0.2f);
		}
	}

	private void OverrideRollTransition()
	{
		string run = animations.Run;
		if (helmet.IsActive)
		{
			characterAnimation.CrossFadeQueued(run, 0.2f);
			if (helmetAnimation != null)
			{
				helmetAnimation.CrossFadeQueued(GetHelmetAnimationName(run), helmet.IsActive ? 0.2f : 0f);
			}
		}
		else
		{
			characterAnimation.CrossFadeQueued(run, 0.1f);
		}
	}

	private void SpringJumpOnHangtime()
	{
		string hangtime = animations.Hangtime;
		characterAnimation.CrossFade(hangtime, 0.2f);
	}

	private void SpringJumpOnStart()
	{
		OnSwitchToPogostickJump();
	}

	private void SpringJumpOnStop()
	{
		if (helmet.IsActive)
		{
			OnSwitchToHelmet(currentHelmet);
		}
		else if (superShoes.IsActive)
		{
			OnSwitchToSuperShoes();
			OnHangtime();
		}
		else
		{
			OnSwitchToRunning();
			OnHangtime();
		}
	}

	private void SuperShoesOnStop()
	{
		if (helmetRendering == null)
		{
			OnSwitchToRunning();
			OnRun();
		}
	}

	private void Start()
	{
		if (this.CharacterModelInitialized != null)
		{
			this.CharacterModelInitialized(characterModel.BoneHelmet.gameObject);
		}
	}

	private void ToggleCustomHelmet(GameObject newHelmet)
	{
		if (currentHelmet != null && currentHelmet != newHelmet)
		{
			UnityEngine.Object.Destroy(currentHelmet);
		}
		currentHelmet = newHelmet;
		if (newHelmet != null)
		{
			characterModel.currentHelmet.gameObject.SetActive(true);
			helmetAnimation = newHelmet.GetComponent<Animation>();
			helmetRendering = newHelmet.GetComponent<HelmetRendering>();
			if (helmetRendering != null)
			{
				helmetRendering.Initialize(characterAnimation, helmetAnimation, defaultHelmetAnimationList, addedAnimClipsNames);
				return;
			}
			helmetRendering = defaultHelmetRendering;
			helmetRendering.Initialize(characterAnimation, helmetAnimation, defaultHelmetAnimationList, addedAnimClipsNames);
		}
		else
		{
			helmetAnimation = null;
			helmetRendering = null;
		}
	}

	private void ToggleRaft(GameObject raft)
	{
		if (raft != null)
		{
			raftAnimation = raft.GetComponentInChildren<Animation>();
			raftRendering = raft.GetComponent<RaftRendering>();
			if (raftRendering != null)
			{
				raftRendering.Initialize(characterAnimation, helmetAnimation, addedAnimClipsNames);
			}
		}
		else
		{
			raftAnimation = null;
			raftRendering = null;
		}
	}
}
