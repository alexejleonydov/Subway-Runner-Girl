using System.Collections;
using UnityEngine;

public class WallWalking : CharacterState
{
	public delegate void OnStartWallDelegate(SwipeDir dir);

	public delegate void OnLeaveWallDelegate(SwipeDir dir);

	public delegate void OnJumpAheadStartDelegate(SwipeDir dir);

	public delegate void OnJumpAheadEndDelegate(SwipeDir dir);

	private Character character;

	private CharacterCamera characterCamera;

	private Game game;

	private TrackController trackController;

	[SerializeField]
	private float deltaX = 7.2f;

	private float jumpHeight;

	private SwipeDir lastWallDir;

	private float endZ;

	[SerializeField]
	private float aheadDuration;

	[SerializeField]
	private AnimationCurve jumpAC;

	[SerializeField]
	private float characterChangeTrackLength = 30f;

	private static WallWalking instance;

	public OnStartWallDelegate OnStartWall;

	public OnLeaveWallDelegate OnLeaveWall;

	public OnJumpAheadStartDelegate OnJumpAheadStart;

	public OnJumpAheadEndDelegate OnJumpAheadEnd;

	public static WallWalking Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(WallWalking)) as WallWalking;
			}
			return instance;
		}
	}

	public bool IsActive { get; private set; }

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterCamera = CharacterCamera.Instance;
		trackController = TrackController.Instance;
	}

	public override IEnumerator Begin()
	{
		game.ResetEnemy();
		character.inAirJump = false;
		character.IsGrounded.Value = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		character.characterController.detectCollisions = false;
		IsActive = true;
		game.Attachment.PauseInFlypackMode();
		if (OnStartWall != null)
		{
			OnStartWall(lastWallDir);
		}
		Vector3 startPosition = character.transform.position;
		float speed = game.currentLevelSpeed;
		float startX = character.x;
		float endX = trackController.GetTrackX(character.TrackIndexTarget) + (float)((lastWallDir != SwipeDir.Left) ? 1 : (-1)) * deltaX;
		float trackIndexPositionBegin = character.trackIndexPosition;
		float newTrackIndexPosition = trackIndexPositionBegin + (float)((lastWallDir != SwipeDir.Left) ? 1 : (-1)) * deltaX / 20f;
		float time = 0f;
		float normalizedPosition2 = 0f;
		characterCamera.SetCameraTransition(CameraFollowMode.WallUp, aheadDuration);
		if (OnJumpAheadStart != null)
		{
			OnJumpAheadStart(lastWallDir);
		}
		while (time < aheadDuration)
		{
			normalizedPosition2 = time / aheadDuration;
			character.x = Mathf.Lerp(startX, endX, normalizedPosition2);
			character.trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndexPosition, normalizedPosition2);
			character.z += speed * Time.deltaTime;
			Vector3 pivot2 = trackController.GetPosition(character.x, character.z) + Vector3.up * (startPosition.y + jumpAC.Evaluate(normalizedPosition2) * jumpHeight);
			character.transform.position = pivot2;
			characterCamera.UpdatePosition(pivot2, Quaternion.identity, Time.deltaTime, true);
			time += Time.deltaTime;
			game.UpdateMeters();
			yield return null;
		}
		if (OnJumpAheadEnd != null)
		{
			OnJumpAheadEnd(lastWallDir);
		}
		while (character.z < endZ)
		{
			game.HandleControls();
			character.z += speed * Time.deltaTime;
			Vector3 pivot2 = trackController.GetPosition(endX, character.z) + Vector3.up * (startPosition.y + jumpHeight);
			character.transform.position = pivot2;
			characterCamera.UpdatePosition(pivot2, Quaternion.identity, Time.deltaTime, true);
			game.UpdateMeters();
			yield return null;
		}
		EndWallWalking();
		character.ForceChangeTrack(0, aheadDuration * 2f, true);
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Up:
			EndWallWalking();
			character.ForceChangeTrack(0, 0.5f, true);
			character.Jump();
			break;
		case SwipeDir.Down:
			EndWallWalking();
			character.ForceChangeTrack(0, 0.05f, true);
			character.Roll();
			break;
		case SwipeDir.Left:
			if (lastWallDir == SwipeDir.Right && character.trackIndex != 0)
			{
				EndWallWalking();
				character.Jump();
				character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
			}
			break;
		case SwipeDir.Right:
			if (lastWallDir == SwipeDir.Left && character.trackIndex != 2)
			{
				EndWallWalking();
				character.Jump();
				character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
			}
			break;
		}
	}

	public void EndWallWalking()
	{
		character.characterController.detectCollisions = true;
		IsActive = false;
		game.Attachment.Resume();
		if (OnLeaveWall != null)
		{
			OnLeaveWall(lastWallDir);
		}
		characterCamera.SetCameraTransition(CameraFollowMode.WallDown, aheadDuration);
		character.verticalSpeed = character.CalculateJumpVerticalSpeed(0f);
		character.characterController.Move(Vector3.up * 0.1f);
		game.ChangeState(game.Running);
		endZ = -1f;
		character.WallEndWithSwipeDir(lastWallDir);
	}

	public bool CanNotLeaveWall(SwipeDir dir)
	{
		return dir == lastWallDir;
	}

	public void SetData(SwipeDir dir, float height, float endZ)
	{
		lastWallDir = dir;
		jumpHeight = height;
		this.endZ = endZ;
	}
}
