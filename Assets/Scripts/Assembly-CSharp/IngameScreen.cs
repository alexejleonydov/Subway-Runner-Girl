using System;
using System.Collections;
using UnityEngine;

public class IngameScreen : UIBaseScreen
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UILabel sliderLbl;

	[SerializeField]
	private UISprite sliderIcon;

	[SerializeField]
	private UILabel scoreLabel;

	public UILabel multiplierLabel;

	[SerializeField]
	private UILabel coinLabel;

	[SerializeField]
	private Transform _multiplier;

	[SerializeField]
	private SlideinPowerupHelper slideinPowerupHelper;

	[SerializeField]
	private IngameChestPickedHelper chestPickedHelper;

	[SerializeField]
	private UILabel countdownStartingLabel;

	[SerializeField]
	private UILabel countdownLabel;

	public Color SCOREBOOSTER_ACTIVE_COLOR = new Color(0.7490196f, 0.9137255f, 0.9921569f);

	public Color MULTIPLIER_LABEL_ORIGINAL_COLOR = new Color(1f, 0.8588235f, 0f);

	[SerializeField]
	private GameObject pauseButton;

	[SerializeField]
	private UIAnchor topLeft;

	[SerializeField]
	private UIAnchor topRight;

	[SerializeField]
	private HelmetButtonHelp helmetBtnHelp;

	private TrialInfo info;

	private Vector3 _cachedCountdownLabelScale = Vector3.zero;

	private float _countdownSeconds;

	private bool _countingDown;

	private bool _isMultiplierLabelUpdated;

	private bool start;

	private int digits;

	private bool doLastIteration;

	private int lastKnowDigits;

	private float lerpFactor;

	private bool mShowBanner;

	private float mActual;

	private float mTimeDelta;

	private float mTimeStart;

	private bool mTimeStarted;

	private int score;

	private Color SCOREBOOSTER_ELECTRICBLUE = new Color(0f, 52f / 85f, 1f);

	private Color SCOREBOOSTER_WHITE = new Color(1f, 1f, 1f);

	private int progress;

	private float value;

	private bool trialCharge = true;

	private int extraNumberOfChars
	{
		get
		{
			return Mathf.Clamp(digits - 6, 0, 28);
		}
	}

	public IngameScreen()
	{
		score = -1;
	}

	private IEnumerator AnimateColor(UISprite sprite, Color32 startValue, Color32 endValue, float speedFactor)
	{
		yield return new WaitForSeconds(0.2f);
		float Factor = 0f;
		while (Factor < 1f)
		{
			Factor += RealTimeTracker.deltaTime * speedFactor;
			sprite.color = Color.Lerp(startValue, endValue, Factor);
			yield return null;
		}
	}

	private IEnumerator AnimateSize(UISprite sprite, int startHeightValue, int endHeightValue, int startWidthValue, int endWidthValue, float speedFactor)
	{
		float Factor = 0f;
		while (Factor < 1f)
		{
			Factor += RealTimeTracker.deltaTime * speedFactor;
			sprite.width = Mathf.CeilToInt(Mathf.Lerp(startWidthValue, endWidthValue, Factor));
			sprite.height = Mathf.CeilToInt(Mathf.Lerp(startHeightValue, endHeightValue, Factor));
			yield return null;
		}
	}

	private void FadeFromWhiteToDarkBlue()
	{
		multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ELECTRICBLUE, lerpFactor);
		if (multiplierLabel.text.Contains(string.Empty + PlayerInfo.Instance.scoreMultiplier) && doLastIteration)
		{
			multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ACTIVE_COLOR, lerpFactor);
		}
	}

	public override void Hide()
	{
		resetMultiplierLabel();
		_countingDown = false;
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
		if (mShowBanner)
		{
			RiseSdk.Instance.CloseBanner();
		}
		base.Hide();
	}

	public override void Init()
	{
		base.Init();
		resetMultiplierLabel();
		scoreLabel.text = GameStats.Instance.score.ToString();
		countdownStartingLabel.text = string.Empty;
		countdownLabel.text = string.Empty;
		helmetBtnHelp.Init();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onScoreMultiplierChanged = (Action)Delegate.Combine(instance.onScoreMultiplierChanged, new Action(resetMultiplierLabel));
		GameStats instance2 = GameStats.Instance;
		instance2.OnCoinsChanged = (Action)Delegate.Combine(instance2.OnCoinsChanged, new Action(OnCoinsChanged));
		instance2.OnCoinsWithHelmetChanged = (Action)Delegate.Combine(instance2.OnCoinsWithHelmetChanged, new Action(OnCoinsWithHelmetChanged));
		instance2.OnChestsChanged = (Action)Delegate.Combine(instance2.OnChestsChanged, new Action(OnChestsChanged));
		Game.Instance.OnGameStarted = (Action)Delegate.Combine(Game.Instance.OnGameStarted, new Action(OnGameStarted));
	}

	private void OnCoinsChanged()
	{
		coinLabel.text = GameStats.Instance.coins.ToString();
		ResizeCoinBox();
	}

	private void OnCoinsWithHelmetChanged()
	{
		if (!trialCharge && TrialManager.Instance.IsTestHelm)
		{
			progress = GameStats.Instance.coinsWithHelmet + PlayerInfo.Instance.CurrentTrialInfoProgress();
			value = (float)progress / (float)info.taskAim;
			if (value >= 1f)
			{
				slider.value = 1f;
				sliderLbl.text = info.taskAim + "/" + info.taskAim;
				slider.gameObject.SetActive(false);
				UISliderInController.Instance.OnTrialFinished();
				trialCharge = true;
			}
			else
			{
				slider.value = value;
				sliderLbl.text = progress + "/" + info.taskAim;
			}
		}
	}

	private void OnChestsChanged()
	{
		chestPickedHelper.Show();
	}

	private void OnEnable()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
	}

	private void OnGameStarted()
	{
		panel.alpha = 1f;
		start = true;
		lastKnowDigits = 0;
		digits = 0;
		Game instance = Game.Instance;
		if (!instance.isReadyForSlideinPowerups)
		{
			slideinPowerupHelper.HidePowerups(true);
		}
		trialCharge = true;
		if (TrialManager.Instance.IsInTest())
		{
			info = TrialManager.Instance.currentTrialInfo;
			if (info != null)
			{
				int num = PlayerInfo.Instance.CurrentTrialInfoProgress();
				if (info.type == TrialType.Character)
				{
					trialCharge = num >= info.taskAim * 1000;
					slider.value = (float)num * 0.001f / (float)info.taskAim;
					sliderLbl.text = num / 1000 + "/" + info.taskAim;
				}
				else if (info.type == TrialType.Helmet)
				{
					trialCharge = num >= info.taskAim;
					slider.value = (float)num / (float)info.taskAim;
					sliderLbl.text = num + "/" + info.taskAim;
				}
				slider.gameObject.SetActive(true);
				sliderIcon.spriteName = info.icon;
			}
		}
		else
		{
			slider.gameObject.SetActive(false);
			info = null;
		}
		if ((TrialManager.Instance.IsTestHelm || Game.Instance.IsTestHelm) && !instance.Attachment.IsActive(instance.Attachment.Helmet))
		{
			instance.Attachment.Add(instance.Attachment.Helmet);
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet);
		}
		if (Application.internetReachability != 0 && UIScreenController.Instance.curDeviceType != UIScreenController.DeviceType.iPhoneX && !PlayerInfo.Instance.hasRemoveAd)
		{
			topLeft.pixelOffset = new Vector2(0f, 0f - UIScreenController.Instance.bannerHeight);
			topRight.pixelOffset = new Vector2(0f, 0f - UIScreenController.Instance.bannerHeight);
			RiseSdk.Instance.ShowBanner(3);
			mShowBanner = true;
			return;
		}
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			topLeft.pixelOffset = new Vector2(0f, -132f);
			topRight.pixelOffset = new Vector2(0f, -132f);
		}
		else
		{
			topLeft.pixelOffset = Vector3.zero;
			topRight.pixelOffset = Vector3.zero;
		}
		mShowBanner = false;
	}

	private void resetMultiplierLabel()
	{
		if (GameStats.Instance.scoreBooster5Activated && !PlayerInfo.Instance.doubleScore)
		{
			int num = int.Parse(multiplierLabel.text.Substring(multiplierLabel.text.IndexOf("x") + 1));
			if (num < PlayerInfo.Instance.scoreMultiplier)
			{
				int num2 = num + 1;
				_isMultiplierLabelUpdated = false;
				doLastIteration = false;
				multiplierLabel.text = "x" + num2;
				return;
			}
			multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
			if (doLastIteration)
			{
				_isMultiplierLabelUpdated = true;
			}
			doLastIteration = true;
		}
		else
		{
			multiplierLabel.text = "x" + PlayerInfo.Instance.scoreMultiplier;
		}
	}

	public void ResetMultiplierLabelColour()
	{
		if (GameStats.Instance.scoreBooster5Activated)
		{
			multiplierLabel.color = SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (multiplierLabel.color != MULTIPLIER_LABEL_ORIGINAL_COLOR)
		{
			multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
		}
	}

	private void ResizeCoinBox()
	{
		int length = coinLabel.text.Length;
		float num = 64f;
		if (length > 1)
		{
			num += (float)(12 * (length - 1));
		}
	}

	private void ResizeMultiplierBox()
	{
		int length = multiplierLabel.text.Length;
		float num = 50f;
		if (length > 2)
		{
			num += (float)(10 * (length - 2));
		}
		if (_multiplier.transform.localScale.x != num)
		{
			_multiplier.transform.localScale = new Vector3(num, _multiplier.transform.localScale.y, _multiplier.transform.localScale.z);
		}
	}

	public void SetPauseButtonVisibility(bool isActive)
	{
		pauseButton.SetActive(isActive);
	}

	private void SetScoreLabel()
	{
		score = GameStats.Instance.score;
		digits = Utility.NumberOfDigits(score);
		string text;
		switch (digits)
		{
		case 1:
			text = "00000";
			break;
		case 2:
			text = "0000";
			break;
		case 3:
			text = "000";
			break;
		case 4:
			text = "00";
			break;
		case 5:
			text = "0";
			break;
		default:
			text = string.Empty;
			break;
		}
		scoreLabel.text = text + score;
		if (lastKnowDigits != digits)
		{
			lastKnowDigits = digits;
		}
	}

	public void CountDown()
	{
		_countdownSeconds = 3f;
		_countingDown = true;
		mTimeStarted = false;
		Game.Instance.TriggerPause(true);
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance == null)
		{
			Debug.LogError("You must be running the wrong scene");
			return;
		}
		chestPickedHelper.Hide();
		if (Game.Instance.isPaused && UIScreenController.Instance.GameIsFocused)
		{
			_countdownSeconds = 3f;
			_countingDown = true;
			if (mShowBanner)
			{
				RiseSdk.Instance.ShowBanner(3);
			}
		}
		if (!_countingDown)
		{
			TasksManager.Instance.inRun = true;
			if (multiplierLabel.color != MULTIPLIER_LABEL_ORIGINAL_COLOR)
			{
				multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
			}
			panel.alpha = 0f;
			start = false;
		}
		SetPauseButtonVisibility(true);
	}

	public override void GainFocus()
	{
		base.GainFocus();
		if (mShowBanner)
		{
			RiseSdk.Instance.ShowBanner(3);
		}
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		if (mShowBanner)
		{
			RiseSdk.Instance.CloseBanner();
		}
	}

	private void Update()
	{
		UpdateMultiplierLable();
		if (!start)
		{
			return;
		}
		GameStats.Instance.CalculateScore();
		if (score != GameStats.Instance.score)
		{
			SetScoreLabel();
		}
		if (Game.Instance.isReadyForSlideinPowerups && !Game.Instance.trackController.IsRunningOnTutorialTrack)
		{
			Game.Instance.isReadyForSlideinPowerups = false;
			helmetBtnHelp.Show();
			slideinPowerupHelper.ShowPowerups();
		}
		if (!trialCharge && TrialManager.Instance.IsTestChar)
		{
			progress = (int)(Game.Instance.GetStartGameDuration() * 1000f) + PlayerInfo.Instance.CurrentTrialInfoProgress();
			value = (float)progress * 0.001f / (float)info.taskAim;
			if (value >= 1f)
			{
				slider.value = 1f;
				sliderLbl.text = info.taskAim + "/" + info.taskAim;
				slider.gameObject.SetActive(false);
				UISliderInController.Instance.OnTrialFinished();
				trialCharge = true;
			}
			else
			{
				slider.value = value;
				sliderLbl.text = progress / 1000 + "/" + info.taskAim;
			}
		}
	}

	private void UpdateMultiplierLable()
	{
		if (GameStats.Instance.scoreBooster5Activated && !_isMultiplierLabelUpdated && !PlayerInfo.Instance.doubleScore)
		{
			lerpFactor += Time.deltaTime * 2f;
			if (lerpFactor > 1f)
			{
				resetMultiplierLabel();
				lerpFactor = 0f;
			}
			else
			{
				FadeFromWhiteToDarkBlue();
			}
		}
		if (!_countingDown || !UIScreenController.Instance.GameIsFocused)
		{
			return;
		}
		float num = UpdateRealTimeDelta() * 1.75f;
		_countdownSeconds -= num;
		countdownStartingLabel.text = Strings.Get(LanguageKey.INGAME_UI_COUNTDOWN_STARTING);
		countdownLabel.text = Mathf.CeilToInt(_countdownSeconds).ToString();
		if (!countdownLabel.enabled)
		{
			countdownStartingLabel.enabled = true;
			countdownLabel.enabled = true;
		}
		if (_cachedCountdownLabelScale == Vector3.zero)
		{
			_cachedCountdownLabelScale = countdownLabel.cachedTransform.localScale;
		}
		countdownLabel.cachedTransform.localScale = _cachedCountdownLabelScale * ((1f - _countdownSeconds % 1f) * 0.5f + 1f);
		if (_countdownSeconds < 0f)
		{
			_countingDown = false;
			countdownStartingLabel.text = string.Empty;
			countdownLabel.text = string.Empty;
			countdownStartingLabel.enabled = false;
			countdownLabel.enabled = false;
			if (Game.Instance != null)
			{
				Game.Instance.TriggerPause(false);
				Character.Instance.sameLaneTimeStamp = Time.time;
			}
		}
	}

	private float UpdateRealTimeDelta()
	{
		if (mTimeStarted)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float b = realtimeSinceStartup - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			mTimeStart = realtimeSinceStartup;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = Time.realtimeSinceStartup;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}
}
