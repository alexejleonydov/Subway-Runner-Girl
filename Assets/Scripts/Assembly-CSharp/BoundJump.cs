using System.Collections;
using UnityEngine;

public class BoundJump : CharacterState
{
	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();

	[HideInInspector]
	public bool isActive;

	[SerializeField]
	private float characterChangeTrackLength = 60f;

	[SerializeField]
	private float fadeInPosition = 0.1f;

	[SerializeField]
	private float yOffset = 1f;

	private float jumpHeight = 95f;

	private float jumpDistance = 800f;

	private float totalDistance = 800f;

	private bool TransitionDownFirstUpdate;

	private Character character;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private CharacterController characterController;

	private Transform characterTransform;

	private CoinLineManager coinLineManager;

	private Game game;

	private static BoundJump instance;

	private TrackController trackController;

	public OnStartDelegate OnStart;

	public OnStopDelegate OnStop;

	private Vector3 startPosition;

	public static BoundJump Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(BoundJump)) as BoundJump;
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

	public void Awake()
	{
		game = Game.Instance;
		trackController = TrackController.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		characterTransform = characterController.transform;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		coinLineManager = CoinLineManager.Instance;
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		GameStats.Instance.pickedUpPowerups++;
		coinLineManager.ToggleLines(true);
		character.characterModel.HideBlobShadow();
		game.ResetEnemy();
		character.inAirJump = false;
		character.IsGrounded.Value = false;
		game.Attachment.PauseInFlypackMode();
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		TransitionDownFirstUpdate = true;
		NotifyOnStart();
		float normalizedPosition = ObliqueMotion.CalcTB((characterTransform.position.y - startPosition.y) / jumpHeight);
		startPosition.z = characterTransform.position.z - jumpDistance * normalizedPosition;
		Vector3 endPosition = startPosition + Vector3.forward * totalDistance;
		float speed = game.currentSpeed;
		float progress3 = 0f;
		characterCamera.SetStartPositionY(characterCameraTransform.position.y);
		characterCamera.SetCameraTransition(CameraFollowMode.SpringUp);
		while (character.z < endPosition.z)
		{
			game.HandleControls();
			character.z += speed * Time.deltaTime;
			normalizedPosition = (character.z - startPosition.z) / jumpDistance;
			Vector3 pivot = trackController.GetPosition(character.x, character.z) + Vector3.up * (ObliqueMotion.CalcHeight(normalizedPosition) * jumpHeight + startPosition.y + yOffset);
			if (normalizedPosition <= fadeInPosition)
			{
				pivot.y = Mathf.Lerp(startPosition.y + yOffset, pivot.y, normalizedPosition / fadeInPosition);
			}
			characterTransform.position = pivot;
			progress3 = normalizedPosition * 2f;
			characterCamera.CurrentSpringProgress = progress3;
			if (normalizedPosition > 0.5f && !TransitionDownFirstUpdate)
			{
				progress3 = Mathf.Clamp01((normalizedPosition * 2f - 1f) * (jumpDistance * 0.5f / (totalDistance - jumpDistance * 0.5f)));
				characterCamera.CurrentSpringProgress = progress3;
			}
			if (normalizedPosition > 0.5f && TransitionDownFirstUpdate)
			{
				TransitionDownFirstUpdate = false;
			}
			characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
		}
		isActive = false;
		NotifyOnStop();
		game.ChangeState(game.Running);
		character.verticalSpeed = Mathf.Min(0f - character.CalculateJumpVerticalSpeed(ObliqueMotion.CalcHeight(totalDistance / jumpDistance) * jumpHeight + yOffset), character.verticalFallSpeedLimit) - 1f;
		character.IsFalling = true;
		game.Attachment.Resume();
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

	public void SetBoundJumpData(float jumpHeight, float jumpDistance, float totalDistance, Vector3 position)
	{
		this.jumpHeight = jumpHeight;
		this.jumpDistance = jumpDistance;
		this.totalDistance = totalDistance;
		startPosition = position;
	}
}
