using System.Collections;
using UnityEngine;

public class SaveMePopup : UIBaseScreen
{
	public SaveMeAnimateClock saveMeAnimateClock;

	[SerializeField]
	private UILabel titleLabel;

	public Transform WatchVideoBtn;

	public Transform UseKeysBtn;

	public Transform FreeBtn;

	private const float ANIMATION_DURATION = 0f;

	private float timeLeft;

	private Vector3 watchVideoOriginePos;

	private Vector3 useKeyOriginePos;

	private void Awake()
	{
		titleLabel.text = Strings.Get(LanguageKey.SAVE_ME_POPUP_BUTTON_LABEL);
		watchVideoOriginePos = WatchVideoBtn.localPosition;
		useKeyOriginePos = UseKeysBtn.localPosition;
	}

	public float getAnimationDuration()
	{
		return 0f;
	}

	public float getAnimationTimeLeft()
	{
		return timeLeft;
	}

	private void OnEnable()
	{
		StartCoroutine(startClockAnimation());
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = true;
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(false);
		}
		if (GameStats.Instance.reviveCount > 0)
		{
			WatchVideoBtn.gameObject.SetActive(false);
			UseKeysBtn.gameObject.SetActive(false);
			FreeBtn.gameObject.SetActive(true);
			GameStats.Instance.reviveCount--;
		}
		else if (!RiseSdk.Instance.HasRewardAd() || !UIScreenController.Instance.CheckNetwork())
		{
			WatchVideoBtn.gameObject.SetActive(false);
			FreeBtn.gameObject.SetActive(false);
			UseKeysBtn.gameObject.SetActive(true);
			UseKeysBtn.localPosition = new Vector3(0f, useKeyOriginePos.y, 0f);
		}
		else if (SaveMeManager._numberOfUsedKeysInCurrentRun == 2)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_saveme", 0);
			UseKeysBtn.gameObject.SetActive(false);
			FreeBtn.gameObject.SetActive(false);
			WatchVideoBtn.gameObject.SetActive(true);
			WatchVideoBtn.localPosition = new Vector3(0f, watchVideoOriginePos.y, 0f);
		}
		else
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_saveme", 0);
			FreeBtn.gameObject.SetActive(false);
			WatchVideoBtn.gameObject.SetActive(true);
			UseKeysBtn.gameObject.SetActive(true);
			WatchVideoBtn.localPosition = watchVideoOriginePos;
			UseKeysBtn.localPosition = useKeyOriginePos;
		}
	}

	private IEnumerator startClockAnimation()
	{
		timeLeft = 0f;
		while (timeLeft > -0.2f)
		{
			float spriteAmount = Mathf.Clamp01(timeLeft / 10f);
			saveMeAnimateClock.FillSpriteAmount(spriteAmount);
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		UIScreenController.Instance.ClosePopup(null);
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		Time.timeScale = 0f;
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		Time.timeScale = 0f;
	}
}
