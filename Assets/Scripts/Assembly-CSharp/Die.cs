using System.Collections;
using UnityEngine;

public class Die : CharacterState
{
	private Game game;

	private Character character;

	private CharacterCamera characterCamera;

	[SerializeField]
	private float waitTimeBeforeScreen = 3f;

	private static Die instance;

	private bool skipRevive;

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	public bool SkipRevive
	{
		set
		{
			skipRevive = true;
		}
	}

	public static Die Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(Die)) as Die;
			}
			return instance;
		}
	}

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterCamera = CharacterCamera.Instance;
	}

	public override IEnumerator Begin()
	{
		float time3 = 0f;
		skipRevive = false;
		if (game.HitType != Character.CriticalHitType.FallIntoWater)
		{
			while (!character.characterController.isGrounded && time3 < 1f)
			{
				character.MoveWithGravity();
				time3 += Time.deltaTime;
				characterCamera.UpdatePosition(character.transform.position, Quaternion.identity, Time.deltaTime, true);
				yield return null;
			}
		}
		else
		{
			while (time3 < 0.5f)
			{
				time3 += Time.deltaTime;
				characterCamera.UpdatePosition(character.transform.position, Quaternion.identity, Time.deltaTime, true);
				yield return null;
			}
		}
		time3 = 0f;
		while (time3 < 0.5f)
		{
			time3 += Time.deltaTime;
			characterCamera.UpdatePosition(character.transform.position, Quaternion.identity, Time.deltaTime, true);
			yield return null;
		}
		bool isShowHelpMe = false;  //true;
		if (isShowHelpMe)
		{
			UIScreenController.Instance.QueuePopup("SaveMePopup");
		}
		time3 = 0f;
		while (time3 < waitTimeBeforeScreen && !Input.GetMouseButtonUp(0))
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			if (skipRevive)
			{
				break;
			}
			time3 += Time.deltaTime;
			yield return null;
		}
		if (isShowHelpMe)
		{
			while (!skipRevive)
			{
				yield return null;
			}
		}
		StartCoroutine("DelayGameOverScreen");
		game.NormalPlayerRunDuration();
	}

	private IEnumerator DelayGameOverScreen()
	{
		if (!PlayerInfo.Instance.hasRemoveAd && PlayerPrefs.GetInt("IsNewPlayerStatus") == 1 && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.TrackEvent("interstitial_endless", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_endless", 0);
			RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
			Game.Instance.closePopupOnAdEvent = false;
			Game.Instance.lastShowAd = "show_interstitial__endless";
			if (RiseSdk.Instance.GetRemoteConfigInt("Show_video_inter_loading_config") == 1)
			{
				VideoLoadingPopup.adType = 1;
				VideoLoadingPopup.rewardId = 1;
				UIScreenController.Instance.PushPopup("VideoLoadingPopup");
			}
			else
			{
				RiseSdk.Instance.ShowAd("custom");
			}
			game.NewPlayerRunDuration();
			PlayerPrefs.SetInt("IsNewPlayerStatus", 2);
		}
		else if (Game.Instance.GetDuration() > 15f && !PlayerInfo.Instance.hasRemoveAd && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.TrackEvent("interstitial_endless", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_endless", 0);
			RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
			Game.Instance.lastShowAd = "show_interstitial__endless";
			Game.Instance.closePopupOnAdEvent = false;
			if (RiseSdk.Instance.GetRemoteConfigInt("Show_video_inter_loading_config") == 1)
			{
				VideoLoadingPopup.adType = 1;
				VideoLoadingPopup.rewardId = 1;
				UIScreenController.Instance.PushPopup("VideoLoadingPopup");
			}
			else
			{
				RiseSdk.Instance.ShowAd("custom");
			}
		}
		yield return null;
		UIScreenController.Instance.GameOverTriggered();
		yield return null;
		game.StartTopMenu();
	}
}
