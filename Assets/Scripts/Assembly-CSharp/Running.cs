using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : CharacterState
{
	public enum RunPositions
	{
		ground = 0,
		station = 1,
		train = 2,
		movingTrain = 3,
		bus = 4,
		movingBus = 5,
		car = 6,
		movingCar = 7,
		flowerBed = 8,
		air = 9
	}

	private class AnimationPlayingData
	{
		public AnimationState state;

		public float time;

		public bool isQueued;
	}

	public bool intro;

	public float transitionTimeFlypack = 1f;

	public float transitionTimeMax = 1.2f;

	public float characterChangeTrackLength = 30f;

	public float ySmoothDuration = 0.1f;

	public float speedupTransitionTime = 6f;

	public float speedupTransitionTimeLow = 10f;

	public RunPositions currentRunPosition;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private Game game;

	public bool inStair;

	public bool transitionFromHeight;

	public bool transitionFromPogostick;

	private Queue<Collider> GrindedTrains = new Queue<Collider>();

	private int GrindedTrainsBufferSize = 5;

	private static Running instance;

	private bool lastOnGrounded;

	private Vector3 tunnelStartPos;

	private float slopeOfTunnel = 1f;

	private float tunnelTotalDistance = 300f;

	private List<AnimationPlayingData> animatinData = new List<AnimationPlayingData>();

	public static Running Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Running)) as Running;
			}
			return instance;
		}
	}

	public bool Pause { get; set; }

	public void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		characterController = character.characterController;
		characterCamera = CharacterCamera.Instance;
		character.OnStumble += delegate
		{
			character.characterCamera.CameraShakeController.Shake();
		};
		character.OnLanding += UpdateGroundTag;
		game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(game.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
	}

	private void OnIntroRun()
	{
		intro = true;
		game.topMenu.AddPlayerOnStopEvent(IntroEnd);
	}

	private void IntroEnd()
	{
		intro = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		game.GameStart();
		game.topMenu.RemovePlayerOnStopEvent(IntroEnd);
	}

	public override IEnumerator Begin()
	{
		inStair = false;
		transitionFromHeight = false;
		bool transitionWithRoll = false;
		bool fastTransition = false;
		float transitionTimeLength = 0f;
		if (characterTransform.position.y > 70f && characterTransform.position.y < 125f)
		{
			transitionTimeLength = transitionTimeFlypack;
			transitionFromHeight = true;
		}
		if (transitionFromPogostick)
		{
			transitionTimeLength = transitionTimeMax;
			transitionFromHeight = true;
			if (characterTransform.position.y < 160f)
			{
				transitionWithRoll = true;
			}
		}
		character.characterCollider.enabled = true;
		character.characterCamera.enabled = true;
		float transitionTime = 0f;
		character.lastGroundedY = character.transform.position.y;
		lastOnGrounded = characterController.isGrounded;
		characterController.Move(Vector3.down * 2f);
		tunnelStartPos = Vector3.zero;
		while (true)
		{
			if (Pause)
			{
				yield return null;
				continue;
			}
			if (intro && characterTransform.position.z > 0f)
			{
				IntroEnd();
			}
			if (!intro)
			{
				game.currentSpeed = game.currentLevelSpeed;
				if (!game.GoingBackToCheckpoint)
				{
					game.LayTrackChunks();
					game.HandleControls();
					character.ApplyGravity();
				}
				character.MoveForward();
			}
			if (character.IsGrounded.Value && transitionFromHeight)
			{
				transitionTimeLength = (transitionWithRoll ? (transitionTimeLength - Time.deltaTime * speedupTransitionTimeLow) : (transitionTimeLength - Time.deltaTime * speedupTransitionTime));
			}
			Vector3 position = characterTransform.position;
			float y = character.lastGroundedY;
			if (game.GoingBackToCheckpoint)
			{
				y = character.lastGroundedY;
			}
			else if (inStair && !characterController.isGrounded)
			{
				y = position.y;
			}
			else if (character.InSuperShoesJump && !transitionFromHeight)
			{
				y = 0.5f * (character.lastGroundedY + position.y);
			}
			else if (!lastOnGrounded && characterController.isGrounded)
			{
				y = position.y;
			}
			else if (characterController.isGrounded)
			{
				y = character.lastGroundedY;
			}
			else if (position.y < character.lastGroundedY)
			{
				y = position.y;
			}
			position.y = y;
			lastOnGrounded = characterController.isGrounded;
			characterCamera.CurrentTunnelProgress = (character.z - tunnelStartPos.z) / tunnelTotalDistance;
			CharacterCamera obj = characterCamera;
			float num = (character.z - tunnelStartPos.z) / tunnelTotalDistance;
			characterCamera.SubwayExitCameraProgress = num;
			obj.SubwayEnterCameraProgress = num;
			if (character.IsInsideSubway)
			{
				characterCamera.SetMaxCameraHeight(character.subwayMaxY);
			}
			if (transitionFromHeight)
			{
				transitionTime += Time.deltaTime;
				characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				if (!transitionWithRoll)
				{
					transitionFromPogostick = false;
				}
				else
				{
					transitionWithRoll = false;
					characterCamera.SetCameraTransition(CameraFollowMode.SpringDown);
				}
				if (character.IsGrounded.Value || (character.inAirJump && characterController.isGrounded))
				{
					transitionFromHeight = false;
					transitionFromPogostick = false;
					transitionWithRoll = false;
					fastTransition = true;
					position.y = characterTransform.position.y;
					character.ForceLeaveSubway();
				}
				characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, false);
			}
			else if (fastTransition)
			{
				transitionTime += Time.deltaTime * 2f;
				characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, false);
				if (transitionTime >= transitionTimeLength)
				{
					fastTransition = false;
				}
			}
			else
			{
				characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, true);
			}
			if (!intro)
			{
				character.CheckInAirJump();
				game.UpdateMeters();
				UpdateInAirRunPosition();
				UpdateRunStateMeters();
			}
			yield return null;
		}
	}

	public override void HandleCriticalHit(bool isShake = true)
	{
		if (isShake)
		{
			characterCamera.CameraShakeController.Shake();
		}
		game.WillDie();
	}

	public override void HandleDoubleTap()
	{
		if (!intro && PlayerInfo.Instance.tutorialStep >= 1 && PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) > 0 && !game.Attachment.IsActive(game.Attachment.Helmet))
		{
			game.Attachment.Add(game.Attachment.Helmet);
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (intro)
		{
			return;
		}
		switch (swipeDir)
		{
		case SwipeDir.Up:
			if (!inStair)
			{
				character.Jump();
			}
			break;
		case SwipeDir.Down:
			character.Roll();
			break;
		case SwipeDir.Left:
			if (!character.forceToOneway)
			{
				Wall wall2 = character.CanWallWithSwipeDir(SwipeDir.Left);
				if (wall2 != null)
				{
					game.OnStartWall(SwipeDir.Left, wall2);
				}
				else
				{
					character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
				}
			}
			break;
		case SwipeDir.Right:
			if (!character.forceToOneway)
			{
				Wall wall = character.CanWallWithSwipeDir(SwipeDir.Right);
				if (wall != null)
				{
					game.OnStartWall(SwipeDir.Right, wall);
				}
				else
				{
					character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
				}
			}
			break;
		}
	}

	public void OnPause(bool pause)
	{
		Pause = pause;
		if (pause)
		{
			foreach (AnimationState item in characterRendering.characterAnimation)
			{
				if (item.enabled)
				{
					AnimationPlayingData animationPlayingData = new AnimationPlayingData();
					animationPlayingData.state = item;
					animationPlayingData.time = item.time;
					animationPlayingData.isQueued = false;
					animatinData.Add(animationPlayingData);
					item.enabled = false;
				}
				else if (item.name.EndsWith("Queued Clone"))
				{
					AnimationPlayingData animationPlayingData2 = new AnimationPlayingData();
					animationPlayingData2.state = item;
					animationPlayingData2.time = 0f;
					animationPlayingData2.isQueued = true;
					animatinData.Add(animationPlayingData2);
				}
			}
			characterRendering.characterAnimation.Sample();
			return;
		}
		int i = 0;
		for (int count = animatinData.Count; i < count; i++)
		{
			AnimationPlayingData animationPlayingData3 = animatinData[i];
			if (!animationPlayingData3.isQueued)
			{
				animationPlayingData3.state.time = animationPlayingData3.time;
				animationPlayingData3.state.enabled = true;
			}
			else
			{
				characterRendering.characterAnimation.CrossFadeQueued(animationPlayingData3.state.clip.name, 0.2f);
			}
		}
		animatinData.Clear();
	}

	private void LandedOnTrain(Collider trainCollider)
	{
		if (character.helmet.IsActive && !GrindedTrains.Contains(trainCollider))
		{
			if (GrindedTrains.Count > GrindedTrainsBufferSize)
			{
				GrindedTrains.Dequeue();
			}
			GrindedTrains.Enqueue(trainCollider);
			GameStats.Instance.grindedTrains++;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.LandOnTrainInRow);
	}

	public void StartTunnel(float tunnelLength)
	{
		tunnelStartPos = characterTransform.position;
		tunnelTotalDistance = tunnelLength;
		characterCamera.SetCameraToTunnleTransition();
	}

	public void StartUpstair(float stairLength, float slopeOfTunnel)
	{
		inStair = true;
		this.slopeOfTunnel = slopeOfTunnel;
		tunnelStartPos = characterTransform.position;
		tunnelTotalDistance = stairLength;
		characterCamera.SetCameraTransition(CameraFollowMode.StairUp);
	}

	public void StartDownstair(float stairLength, float slopeOfTunnel)
	{
		inStair = true;
		this.slopeOfTunnel = slopeOfTunnel;
		tunnelStartPos = characterTransform.position;
		tunnelTotalDistance = stairLength;
		characterCamera.SetCameraTransition(CameraFollowMode.StairDown);
	}

	public void EndUpstair()
	{
		inStair = false;
	}

	public void EndDownstair()
	{
		inStair = false;
		character.lastGroundedY = characterTransform.position.y;
	}

	public float GetStairYAtZ(float z)
	{
		return tunnelStartPos.y + (z - tunnelStartPos.z) * slopeOfTunnel;
	}

	private void UpdateGroundTag(Transform characterTransform)
	{
		Ray ray = new Ray(character.characterRoot.position, -Vector3.up);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo))
		{
			return;
		}
		string text = hitInfo.collider.tag;
		if (text != null)
		{
			switch (text)
			{
			case "Ground":
				currentRunPosition = RunPositions.ground;
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.LandOnTrainInRow);
				break;
			case "HitTrain":
				currentRunPosition = RunPositions.train;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitMovingTrain":
				currentRunPosition = RunPositions.movingTrain;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitBus":
				currentRunPosition = RunPositions.bus;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitMovingBus":
				currentRunPosition = RunPositions.movingBus;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitCar":
				currentRunPosition = RunPositions.car;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitMovingCar":
				currentRunPosition = RunPositions.movingCar;
				LandedOnTrain(hitInfo.collider);
				break;
			case "HitFlowerBed":
				currentRunPosition = RunPositions.flowerBed;
				break;
			case "Station":
				currentRunPosition = RunPositions.station;
				break;
			}
		}
	}

	private void UpdateInAirRunPosition()
	{
		if (!characterController.isGrounded)
		{
			currentRunPosition = RunPositions.air;
		}
	}

	private void UpdateRunStateMeters()
	{
		float num = game.currentSpeed * Time.deltaTime;
		GameStats gameStats = GameStats.Instance;
		if (currentRunPosition != RunPositions.air)
		{
			if (character.trackIndex == 0)
			{
				gameStats.metersRunLeftTrack += num;
			}
			if (character.trackIndex == 1)
			{
				gameStats.metersRunCenterTrack += num;
			}
			if (character.trackIndex == 2)
			{
				gameStats.metersRunRightTrack += num;
			}
		}
		if (currentRunPosition == RunPositions.ground)
		{
			gameStats.metersRunGround += num;
		}
		if (currentRunPosition == RunPositions.air)
		{
			gameStats.metersFly += num;
		}
		if (currentRunPosition == RunPositions.station)
		{
			gameStats.metersRunStation += num;
		}
		if (currentRunPosition == RunPositions.train)
		{
			gameStats.metersRunTrain += num;
		}
		if (currentRunPosition == RunPositions.movingTrain)
		{
			gameStats.metersRunTrain += num;
		}
	}
}
