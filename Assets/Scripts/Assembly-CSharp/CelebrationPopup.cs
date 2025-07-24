using System.Collections;
using UnityEngine;

public class CelebrationPopup : UIBaseScreen
{
	public GameObject boxParent;

	public GameObject rewardLabelTemplate;

	[SerializeField]
	private Collider openButton;

	[SerializeField]
	private Collider continueButton;

	[SerializeField]
	private Collider skipDoubleButton;

	[SerializeField]
	private Collider watchDoubleButton;

	[SerializeField]
	private UISprite[] watchSprites;

	[SerializeField]
	private GameObject tapToStartLabelPrefab;

	[SerializeField]
	private GameObject watchDoublePrefab;

	[SerializeField]
	private GameObject chestPrefab;

	[SerializeField]
	private GameObject superChestPrefab;

	[SerializeField]
	private GameObject miniChestPrefab;

	[SerializeField]
	private GameObject superChestEffectPrefab;

	[SerializeField]
	private GameObject rewardCoins;

	[SerializeField]
	private GameObject rewardSymbolLee;

	[SerializeField]
	private GameObject rewardSymbolTurtlefok;

	[SerializeField]
	private GameObject rewardPowerupHeadstart2000;

	[SerializeField]
	private GameObject rewardPowerupScoreBooster;

	[SerializeField]
	private GameObject rewardKey;

	[SerializeField]
	private MeshRenderer GlowEffect;

	[SerializeField]
	private UIPanel _backgroundPanel;

	[SerializeField]
	private Texture2D itemHuntStripe;

	[SerializeField]
	private GameObject stripePanel;

	[SerializeField]
	private NewHighScoreHandler _NewHighScoreHandlerPrefab;

	[SerializeField]
	private UILabel doubleLabe;

	[SerializeField]
	private UILabel doubleScriptLabel;

	[SerializeField]
	private Vector3 _boxRotation = new Vector3(0f, 250f, 20f);

	[SerializeField]
	private Vector3 _boxScale = new Vector3(6f, 6f, 6f);

	[SerializeField]
	private Vector3 _continueLabelPosition = new Vector3(0f, -330f, -5f);

	[SerializeField]
	private Vector3 _continueHighestscoreLabelPosition = new Vector3(0f, -400f, -5f);

	[SerializeField]
	private Vector3 _labelPosition = new Vector3(0f, -110f, -5f);

	[SerializeField]
	private Vector3 _labelPositionUnlock = new Vector3(0f, -160f, -5f);

	[SerializeField]
	private Vector3 _outOfScreenPosition = new Vector3(0f, -500f, 0f);

	[SerializeField]
	private Vector3 _rewardLocalPosition = new Vector3(0f, 20f, 0f);

	[SerializeField]
	private Vector3 _rewardStartScale = new Vector3(4f, 4f, 4f);

	[SerializeField]
	private Vector3 _rewardStartRotation = new Vector3(0f, -10.5f, 0f);

	[SerializeField]
	private Vector3 _rewardEndScale = new Vector3(10f, 10f, 10f);

	private float _animationLerpFactor;

	private GameObject[] _celebrationRewardContainer;

	private NewHighScoreHandler _currentNewHighScoreHandler;

	private int _currentReward;

	private bool _isFingerPressed;

	private bool _maySetTimeScale;

	private int _numberOfRewards;

	private bool _stripeCoroutineRunning;

	private UITexture[] _stripeTextures;

	private SuperChestEffect _containerEffect;

	private float _timeScaleBeforeCelebrationPopup = 1f;

	private bool anotherReward = true;

	private bool celebrationHasStarted;

	private static readonly Vector3 FIRST_SLOT_POSITION = new Vector3(0f, 15f, -50f);

	private bool isWaitingForInput;

	private static readonly Vector3 OTHER_SLOT_OFFSET = new Vector3(0f, 0f, 9370f);

	private CelebrationReward[] rewardsToUnlock;

	private bool skipWaitBackButtonPressed;

	private GameObject[] slots;

	private bool stopIdleAnim;

	private Vector3 SUPERBOX_EFFECT_POSITION = new Vector3(0f, 0f, -180f);

	private GameObject tapToStartGo;

	private UILabel tapToStartLabel;

	private CelebrationReward canDoubleReward;

	private CelebrationPopupLabelTemplate celPopLabelTemple;

	public override void Show()
	{
		base.Show();
		_timeScaleBeforeCelebrationPopup = Time.timeScale;
		Time.timeScale = 1f;
		GlowEffect.enabled = false;
		SetupCelebrationScreen();
	}

	private void _FinishOpening()
	{
		celebrationHasStarted = false;
		_isFingerPressed = false;
		if (anotherReward)
		{
			StartCoroutine(MoveNextBoxToFront());
			return;
		}
		int i = 0;
		for (int num = slots.Length; i < num; i++)
		{
			Object.Destroy(slots[i]);
		}
		ResetBackgroundToNormal();
		continueButton.enabled = true;
		openButton.enabled = false;
		Object.Destroy(tapToStartGo);
		if (RewardManager.rewardsToUnlockCount > 0 && RewardManager.canShowMultipleQueuedCelebrations)
		{
			SetupCelebrationScreen();
			return;
		}
		if (RewardManager.canShowMultipleQueuedCelebrations)
		{
			RewardManager.canShowMultipleQueuedCelebrations = false;
		}
		UIScreenController.Instance.ClosePopup(null);
	}

	private IEnumerator _ShowReward(int currentRewardIndex)
	{
		CelebrationReward reward = rewardsToUnlock[currentRewardIndex];
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest)
		{
			PlayerInfo.Instance.stats[Stat.ChestesOpened]++;
		}
		GameObject rewardGo = null;
		bool flag = false;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			ShowModel(reward.characterType, reward.characterThemeIndex, UIModelController.ModelScreen.CelebrationCharacterUnlock);
			AudioPlayer.Instance.PlaySound("leyou_Hr_unlock", 0.5f, 0.5f, 1f);
			flag = true;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock)
		{
			ShowBackgroundWithStripes(reward.helmType, UIModelController.ModelScreen.CelebrationHelmUnlock);
			AudioPlayer.Instance.PlaySound("leyou_Hr_unlock", 0.5f, 0.5f, 1f);
			flag = true;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			ShowBackgroundWithStripes(PlayerInfo.Instance.currentHelmet, UIModelController.ModelScreen.CelebrationHighScore);
			AudioPlayer.Instance.PlaySound("leyou_Hr_unlock", 0.5f, 0.5f, 1f);
			flag = true;
		}
		if (!flag)
		{
			GlowEffect.enabled = true;
			rewardGo = NGUITools.AddChild(prefab: ChooseRewardPrefab(reward), parent: slots[0]);
			rewardGo.transform.localPosition = _rewardLocalPosition;
			rewardGo.transform.localScale = _rewardStartScale;
			rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotation);
			Utility.SetLayerRecursively(rewardGo.transform, boxParent.layer);
			AudioPlayer.Instance.PlaySound("leyou_Hr_ChestOpen", 0.5f, 0.5f, 1f);
			Animation animation2 = null;
			if (IsTwoStepsCelebration(currentRewardIndex))
			{
				animation2 = _celebrationRewardContainer[currentRewardIndex].GetComponentInChildren<Animation>();
				if (!(animation2 == null) && !(animation2["up"] == null))
				{
					animation2.Play("up");
					while (animation2["up"].normalizedTime < 0.5f)
					{
						yield return null;
					}
				}
			}
			_maySetTimeScale = true;
			StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale));
			if (IsTwoStepsCelebration(currentRewardIndex))
			{
				StartCoroutine(MoveGameObject(_celebrationRewardContainer[currentRewardIndex].transform, 0.7f, _outOfScreenPosition));
			}
			StartCoroutine(RotateGameObject(rewardGo.transform, 4f, new Vector3(0f, 1500f, 0f)));
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(AnimateColor(GlowEffect.material, 1.5f, Color.white));
			StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
			yield return new WaitForSeconds(1.3f);
		}
		CelebrationPopupLabelTemplate template = (celPopLabelTemple = InitRewardLabelTemplate(reward));
		if (reward.rewardType != CelebrationRewardType.highscore)
		{
			StartCoroutine(AnimateAlpha(template, 0.2f, 1f));
		}
		yield return new WaitForSeconds(0.5f);
		if (reward.rewardType != CelebrationRewardType.coins)
		{
			if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.HelmetUnlock)
			{
				yield return new WaitForSeconds(1f);
			}
		}
		else
		{
			StartCoroutine(CountUpCoins(reward.amount, template));
			yield return new WaitForSeconds(2.5f);
		}
		StartCoroutine(AnimateColor(GlowEffect.material, 0.5f, Color.black));
		PayoutReward(reward);
		if (UIScreenController.Instance.CheckNetwork() && UIScreenController.Instance.GetTopScreenName().Equals("GameoverUI") && reward.rewardType != CelebrationRewardType.highscore && reward.rewardType != CelebrationRewardType.character && reward.rewardType != CelebrationRewardType.specialHelm && reward.rewardType != 0)
		{
			canDoubleReward = reward;
			watchDoublePrefab.SetActive(true);
			openButton.enabled = false;
			continueButton.enabled = false;
			if (RiseSdk.Instance.HasRewardAd())
			{
				watchDoubleButton.enabled = true;
				for (int i = 0; i < watchSprites.Length; i++)
				{
					watchSprites[i].color = Color.white;
				}
			}
			else
			{
				watchDoubleButton.enabled = false;
				for (int j = 0; j < watchSprites.Length; j++)
				{
					watchSprites[j].color = Color.cyan;
				}
			}
		}
		else
		{
			openButton.enabled = true;
			continueButton.enabled = true;
			watchDoublePrefab.SetActive(false);
			canDoubleReward = null;
		}
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.HelmetUnlock)
		{
			yield return new WaitForSeconds(1f);
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			tapToStartGo.transform.localPosition = _continueHighestscoreLabelPosition;
		}
		else
		{
			tapToStartGo.transform.localPosition = _continueLabelPosition;
		}
		tapToStartLabel.text = Strings.Get(LanguageKey.CELEBRATION_POPUP_CONTINUE);
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		_maySetTimeScale = false;
		Time.timeScale = 1f;
		isWaitingForInput = true;
		while (ShouldWaitForInput())
		{
			yield return null;
		}
		isWaitingForInput = false;
		if (watchDoublePrefab.activeInHierarchy)
		{
			watchDoublePrefab.SetActive(false);
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			HideModel();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock)
		{
			HideSpecialHelm();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			HideModel();
		}
		Object.Destroy(rewardGo);
		Object.Destroy(template.gameObject);
		Object.Destroy(_celebrationRewardContainer[currentRewardIndex]);
		if (_currentNewHighScoreHandler != null)
		{
			Object.Destroy(_currentNewHighScoreHandler.gameObject);
			_currentNewHighScoreHandler = null;
		}
		_FinishOpening();
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	private void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type != RiseSdk.AdEventType.RewardAdShowFinished)
		{
			return;
		}
		if (id == 20)
		{
			if (canDoubleReward.rewardType == CelebrationRewardType.coins)
			{
				PlayerInfo.Instance.amountOfCoins += canDoubleReward.amount;
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, canDoubleReward.amount);
				celPopLabelTemple.SetDoubleCions(canDoubleReward.amount * 2);
			}
			else if (canDoubleReward.rewardType == CelebrationRewardType.powerup)
			{
				celPopLabelTemple.SetDoublePowerup(canDoubleReward.powerupType, canDoubleReward.amount * 2);
			}
			else if (canDoubleReward.rewardType == CelebrationRewardType.symbol)
			{
				celPopLabelTemple.SetupDoubleSymbol(canDoubleReward.characterType, canDoubleReward.amount);
				TasksManager.Instance.PlayerDidThis(TaskTarget.Symbols, canDoubleReward.amount);
			}
			else if (canDoubleReward.rewardType == CelebrationRewardType.keys)
			{
				celPopLabelTemple.SetupDoubleKeys(canDoubleReward.amount * 2);
			}
			PayoutReward(canDoubleReward);
			watchDoublePrefab.SetActive(false);
		}
		canDoubleReward = null;
	}

	public void OnDoubleClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(20);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	private CelebrationPopupLabelTemplate InitRewardLabelTemplate(CelebrationReward reward)
	{
		GameObject gameObject = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, rewardLabelTemplate);
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.HelmetUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			gameObject.transform.localPosition = _labelPositionUnlock;
		}
		else
		{
			gameObject.transform.localPosition = _labelPosition;
		}
		CelebrationPopupLabelTemplate component = gameObject.GetComponent<CelebrationPopupLabelTemplate>();
		component.Init(_backgroundPanel.depth);
		if (_stripeCoroutineRunning)
		{
			component.bigLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.subLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.bigLabel.effectColor = new Color32(250, 187, 231, byte.MaxValue);
			component.subLabel.effectColor = new Color32(250, 187, 231, byte.MaxValue);
		}
		else
		{
			component.bigLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.subLabel.color = new Color32(0, 0, 0, byte.MaxValue);
			component.bigLabel.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			component.subLabel.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		if (reward.rewardType == CelebrationRewardType.coins)
		{
			component.SetupCoins();
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			component.SetupPowerup(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.symbol)
		{
			component.SetupSymbol(reward.characterType, reward.amount);
			TasksManager.Instance.PlayerDidThis(TaskTarget.Symbols, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			component.SetupKeys(reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.character)
		{
			CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(reward.characterType, reward.characterThemeIndex);
			if (themeForCharacter != null)
			{
				component.SetupCharacter(Strings.Get(themeForCharacter.title));
			}
			else
			{
				Characters.Model model = Characters.characterData[reward.characterType];
				component.SetupCharacter(Strings.Get(model.name));
			}
		}
		else if (reward.rewardType == CelebrationRewardType.specialHelm)
		{
			component.SetupEventSpecialHelm(Strings.Get(Helmets.helmData[reward.helmType].name));
		}
		else if (reward.rewardType == CelebrationRewardType.highscore)
		{
			gameObject.SetActive(false);
		}
		return component;
	}

	private IEnumerator AnimateAlpha(CelebrationPopupLabelTemplate template, float duration, float toAlpha)
	{
		float fromAlpha = template.Alpha;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			template.Alpha = Mathf.Lerp(fromAlpha, toAlpha, factor2);
			yield return null;
		}
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor2);
			yield return null;
		}
	}

	private IEnumerator AnimateColor(Material material, float duration, Color toColor)
	{
		Color fromColor = material.GetColor(Shaders.Instance.MainColor);
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			material.SetColor(Shaders.Instance.MainColor, Color.Lerp(fromColor, toColor, factor2));
			yield return null;
		}
	}

	private void ResetBackgroundToNormal()
	{
		_stripeCoroutineRunning = false;
		StopCoroutine("AnimateStripes");
		if (_stripeTextures != null)
		{
			for (int i = 0; i < _stripeTextures.Length; i++)
			{
				_stripeTextures[i].gameObject.SetActive(false);
				Object.Destroy(_stripeTextures[i].gameObject);
			}
		}
		_stripeTextures = null;
	}

	private IEnumerator AnimateStripes()
	{
		_stripeCoroutineRunning = true;
		Color32 colorOfStripe = new Color32(233, 132, 196, byte.MaxValue);
		int minSpeed = 600;
		int maxSpeed = 2000;
		float minAlpha = 0.3f;
		float maxAlpha = 0.9f;
		float minScale = 0.5f;
		float maxScale = 1f;
		int numberOfStripes = 12;
		Vector2 minimumThreshold = new Vector2(-150f, -225f);
		Vector2 maximumThreshold = new Vector2(150f, 700f);
		if (_stripeTextures != null)
		{
			for (int k = 0; k < _stripeTextures.Length; k++)
			{
				_stripeTextures[k].gameObject.SetActive(false);
				Object.Destroy(_stripeTextures[k].gameObject);
			}
		}
		_stripeTextures = new UITexture[numberOfStripes];
		float[] stripeSpeeds = new float[numberOfStripes];
		for (int i = 0; i < numberOfStripes; i++)
		{
			stripeSpeeds[i] = Random.Range(minSpeed, maxSpeed);
			_stripeTextures[i] = NGUITools.AddWidget<UITexture>(stripePanel);
			_stripeTextures[i].mainTexture = itemHuntStripe;
			_stripeTextures[i].shader = Shader.Find("Unlit/Transparent Colored");
			_stripeTextures[i].MakePixelPerfect();
			_stripeTextures[i].color = colorOfStripe;
			_stripeTextures[i].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[i] / (float)maxSpeed);
			Vector3 localScale = _stripeTextures[i].cachedTransform.localScale;
			_stripeTextures[i].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale.x * minScale, localScale.y * minScale, localScale.z), new Vector3(localScale.x * maxScale, localScale.y * maxScale, localScale.z), stripeSpeeds[i] / (float)maxSpeed);
			float x = Random.Range(minimumThreshold.x, maximumThreshold.x);
			float y = Random.Range(minimumThreshold.y, maximumThreshold.y);
			_stripeTextures[i].cachedTransform.localPosition = new Vector3(x, y, 0f);
		}
		yield return null;
		while (true)
		{
			for (int j = 0; j < _stripeTextures.Length; j++)
			{
				Vector3 localPosition = _stripeTextures[j].cachedTransform.localPosition;
				float num = stripeSpeeds[j] * Time.deltaTime;
				if (localPosition.y > maximumThreshold.y)
				{
					localPosition.x = Random.Range(minimumThreshold.x, maximumThreshold.x);
					stripeSpeeds[j] = Random.Range(minSpeed, maxSpeed);
					num = stripeSpeeds[j] * Time.deltaTime;
					_stripeTextures[j].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[j] / (float)maxSpeed);
					_stripeTextures[j].MakePixelPerfect();
					Vector3 localScale2 = _stripeTextures[j].cachedTransform.localScale;
					_stripeTextures[j].cachedTransform.localScale = Vector3.Lerp(new Vector3(localScale2.x * minScale, localScale2.y * minScale, localScale2.z), new Vector3(localScale2.x * maxScale, localScale2.y * maxScale, localScale2.z), stripeSpeeds[j] / (float)maxSpeed);
					localPosition.y = minimumThreshold.y - num;
				}
				_stripeTextures[j].cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y + num, localPosition.z);
			}
			yield return null;
		}
	}

	private void Awake()
	{
		if (Application.isEditor && _NewHighScoreHandlerPrefab == null)
		{
			Debug.LogError("NewHighScoreHandler not set in CelebrationPopup");
		}
	}

	private IEnumerator BoxIdleAnimCoroutine(Transform rewardTrans)
	{
		Vector3 baseLocalPos = rewardTrans.parent.localPosition;
		stopIdleAnim = false;
		float t = Random.Range(0f, 6f);
		Vector3 newLocalPos = baseLocalPos;
		while (!stopIdleAnim)
		{
			t += Time.deltaTime;
			newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
		bool doneResetting = false;
		while (!doneResetting)
		{
			newLocalPos.y = Mathf.MoveTowards(newLocalPos.y, baseLocalPos.y, Time.deltaTime * 20f);
			if (Mathf.Approximately(newLocalPos.y, baseLocalPos.y))
			{
				doneResetting = true;
			}
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
	}

	private IEnumerator CharacterIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		while (!stopIdleAnim)
		{
			yield return null;
		}
	}

	private GameObject ChooseRewardPrefab(CelebrationReward reward)
	{
		GameObject result = rewardCoins;
		switch (reward.rewardType)
		{
			case CelebrationRewardType.coins:
				return rewardCoins;
			case CelebrationRewardType.powerup:
				switch (reward.powerupType)
				{
					case PropType.headstart2000:
						return rewardPowerupHeadstart2000;
					case PropType.scorebooster:
						return rewardPowerupScoreBooster;
					default:
						return result;
				}
			case CelebrationRewardType.symbol:
				switch (reward.characterType)
				{
					case Characters.CharacterType.lee:
						return rewardSymbolLee;
					case Characters.CharacterType.turtlefok:
						return rewardSymbolTurtlefok;
					default:
						return result;
				}
			case CelebrationRewardType.keys:
				return rewardKey;
			default:
				return result;
		}
	}

	private IEnumerator CountUpCoins(int amount, CelebrationPopupLabelTemplate rewardTemplate)
	{
		PlayerInfo.Instance.amountOfCoins += amount;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, amount);
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_game_mystery_box", 0, amount);
		float countFactor = 0f;
		float countTime = Mathf.Lerp(1.5f, 4f, (float)amount / 100000f);
		int rewardLabelFrom = 0;
		yield return new WaitForSeconds(0.5f);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			rewardTemplate.UpdateCoins(Mathf.RoundToInt(Mathf.SmoothStep(rewardLabelFrom, amount, countFactor)));
			yield return null;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, amount);
	}

	private void InitSlotsAndRewardContainers(int length)
	{
		slots = new GameObject[length];
		_celebrationRewardContainer = new GameObject[length];
		for (int i = 0; i < _numberOfRewards; i++)
		{
			slots[i] = NGUITools.AddChild(boxParent);
			slots[i].transform.localPosition = FIRST_SLOT_POSITION + OTHER_SLOT_OFFSET * ((i > 0) ? 1 : 0);
		}
	}

	private void FillOutAllTheSlotsWithRewardContainers()
	{
		for (int i = 0; i < _numberOfRewards; i++)
		{
			GameObject gameObject = null;
			CelebrationRewardOrigin celebrationRewardOrigin = rewardsToUnlock[i].CelebrationRewardOrigin;
			bool flag = true;
			switch (celebrationRewardOrigin)
			{
				case CelebrationRewardOrigin.Chest:
					gameObject = NGUITools.AddChild(slots[i], chestPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.SuperChest:
					gameObject = NGUITools.AddChild(slots[i], superChestPrefab);
					flag = false;
					break;
				case CelebrationRewardOrigin.ChestMini:
					gameObject = NGUITools.AddChild(slots[i], miniChestPrefab);
					flag = false;
					break;
			}
			if (!flag)
			{
				gameObject.transform.localScale = _boxScale;
				gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
				Utility.SetLayerRecursively(gameObject.transform, boxParent.layer);
				_celebrationRewardContainer[i] = gameObject;
			}
			else
			{
				_celebrationRewardContainer[i] = null;
			}
		}
		GlowEffect.material.SetColor(Shaders.Instance.MainColor, Color.black);
	}

	public void OnPressed()
	{
		if (_isFingerPressed)
		{
			return;
		}
		_isFingerPressed = true;
		stopIdleAnim = true;
		Animation animation = null;
		if (IsTwoStepsCelebration(_currentReward))
		{
			animation = _celebrationRewardContainer[_currentReward].GetComponentInChildren<Animation>();
			if (animation != null && animation["down"] != null)
			{
				animation.Play("down");
			}
		}
	}

	public void OnReleased()
	{
		if (!celebrationHasStarted)
		{
			openButton.enabled = false;
			if (_containerEffect != null)
			{
				_containerEffect.StopEffect();
			}
			StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 0f));
			StartCoroutine(_ShowReward(_currentReward));
			celebrationHasStarted = true;
		}
	}

	public override void Hide()
	{
		Time.timeScale = _timeScaleBeforeCelebrationPopup;
		StopCoroutine("AnimateStripes");
		base.Hide();
	}

	private void HideModel()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private void HideSpecialHelm()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private bool IsTwoStepsCelebration(int rewardIndex)
	{
		if (_celebrationRewardContainer[rewardIndex] == null)
		{
			return false;
		}
		return true;
	}

	private IEnumerator MoveGameObject(Transform trans, float duration, Vector3 toPos)
	{
		if (!(trans == null))
		{
			Vector3 fromPos = trans.localPosition;
			float factor2 = 0f;
			while (factor2 < 1f && trans != null)
			{
				factor2 += Time.deltaTime / duration;
				factor2 = Mathf.Clamp01(factor2);
				trans.localPosition = Vector3.Lerp(fromPos, toPos, 0.5f * (Mathf.Sin((factor2 - 0.5f) * 3.141593f) + 1f));
				yield return null;
			}
			if (trans != null)
			{
				trans.localPosition = toPos;
			}
		}
	}

	private IEnumerator MoveNextBoxToFront()
	{
		Time.timeScale = 1f;
		_currentReward++;
		if (_currentReward >= _numberOfRewards - 1)
		{
			anotherReward = false;
		}
		UpdateGui(rewardsToUnlock[_currentReward]);
		GlowEffect.enabled = false;
		GlowEffect.material.SetColor(Shaders.Instance.MainColor, Color.black);
		ToggleSuperChestEffect(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
		openButton.enabled = false;
		if (!IsTwoStepsCelebration(_currentReward))
		{
			OnReleased();
			yield break;
		}
		_celebrationRewardContainer[_currentReward].transform.parent = slots[0].transform;
		StartCoroutine(MoveGameObject(_celebrationRewardContainer[_currentReward].transform, 0.35f, Vector3.zero));
		StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, rewardsToUnlock[_currentReward]);
		yield return new WaitForSeconds(0.35f);
		openButton.enabled = true;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && _isFingerPressed)
		{
			OnReleased();
		}
	}

	private void PayoutReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.topRun)
		{
			UIScreenController.Instance.PayoutCelebrationReward(reward);
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.symbol)
		{
			PlayerInfo.Instance.CollectSymbol(reward.characterType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo.Instance.amountOfKeys += reward.amount;
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.ChestMini || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, reward.amount);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_game_mystery_box", 0, reward.amount);
			}
		}
		RewardManager.RewardPayedOut(reward);
		PlayerInfo.Instance.SaveIfDirty();
	}

	private IEnumerator RotateGameObject(Transform trans, float duration, Vector3 angleToRotate)
	{
		if (!(trans == null))
		{
			Quaternion fromRotation = trans.localRotation;
			float factor2 = 0f;
			while (factor2 < 1f && trans != null)
			{
				factor2 += Time.deltaTime / duration;
				factor2 = Mathf.Clamp01(factor2);
				float angle2 = 0f;
				angle2 = Mathf.Lerp(4.712389f, 6.283185f, factor2);
				float cosFactor = Mathf.Cos(angle2) * 0.5f + 0.5f;
				trans.localRotation = fromRotation;
				trans.Rotate(angleToRotate * cosFactor, Space.World);
				yield return null;
			}
		}
	}

	private IEnumerator ScaleGameObject(Transform trans, float duration, Vector3 toScale)
	{
		if (!(trans == null))
		{
			float factor2 = 0f;
			Vector3 fromScale = trans.localScale;
			while (factor2 < 1f && trans != null)
			{
				factor2 += Time.deltaTime / duration;
				factor2 = Mathf.Clamp01(factor2);
				float angle2 = 0f;
				angle2 = Mathf.Lerp(3.141593f, 6.283185f, factor2);
				float cosFactor = Mathf.Cos(angle2) * 0.5f + 0.5f;
				trans.localScale = Vector3.Lerp(fromScale, toScale, cosFactor);
				yield return null;
			}
		}
	}

	private void SetupCelebrationScreen()
	{
		boxParent.transform.position = UIModelController.Instance.CelebrationPopupAnchor.transform.position;
		rewardsToUnlock = RewardManager.GetRewardsToUnlockForCelebration();
		doubleLabe.text = Strings.Get(LanguageKey.UI_POPUP_CELEBRATION_DOUBLE);
		doubleScriptLabel.text = Strings.Get(LanguageKey.UI_POPUP_CELEBRATION_DOUBLE_DESCRIPT);
		_numberOfRewards = rewardsToUnlock.Length;
		if (_numberOfRewards <= 0)
		{
			anotherReward = false;
			UIScreenController.Instance.ClosePopup(null);
			openButton.enabled = false;
			return;
		}
		if (_numberOfRewards == 1)
		{
			anotherReward = false;
		}
		else
		{
			anotherReward = true;
		}
		_currentReward = 0;
		InitTapToStart();
		InitSlotsAndRewardContainers(_numberOfRewards);
		FillOutAllTheSlotsWithRewardContainers();
		InitEffect();
		UpdateGui(rewardsToUnlock[_currentReward]);
		if (IsTwoStepsCelebration(_currentReward))
		{
			StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, rewardsToUnlock[_currentReward]);
		}
		else
		{
			StartOneStepUnlock();
		}
	}

	private void InitEffect()
	{
		if (superChestEffectPrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(slots[_currentReward], superChestEffectPrefab);
			gameObject.transform.transform.position = SUPERBOX_EFFECT_POSITION;
			_containerEffect = gameObject.GetComponent<SuperChestEffect>();
			if (_containerEffect == null)
			{
				Debug.LogError("CelebrationPopup ERROR: unable to find SuperChestEffect component on superChestEffectPrefab", this);
			}
			else
			{
				ToggleSuperChestEffect(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
			}
		}
		else
		{
			Debug.LogError("CelebrationPopup ERROR: superChestEffectPrefab is not assigned", this);
		}
	}

	private void InitTapToStart()
	{
		openButton.enabled = true;
		continueButton.enabled = true;
		watchDoublePrefab.SetActive(false);
		tapToStartGo = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, tapToStartLabelPrefab);
		tapToStartGo.transform.localPosition = _continueLabelPosition;
		tapToStartLabel = tapToStartGo.GetComponentInChildren<UILabel>();
		tapToStartLabel.alpha = 0f;
		tapToStartGo.GetComponent<UIPanel>().depth = _backgroundPanel.depth + 1;
	}

	private bool ShouldWaitForInput()
	{
		if (skipWaitBackButtonPressed)
		{
			skipWaitBackButtonPressed = false;
			return false;
		}
		return !Input.GetMouseButtonUp(0);
	}

	private void ShowBackgroundWithStripes(Helmets.HelmType helmType, UIModelController.ModelScreen modelScreen)
	{
		UIModelController.Instance.ActivateCelebrationHelmWithStripes(helmType, modelScreen);
	}

	private GameObject ShowModel(Characters.CharacterType charType, int themeIndex, UIModelController.ModelScreen modelScreen)
	{
		return UIModelController.Instance.ShowCharacterInCelebration(charType, themeIndex, modelScreen);
	}

	public void SkipNow()
	{
		if (_maySetTimeScale)
		{
			Time.timeScale = 4f;
		}
	}

	private void StartIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		tapToStartLabel.text = Strings.Get(LanguageKey.CELEBRATION_POPUP_OPEN);
		tapToStartLabel.alpha = 1f;
		StartCoroutine(BoxIdleAnimCoroutine(rewardTrans));
	}

	private void StartOneStepUnlock()
	{
		OnReleased();
	}

	private void ToggleSuperChestEffect(CelebrationRewardOrigin origin)
	{
		if (_containerEffect != null)
		{
			if (origin == CelebrationRewardOrigin.SuperChest)
			{
				_containerEffect.FastForwardEffect(3);
				_containerEffect.StartEffect();
			}
			else
			{
				_containerEffect.StopEffect();
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (openButton.enabled)
			{
				OnPressed();
				OnReleased();
			}
			if (continueButton.enabled)
			{
				SkipNow();
			}
			if (isWaitingForInput)
			{
				skipWaitBackButtonPressed = true;
			}
		}
	}

	private void UpdateGui(CelebrationReward reward)
	{
		switch (reward.CelebrationRewardOrigin)
		{
			case CelebrationRewardOrigin.Chest:
			case CelebrationRewardOrigin.SuperChest:
			case CelebrationRewardOrigin.ChestMini:
				ResetBackgroundToNormal();
				openButton.enabled = true;
				break;
			case CelebrationRewardOrigin.CharacterUnlock:
			case CelebrationRewardOrigin.HelmetUnlock:
				ResetBackgroundToNormal();
				openButton.enabled = false;
				break;
			case CelebrationRewardOrigin.NewHighScore:
				ResetBackgroundToNormal();
				openButton.enabled = false;
				_currentNewHighScoreHandler = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, _NewHighScoreHandlerPrefab.gameObject).GetComponent<NewHighScoreHandler>();
				Utility.SetLayerRecursively(_currentNewHighScoreHandler.gameObject.transform, UIScreenController.Instance.CameraOverlay2d.gameObject.layer);
				break;
		}
	}
}
