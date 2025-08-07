using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenController : MonoBehaviour
{
	public enum DeviceType
	{
		Android = 0,
		iPhone = 1,
		iPad = 2,
		iPhoneX = 3
	}

	public GameObject backgroundAnchor;

	public GameObject screenAnchor;

	public GameObject popupAnchor;

	public GameObject superPopupAnchor;

	public UICamera nguiCamera;

	public Camera CameraOverlay2d;

	public UIRoot root;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public Material chestBox;

	public Font FloatingTextFont;

	public Action<string> OnChangedScreen;

	[SerializeField]
	private Font lilitaRegularFont;

	[SerializeField]
	private Font titanRegularFont;

	public string text;

	public int size;

	public FontStyle style;

	public bool stoppingFromEditor;

	public bool doSaveAndDelayRemainingPopups;

	public bool ignoreLastPopupClosed;

	public UIMessageHelper messageHelper;

	public static bool isGetBtnPressed;

	[SerializeField]
	private int minCycleCount = 6;

	[SerializeField]
	private int maxCycleCount = 9;

	private Dictionary<string, UIBaseScreen> _cachedScreens = new Dictionary<string, UIBaseScreen>();

	private List<string> _delayedPopups = new List<string>();

	private bool _gameIsFocused = true;

	private static UIScreenController _instance;

	private bool _isApplicationResuming;

	private bool _popupActive;

	private List<string> _popupQueue = new List<string>();

	private List<string> _screenNamesWithoutBackground;

	private List<string> _screenNamesWithBackground2;

	private List<string> _screenNamesWithCycleAd;

	private List<string> _screenStack = new List<string>();

	private string lastAdScreenName;

	private int curAdCount;

	private int currentCycleCount;

	private int lastPopupNameIndex = -1;

	private Camera mainCamera;

	public string uiPrefabNameSuffix = string.Empty;

	public const string iPadNameSuffix = "_ipad";

	public const string iPhoneXNameSuffix = "_iphoneX";

	public float bannerHeight;

	public DeviceType curDeviceType;

	public bool GameIsFocused
	{
		get
		{
			return _gameIsFocused;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance != null;
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return _popupQueue != null && _popupQueue.Count > 0;
		}
	}

	public static event Action OnApplicationResumed;

	public event Action OnLastPopupClosed;

	public event Action<string> OnPopupClosed;

	public event Action OnPopupShown;

	public UIScreenController()
	{
		_screenNamesWithoutBackground = new List<string> { "FrontUI", "IngameUI" };
		_screenNamesWithBackground2 = new List<string> { "CoinsUI_shop", "CharacterScreen", "HelmScreen", "UpgradesUI_shop", "RankScreen" };
		_screenNamesWithCycleAd = new List<string> { "SettingsPopup", "Task_popup", "DailyRewardsPopup", "HelmetPopup", "CoinsUI_shop", "UpgradesUI_shop", "CharacterScreen", "HelmScreen" };
		text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		size = 16;
		DebugShow.SetWillShowDebugInfoByGUI(false);
	}

	private void Awake()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		if (lilitaRegularFont != null)
		{
			lilitaRegularFont.RequestCharactersInTexture(text, size, style);
		}
		if (titanRegularFont != null)
		{
			titanRegularFont.RequestCharactersInTexture(text, size, style);
		}
		currentCycleCount = UnityEngine.Random.Range(minCycleCount, maxCycleCount + 1);
		uiPrefabNameSuffix = string.Empty;
		curDeviceType = DeviceType.Android;
		bannerHeight = 50f * (Screen.dpi / 160f);
	}

	public bool CheckNetwork()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		return true;
	}

	private void _ActivateNextPopup(string oldPopupName = "")
	{
		if (this.OnPopupShown != null)
		{
			this.OnPopupShown();
		}
		if (_popupQueue.Count > 0)
		{
			string text = QueuePeek(_popupQueue);
			int i = 0;
			for (int count = _popupQueue.Count; i < count; i++)
			{
				if (!_cachedScreens.ContainsKey(_popupQueue[i]))
				{
					continue;
				}
				UIBaseScreen uIBaseScreen = _cachedScreens[_popupQueue[i]];
				if (uIBaseScreen.isActive)
				{
					if (_popupQueue[i] != text)
					{
						uIBaseScreen.LooseFocus();
					}
					else
					{
						uIBaseScreen.GainFocus();
					}
				}
			}
			ActivateAnchors(false);
			if (!_cachedScreens.ContainsKey(text))
			{
				_LoadScreenToCache(text, true);
			}
			if (text == "CelebrationPopup" && _screenStack.Count > 0)
			{
				if (_screenStack[_screenStack.Count - 1] == "GameoverUI")
				{
					RewardManager.canShowMultipleQueuedCelebrations = true;
				}
				if (_screenStack[_screenStack.Count - 1] != "GameoverUI")
				{
					_cachedScreens[_screenStack[_screenStack.Count - 1]].Hide();
				}
			}
			if (_popupQueue.Count == 1)
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].LooseFocus();
			}
			_SetCycleAd(text);
			UIBaseScreen uIBaseScreen2 = _cachedScreens[text];
			if (uIBaseScreen2 != null && !uIBaseScreen2.isActive)
			{
				if (oldPopupName != string.Empty)
				{
					uIBaseScreen2.parentScreen = oldPopupName;
				}
				uIBaseScreen2.Show();
			}
			_popupActive = true;
			if (OnChangedScreen != null)
			{
				OnChangedScreen(text);
			}
		}
		else
		{
			ActivateAnchors(true);
		}
	}

	private void _PushPopup(string name)
	{
		int num = -1;
		string text = QueuePeek(_popupQueue);
		if (!string.IsNullOrEmpty(text) && _cachedScreens.ContainsKey(text))
		{
			UIPanel[] componentsInChildren = _cachedScreens[text].GetComponentsInChildren<UIPanel>(true);
			int i = 0;
			for (int num2 = componentsInChildren.Length; i < num2; i++)
			{
				if (componentsInChildren[i].depth > num)
				{
					num = componentsInChildren[i].depth;
				}
			}
		}
		_popupQueue.Insert(0, name);
		_ActivateNextPopup(text);
		if (!_cachedScreens.ContainsKey(name))
		{
			return;
		}
		UIPanel[] componentsInChildren2 = _cachedScreens[name].GetComponentsInChildren<UIPanel>(true);
		int num3 = -1;
		if (componentsInChildren2 == null || componentsInChildren2.Length <= 0)
		{
			return;
		}
		UIPanel uIPanel = componentsInChildren2[0];
		int j = 0;
		for (int num4 = componentsInChildren2.Length; j < num4; j++)
		{
			if (componentsInChildren2[j].depth < uIPanel.depth)
			{
				uIPanel = componentsInChildren2[j];
			}
		}
		if (uIPanel != null && num > -1)
		{
			num3 = num + 1 - uIPanel.depth;
		}
		if (num3 > -1)
		{
			int k = 0;
			for (int num5 = componentsInChildren2.Length; k < num5; k++)
			{
				componentsInChildren2[k].depth += num3;
			}
		}
	}

	private void _QueuePopup(string name)
	{
		messageHelper.DisableShowLabel();
		_popupQueue.Add(name);
		if (!_popupActive)
		{
			_ActivateNextPopup(string.Empty);
		}
	}

	private void _RemovePopup(string screenName = "")
	{
		if (_popupQueue.Count < 1)
		{
			return;
		}
		if (screenName == string.Empty)
		{
			screenName = Dequeue(_popupQueue);
		}
		else
		{
			_popupQueue.Remove(screenName);
		}
		UIBaseScreen uIBaseScreen = _cachedScreens[screenName];
		uIBaseScreen.Closed = delegate
		{
			PopupClosed(screenName);
		};
		uIBaseScreen.TryHide();
		uIBaseScreen.parentScreen = string.Empty;
		string topScreenName = GetTopScreenName();
		if ("HelmScreen".Equals(topScreenName) && _popupQueue.Count == 0)
		{
			((HelmScreen)_cachedScreens["HelmScreen"]).resetHelmAnimation = true;
		}
		if (screenName == "CelebrationPopup")
		{
			if (!"GameoverUI".Equals(GetTopScreenName()))
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].Show();
			}
			else if (GameStats.Instance.chestPickups > 0)
			{
				ShopManager.Instance.chestType = ChestType.Game;
				_QueuePopup("Box_Open");
			}
			else
			{
				_cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().SetupAfterChest();
			}
		}
		if (screenName == "Box_Open" && "GameoverUI".Equals(GetTopScreenName()))
		{
			_cachedScreens["GameoverUI"].GetComponent<GameOverScreen>().SetupAfterChest();
		}
	}

	private void PopupClosed(string screenName = "")
	{
		if (this.OnPopupClosed != null)
		{
			this.OnPopupClosed(screenName);
		}
		_popupActive = false;
		if (_popupQueue.Count == 0)
		{
			_cachedScreens[_screenStack[_screenStack.Count - 1]].GainFocus();
			if (OnChangedScreen != null)
			{
				OnChangedScreen(GetTopScreenName());
			}
		}
		if (doSaveAndDelayRemainingPopups)
		{
			SaveAndDelayPopups();
		}
		_ActivateNextPopup(string.Empty);
		if (_popupQueue.Count == 0 && this.OnLastPopupClosed != null)
		{
			if (!ignoreLastPopupClosed)
			{
				this.OnLastPopupClosed();
			}
			else
			{
				ignoreLastPopupClosed = false;
			}
		}
	}

	public void ClosePopup(string screenName)
	{
		_RemovePopup(string.Empty);
	}

	public void ClosePopupHandle(string screenName)
	{
		_RemovePopup(screenName);
	}

	private UIBaseScreen _ActivateScreen(string screenName)
	{
		UIBaseScreen screen = null;
		bool flag = true;
		if (_cachedScreens.ContainsKey(screenName))
		{
			if (_screenStack.Count > 0)
			{
				int num = _screenStack.LastIndexOf(screenName);
				if (num >= 0)
				{
					flag = false;
					if (num < _screenStack.Count - 1)
					{
						int num2 = num + 1;
						int num3 = _screenStack.Count - num2;
						_screenStack.RemoveRange(num2, num3 - 1);
					}
				}
			}
			screen = _cachedScreens[screenName];
		}
		if (flag || !screenName.Equals(_screenStack[_screenStack.Count - 1]))
		{
			if (_screenStack.Count > 0)
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].Closed = delegate
				{
					AddScreenStack(screenName);
					if (screen == null)
					{
						screen = _LoadScreenToCache(screenName);
					}
					_ShowScreen(screenName, screen);
				};
				_cachedScreens[_screenStack[_screenStack.Count - 1]].TryHide();
			}
			else
			{
				_screenStack.Add(screenName);
				if (screen == null)
				{
					screen = _LoadScreenToCache(screenName);
				}
				_ShowScreen(screenName, screen);
			}
		}
		else
		{
			if (screen == null)
			{
				screen = _LoadScreenToCache(screenName);
			}
			_ShowScreen(screenName, screen);
		}
		return screen;
	}

	private void AddScreenStack(string screenName)
	{
		_screenStack.Add(screenName);
	}

	private void _BackToPreviousScreen()
	{
		if (_screenStack.Count > 1)
		{
			string key = Pop(_screenStack);
			_cachedScreens[key].TryHide();
			key = Peek(_screenStack);
			_cachedScreens[key].Show();
			_SetBackground(key);
			ScreenDidChange(key);
		}
		else
		{
			LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
	}

	private UIBaseScreen _LoadScreenToCache(string screenName, bool isPopup = false)
	{
		GameObject gameObject;
		if (!isPopup)
		{
			GameObject prefab = Resources.Load("Prefabs/screens/" + screenName + uiPrefabNameSuffix, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(screenAnchor, prefab);
		}
		else
		{
			GameObject prefab2 = Resources.Load("Prefabs/popups/" + screenName + uiPrefabNameSuffix, typeof(GameObject)) as GameObject;
			gameObject = NGUITools.AddChild(popupAnchor, prefab2);
		}
		UIBaseScreen component = gameObject.GetComponent<UIBaseScreen>();
		_cachedScreens.Add(screenName, component);
		component.Init();
		return component;
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform trans2 = (Transform)enumerator.Current;
				_PauseAnimations(pause, trans2);
			}
		}
		finally
		{
		}
		CharacterModel component = trans.GetComponent<CharacterModel>();
		if (component != null)
		{
			if (pause)
			{
				component.StopIdleAnimations();
			}
			else if (Peek(_screenStack) != "HelmScreen")
			{
				component.StartIdleAnimations();
			}
		}
	}

	private void _PauseApplication(bool paused)
	{
		if (paused)
		{
			PlayerInfo.Instance.SaveIfDirty();
		}
		else
		{
			_isApplicationResuming = true;
		}
	}

	private void _SetBackground(string screenName)
	{
		bool flag = !_screenNamesWithoutBackground.Contains(screenName);
		string text = "NotebookPanel";
		if (flag)
		{
			if (!_cachedScreens.ContainsKey(text))
			{
				GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
				_cachedScreens.Add(text, NGUITools.AddChild(backgroundAnchor, prefab).GetComponent<UIBaseScreen>());
				_cachedScreens[text].Init();
			}
			ShopBackgroundScreen.one = !_screenNamesWithBackground2.Contains(screenName);
			_cachedScreens[text].Show();
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			mainCamera.enabled = false;
		}
		else
		{
			if (_cachedScreens.ContainsKey(text))
			{
				_cachedScreens[text].Hide();
			}
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			mainCamera.enabled = true;
		}
	}

	public void ReadyTutorial()
	{
		string text = "TutorialPanel";
		if (!_cachedScreens.ContainsKey(text))
		{
			GameObject prefab = Resources.Load("Prefabs/Screens/" + text, typeof(GameObject)) as GameObject;
			_cachedScreens.Add(text, NGUITools.AddChild(backgroundAnchor, prefab).GetComponent<UIBaseScreen>());
			_cachedScreens[text].Init();
		}
		_cachedScreens[text].Show();
		(_cachedScreens[text] as TutorialPanel).Ready();
	}

	public void ShowTutorial(GameObject go, Vector3 offset, float angle)
	{
		string key = "TutorialPanel";
		if (_cachedScreens.ContainsKey(key))
		{
			(_cachedScreens[key] as TutorialPanel).Show(go, offset, angle);
		}
	}

	public void HideTutorial()
	{
		string key = "TutorialPanel";
		if (_cachedScreens.ContainsKey(key))
		{
			_cachedScreens[key].Hide();
		}
	}

	private void _SetCycleAd(string screenName)
	{
		if (!_screenNamesWithCycleAd.Contains(screenName) || screenName.Equals(lastAdScreenName))
		{
			return;
		}
		curAdCount++;
		if (curAdCount >= currentCycleCount)
		{
			currentCycleCount = UnityEngine.Random.Range(minCycleCount, maxCycleCount + 1);
			curAdCount = 0;
			if (!PlayerInfo.Instance.hasRemoveAd && !Game.Instance.show20sAd && Game.Instance.GetNextAdDuration() > 20f)
			{
				Game.Instance.showAdTime = Time.time;
				RiseSdk.Instance.TrackEvent("interstitial_betweenUI", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_betweenUI", 0);
				RiseSdk.Instance.TrackEvent("interstitial_all_success", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_all_success", 0);
				Game.Instance.lastShowAd = "show_interstitial_betweenUI";
				Game.Instance.closePopupOnAdEvent = false;
				if (RiseSdk.Instance.GetRemoteConfigInt("Show_video_inter_loading_config") == 1)
				{
					VideoLoadingPopup.adType = 1;
					VideoLoadingPopup.rewardId = 1;
					PushPopup("VideoLoadingPopup");
				}
				else
				{
					RiseSdk.Instance.ShowAd("custom");
				}
			}
		}
		lastAdScreenName = screenName;
	}

	private void _ShowScreen(string screenName, UIBaseScreen screen)
	{
		_SetBackground(screenName);
		_SetCycleAd(screenName);
		screen.Show();
		if (OnChangedScreen != null)
		{
			OnChangedScreen(screenName);
		}
		ScreenDidChange(screenName);
	}

	private void _SwitchScreen(string screenName)
	{
		_ActivateScreen(screenName);
	}

	public void ActivateAnchors(bool active)
	{
		NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.CelebrationPopupAnchor, active);
		NGUITools.SetActive(UIModelController.Instance.PauseScreenAnchor, active);
		_PauseAnimations(!active, MenuElements3D.transform);
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

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return StartCoroutine(AnimateAlpha(collectText, 0.1f, 1f));
		StartCoroutine(MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
	}

	public void BackToPrevious()
	{
		_BackToPreviousScreen();
	}

	public void CloseActivePopups(string popupToStopAt)
	{
		List<string> list = new List<string>();
		if (_popupQueue.Contains(popupToStopAt))
		{
			int i = 0;
			for (int count = _popupQueue.Count; i < count; i++)
			{
				UIBaseScreen uIBaseScreen = _cachedScreens[_popupQueue[i]];
				if (uIBaseScreen.isActive)
				{
					list.Add(_popupQueue[i]);
					if (_popupQueue[i] == popupToStopAt)
					{
						break;
					}
				}
			}
		}
		int j = 0;
		for (int count2 = list.Count; j < count2; j++)
		{
			_RemovePopup(list[j]);
		}
	}

	public void CloseAllPopups()
	{
		int i = 0;
		for (int count = _popupQueue.Count; i < count; i++)
		{
			_RemovePopup(_popupQueue[i]);
		}
	}

	private string Dequeue(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
			list.RemoveAt(0);
		}
		return result;
	}

	public void GameOverTriggered()
	{
		if (!GetTopScreenName().Equals("GameoverUI"))
		{
			PlayerInfo.Instance.RunCompleted();
			TasksManager.Instance.inRun = false;
			_ActivateScreen("GameoverUI");
		}
	}

	public string GetCurrentPopupName()
	{
		return QueuePeek(_popupQueue);
	}

	public UIBaseScreen GetScreenFromCache(string screenName)
	{
		if (_cachedScreens.ContainsKey(screenName))
		{
			return _cachedScreens[screenName];
		}
		return null;
	}

	public string GetTopScreenName()
	{
		if (_screenStack != null && _screenStack.Count > 0)
		{
			return Peek(_screenStack);
		}
		return null;
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		StartCoroutine(GoToMainMenuFromGame());
	}

	public IEnumerator GoToMainMenuFromGame()
	{
		if (Game.Instance != null)
		{
			TrialManager.Instance.End();
			Game.Instance.ResetTest();
			TasksManager.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		yield return null;
		yield return null;
		_ActivateScreen("FrontUI");
	}

	public bool IsPopupAlreadyQueued(string popupName)
	{
		int i = 0;
		for (int count = _popupQueue.Count; i < count; i++)
		{
			if (_popupQueue[i] == popupName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPopupQueueEmpty()
	{
		return _popupQueue.Count <= 0;
	}

	private void LateUpdate()
	{
		if (_isApplicationResuming)
		{
			_isApplicationResuming = false;
			if (_screenStack.Count > 0 && Peek(_screenStack) == "IngameUI" && (Game.Instance == null || (!Game.Instance.isDead && Game.Instance.IsIntroEnd)))
			{
				PushScreen("PauseUI");
			}
			if (UIScreenController.OnApplicationResumed != null)
			{
				UIScreenController.OnApplicationResumed();
			}
		}
	}

	public static void LogError(string msg, UnityEngine.Object context)
	{
		Debug.LogError(msg, context);
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos__0 = trans.localPosition;
		float factor__2 = 0f;
		while (factor__2 < 1f)
		{
			factor__2 += Time.deltaTime / duration;
			factor__2 = Mathf.Clamp01(factor__2);
			trans.localPosition = Vector3.Lerp(fromPos__0, toPos, factor__2);
			yield return null;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		_gameIsFocused = focus;
		if (!isGetBtnPressed)
		{
			_PauseApplication(!focus);
			if (Application.platform == RuntimePlatform.Android && !focus && Peek(_screenStack) == "IngameUI" && !Game.Instance.isDead)
			{
				Time.timeScale = 0f;
			}
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!isGetBtnPressed)
		{
			_PauseApplication(paused);
			if (Application.platform == RuntimePlatform.WP8Player && paused && Peek(_screenStack) == "IngameUI" && !Game.Instance.isDead)
			{
				Time.timeScale = 0f;
			}
		}
	}

	public void PayoutCelebrationReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			PlayerInfo.Instance.amountOfCoins += reward.amount;
			TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo.Instance.amountOfKeys += reward.amount;
		}
		RewardManager.RewardPayedOut(reward);
		PlayerInfo.Instance.SaveIfDirty();
	}

	private string Peek(List<string> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return string.Empty;
	}

	private string Pop(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		return result;
	}

	public void PushPopup(string name)
	{
		_PushPopup(name);
	}

	public void PushScreen(string screenName)
	{
		_PushScreen(screenName);
	}

	private void _PushScreen(string screenOverride)
	{
		if (!string.IsNullOrEmpty(screenOverride) && (screenOverride != "PauseUI" || (Game.Instance != null && !Game.Instance.isDead)) && (Peek(_screenStack) != "PauseUI" || screenOverride != "IngameUI"))
		{
			_ActivateScreen(screenOverride);
		}
	}

	public void QueueChest()
	{
		if (_popupQueue.Count <= 0 || QueuePeek(_popupQueue) != "CelebrationPopup")
		{
			if (_popupQueue.Count > 0)
			{
				PushPopup("CelebrationPopup");
			}
			else
			{
				_QueuePopup("CelebrationPopup");
			}
		}
	}

	private string QueuePeek(List<string> list)
	{
		string result = string.Empty;
		if (list != null && list.Count > 0)
		{
			result = list[0];
		}
		return result;
	}

	public void QueuePopup(string popupName)
	{
		_QueuePopup(popupName);
	}

	public void QueuePopupRightAfterCurrentActivePopup(string popupName)
	{
		if (!string.IsNullOrEmpty(QueuePeek(_popupQueue)))
		{
			_popupQueue.Insert(1, popupName);
		}
		else
		{
			_QueuePopup(popupName);
		}
	}

	public void RecalculateViewportRectForClipCamera(Camera camera, bool popupSizedClip)
	{
		if (camera != null)
		{
			UICameraScreenClipping component = camera.GetComponent<UICameraScreenClipping>();
			if (component != null)
			{
				component.CalculateClipping(popupSizedClip);
			}
		}
	}

	public void RequeueDelayedPopups()
	{
		int i = 0;
		for (int count = _delayedPopups.Count; i < count; i++)
		{
			QueuePopup(_delayedPopups[i]);
		}
		_delayedPopups.Clear();
	}

	public void SaveAndDelayPopups()
	{
		doSaveAndDelayRemainingPopups = false;
		int i = 0;
		for (int count = _popupQueue.Count; i < count; i++)
		{
			_delayedPopups.Add(_popupQueue[i]);
		}
		_popupQueue.Clear();
	}

	private void ScreenDidChange(string newScreenName)
	{
		if (newScreenName != "IngameUI")
		{
			messageHelper.DisableShowLabel();
		}
	}

	public void ShowMainMenu()
	{
		StartCoroutine(ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		while (!LoadScene.finished)
		{
			yield return null;
		}
		_ActivateScreen("FrontUI");
	}

	public void ShowUnlockAnimationForCharacter(Characters.CharacterType ctype, int themeIndex = 0)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
		celebrationReward.rewardType = CelebrationRewardType.character;
		celebrationReward.characterType = ctype;
		celebrationReward.characterThemeIndex = themeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
		QueueChest();
	}

	public void ShowUnlockAnimationForHelmet(Helmets.HelmType helmType)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.HelmetUnlock;
		celebrationReward.rewardType = CelebrationRewardType.specialHelm;
		celebrationReward.helmType = helmType;
		RewardManager.AddRewardToUnlock(celebrationReward);
		QueueChest();
	}

	public void AddUnlockForCharacterToReward(Characters.CharacterType ctype, int themeIndex = 0)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
		celebrationReward.rewardType = CelebrationRewardType.character;
		celebrationReward.characterType = ctype;
		celebrationReward.characterThemeIndex = themeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
	}

	public void AddUnlockForHelmetToReward(Helmets.HelmType helmType)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.HelmetUnlock;
		celebrationReward.rewardType = CelebrationRewardType.specialHelm;
		celebrationReward.helmType = helmType;
		RewardManager.AddRewardToUnlock(celebrationReward);
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uILabel = NGUITools.AddWidget<UILabel>(superPopupAnchor);
		Utility.SetLayerRecursively(uILabel.gameObject.transform, superPopupAnchor.layer);
		uILabel.text = text;
		uILabel.transform.position = new Vector3(startPosition.x, startPosition.y, uILabel.cachedTransform.position.z);
		uILabel.trueTypeFont = FloatingTextFont;
		uILabel.fontSize = 20;
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.keepCrispWhenShrunk = UILabel.Crispness.Always;
		uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
		uILabel.MakePixelPerfect();
		uILabel.color = new Color(0.9803922f, 66f / 85f, 0.2352941f, 0f);
		uILabel.gameObject.AddComponent<UIPanel>().depth = 11;
		StartCoroutine(AnimateCollectText(uILabel));
	}

	private void Start()
	{
		if (LoadMenuOnStart)
		{
			ShowMainMenu();
		}
		PlayerInfo.Instance.BragCompleted();
		UpdateNguiTouchThresholds();
	}

	public void SwitchScreen(string screenName)
	{
		_SwitchScreen(screenName);
	}

	private void UpdateNguiTouchThresholds()
	{
		float num = 0f;
		float num2 = 0f;
		num = ((!(Screen.dpi <= 0f)) ? (0.1f * Screen.dpi) : 30f);
		num2 = ((!(Screen.dpi <= 0f)) ? (0.1f * Screen.dpi) : 30f);
		nguiCamera.touchDragThreshold = num;
		nguiCamera.touchClickThreshold = num2;
	}
}
