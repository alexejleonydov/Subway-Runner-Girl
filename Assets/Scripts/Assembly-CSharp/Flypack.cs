using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flypack : CharacterState
{
	[Serializable]
	public class FlyAheadInfo
	{
		public AnimationCurve cameraMovement;
	}

	public delegate void OnActivateTurboHeadstartDelegate();

	public delegate void OnDeactivateTurboHeadstartDelegate();

	public delegate void OnFlyAheadStartDelegate();

	public delegate void OnFlyAheadUpdateDelegate(float ratio);

	public delegate void OnFlyAheadEndDelegate();

	public delegate void OnHidTurboHeadstartButtonsDelegate();

	public delegate void OnStartDelegate(bool isHeadStart);

	public delegate void OnStopDelegate();

	[SerializeField]
	private float speedup = 2f;

	[SerializeField]
	private float flyHeight = 95f;

	[SerializeField]
	private float hitCeilingZPosition = 10f;

	[SerializeField]
	private ParticleSystem ceilingBrickExpolsion;

	[SerializeField]
	private float coinOffset = 200f;

	[SerializeField]
	private float flyAheadDuration = 1.5f;

	[SerializeField]
	private float flyAfterDuration = 0.2f;

	[SerializeField]
	private float stopBeforeLandingChunkDistance = 50f;

	public float characterChangeTrackLength = 60f;

	[HideInInspector]
	public bool isActive;

	[HideInInspector]
	public bool headStart;

	[SerializeField]
	private GameObject HeadstartPickupPrefab;

	[HideInInspector]
	public PropType powerType;

	private ActiveProp Powerup;

	[HideInInspector]
	public InAirCoinsManager coinsManager;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private Transform characterTransform;

	private Game game;

	private static Flypack instance;

	private bool activateHeadstart;

	private float extendedFlyDuration;

	private float flyAheadDistance;

	private float headStartSpeed;

	private float flypackDistance;

	private float flypackSpeed;

	private float startTime;

	private List<IPickup[]> pickupLists = new List<IPickup[]>();

	private TrackController trackController;

	private GameObject[] HeadstartPickups;

	public OnStartDelegate OnStart;

	public OnStopDelegate OnStop;

	public OnFlyAheadStartDelegate OnFlyAheadStart;

	public OnFlyAheadUpdateDelegate OnFlyAheadUpdate;

	public OnFlyAheadEndDelegate OnFlyAheadEnd;

	public bool ActivateTurboHeadstart
	{
		get
		{
			return activateHeadstart;
		}
	}

	public static Flypack Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Flypack)) as Flypack;
			}
			return instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	public event OnDeactivateTurboHeadstartDelegate OnDeactivateTurboHeadstart;

	public event OnHidTurboHeadstartButtonsDelegate OnHidTurboHeadstartButtons;

	public void Awake()
	{
		game = Game.Instance;
		game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(HandleOnStageMenuSequence));
		trackController = TrackController.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		characterTransform = characterController.transform;
		characterCamera = CharacterCamera.Instance;
		coinsManager = this.FindObject<InAirCoinsManager>();
		HeadstartPickups = new GameObject[3];
		for (int i = 0; i < 3; i++)
		{
			HeadstartPickups[i] = UnityEngine.Object.Instantiate(HeadstartPickupPrefab);
			pickupLists.Add(HeadstartPickups[i].GetComponentsInChildren<IPickup>());
			HeadstartPickups[i].SetActive(false);
		}
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		character.IsGrounded.Value = false;
		character.ResetOneway();
		if (powerType != PropType.headstart)
		{
			GameStats.Instance.pickedUpPowerups++;
		}
		Powerup = GameStats.Instance.RegisterPowerup(powerType);
		game.Attachment.PauseInFlypackMode();
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		if (SpringJump.Instance.isActive)
		{
			SpringJump.Instance.Stop();
		}
		NotifyOnStart(headStart);
		CoinLineManager.Instance.ClearTopLevelLines();
		BoundJumpCoinManager.Instance.ClearTopLevelLines();
		characterController.detectCollisions = false;
		character.characterCollider.enabled = false;
		float startY = characterTransform.position.y;
		float flyingDuration = 0f;
		if (!headStart)
		{
			flyingDuration = Powerup.timeLeft;
		}
		else
		{
			flyingDuration = PlayerInfo.Instance.GetPowerupDuration(powerType) - flyAheadDuration;
			headStartSpeed = PlayerInfo.Instance.GetPowerupSpeed(powerType);
		}
		flypackSpeed = (headStart ? headStartSpeed : (game.currentLevelSpeed * speedup));
		float flyDistance = flypackSpeed * flyingDuration;
		flyAheadDistance = flypackSpeed * flyAheadDuration;
		flypackDistance = flyAheadDistance + flyDistance;
		flypackDistance = trackController.LayJetpackPieces(character.z, flypackDistance) - stopBeforeLandingChunkDistance * Game.Instance.NormalizedGameSpeed;
		float extendedJetpackDuration = flypackDistance / flypackSpeed;
		extendedFlyDuration = extendedJetpackDuration - flyAheadDuration;
		flyDistance = extendedFlyDuration * flypackSpeed;
		float length = flyDistance - coinOffset;
		float startZ = character.z + flyAheadDistance + coinOffset;
		coinsManager.Spawn(startZ, length, flyHeight);
		game.currentSpeed = flypackSpeed;
		if (OnFlyAheadStart != null)
		{
			OnFlyAheadStart();
		}
		bool hasExploded = false;
		startTime = Time.time;
		characterCamera.SetCameraTransition(CameraFollowMode.FlyUp, flyAheadDuration);
		float ratio2 = 0f;
		while (ratio2 < 1f)
		{
			ratio2 = (Time.time - startTime) / flyAheadDuration;
			game.HandleControls();
			character.z += flypackSpeed * Time.deltaTime;
			Vector3 pivot = trackController.GetPosition(character.x, character.z) + Vector3.up * (startY + (flyHeight - startY) * Mathf.SmoothStep(0f, 1f, ratio2));
			characterTransform.position = pivot;
			if (OnFlyAheadUpdate != null)
			{
				OnFlyAheadUpdate(ratio2);
			}
			if (!hasExploded && pivot.y > hitCeilingZPosition && character.IsInsideSubway)
			{
				hasExploded = true;
				ceilingBrickExpolsion.gameObject.SetActive(true);
				ceilingBrickExpolsion.Play();
				character.ForceLeaveSubway();
				InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit();
			}
			characterCamera.SubwayExitCameraProgress = ratio2;
			characterCamera.UpdatePosition(characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
		}
		if (OnFlyAheadEnd != null)
		{
			OnFlyAheadEnd();
		}
		character.characterCollider.enabled = true;
		startTime = Time.time;
		ratio2 = 0f;
		while (ratio2 < 1f)
		{
			ratio2 = (Time.time - startTime) / extendedFlyDuration;
			if (ratio2 > 0.8f && !activateHeadstart && this.OnHidTurboHeadstartButtons != null)
			{
				this.OnHidTurboHeadstartButtons();
			}
			if (ratio2 > 0.95f && activateHeadstart)
			{
				activateHeadstart = false;
				if (this.OnDeactivateTurboHeadstart != null)
				{
					this.OnDeactivateTurboHeadstart();
				}
			}
			game.HandleControls();
			character.z += flypackSpeed * Time.deltaTime;
			character.transform.position = trackController.GetPosition(character.x, character.z) + Vector3.up * flyHeight;
			characterCamera.UpdatePosition(characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
		}
		characterCamera.SetCameraTransition(CameraFollowMode.FlyDown, flyAfterDuration);
		isActive = false;
		NotifyOnStop();
		characterController.detectCollisions = true;
		coinsManager.ReleaseCoins();
		if (headStart)
		{
			game.ChangeCurrentSpeed(game.speed.rampUpDuration * PlayerInfo.Instance.GetPowerupLandSpeed(PropType.headstart));
		}
		game.ChangeState(game.Running);
		game.Attachment.Resume();
	}

	private void HandleOnStageMenuSequence()
	{
		ResetPickups();
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Left:
			character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
			break;
		case SwipeDir.Right:
			character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
			break;
		}
	}

	private void NotifyOnStart(bool isHeadstart)
	{
		if (OnStart != null)
		{
			OnStart(isHeadstart);
		}
	}

	private void NotifyOnStop()
	{
		if (OnStop != null)
		{
			OnStop();
		}
	}

	public void PlacePickups(float z)
	{
		if (!headStart)
		{
			return;
		}
		float y = flyHeight - 5f;
		float z2 = z - 30f;
		int[] array = new int[3] { -1, -1, -1 };
		for (int i = 0; i < HeadstartPickups.Length; i++)
		{
			HeadstartPickups[i].SetActive(true);
			HeadstartPickups[i].transform.position = new Vector3(-20 + 20 * i, y, z2);
			int num = -1;
			while (num == array[0] || num == array[1] || num == array[2])
			{
				num = UnityEngine.Random.Range(0, HeadstartPickups.Length);
				if (num != array[0] && num != array[1] && num != array[2])
				{
					array[i] = num;
					break;
				}
			}
			for (int j = 0; j < pickupLists[i].Length; j++)
			{
				if (j != num)
				{
					pickupLists[i][j].Deactivate();
				}
			}
			pickupLists[i][num].Activate();
		}
	}

	private void ResetPickups()
	{
		int i = 0;
		for (int num = HeadstartPickups.Length; i < num; i++)
		{
			HeadstartPickups[i].SetActive(false);
		}
	}

	public void ResetTurboHeadstart()
	{
		activateHeadstart = false;
	}
}
