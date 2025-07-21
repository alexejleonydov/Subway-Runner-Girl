using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum CriticalHitType
	{
		Train = 0,
		Barrier = 1,
		MovingTrain = 2,
		FallIntoWater = 3,
		Bus = 4,
		MovingBus = 5,
		Car = 6,
		MovingCar = 7,
		FlowerBed = 8,
		None = 9
	}

	private enum ImpactX
	{
		Left = 0,
		Middle = 1,
		Right = 2
	}

	private enum ImpactY
	{
		Upper = 0,
		Middle = 1,
		Lower = 2
	}

	private enum ImpactZ
	{
		Before = 0,
		Middle = 1,
		After = 2
	}

	public enum ObstacleType
	{
		JumpHighBarrier = 0,
		RollBarrier = 1,
		JumpBarrier = 2,
		None = 3
	}

	public enum OnChangeTrackDirection
	{
		Left = 0,
		Right = 1
	}

	public delegate void OnChangeTrackDelegate(OnChangeTrackDirection direction);

	public delegate void OnCriticalHitDelegate(CriticalHitType type);

	public delegate void OnHangtimeDelegate();

	public delegate void OnJumpRollDelegate();

	public delegate void OnHitByTrainDelegate();

	public delegate void OnFallIntoWaterDelegate();

	public delegate void OnJumpDelegate();

	public delegate void OnJumpIfHitByTrainDelegate();

	public delegate void OnJumpOverTrainDelegate();

	public delegate void OnJumpOverBusDelegate();

	public delegate void OnJumpOverCarDelegate();

	public delegate void OnLandingDelegate(Transform characterTransform);

	public delegate void OnPassedObstacleDelegate(ObstacleType type);

	public delegate void OnRollDelegate();

	public delegate void OnRollGuardDelegate();

	public delegate void OnStumbleDelegate(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName);

	public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

	public delegate void OnTutorialStartFromCheckPointDelegate();

	public delegate void OnHandleWaterBoardDelegate(int x);

	public enum StumbleHorizontalHit
	{
		Left = 0,
		LeftCorner = 1,
		Center = 2,
		RightCorner = 3,
		Right = 4
	}

	public enum StumbleType
	{
		Normal = 0,
		Bush = 1,
		Side = 2
	}

	public enum StumbleVerticalHit
	{
		Upper = 0,
		Middle = 1,
		Lower = 2
	}

	private struct JumpCurveData
	{
		public float z_start;

		public float z_length;

		public float z_end;

		public float y_start;
	}

	public Transform characterRoot;

	public CapsuleCollider characterCollider;

	public OnTriggerObject coinMagnetCollider;

	public OnTriggerObject coinMagnetLongCollider;

	[SerializeField]
	private float characterAngle = 45f;

	public ParticleSystem helmetCrashParticleSystem;

	public PickupParticles CharacterPickupParticleSystem;

	public float ColliderTrackWidth = 17f;

	[HideInInspector]
	public CharacterController characterController;

	[HideInInspector]
	public OnTriggerObject characterColliderTrigger;

	[HideInInspector]
	public CharacterModel characterModel;

	[HideInInspector]
	public CharacterCamera characterCamera;

	[HideInInspector]
	public Helmet helmet;

	[HideInInspector]
	public SuperShoes superShoes;

	[HideInInspector]
	public Running running;

	[HideInInspector]
	public bool immuneToCriticalHit;

	public int trackIndex;

	public float x;

	public float z;

	public float verticalSpeed;

	public float lastGroundedY;

	public float subwayMaxY;

	public float underpassMaxY;

	private float jumpHeight;

	public float gravity = 200f;

	public float jumpHeightNormal = 20f;

	public float jumpHeightSuperShoes = 40f;

	public float verticalFallSpeedLimit = -1f;

	public float stumbleCornerTolerance = 15f;

	public float stumbleDecayTime = 5f;

	public bool inAirJump;

	public Variable<bool> IsGrounded;

	[SerializeField]
	private AnimationCurve superSneakersJumpCurve;

	[SerializeField]
	private float superShoesJumpApexRatio;

	[HideInInspector]
	public bool stopColliding;

	public float sameLaneTimeStamp;

	private Vector3 characterColliderCenter;

	private float characterColliderHeight;

	private Vector3 characterControllerCenter;

	private float characterControllerHeight;

	private float characterRotation;

	private Game game;

	private Boss boss;

	private int initialTrackIndex = 1;

	private static Character instance;

	private bool isFalling;

	private bool isJumping;

	private bool isJumpingWithSuperShoes;

	private bool isJumpingHigher;

	private bool isRolling;

	private bool isStumbling;

	private string lastHitTag;

	private int onewayId;

	private int lastObstacleTriggerTrackIndex;

	private ObstacleType lastObstacleTriggerType;

	private float lastZ;

	private Layers layers;

	private Revive revive;

	private VariableBool squeezeCollider;

	private bool startedJumpFromGround;

	private HashSet<Collider> subwayColliders;

	private bool isInsideSubway;

	private HashSet<Collider> underpassColliders;

	private bool isInsideUnderpass;

	private Wall[] walls;

	private HashSet<Collider> onewayColliders;

	private bool isInOneway;

	private HashSet<Collider> speedupColliders;

	private bool isInspeedup;

	[HideInInspector]
	public bool forceToOneway;

	private JumpCurveData? superShoesJump;

	private TrackController trackController;

	public float trackIndexPosition;

	private int trackIndexTarget;

	private int trackMovement;

	private int trackMovementNext;

	private bool trainJump;

	private float trainJumpSampleLength;

	private float trainJumpSampleZ;

	private bool busJump;

	private float busJumpSampleLength;

	private float busJumpSampleZ;

	private bool carJump;

	private float carJumpSampleLength;

	private float carJumpSampleZ;

	private float verticalSpeed_jumpTolerance;

	public static Character Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character;
			}
			return instance;
		}
	}

	public int TrackIndexTarget
	{
		get
		{
			return trackIndexTarget;
		}
	}

	public bool IsAboveGround
	{
		get
		{
			return base.transform.position.y > 20f;
		}
	}

	public bool IsFalling
	{
		get
		{
			return isFalling;
		}
		set
		{
			isFalling = value;
		}
	}

	public bool isStairing
	{
		get
		{
			return running.inStair;
		}
	}

	public bool IsInsideSubway
	{
		get
		{
			return isInsideSubway;
		}
	}

	public bool IsJumpingHigher
	{
		get
		{
			return isJumpingHigher;
		}
		set
		{
			isJumpingHigher = value;
		}
	}

	public bool InSuperShoesJump
	{
		get
		{
			return isJumpingHigher || superShoesJump.HasValue || isJumpingWithSuperShoes;
		}
	}

	public bool IsInspeedup
	{
		get
		{
			return isInspeedup;
		}
	}

	public bool IsJumping
	{
		get
		{
			return isJumping;
		}
		set
		{
			isJumping = value;
		}
	}

	public bool IsRolling
	{
		get
		{
			return isRolling;
		}
	}

	public bool IsStumbling
	{
		get
		{
			return isStumbling;
		}
		set
		{
			isStumbling = value;
		}
	}

	public int TrackIndex
	{
		get
		{
			return trackIndex;
		}
	}

	public event OnChangeTrackDelegate OnChangeTrack;

	public event OnCriticalHitDelegate OnCriticalHit;

	public event OnJumpRollDelegate OnJumpRoll;

	public event OnHangtimeDelegate OnHangtime;

	public event OnHitByTrainDelegate OnHitByTrain;

	public event OnFallIntoWaterDelegate OnFallIntoWater;

	public event OnJumpDelegate OnJump;

	public event OnJumpIfHitByTrainDelegate OnJumpIfHitByTrain;

	public event OnJumpOverTrainDelegate OnJumpOverTrain;

	public event OnJumpOverTrainDelegate OnJumpOverBus;

	public event OnJumpOverTrainDelegate OnJumpOverCar;

	public event OnLandingDelegate OnLanding;

	public event OnPassedObstacleDelegate OnPassedObstacle;

	public event OnRollDelegate OnRoll;

	public event OnRollGuardDelegate OnRollGuard;

	public event OnStumbleDelegate OnStumble;

	public event OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint;

	public event OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint;

	public event OnHandleWaterBoardDelegate OnHandleWaterBoard;

	public Character()
	{
		IsGrounded = new Variable<bool>(false);
		subwayColliders = new HashSet<Collider>();
		underpassColliders = new HashSet<Collider>();
		walls = new Wall[2];
		onewayColliders = new HashSet<Collider>();
		speedupColliders = new HashSet<Collider>();
		squeezeCollider = new VariableBool();
		superShoesJumpApexRatio = 0.5f;
		trainJumpSampleLength = 10f;
		busJumpSampleLength = 10f;
		carJumpSampleLength = 5f;
		verticalSpeed_jumpTolerance = -30f;
	}

	public void ApplyGravity()
	{
		RaycastHit hitInfo;
		if (verticalSpeed < 0f && characterController.isGrounded)
		{
			if (startedJumpFromGround && trainJump && IsRunningOnGround())
			{
				NotifyOnJumpOverTrain();
			}
			if (startedJumpFromGround && busJump && IsRunningOnGround())
			{
				NotifyOnJumpOverBus();
			}
			if (startedJumpFromGround && carJump && IsRunningOnGround())
			{
				NotifyOnJumpOverCar();
			}
			if (running.currentRunPosition != Running.RunPositions.air)
			{
				startedJumpFromGround = false;
			}
			verticalSpeed = 0f;
			IsGrounded.Value = true;
			if (isJumping || isFalling)
			{
				isJumping = false;
				isJumpingWithSuperShoes = false;
				isFalling = false;
				IsGrounded.Value = true;
				NotifyOnLanding();
				if (!helmet.IsActive && isRolling && running.currentRunPosition == Running.RunPositions.ground)
				{
					NotifyOnJumpRoll();
				}
			}
		}
		else if (startedJumpFromGround && trainJumpSampleZ < z && Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out hitInfo))
		{
			if (hitInfo.collider.CompareTag("HitMovingTrain") || hitInfo.collider.CompareTag("HitTrain"))
			{
				trainJump = true;
				trainJumpSampleZ += trainJumpSampleLength;
			}
			if (hitInfo.collider.CompareTag("HitMovingBus") || hitInfo.collider.CompareTag("HitBus"))
			{
				busJump = true;
				busJumpSampleZ += busJumpSampleLength;
			}
			if (hitInfo.collider.CompareTag("HitMovingCar") || hitInfo.collider.CompareTag("HitCar"))
			{
				carJump = true;
				carJumpSampleZ += carJumpSampleLength;
			}
		}
		if (isStairing)
		{
			verticalSpeed -= 1.5f * gravity * Time.deltaTime;
		}
		else
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		if (!characterController.isGrounded && !isFalling && verticalSpeed < verticalFallSpeedLimit && !isRolling && !isStairing)
		{
			isFalling = true;
			NotifyOnHangtime();
			IsGrounded.Value = false;
		}
	}

	public float CalculateJumpVerticalSpeed()
	{
		return CalculateJumpVerticalSpeed(jumpHeight);
	}

	public float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2f * jumpHeight * gravity);
	}

	public void ChangeTrack(int movement, float duration)
	{
		TasksManager.Instance.PlayerDidThis(TaskTarget.StayInOneLane, (int)(Time.time - sameLaneTimeStamp));
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.StayInOneLane);
		sameLaneTimeStamp = Time.time;
		if (trackMovement != movement)
		{
			ForceChangeTrack(movement, duration);
		}
		else
		{
			trackMovementNext = movement;
		}
	}

	private IEnumerator ChangeTrackCoroutine(int move, float duration, bool force = false)
	{
		if (isInOneway)
		{
			if (trackIndexTarget > onewayId)
			{
				HandleStumble(StumbleType.Side, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, "side");
			}
			else if (trackIndexTarget < onewayId)
			{
				HandleStumble(StumbleType.Side, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "side");
			}
			yield break;
		}
		trackMovement = move;
		trackMovementNext = 0;
		int newTrackIndex = trackIndexTarget + move;
		float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - trackIndexPosition);
		float trackIndexPositionBegin = trackIndexPosition;
		float startX = x;
		float endX = trackController.GetTrackX(newTrackIndex);
		float dir = Mathf.Sign(newTrackIndex - trackIndexTarget);
		float startRotation = characterRotation;
		if (newTrackIndex >= 0)
		{
			if (newTrackIndex >= trackController.NumberOfTracks)
			{
				HandleStumble(StumbleType.Side, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, "side");
				yield break;
			}
			if (!force && this.OnChangeTrack != null)
			{
				this.OnChangeTrack((move >= 0) ? OnChangeTrackDirection.Right : OnChangeTrackDirection.Left);
			}
			trackIndexTarget = newTrackIndex;
			HandleWaterBoard();
			yield return StartCoroutine(myTween.To(trackChangeIndexDistance * duration, delegate(float t)
			{
				trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndex, t);
				x = Mathf.Lerp(startX, endX, t);
				characterRotation = pMath.Bell(t) * dir * characterAngle + Mathf.Lerp(startRotation, 0f, t);
				characterRoot.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
			}));
			trackIndex = newTrackIndex;
			trackMovement = 0;
			if (trackMovementNext != 0)
			{
				StartCoroutine(ChangeTrackCoroutine(trackMovementNext, duration, force));
			}
		}
		else
		{
			HandleStumble(StumbleType.Side, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "side");
		}
	}

	public void CheckInAirJump()
	{
		if (characterController.isGrounded && inAirJump)
		{
			Jump();
			inAirJump = false;
		}
	}

	public void EndRoll()
	{
		squeezeCollider.Remove(this);
		isRolling = false;
	}

	public void ForceChangeTrack(int movement, float duration, bool force = false)
	{
		StopAllCoroutines();
		StartCoroutine(ChangeTrackCoroutine(movement, duration, force));
	}

	public void ForceLeaveSubway()
	{
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	private ImpactX GetImpactX(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.x, bounds2.min.x);
		float num2 = Mathf.Min(bounds.max.x, bounds2.max.x);
		float num3 = (num + num2) * 0.5f;
		float num4 = num3 - bounds2.min.x;
		if ((double)num4 > (double)bounds2.size.x - (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Right;
		}
		if ((double)num4 < (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Left;
		}
		return ImpactX.Middle;
	}

	private ImpactY GetImpactY(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.y, bounds2.min.y);
		float num2 = Mathf.Min(bounds.max.y, bounds2.max.y);
		float num3 = (num + num2) * 0.5f;
		float num4 = (num3 - bounds.min.y) / bounds.size.y;
		if (num4 < 0.33f)
		{
			return ImpactY.Lower;
		}
		if (num4 < 0.66f)
		{
			return ImpactY.Middle;
		}
		return ImpactY.Upper;
	}

	private ImpactZ GetImpactZ(Collider collider)
	{
		Vector3 position = base.transform.position;
		Bounds bounds = collider.bounds;
		if (position.z > bounds.max.z - ((!(bounds.max.z - bounds.min.z <= 30f)) ? stumbleCornerTolerance : ((bounds.max.z - bounds.min.z) * 0.5f)))
		{
			return ImpactZ.After;
		}
		if (position.z < bounds.min.z + stumbleCornerTolerance)
		{
			return ImpactZ.Before;
		}
		return ImpactZ.Middle;
	}

	public float GetTrackX()
	{
		return trackController.GetPosition(trackController.GetTrackX(trackIndex), 0f).x;
	}

	private void HandleRevive()
	{
		StopAllCoroutines();
		StopStumble();
	}

	private void HandleStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (!game.IsInFlypackMode && !game.IsInSpringJumpMode && !game.IsInBoundJumpMode)
		{
			NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
			StartStumble();
		}
	}

	private void HitByTrainSequence()
	{
		if (helmet.IsActive)
		{
			NotifyOnJumpIfHitByTrain();
		}
		else
		{
			NotifyOnHitByTrain();
		}
	}

	private void FallIntoWater()
	{
		if (helmet.IsActive)
		{
			NotifyOnJumpIfHitByTrain();
		}
		else
		{
			NotifyOnFallIntoWater();
		}
	}

	public void DelegateIsInGame(bool isInGame)
	{
		if (!isInGame)
		{
			StopAllCoroutines();
			immuneToCriticalHit = false;
			characterController.enabled = true;
			stopColliding = false;
		}
	}

	public void DelegateSqueeze(bool squeeze)
	{
		if (squeeze)
		{
			characterController.height = 4f;
			characterController.center = new Vector3(0f, 2f, characterControllerCenter.z);
			characterCollider.height = 4f;
			characterCollider.center = new Vector3(0f, 4f, characterColliderCenter.z);
		}
		else
		{
			characterController.center = characterControllerCenter;
			characterController.height = characterControllerHeight;
			characterCollider.center = characterColliderCenter;
			characterCollider.height = characterColliderHeight;
		}
	}

	public void Initialize()
	{
		layers = Layers.Instance;
		game = Game.Instance;
		game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(DelegateIsInGame));
		squeezeCollider.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(squeezeCollider.OnChange, new VariableBool.OnChangeDelegate(DelegateSqueeze));
		trackController = TrackController.Instance;
		characterController = Game.Charactercontroller;
		running = Running.Instance;
		revive = Revive.Instance;
		revive.OnRevive += HandleRevive;
		GetComponent<CharacterRendering>().Initialize();
		superShoes = SuperShoes.Instance;
		characterRoot = characterModel.transform;
		helmet = Helmet.Instance;
		helmet.OnJumpFromWater += OnJumpFromWater;
		characterCamera = CharacterCamera.Instance;
		boss = Boss.Instance;
		CharacterPickupParticleSystem = GetComponentInChildren<PickupParticles>();
		characterColliderTrigger = characterCollider.GetComponent<OnTriggerObject>();
		characterColliderTrigger.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(characterColliderTrigger.OnEnter, new OnTriggerObject.OnEnterDelegate(OnCharacterColliderEnter));
		characterColliderTrigger.OnExit = (OnTriggerObject.OnExitDelegate)Delegate.Combine(characterColliderTrigger.OnExit, new OnTriggerObject.OnExitDelegate(OnCharacterColliderExit));
		characterControllerCenter = characterController.center;
		characterControllerHeight = characterController.height;
		characterColliderCenter = characterCollider.center;
		characterColliderHeight = characterCollider.height;
		this.OnLanding = (OnLandingDelegate)Delegate.Combine(this.OnLanding, new OnLandingDelegate(TasksManager.Instance.OnChangeIsCharacterOnGround));
	}

	private bool IsRunningOnGround()
	{
		return running.currentRunPosition == Running.RunPositions.ground;
	}

	public void Jump()
	{
		if (isInsideUnderpass)
		{
			return;
		}
		bool flag = !isJumping && verticalSpeed <= 0f && verticalSpeed > verticalSpeed_jumpTolerance;
		if (characterController.isGrounded || (flag && !running.transitionFromHeight))
		{
			isJumping = true;
			isFalling = false;
			IsGrounded.Value = false;
			if (isJumpingHigher)
			{
				isJumpingWithSuperShoes = true;
				Vector3 position = base.transform.position;
				JumpCurveData value = default(JumpCurveData);
				value.z_start = position.z;
				value.z_length = JumpLength(game.currentSpeed, jumpHeightSuperShoes) * superShoesJumpApexRatio;
				value.z_end = value.z_start + value.z_length;
				value.y_start = position.y;
				superShoesJump = value;
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				NotifyOnJump();
			}
			if (IsRunningOnGround())
			{
				startedJumpFromGround = true;
				trainJump = false;
				trainJumpSampleZ = z + trainJumpSampleLength;
				busJump = false;
				busJumpSampleZ = z + busJumpSampleLength;
				carJump = false;
				carJumpSampleZ = z + carJumpSampleLength;
			}
		}
		else if (verticalSpeed < 0f)
		{
			inAirJump = true;
		}
	}

	public float JumpLength(float speed, float jumpHeight)
	{
		return speed * 2f * CalculateJumpVerticalSpeed(jumpHeight) / gravity;
	}

	private IEnumerator MoveCharacterToPosition(float newX, float newZ, float time)
	{
		float oldX = x;
		float oldZ = z;
		Vector3 pos = base.transform.position;
		pos.y = lastGroundedY;
		base.transform.position = pos;
		immuneToCriticalHit = true;
		stopColliding = true;
		characterController.enabled = false;
		NotifyOnTutorialMoveBackToCheckPoint(time);
		yield return StartCoroutine(myTween.To(time, delegate(float t)
		{
			x = Mathf.SmoothStep(oldX, newX, t);
			z = Mathf.SmoothStep(oldZ, newZ, t);
		}));
		immuneToCriticalHit = false;
		characterController.enabled = true;
		NotifyOnTutorialStartFromCheckPoint();
		stopColliding = false;
		ResetWalls();
	}

	public void HandleWaterBoard()
	{
		if (this.OnHandleWaterBoard != null)
		{
			this.OnHandleWaterBoard(trackIndexTarget);
		}
	}

	public void MoveForward()
	{
		Vector3 position = base.transform.position;
		float num = z + game.currentSpeed * Time.deltaTime;
		Vector3 vector = verticalSpeed * Time.deltaTime * Vector3.up;
		Vector3 position2 = trackController.GetPosition(x, num);
		Vector3 vector2 = new Vector3(position.x, 0f, position.z);
		if (superShoesJump.HasValue)
		{
			JumpCurveData value = superShoesJump.Value;
			if (z < value.z_end)
			{
				float num2 = superSneakersJumpCurve.Evaluate((num - value.z_start) / value.z_length) * jumpHeightSuperShoes + value.y_start;
				float num3 = num2 - position.y;
				vector = Vector3.up * num3;
			}
			else
			{
				superShoesJump = null;
				verticalSpeed = 0f;
				vector = Vector3.zero;
			}
		}
		if (isStairing && !isJumping)
		{
			vector = Vector3.down * 100f;
		}
		Vector3 vector3 = position2 - vector2;
		if (characterController.enabled)
		{
			characterController.Move(vector + vector3);
		}
		else
		{
			characterController.transform.position += vector3;
		}
		z = base.transform.position.z;
		if (characterController.isGrounded)
		{
			lastGroundedY = position.y;
		}
	}

	public void MoveWithGravity()
	{
		if (characterController.enabled)
		{
			verticalSpeed -= gravity * Time.deltaTime;
			if (verticalSpeed > 0f)
			{
				verticalSpeed = 0f;
			}
			Vector3 motion = verticalSpeed * Time.deltaTime * Vector3.up;
			characterController.Move(motion);
		}
	}

	private void NotifyCriticalHit()
	{
		CriticalHitType type = CriticalHitType.None;
		if (this.OnCriticalHit == null)
		{
			return;
		}
		string text = lastHitTag;
		if (text != null)
		{
			switch (text)
			{
			case "HitTrain":
				type = CriticalHitType.Train;
				break;
			case "HitBarrier":
				type = CriticalHitType.Barrier;
				break;
			case "HitMovingTrain":
				type = CriticalHitType.MovingTrain;
				break;
			case "FallIntoWater":
				type = CriticalHitType.FallIntoWater;
				break;
			case "HitBus":
				type = CriticalHitType.Bus;
				break;
			case "HitMovingBus":
				type = CriticalHitType.MovingBus;
				break;
			case "HitCar":
				type = CriticalHitType.Car;
				break;
			case "HitMovingCar":
				type = CriticalHitType.MovingCar;
				break;
			case "HitFlowerBed":
				type = CriticalHitType.FlowerBed;
				break;
			}
		}
		this.OnCriticalHit(type);
	}

	private void NotifyOnChangeTrack(OnChangeTrackDirection direction)
	{
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack(direction);
		}
	}

	private void NotifyOnCriticalHit(CriticalHitType type)
	{
		if (this.OnCriticalHit != null)
		{
			this.OnCriticalHit(type);
		}
	}

	private void NotifyOnHangtime()
	{
		if (this.OnHangtime != null)
		{
			this.OnHangtime();
		}
	}

	private void NotifyOnFallIntoWater()
	{
		if (this.OnFallIntoWater != null)
		{
			this.OnFallIntoWater();
		}
	}

	private void NotifyOnHitByTrain()
	{
		if (this.OnHitByTrain != null)
		{
			this.OnHitByTrain();
		}
	}

	private void NotifyOnJump()
	{
		if (this.OnJump != null)
		{
			this.OnJump();
		}
	}

	private void NotifyOnJumpIfHitByTrain()
	{
		if (this.OnJumpIfHitByTrain != null)
		{
			this.OnJumpIfHitByTrain();
		}
	}

	private void NotifyOnJumpOverTrain()
	{
		if (this.OnJumpOverTrain != null)
		{
			this.OnJumpOverTrain();
		}
	}

	private void NotifyOnJumpOverBus()
	{
		if (this.OnJumpOverBus != null)
		{
			this.OnJumpOverBus();
		}
	}

	private void NotifyOnJumpOverCar()
	{
		if (this.OnJumpOverCar != null)
		{
			this.OnJumpOverCar();
		}
	}

	private void NotifyOnLanding()
	{
		if (this.OnLanding != null)
		{
			this.OnLanding(base.transform);
		}
	}

	private void NotifyOnJumpRoll()
	{
		if (this.OnJumpRoll != null)
		{
			this.OnJumpRoll();
		}
	}

	private void NotifyOnRoll()
	{
		if (this.OnRoll != null)
		{
			this.OnRoll();
		}
	}

	private void NotifyOnStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.OnStumble != null)
		{
			this.OnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
		}
	}

	private void NotifyOnTutorialMoveBackToCheckPoint(float duration)
	{
		if (this.OnTutorialMoveBackToCheckPoint != null)
		{
			this.OnTutorialMoveBackToCheckPoint(duration);
		}
	}

	private void NotifyOnTutorialStartFromCheckPoint()
	{
		if (this.OnTutorialStartFromCheckPoint != null)
		{
			this.OnTutorialStartFromCheckPoint();
		}
	}

	public void NotifyPickup(IPickup pickup)
	{
		if (pickup != null)
		{
			pickup.NotifyPickup(CharacterPickupParticleSystem);
		}
	}

	public void NotifyPickupCheck(ChestPickup pickup)
	{
		if (pickup != null)
		{
			pickup.NotifyPickup(CharacterPickupParticleSystem);
		}
	}

	private ObstacleType ObstacleTypeByTag(string tag)
	{
		if (tag != null)
		{
			switch (tag)
			{
			case "RollBarrier":
				return ObstacleType.RollBarrier;
			case "JumpBarrier":
				return ObstacleType.JumpBarrier;
			case "JumpHighBarrier":
				return ObstacleType.JumpHighBarrier;
			}
		}
		return ObstacleType.None;
	}

	private void OnCharacterColliderEnter(Collider collider)
	{
		if (!game.IsInGame.Value)
		{
			return;
		}
		if (collider.CompareTag("Underpass"))
		{
			underpassColliders.Add(collider);
			isInsideUnderpass = underpassColliders.Count > 0;
		}
		else if (collider.CompareTag("Subway"))
		{
			subwayColliders.Add(collider);
			isInsideSubway = subwayColliders.Count > 0;
			subwayMaxY = collider.bounds.max.y - 3f;
		}
		else if (collider.CompareTag("OneWay") && !game.IsInFlypackMode)
		{
			onewayColliders.Add(collider);
			isInOneway = onewayColliders.Count > 0;
			onewayId = Mathf.FloorToInt(collider.bounds.center.x / 20f) + 1;
		}
		else if (collider.CompareTag("Speedup"))
		{
			speedupColliders.Add(collider);
			isInspeedup = speedupColliders.Count > 0;
			game.StartSpeedUp();
		}
		else
		{
			if (stopColliding || collider.gameObject.layer == layers.KeepOnHelmet)
			{
				return;
			}
			IPickup componentInChildren = collider.GetComponentInChildren<IPickup>();
			if (componentInChildren != null)
			{
				NotifyPickup(componentInChildren);
				return;
			}
			ChestPickup componentInChildren2 = collider.GetComponentInChildren<ChestPickup>();
			if (componentInChildren2 != null)
			{
				NotifyPickupCheck(componentInChildren2);
				return;
			}
			ITouchByCharacter componentInChildren3 = collider.GetComponentInChildren<ITouchByCharacter>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.BeTouched();
				return;
			}
			Wall componentInChildren4 = collider.GetComponentInChildren<Wall>();
			if (componentInChildren4 != null)
			{
				if (componentInChildren4.swipeDir == SwipeDir.Left)
				{
					walls[0] = componentInChildren4;
				}
				else
				{
					walls[1] = componentInChildren4;
				}
				componentInChildren4.OnTrigger();
				return;
			}
			if (collider.gameObject.layer == layers.Default)
			{
				if (collider.isTrigger && characterController.isGrounded)
				{
					IsGrounded.Value = true;
				}
				if (collider.isTrigger)
				{
					ObstacleType obstacleType = ObstacleTypeByTag(collider.tag);
					if (obstacleType != ObstacleType.None)
					{
						lastObstacleTriggerType = obstacleType;
						lastObstacleTriggerTrackIndex = trackIndex;
					}
				}
				return;
			}
			if (collider.isTrigger)
			{
				string text = collider.name;
				if ("bush".Equals(text) || "powerbox".Equals(text))
				{
					HandleStumble(StumbleType.Bush, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, text);
				}
				else if ("water".Equals(text))
				{
					lastHitTag = collider.tag;
					FallIntoWater();
					NotifyCriticalHit();
				}
				else
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, text);
				}
				return;
			}
			lastHitTag = collider.tag;
			if (!game.IsInFlypackMode)
			{
				if (lastHitTag == "HitLeftBoundSide")
				{
					ChangeTrack(1, 0.2f);
					forceToOneway = true;
					return;
				}
				if (lastHitTag == "HitRightBoundSide")
				{
					ChangeTrack(-1, 0.2f);
					forceToOneway = true;
					return;
				}
			}
			ImpactX impactX = GetImpactX(collider);
			ImpactY impactY = GetImpactY(collider);
			ImpactZ impactZ = GetImpactZ(collider);
			float num = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
			float num2 = base.transform.position.x;
			int num3 = ((num2 < num) ? 1 : ((num2 > num) ? (-1) : 0));
			bool flag = num3 == 0 || trackMovement == num3;
			bool flag2 = characterCollider.bounds.center.z < collider.bounds.min.z;
			bool flag3 = impactZ == ImpactZ.Before && !flag2 && flag;
			if (impactZ == ImpactZ.Middle || flag3)
			{
				if (trackMovement != 0)
				{
					float duration = 0.5f;
					if (trackController.IsRunningOnTutorialTrack)
					{
						duration = 0.2f;
					}
					ChangeTrack(-trackMovement, duration);
				}
				switch (impactX)
				{
				case ImpactX.Left:
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, collider.name);
					break;
				case ImpactX.Right:
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, collider.name);
					break;
				}
				return;
			}
			if (impactX == ImpactX.Middle || trackMovement == 0)
			{
				if (impactZ == ImpactZ.Before)
				{
					if (impactY == ImpactY.Lower)
					{
						verticalSpeed = CalculateJumpVerticalSpeed(8f);
						HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
					}
					else if (collider.gameObject.CompareTag("HitMovingTrain"))
					{
						HitByTrainSequence();
						NotifyCriticalHit();
					}
					else if (impactY == ImpactY.Middle)
					{
						HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
						NotifyCriticalHit();
					}
					else
					{
						HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Upper, collider.name);
						NotifyCriticalHit();
					}
				}
				return;
			}
			if (impactZ == ImpactZ.Before && flag)
			{
				if (collider.gameObject.CompareTag("HitMovingTrain"))
				{
					HitByTrainSequence();
					NotifyCriticalHit();
				}
				else if (collider.gameObject.layer == layers.HitBounceOnly)
				{
					HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
				}
				else
				{
					ForceChangeTrack(-trackMovement, 0.5f);
				}
			}
			else if (collider.gameObject.layer == layers.HitBounceOnly)
			{
				ForceChangeTrack(-trackMovement, 0.5f);
			}
			switch (impactX)
			{
			case ImpactX.Left:
				HandleStumble(StumbleType.Normal, StumbleHorizontalHit.LeftCorner, StumbleVerticalHit.Middle, collider.name);
				break;
			case ImpactX.Right:
				HandleStumble(StumbleType.Normal, StumbleHorizontalHit.RightCorner, StumbleVerticalHit.Middle, collider.name);
				break;
			}
		}
	}

	private void OnCharacterColliderExit(Collider collider)
	{
		if (collider.CompareTag("Underpass"))
		{
			if (underpassColliders.Contains(collider))
			{
				underpassColliders.Remove(collider);
				isInsideUnderpass = underpassColliders.Count > 0;
			}
			return;
		}
		if (collider.CompareTag("Subway"))
		{
			if (subwayColliders.Contains(collider))
			{
				subwayColliders.Remove(collider);
				isInsideSubway = subwayColliders.Count > 0;
			}
			return;
		}
		if (collider.CompareTag("OneWay"))
		{
			if (onewayColliders.Contains(collider))
			{
				onewayColliders.Remove(collider);
				isInOneway = onewayColliders.Count > 0;
				forceToOneway = isInOneway;
			}
			return;
		}
		if (collider.CompareTag("Speedup"))
		{
			if (speedupColliders.Contains(collider))
			{
				speedupColliders.Remove(collider);
				isInspeedup = speedupColliders.Count > 0;
			}
			return;
		}
		ObstacleType obstacleType = ObstacleTypeByTag(collider.tag);
		if (obstacleType == lastObstacleTriggerType && lastObstacleTriggerTrackIndex == trackIndex && this.OnPassedObstacle != null)
		{
			this.OnPassedObstacle(obstacleType);
		}
		if (obstacleType == ObstacleType.RollBarrier && this.OnRollGuard != null)
		{
			this.OnRollGuard();
		}
	}

	public void Restart()
	{
		trackIndex = initialTrackIndex;
		trackIndexTarget = initialTrackIndex;
		x = trackController.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		z = 0f;
		trackMovement = 0;
		trackMovementNext = 0;
		squeezeCollider.Clear();
		characterController.Move(-5f * Vector3.up);
		verticalSpeed = 0f;
		superShoesJump = null;
		jumpHeight = jumpHeightNormal;
		inAirJump = false;
		isJumping = false;
		isJumpingWithSuperShoes = false;
		isRolling = false;
		IsGrounded.Value = false;
		lastGroundedY = 0f;
		boss.Restart(true);
		StartStumble();
		startedJumpFromGround = false;
		sameLaneTimeStamp = Time.time;
		subwayColliders.Clear();
		underpassColliders.Clear();
		walls[0] = null;
		walls[1] = null;
		onewayColliders.Clear();
		speedupColliders.Clear();
		isInsideSubway = false;
		isInsideUnderpass = false;
		isInOneway = false;
		forceToOneway = false;
		isInspeedup = false;
		onewayId = -1;
	}

	public void Roll()
	{
		if (!isRolling)
		{
			if (superShoesJump.HasValue)
			{
				superShoesJump = null;
			}
			squeezeCollider.Add(this);
			if (!running.transitionFromHeight)
			{
				verticalSpeed = 0f - CalculateJumpVerticalSpeed(jumpHeight);
			}
			else
			{
				verticalSpeed = -250f;
			}
			isRolling = true;
			NotifyOnRoll();
		}
	}

	private void OnJumpFromWater()
	{
		IsJumping = true;
		IsFalling = false;
		verticalSpeed = CalculateJumpVerticalSpeed(60f);
	}

	public void SetBackToCheckPoint(float zoomTime)
	{
		float lastCheckPoint = trackController.GetLastCheckPoint(z);
		trackIndex = initialTrackIndex;
		trackIndexTarget = initialTrackIndex;
		float trackX = trackController.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		trackMovement = 0;
		trackMovementNext = 0;
		StartCoroutine(MoveCharacterToPosition(trackX, lastCheckPoint, zoomTime));
	}

	public void SetFrontToCheckPoint()
	{
		TrackPiece.TrackCheckPoint nextCheckPoint = trackController.GetNextCheckPoint(z);
		if (nextCheckPoint != null)
		{
			trackIndex = initialTrackIndex;
			trackIndexTarget = initialTrackIndex;
			trackIndexPosition = trackIndex;
			trackMovement = 0;
			trackMovementNext = 0;
			x = trackController.GetTrackX(trackIndex);
			z = nextCheckPoint.Z;
			base.transform.position = trackController.GetPosition(x, z) + Vector3.up * nextCheckPoint.y;
			characterCamera.Reset(base.transform.position, Quaternion.identity, true);
		}
	}

	private void StartStumble()
	{
		isStumbling = true;
		if (!trackController.IsRunningOnTutorialTrack)
		{
			boss.CatchUp();
		}
		boss.StartCoroutine(StumbleDecay());
	}

	public void StopStumble()
	{
		boss.ResetCatchUp();
		isStumbling = false;
	}

	private IEnumerator StumbleDecay()
	{
		yield return new WaitForSeconds(stumbleDecayTime);
		StopStumble();
	}

	public Wall CanWallWithSwipeDir(SwipeDir dir)
	{
		if (!characterController.isGrounded)
		{
			return null;
		}
		Wall wall = null;
		wall = ((dir != SwipeDir.Left) ? walls[1] : walls[0]);
		if (wall != null && wall.Bounds.Contains(base.transform.position))
		{
			return wall;
		}
		return null;
	}

	public void WallEndWithSwipeDir(SwipeDir dir)
	{
		if (dir == SwipeDir.Left)
		{
			walls[0] = null;
		}
		walls[1] = null;
	}

	public void ResetWalls()
	{
		if (walls[0] != null)
		{
			walls[0].Collider.enabled = true;
		}
		walls[0] = null;
		if (walls[1] != null)
		{
			walls[1].Collider.enabled = true;
		}
		walls[1] = null;
	}

	public void ResetOneway()
	{
		onewayColliders.Clear();
		isInOneway = false;
		forceToOneway = false;
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < -200f)
		{
			position.y = -199f;
			base.transform.position = position;
		}
	}
}
