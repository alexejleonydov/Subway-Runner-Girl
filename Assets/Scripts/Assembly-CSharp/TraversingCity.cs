using System.Collections;
using UnityEngine;

public class TraversingCity : CharacterState
{
	[SerializeField]
	private Running.RunPositions currentRunPosition;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private CharacterController characterController;

	private Game game;

	private CharacterCamera characterCamera;

	[SerializeField]
	private CameraCulling cameraCulling;

	private TrackController trackController;

	private Vector3 position;

	private AsyncOperation ao;

	private float progress;

	private string nextCitySceneName;

	private static TraversingCity instance;

	public static TraversingCity Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(TraversingCity)) as TraversingCity;
			}
			return instance;
		}
	}

	private void Awake()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		game = Game.Instance;
		characterCamera = CharacterCamera.Instance;
		trackController = TrackController.Instance;
		progress = 0f;
		ao = null;
	}

	public override IEnumerator Begin()
	{
		progress = 0f;
		ao = null;
		FadeData fadeData = cameraCulling.SetToAndThenBack();
		float factor5 = 0f;
		while (factor5 < fadeData.fadeInDuration)
		{
			factor5 += Time.deltaTime;
			yield return null;
		}
		factor5 = 0f;
		trackController.ChangeToNextCity(true);
		trackController.SetCharacterPosition(character);
		position = characterTransform.position;
		factor5 += Time.deltaTime;
		yield return null;
		characterCamera.Reset(position, Quaternion.identity, true);
		factor5 += Time.deltaTime;
		yield return null;
		game.LayTrackChunks();
		characterRendering.ActivateSnow(trackController.GetCurrentCity().allowSnow);
		factor5 += Time.deltaTime;
		yield return null;
		while (factor5 < fadeData.onDuration - 0.1f)
		{
			factor5 += Time.deltaTime;
			yield return null;
		}
		game.ChangeState(game.Running);
	}
}
