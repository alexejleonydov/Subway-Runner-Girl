using System;
using System.Collections;
using UnityEngine;

public class SpeedUp : CharacterState
{
	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();

	public bool isActive;

	public float characterChangeTrackLength = 30f;

	public AnimationCurve speedCurve;

	public float maxSpeed;

	public float speedupAheadDuration;

	public float totalDuration;

	public float overdriveDuration;

	private float extraSpeed;

	public OnStartDelegate OnStart;

	public OnStartDelegate OnHangtime;

	public OnStopDelegate OnStop;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private Transform characterTransform;

	private TrackController trackController;

	private Game game;

	private static SpeedUp instance;

	public static SpeedUp Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(SpeedUp)) as SpeedUp;
			}
			return instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return false;
		}
	}

	public void Awake()
	{
		game = Game.Instance;
		game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(HandleOnStageMenu));
		trackController = TrackController.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		characterTransform = characterController.transform;
		characterCamera = CharacterCamera.Instance;
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		character.characterModel.HideBlobShadow();
		character.inAirJump = false;
		character.IsGrounded.Value = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		NotifyOnStart();
		extraSpeed = maxSpeed;
		float speed2 = game.currentSpeed + extraSpeed;
		characterCamera.SetCameraTransition(CameraFollowMode.SpeedUp, speedupAheadDuration);
		float t = 0f;
		while (character.IsInspeedup || t < speedupAheadDuration)
		{
			game.HandleControls();
			character.z += speed2 * Time.deltaTime;
			Vector3 pivot2 = trackController.GetPosition(character.x, character.z);
			characterTransform.position = pivot2;
			characterCamera.UpdatePosition(characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			t += Time.deltaTime;
			yield return null;
		}
		characterCamera.SetCameraTransition(CameraFollowMode.SpeedDown, totalDuration - speedupAheadDuration);
		while (t < totalDuration)
		{
			float ratio = t / totalDuration;
			extraSpeed = speedCurve.Evaluate(ratio) * maxSpeed;
			speed2 = game.currentSpeed + extraSpeed;
			game.HandleControls();
			character.z += speed2 * Time.deltaTime;
			Vector3 pivot = trackController.GetPosition(character.x, character.z);
			characterTransform.position = pivot;
			characterCamera.UpdatePosition(characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			t += Time.deltaTime;
			yield return null;
		}
		isActive = false;
		NotifyOnStop();
		game.ChangeState(game.Running);
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Left:
			if (!character.forceToOneway)
			{
				character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
			}
			break;
		case SwipeDir.Right:
			if (!character.forceToOneway)
			{
				character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
			}
			break;
		}
	}

	private void HandleOnStageMenu()
	{
		NotifyOnStop();
	}

	private void NotifyOnStart()
	{
		if (OnStart != null)
		{
			OnStart();
		}
	}

	private void NotifyOnStop()
	{
		if (OnStop != null)
		{
			OnStop();
		}
	}
}
