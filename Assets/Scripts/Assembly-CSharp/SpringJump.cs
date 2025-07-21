using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SpringJump : CharacterState
{
	[Serializable]
	public class MovementCurve
	{
		public AnimationCurve curve;
	}

	public delegate void OnReachedTopDelegate();

	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();

	[Serializable]
	public class pickupWeight
	{
		public string pickupName;

		public PropType type;
	}

	[SerializeField]
	private float jumpHeight = 95f;

	[SerializeField]
	private MovementCurve jumpCurve;

	[SerializeField]
	private float jumpDistance = 800f;

	[SerializeField]
	private float characterChangeTrackLength = 60f;

	[SerializeField]
	private float fadeInPosition = 0.1f;

	[SerializeField]
	private float hangtimePosition = 0.5f;

	[HideInInspector]
	public JumpCoinsManager coinsManager;

	public bool isActive;

	[HideInInspector]
	public PropType powerType = PropType.springJump;

	private ActiveProp Powerup;

	[SerializeField]
	private int rows = 5;

	[SerializeField]
	private int startRowPosition = 1;

	[SerializeField]
	private int endRowPosition = 1;

	[SerializeField]
	private GameObject mysteryBoxPrefab;

	[SerializeField]
	private pickupWeight[] weightList;

	public OnStartDelegate OnStart;

	public OnStartDelegate OnHangtime;

	public OnStopDelegate OnStop;

	private Character character;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private CharacterController characterController;

	private Transform characterTransform;

	private CoinLineManager coinLineManager;

	private Game game;

	private static SpringJump instance;

	private IPickup[] list;

	private bool reachedHangtime;

	private TrackController trackController;

	private IPickup[] weightedList;

	private bool willShowPickup = true;

	public static SpringJump Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(SpringJump)) as SpringJump;
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

	public bool WillShowPickup
	{
		set
		{
			willShowPickup = value;
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
		characterCameraTransform = characterCamera.transform;
		coinsManager = JumpCoinsManager.Instance;
		coinLineManager = CoinLineManager.Instance;
		if (!(mysteryBoxPrefab != null))
		{
			return;
		}
		mysteryBoxPrefab = UnityEngine.Object.Instantiate(mysteryBoxPrefab, new Vector3(0f, -200f, 0f), Quaternion.identity);
		list = mysteryBoxPrefab.GetComponentsInChildren<IPickup>();
		int num = 0;
		int i = 0;
		for (int num2 = weightList.Length; i < num2; i++)
		{
			num += Upgrades.upgrades[weightList[i].type].spawnProbability;
		}
		weightedList = new IPickup[num];
		int num3 = 0;
		for (int j = 0; j < weightList.Length; j++)
		{
			for (int k = 0; k < list.Length; k++)
			{
				if (!(weightList[j].pickupName != list[k].gameObject.name))
				{
					int l = 0;
					for (Upgrade upgrade = Upgrades.upgrades[weightList[j].type]; l < upgrade.spawnProbability; l++)
					{
						weightedList[num3] = list[k];
						num3++;
					}
				}
			}
		}
		mysteryBoxPrefab.SetActive(false);
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		coinLineManager.ToggleLines(true);
		character.characterModel.HideBlobShadow();
		game.ResetEnemy();
		character.inAirJump = false;
		character.IsGrounded.Value = false;
		GameStats.Instance.pickedUpPowerups++;
		game.Attachment.PauseInFlypackMode();
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterController.detectCollisions = false;
		NotifyOnStart();
		float coinOffsetZPosition = jumpDistance / (float)rows;
		for (int i = 0; i < endRowPosition; i++)
		{
			if (i > startRowPosition)
			{
				Vector3 vector = new Vector3(0f, jumpCurve.curve.Evaluate((float)i / (float)rows) * jumpHeight, character.z + (float)i * coinOffsetZPosition);
				coinsManager.placeRow(vector.z, vector.y);
			}
		}
		int randomX = UnityEngine.Random.Range(0, 3);
		float xPosition2 = 0f;
		switch (randomX)
		{
		case 0:
			xPosition2 = -20f;
			break;
		case 1:
			xPosition2 = 0f;
			break;
		default:
			xPosition2 = 20f;
			break;
		}
		Vector3 startPosition = characterTransform.position;
		Vector3 endPosition = startPosition + Vector3.forward * jumpDistance;
		StartCoroutine(RemovePickups(endPosition.z));
		if (mysteryBoxPrefab != null && willShowPickup)
		{
			mysteryBoxPrefab.transform.position = new Vector3(xPosition2, jumpCurve.curve.Evaluate(1f) * jumpHeight, endPosition.z + 20f);
			mysteryBoxPrefab.SetActive(true);
			int index = UnityEngine.Random.Range(0, weightedList.Length);
			IPickup pickup = weightedList.ElementAt(index);
			for (int j = 0; j < list.Length; j++)
			{
				IPickup pickup2 = list[j];
				pickup2.Deactivate();
			}
			pickup.Activate();
		}
		float speed = game.currentSpeed;
		characterCamera.SetCameraTransition(CameraFollowMode.SpringUp);
		characterCamera.SetStartPositionY(characterCameraTransform.position.y);
		while (character.z < endPosition.z)
		{
			game.HandleControls();
			character.z += speed * Time.deltaTime;
			float normalizedPosition = (character.z - startPosition.z) / jumpDistance;
			Vector3 pivot = trackController.GetPosition(character.x, character.z) + Vector3.up * jumpCurve.curve.Evaluate(normalizedPosition) * jumpHeight;
			if (normalizedPosition <= fadeInPosition)
			{
				pivot.y = Mathf.Lerp(startPosition.y, pivot.y, normalizedPosition / fadeInPosition);
			}
			if (normalizedPosition > hangtimePosition && !reachedHangtime)
			{
				NotifyHangtime();
				reachedHangtime = true;
			}
			characterTransform.position = pivot;
			characterCamera.CurrentSpringProgress = normalizedPosition;
			characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, false);
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
		}
		EndSpring();
	}

	private void EndSpring()
	{
		isActive = false;
		NotifyOnStop();
		characterController.detectCollisions = true;
		Running.Instance.transitionFromPogostick = true;
		game.ChangeState(game.Running);
		character.verticalSpeed = character.CalculateJumpVerticalSpeed(0f);
		game.Attachment.Resume();
		reachedHangtime = false;
	}

	private void HandleOnStageMenu()
	{
		if (mysteryBoxPrefab != null)
		{
			mysteryBoxPrefab.SetActive(false);
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Down:
			EndSpring();
			character.Roll();
			break;
		case SwipeDir.Left:
			character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
			break;
		case SwipeDir.Right:
			character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
			break;
		}
	}

	private void NotifyHangtime()
	{
		if (OnHangtime != null)
		{
			OnHangtime();
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

	private IEnumerator RemovePickups(float endPosition)
	{
		while (character.z < endPosition + 100f)
		{
			yield return null;
		}
		coinsManager.ReleaseCoins();
		if (mysteryBoxPrefab != null)
		{
			mysteryBoxPrefab.SetActive(false);
		}
		ResetPickup();
	}

	private void ResetPickup()
	{
		int i = 0;
		for (int num = list.Length; i < num; i++)
		{
			list[i].gameObject.SetActive(true);
		}
	}

	public void Stop()
	{
		isActive = false;
		NotifyOnStop();
	}
}
