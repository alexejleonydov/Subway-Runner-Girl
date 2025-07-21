using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RiseSdk
{
	public enum AdEventType
	{
		FullAdLoadCompleted = 1,
		FullAdLoadFailed = 2,
		RewardAdLoadFailed = 3,
		RewardAdLoadCompleted = 4,
		RewardAdShowStart = 5,
		RewardAdShowFinished = 6,
		RewardAdShowFailed = 7,
		RewardAdClosed = 8,
		VideoAdClicked = 9,
		FullAdClosed = 10,
		FullAdShown = 11,
		FullAdClicked = 12,
		BannerAdClicked = 13,
		CrossAdClicked = 14,
		AdLoadCompleted = 15,
		AdLoadFailed = 16,
		AdShown = 17,
		AdClosed = 18,
		AdClicked = 19,
		IconAdClicked = 20,
		NativeAdClicked = 21
	}

	public enum PaymentResult
	{
		Success = 1,
		Failed = 2,
		Cancel = 3,
		PaymentSystemError = 4,
		PaymentSystemValid = 5
	}

	public enum SnsEventType
	{
		LoginSuccess = 1,
		LoginFailed = 2,
		InviteSuccess = 3,
		InviteFailed = 4,
		ChallengeSuccess = 5,
		ChallengeFailed = 6,
		LikeSuccess = 7,
		LikeFailed = 8,
		ShareSuccess = 9,
		ShareFailed = 10,
		ShareCancel = 11
	}

	public enum LocalPushType
	{
		NoCycle = 0,
		YearCycle = 4,
		MonthCycle = 8,
		DayCycle = 0x10,
		HourCycle = 0x20,
		MinuteCycle = 0x40,
		SecondCycle = 0x80,
		WeekDayCycle = 0x200,
		WeekDayOrDinalCycle = 0x400
	}

	private class RiseEditorAd : MonoBehaviour
	{
		private static RiseEditorAd _editorAdInstance;

		public static bool hasInit;

		private Rect bannerPos;

		private bool bannerShow;

		private string bannerContent = string.Empty;

		private bool interstitialShow;

		private string interstitialContent = string.Empty;

		private bool rewardShow;

		private string rewardContent = string.Empty;

		private float scaleWidth = 1f;

		private float scaleHeight = 1f;

		private int originScreenWidth = 1;

		private int originScreenHeight = 1;

		private bool toastShow;

		private List<string> toastList = new List<string>();

		private GUIStyle toastStyle;

		private int rewardAdId = -10;

		private string rewardAdTag = "DEFAULT";

		private float iconAdWidth = 56f;

		private float iconAdXPercent = 0.2f;

		private float iconAdYPercent = 0.2f;

		private bool iconAdShow;

		private string iconAdContent = "Icon Ad";

		private EventSystem curEvent;

		private const int NONE_REWARD_ID = -10;

		private const string DEFAULT_REWARD_TAG = "DEFAULT";

		private const string BANNER_DEFAULT_TXT = "Banner AD";

		private const string INTERSTITIAL_DEFAULT_TXT = "\nInterstitial AD Test";

		private const string REWARD_DEFAULT_TXT = "Free Coin AD Test: ";

		private const int SCREEN_WIDTH = 854;

		private const int SCREEN_HEIGHT = 480;

		private const int GUI_DEPTH = -99;

		private const int BANNER_WIDTH = 320;

		private const int BANNER_HEIGHT = 50;

		private bool timeCounting;

		public static RiseEditorAd EditorAdInstance
		{
			get
			{
				if (_editorAdInstance == null)
				{
					_editorAdInstance = ((!(UnityEngine.Object.FindObjectOfType<RiseEditorAd>() == null)) ? _editorAdInstance : new GameObject("RiseEditorAd").AddComponent<RiseEditorAd>());
				}
				if (!hasInit)
				{
					Debug.LogError("Fatal Error: \nNeed Call RiseSdk.Instance.Init () First At Initialize Scene");
				}
				return _editorAdInstance;
			}
		}

		private void Awake()
		{
			if (_editorAdInstance == null)
			{
				_editorAdInstance = this;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Screen.width > Screen.height)
			{
				originScreenWidth = 854;
				originScreenHeight = 480;
			}
			else
			{
				originScreenWidth = 480;
				originScreenHeight = 854;
			}
			scaleWidth = (float)Screen.width * 1f / (float)originScreenWidth;
			scaleHeight = (float)Screen.height * 1f / (float)originScreenHeight;
			toastStyle = new GUIStyle();
			toastStyle.fontStyle = FontStyle.Bold;
			toastStyle.alignment = TextAnchor.MiddleCenter;
			toastStyle.fontSize = 30;
		}

		public void ShowBanner(int pos)
		{
		}

		public void ShowBanner(string tag, int pos)
		{
		}

		public void ShowBanner(string tag, int pos, int animate)
		{
		}

		public void CloseBanner()
		{
		}

		private void SetBannerPos(int pos)
		{
		}

		public void ShowAd(string tag)
		{
		}

		public void ShowRewardAd(int id)
		{
		}

		public void ShowRewardAd(string tag, int id)
		{
		}

		public void ShowIconAd(float width, float xPercent, float yPercent)
		{
			iconAdShow = true;
			iconAdWidth = width;
			iconAdXPercent = xPercent;
			iconAdYPercent = yPercent;
		}

		public void CloseIconAd()
		{
			iconAdShow = false;
		}

		public void Pay(int billingId)
		{
		}

		public void Toast(string msg)
		{
		}

		private IEnumerator CheckToast(float time = 2f)
		{
			yield return new WaitForSeconds(time);
			if (toastList.Count > 0)
			{
				toastList.RemoveAt(0);
			}
			if (toastList.Count > 0)
			{
				StartCoroutine(CheckToast());
			}
			else
			{
				timeCounting = false;
			}
		}

		public void Alert(string title, string msg)
		{
		}

		public void OnExit()
		{
		}
	}

	private class FileLRUCache
	{
		public enum FileType
		{
			Image = 0,
			Text = 1
		}

		private int maxCapacity = 10;

		private int size;

		private LinkedNode head;

		private LinkedNode tail;

		private Dictionary<string, LinkedNode> cache;

		private const string CACHE_FILE = "filedirmeta";

		private const string SPLIT_FLAG = "@^@";

		private const string KEY_VALUE_SPLIT_FLAG = "^_^";

		private static string defFilePath = "/";

		private Coroutine writting;

		public FileLRUCache(int capacity)
		{
			maxCapacity = capacity;
			cache = new Dictionary<string, LinkedNode>();
			head = new LinkedNode();
			tail = new LinkedNode();
			head.prev = null;
			head.next = tail;
			tail.prev = head;
			tail.next = null;
			try
			{
				defFilePath = Application.persistentDataPath + "/filecache/";
				if (!Directory.Exists(defFilePath))
				{
					Directory.CreateDirectory(defFilePath);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("FileLRUCache init error\n" + ex.StackTrace);
				defFilePath = Application.persistentDataPath + "/filecache/";
			}
			finally
			{
				if (!Directory.Exists(defFilePath))
				{
					Directory.CreateDirectory(defFilePath);
				}
				string text = defFilePath + "filedirmeta";
				if (!File.Exists(text))
				{
					File.Create(text);
					RiseSdkListener.Instance.StartCoroutine(delayLoad(text, 1f));
				}
				else
				{
					LoadLocalFile(text, loadCache);
				}
			}
		}

		public void DownloadFile(string url, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(url))
			{
				if (resultEvent != null)
				{
					resultEvent(string.Empty, null);
				}
				return;
			}
			string text = CalculateMD5Hash(url);
			if (!File.Exists(defFilePath + text))
			{
				RiseSdkListener.Instance.StartCoroutine(Download(url, text, resultEvent));
			}
			else
			{
				RiseSdkListener.Instance.StartCoroutine(LoadLocal(null, text, resultEvent));
			}
		}

		public void LoadLocalFile(string filePath, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				if (resultEvent != null)
				{
					resultEvent(filePath, null);
				}
			}
			else if (File.Exists(filePath))
			{
				RiseSdkListener.Instance.StartCoroutine(LoadLocal(filePath, null, resultEvent));
			}
			else if (resultEvent != null)
			{
				resultEvent(filePath, null);
			}
		}

		public bool FileDownloaded(string url)
		{
			string text = CalculateMD5Hash(url);
			return File.Exists(defFilePath + text);
		}

		private IEnumerator Download(string url, string saveName, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(url))
			{
				if (resultEvent != null)
				{
					resultEvent(defFilePath + saveName, null);
					Debug.LogWarning("Download File error, url: " + url + ", saveName: " + saveName);
				}
				yield break;
			}
			WWW www = new WWW(url);
			yield return www;
			set(saveName, defFilePath + saveName);
			if (string.IsNullOrEmpty(www.error))
			{
				if (www.bytes != null && www.bytes.Length > 1000)
				{
					byte[] bytes = www.bytes;
					File.WriteAllBytes(defFilePath + saveName, bytes);
					if (resultEvent != null)
					{
						resultEvent(defFilePath + saveName, www);
					}
				}
				else if (resultEvent != null)
				{
					resultEvent(defFilePath + saveName, null);
				}
			}
			else if (resultEvent != null)
			{
				resultEvent(defFilePath + saveName, null);
				Debug.LogError("Download File error, url: " + url + ", saveName: " + saveName + ", www.error: " + www.error);
			}
		}

		private IEnumerator LoadLocal(string filePath, string saveName, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				if (resultEvent != null)
				{
					resultEvent(filePath + saveName, null);
					Debug.LogWarning("LoadLocal File error, filePath: " + filePath + ", saveName: " + saveName);
				}
				yield break;
			}
			if (string.IsNullOrEmpty(filePath))
			{
				filePath = defFilePath;
			}
			if (saveName == null)
			{
				saveName = string.Empty;
			}
			string path = "file:///" + filePath + saveName;
			WWW www = new WWW(path);
			yield return www;
			set(saveName, filePath + saveName);
			if (string.IsNullOrEmpty(www.error))
			{
				if (www.bytes != null && www.bytes.Length > 1000)
				{
					if (resultEvent != null)
					{
						resultEvent(filePath + saveName, www);
					}
				}
				else if (resultEvent != null)
				{
					resultEvent(filePath + saveName, null);
				}
			}
			else if (resultEvent != null)
			{
				resultEvent(filePath + saveName, null);
				Debug.LogError("LoadLocal File error, filePath: " + filePath + ", saveName: " + saveName + ", www.error: " + www.error);
			}
		}

		private IEnumerator delayLoad(string path, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			LoadLocalFile(path, loadCache);
		}

		private void loadCache(string path, WWW www)
		{
			if (www == null)
			{
				return;
			}
			string text = www.text;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string[] array = text.Split("@^@".ToCharArray());
			LinkedNode linkedNode = null;
			LinkedNode linkedNode2 = null;
			size = 0;
			string[] array2 = null;
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				array2 = null;
				if (!string.IsNullOrEmpty(array[i]))
				{
					array2 = array[i].Split("^_^".ToCharArray());
				}
				if (array2 != null && array2.Length > 1 && !string.IsNullOrEmpty(array2[0]) && !string.IsNullOrEmpty(array2[1]))
				{
					linkedNode = new LinkedNode();
					linkedNode.key = array2[0];
					linkedNode.value = array2[1];
					linkedNode2 = tail.prev;
					tail.prev = linkedNode;
					linkedNode.next = tail;
					linkedNode.prev = linkedNode2;
					linkedNode2.next = linkedNode;
					cache.Add(array2[0], linkedNode);
					size++;
				}
			}
		}

		private void writeCache()
		{
			if (writting != null)
			{
				RiseSdkListener.Instance.StopCoroutine(writting);
			}
			writting = RiseSdkListener.Instance.StartCoroutine(delayWrite());
		}

		private IEnumerator delayWrite()
		{
			yield return new WaitForSeconds(1f);
			string str = string.Empty;
			LinkedNode node = head.next;
			while (node != null && node != tail)
			{
				string text = str;
				str = text + node.key + "^_^" + node.value + "@^@";
				node = node.next;
			}
			File.WriteAllText(contents: str.Remove(str.Length - "@^@".Length, "@^@".Length), path: defFilePath + "filedirmeta", encoding: Encoding.UTF8);
		}

		private void set(string key, string value)
		{
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
			{
				LinkedNode linkedNode = null;
				if (cache.ContainsKey(key))
				{
					linkedNode = cache[key];
					moveToFront(linkedNode);
				}
				else
				{
					linkedNode = new LinkedNode();
					linkedNode.key = key;
					linkedNode.value = value;
					linkAtFront(linkedNode);
					cache.Add(key, linkedNode);
					size++;
				}
				checkCapacity();
				writeCache();
			}
		}

		private void checkCapacity()
		{
			while (size > maxCapacity)
			{
				size--;
				removeLast();
			}
		}

		private string get(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				return cache[key].value;
			}
			return string.Empty;
		}

		private void linkAtFront(LinkedNode node)
		{
			LinkedNode next = head.next;
			head.next = node;
			node.prev = head;
			node.next = next;
			next.prev = node;
		}

		private void moveToFront(LinkedNode node)
		{
			LinkedNode prev = node.prev;
			LinkedNode next = node.next;
			if (prev != null && next != null)
			{
				prev.next = next;
				next.prev = prev;
				linkAtFront(node);
			}
		}

		private void removeLast()
		{
			LinkedNode prev = tail.prev;
			LinkedNode prev2 = prev.prev;
			if (prev != head && prev2 != null)
			{
				prev2.next = tail;
				tail.prev = prev2;
				cache.Remove(prev.key);
				File.Delete(prev.value);
			}
		}
	}

	private class LinkedNode
	{
		public string key;

		public string value;

		public LinkedNode prev;

		public LinkedNode next;
	}

	private static RiseSdk _instance;

	private AndroidJavaClass _class;

	private bool paymentSystemValid;

	private string BACK_HOME_ADPOS = "custom";

	private bool BACK_HOME_AD_ENABLE;

	private double BACK_HOME_AD_TIME;

	private bool canShowBackHomeAd;

	private int homeAdMinPauseMillisecond = 1000;

	private double pauseTime;

	private FileLRUCache lruCache;

	public const int POS_BANNER_LEFT_TOP = 1;

	public const int POS_BANNER_MIDDLE_TOP = 3;

	public const int POS_BANNER_RIGHT_TOP = 6;

	public const int POS_BANNER_MIDDLE_MIDDLE = 5;

	public const int POS_BANNER_LEFT_BOTTOM = 2;

	public const int POS_BANNER_MIDDLE_BOTTOM = 4;

	public const int POS_BANNER_RIGHT_BOTTOM = 7;

	public const int POS_BANNER_LEFT_MIDDLE = 8;

	public const int POS_BANNER_RIGHT_MIDDLE = 9;

	public const int ANIMATE_BANNER_NONE = 0;

	public const int ANIMATE_BANNER_TOP = 1;

	public const int ANIMATE_BANNER_BOTTOM = 2;

	public const int ANIMATE_BANNER_LEFT = 4;

	public const int ANIMATE_BANNER_RIGHT = 8;

	public const int ANIMATE_BANNER_ROTATION = 16;

	public const string M_START = "start";

	public const string M_PAUSE = "pause";

	public const string M_PASSLEVEL = "custom";

	public const string M_PASSLEVEL_1 = "passlevel1";

	public const string M_CUSTOM = "custom";

	public const int PAYMENT_RESULT_SUCCESS = 1;

	public const int PAYMENT_RESULT_FAILS = 2;

	public const int PAYMENT_RESULT_CANCEL = 3;

	public const int CONFIG_KEY_APP_ID = 1;

	public const int CONFIG_KEY_LEADER_BOARD_URL = 2;

	public const int CONFIG_KEY_API_VERSION = 3;

	public const int CONFIG_KEY_SCREEN_WIDTH = 4;

	public const int CONFIG_KEY_SCREEN_HEIGHT = 5;

	public const int CONFIG_KEY_LANGUAGE = 6;

	public const int CONFIG_KEY_COUNTRY = 7;

	public const int CONFIG_KEY_VERSION_CODE = 8;

	public const int CONFIG_KEY_VERSION_NAME = 9;

	public const int CONFIG_KEY_PACKAGE_NAME = 10;

	public const int CONFIG_KEY_UUID = 11;

	public const int ADTYPE_OTHER = -1;

	public const int ADTYPE_INTERTITIAL = 1;

	public const int ADTYPE_VIDEO = 2;

	public const int ADTYPE_BANNER = 3;

	public const int ADTYPE_ICON = 4;

	public const int ADTYPE_NATIVE = 5;

	public static RiseSdk Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RiseSdk();
			}
			return _instance;
		}
	}

	public void SetPaymentSystemValid(bool valid)
	{
		paymentSystemValid = valid;
	}

	public void Init()
	{
		RiseEditorAd.hasInit = true;
		/*if (_class != null)
		{
			return;
		}
		try
		{
			RiseSdkListener.Instance.enabled = true;
			_class = new AndroidJavaClass("com.android.client.Unity");
			if (_class == null)
			{
				return;
			}
			AndroidJNIHelper.debug = true;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					_class.CallStatic("onCreate", androidJavaObject);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("RiseSdk Init Error:::\n" + ex.StackTrace + "\n" + ex.Message);
			_class = null;
		}
		finally
		{
			lruCache = new FileLRUCache(20);
		}*/
	}

	public int GetScreenWidth()
	{
		return Screen.width;
	}

	public int GetScreenHeight()
	{
		return Screen.height;
	}

	public void ShowBanner(string tag, int pos)
	{
		/*if (_class != null)
		{
			_class.CallStatic("showBanner", tag, pos);
			Debug.LogWarning("showBanner");
		}*/
	}

	public void ShowBanner(int pos)
	{
		/*if (_class != null)
		{
			_class.CallStatic("showBanner", pos);
			Debug.LogWarning("showBanner");
		}*/
	}

	public void ShowBanner(string tag, int pos, int animate)
	{
		/*if (_class != null)
		{
			_class.CallStatic("showBanner", tag, pos, animate);
			Debug.LogWarning("showBanner");
		}*/
	}

	public bool HasBanner(string tag)
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("hasBanner", new object[1] { tag });
		}
		return false;
	}

	public void CloseBanner()
	{
		//if (_class != null)
		//{
		//	_class.CallStatic("closeBanner");
		//}
	}

	public void ShowAd(string tag)
	{
		/*BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("showFullAd", tag);
		}*/
	}

	public bool HasInterstitial(string tag)
	{
		/*if (_class != null)
		{
			return _class.CallStatic<bool>("hasFull", new object[1] { tag });
		}*/
		return false;
	}

	public void ShowMore()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("moreGame");
		}
	}

	public bool HasRewardAd()
	{
		//if (_class != null)
		//{
		//	return _class.CallStatic<bool>("hasRewardAd", new object[0]);
		//}
		//return false;
		return true;
	}

	public bool HasRewardAd(string tag)
	{
		if (_class != null)
		{
			//return _class.CallStatic<bool>("hasRewardAd", new object[1] { tag });
		}
		return false;
	}

	public void ShowRewardAd(int rewardId)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			//_class.CallStatic("showRewardAd", rewardId);
		}
	}

	public void ShowRewardAd(string tag, int rewardId)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			//_class.CallStatic("showRewardAd", tag, rewardId);
		}
	}

	public void enableBackHomeAd(bool enabled, string adPos, int minPauseMillisecond = 20000)
	{
		BACK_HOME_ADPOS = adPos;
		BACK_HOME_AD_ENABLE = enabled;
		homeAdMinPauseMillisecond = minPauseMillisecond;
	}

	public void OnResume()
	{
		if (_class != null)
		{
			//_class.CallStatic("onResume");
		}
		if (BACK_HOME_AD_ENABLE && canShowBackHomeAd && BACK_HOME_AD_TIME <= 0.0)
		{
			canShowBackHomeAd = false;
			if (GetCurrentTimeInMills() - pauseTime > (double)homeAdMinPauseMillisecond)
			{
				RiseSdkListener.Instance.OnResumeAd();
				ShowAd(BACK_HOME_ADPOS);
			}
		}
	}

	public void OnPause()
	{
		if (_class != null)
		{
			//_class.CallStatic("onPause");
		}
		if (BACK_HOME_AD_ENABLE)
		{
			double currentTimeInMills = GetCurrentTimeInMills();
			double num = currentTimeInMills - BACK_HOME_AD_TIME;
			canShowBackHomeAd = num > 2000.0;
			if (canShowBackHomeAd)
			{
				BACK_HOME_AD_TIME = 0.0;
			}
			pauseTime = currentTimeInMills;
		}
	}

	public void OnStart()
	{
		if (_class != null)
		{
			_class.CallStatic("onStart");
		}
	}

	public void OnStop()
	{
		if (_class != null)
		{
			_class.CallStatic("onStop");
		}
	}

	public void OnDestroy()
	{
		if (_class != null)
		{
			_class.CallStatic("onDestroy");
		}
	}

	public void OnExit()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("onQuit");
		}
	}

	public void HasPaid(int billingId)
	{
		if (_class != null)
		{
			//_class.CallStatic("query", billingId);
		}
	}

	public bool IsPayEnabled()
	{
		return paymentSystemValid;
	}

	public void Pay(int billingId)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
		//	_class.CallStatic("pay", billingId);
		}
	}

	public void Share()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			//_class.CallStatic("share");
		}
	}

	public string GetExtraData()
	{
		if (_class == null)
		{
			return null;
		}
		return _class.CallStatic<string>("getExtraData", new object[0]);
	}

	public void TrackEvent(string category, string action, string label, int value)
	{
		if (_class != null)
		{
			//_class.CallStatic("trackEvent", category, action, label, value);
		}
	}

	public void TrackEvent(string category, string keyValueData)
	{
		if (_class != null)
		{
			//_class.CallStatic("trackEvent", category, keyValueData);
		}
	}

	public void TrackFinishLevel(string level)
	{
		if (_class != null)
		{
			//_class.CallStatic("logFinishLevel", level);
		}
	}

	public void TrackFinishAchievement(string achievement)
	{
		if (_class != null)
		{
			//_class.CallStatic("logFinishAchievement", achievement);
		}
	}

	public void TrackFinishTutorial(string tutorial)
	{
		if (_class != null)
		{
			//_class.CallStatic("logFinishTutorial", tutorial);
		}
	}

	public void Rate()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			//_class.CallStatic("rate");
		}
	}

	public void ShowNativeAd(string tag, int yPercent)
	{
		if (_class != null)
		{
			//_class.CallStatic("showNative", tag, yPercent);
		}
	}

	public bool ShowNativeAd(string tag, int xPixel, int yPixel, string configJson)
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("showNativeBanner", new object[4] { tag, xPixel, yPixel, configJson });
		}
		return false;
	}

	public bool ShowNativeAdWithFrame(string tag, float xPixel, float yPixel, float width, float height, string configJson)
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("showNativeBanner", new object[6] { tag, xPixel, yPixel, width, height, configJson });
		}
		return false;
	}

	public void CloseNativeAd(string tag)
	{
		if (_class != null)
		{
			_class.CallStatic("closeNativeBanner", tag);
		}
	}

	public void HideNativeAd(string tag)
	{
		if (_class != null)
		{
			_class.CallStatic("hideNative", tag);
		}
	}

	public bool HasNativeAd(string tag)
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("hasNative", new object[1] { tag });
		}
		return false;
	}

	public void ShowDeliciousIconAd(float x, float y, float w, float h, string configJson)
	{
		if (_class != null)
		{
			_class.CallStatic("showDeliciousIconAd", (int)x, (int)y, (int)w, (int)h, configJson);
		}
	}

	public void CloseDeliciousIconAd()
	{
		if (_class != null)
		{
			_class.CallStatic("closeDeliciousIconAd");
		}
	}

	public void ShowDeliciousBannerAd(float x, float y, float w, float h, string configJson)
	{
		if (_class != null)
		{
			_class.CallStatic("showDeliciousBannerAd", (int)x, (int)y, (int)w, (int)h, configJson);
		}
	}

	public void CloseDeliciousBannerAd()
	{
		if (_class != null)
		{
			_class.CallStatic("closeDeliciousBannerAd");
		}
	}

	public void ShowDeliciousVideoAd(string configJson)
	{
		if (_class != null)
		{
			_class.CallStatic("showDeliciousVideoAd", configJson);
		}
	}

	public bool HasDeliciousAd()
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("hasDeliciousAd", new object[0]);
		}
		return false;
	}

	public void Login()
	{
		if (_class != null)
		{
			_class.CallStatic("login");
		}
	}

	public bool IsLogin()
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("isLogin", new object[0]);
		}
		return false;
	}

	public void Logout()
	{
		if (_class != null)
		{
			_class.CallStatic("logout");
		}
	}

	public void Invite()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("invite");
		}
	}

	public void Challenge(string title, string message)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("challenge", title, message);
		}
	}

	public string Me()
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("me", new object[0]);
		}
		return string.Empty;
	}

	public string GetFriends()
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("friends", new object[0]);
		}
		return string.Empty;
	}

	public void Like()
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("like");
		}
	}

	public string GetMePictureURL()
	{
		string text = Me();
		string result = string.Empty;
		if (!string.IsNullOrEmpty(text) && text.Length > 5)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)RiseJson.Deserialize(text);
			if (dictionary.ContainsKey("picture"))
			{
				result = dictionary["picture"].ToString();
			}
		}
		return result;
	}

	public string GetPaymentDatas()
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("getPaymentDatas", new object[0]);
		}
		return "{}";
	}

	public string GetPaymentData(int billingId)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("getPaymentData", new object[1] { billingId });
		}
		return "{}";
	}

	public bool IsPaymentValid()
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("isPaymentValid", new object[0]);
		}
		return false;
	}

	public string GetConfig(int configId)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("getConfig", new object[1] { configId });
		}
		return "0";
	}

	public string CacheUrl(string url)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("cacheUrl", new object[1] { url });
		}
		return string.Empty;
	}

	public void CacheUrl(int tag, string url)
	{
		if (_class != null)
		{
			_class.CallStatic("cacheUrl", tag, url);
		}
	}

	public bool HasApp(string packageName)
	{
		if (_class == null)
		{
			return false;
		}
		return _class.CallStatic<bool>("hasApp", new object[1] { packageName });
	}

	public void LaunchApp(string packageName)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("launchApp", packageName);
		}
	}

	public void GetApp(string packageName)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("getApp", packageName);
		}
	}

	public string GetConfig(string packageName, int configId)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("getConfig", new object[2] { packageName, configId });
		}
		return string.Empty;
	}

	public void Alert(string title, string message)
	{
		BACK_HOME_AD_TIME = GetCurrentTimeInMills();
		if (_class != null)
		{
			_class.CallStatic("alert", title, message);
		}
	}

	public void Toast(string message)
	{
		if (_class != null)
		{
			_class.CallStatic("toast", message);
		}
	}

	public bool IsNetworkConnected()
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("isNetworkConnected", new object[0]);
		}
		return true;
	}

	public bool HasGDPR()
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("hasGDPR", new object[0]);
		}
		return true;
	}

	public void ResetGDPR()
	{
		if (_class != null)
		{
			_class.CallStatic("resetGDPR");
		}
	}

	public void Suport(string email, string data)
	{
		if (_class != null)
		{
			_class.CallStatic("support", email, data);
		}
	}

	public void PushNotification(string key, string title, string content, int pushTime, bool localTimeZone, string fbIds, string uuids, string topics, int iosBadge, bool useSound, string soundName, string userInfo)
	{
		if (_class != null)
		{
			_class.CallStatic("pushMessage", key, title, content, pushTime, localTimeZone, fbIds, uuids, topics, iosBadge, useSound, soundName, userInfo);
		}
	}

	public void PushLocalNotification(string key, string title, string content, int pushTime, int interval, bool useSound, string soundName, string userInfo)
	{
		if (_class != null)
		{
			_class.CallStatic("pushLocalMessage", key, title, content, pushTime, interval, useSound, soundName, userInfo);
		}
	}

	public string EncodeParams(string dataStr)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("encodeParams", new object[1] { dataStr });
		}
		return string.Empty;
	}

	public int GetRemoteConfigInt(string remoteKey)
	{
		if (_class != null)
		{
			return _class.CallStatic<int>("getRemoteConfigInt", new object[1] { remoteKey });
		}
		return 0;
	}

	public long GetRemoteConfigLong(string remoteKey)
	{
		if (_class != null)
		{
			return _class.CallStatic<long>("getRemoteConfigLong", new object[1] { remoteKey });
		}
		return 0L;
	}

	public double GetRemoteConfigDouble(string remoteKey)
	{
		if (_class != null)
		{
			return _class.CallStatic<double>("getRemoteConfigDouble", new object[1] { remoteKey });
		}
		return 0.0;
	}

	public bool GetRemoteConfigBoolean(string remoteKey)
	{
		if (_class != null)
		{
			return _class.CallStatic<bool>("getRemoteConfigBoolean", new object[1] { remoteKey });
		}
		return false;
	}

	public string GetRemoteConfigString(string remoteKey)
	{
		if (_class != null)
		{
			return _class.CallStatic<string>("getRemoteConfigString", new object[1] { remoteKey });
		}
		return string.Empty;
	}

	public void SetUserTag(string tag)
	{
		if (_class != null)
		{
			_class.CallStatic("setUserTag", tag);
		}
	}

	public void SetUserProperty(string key, string value)
	{
		if (_class != null)
		{
			_class.CallStatic("setUserProperty", key, value);
		}
	}

	public void UM_setPlayerLevel(int level)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_setPlayerLevel", level);
		}
	}

	public void UM_onEvent(string eventId)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_onEvent", eventId);
		}
	}

	public void UM_onEvent(string eventId, string eventLabel)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_onEvent", eventId, eventLabel);
		}
	}

	public void UM_onEventValue(string eventId, Dictionary<string, string> mapStr)
	{
		if (_class == null)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = null;
		if (mapStr != null)
		{
			try
			{
				androidJavaObject = new AndroidJavaObject("java.util.Map");
				foreach (KeyValuePair<string, string> item in mapStr)
				{
					androidJavaObject.Call<string>("put", new object[2] { item.Key, item.Value });
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("UM_onEventValue Exception msg:\n" + ex.StackTrace);
			}
		}
		_class.CallStatic("UM_onEventValue", androidJavaObject, 1);
	}

	public void UM_onPageStart(string pageName)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_onPageStart", pageName);
		}
	}

	public void UM_onPageEnd(string pageName)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_onPageEnd", pageName);
		}
	}

	public void UM_startLevel(string level)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_startLevel", level);
		}
	}

	public void UM_failLevel(string level)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_failLevel", level);
		}
	}

	public void UM_finishLevel(string level)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_finishLevel", level);
		}
	}

	public void UM_pay(double money, string itemName, int number, double price)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_pay", money, itemName, number, price);
		}
	}

	public void UM_buy(string itemName, int count, double price)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_buy", itemName, count, price);
		}
	}

	public void UM_use(string itemName, int number, double price)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_use", itemName, number, price);
		}
	}

	public void UM_bonus(string itemName, int number, double price, int trigger)
	{
		if (_class != null)
		{
			_class.CallStatic("UM_bonus", itemName, number, price, trigger);
		}
	}

	public string GetMeFirstName()
	{
		return "FirstName";
	}

	public string GetMeLastName()
	{
		return "LastName";
	}

	public string GetMeId()
	{
		return "MeId";
	}

	public string GetMeName()
	{
		return "MeName";
	}

	public void FetchFriends(bool invitable)
	{
	}

	public void FetchScores()
	{
	}

	public void Share(string contentURL, string tag, string quote)
	{
	}

	public void RestorePayments()
	{
	}

	public void SdkLog(string message)
	{
	}

	public void LoadAd(string tag)
	{
	}

	public void ShowPopupIconAd()
	{
	}

	public string GetPushData()
	{
		return "{}";
	}

	public void DownloadFile(string url, Action<string, WWW> resultEvent)
	{
		lruCache.DownloadFile(url, resultEvent);
	}

	public void LoadLocalFile(string filePath, Action<string, WWW> resultEvent)
	{
		lruCache.LoadLocalFile(filePath, resultEvent);
	}

	public static double GetCurrentTimeInMills()
	{
		return DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
	}

	public static string CalculateMD5Hash(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			bytes = mD.ComputeHash(bytes);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString("X2"));
			}
			return stringBuilder.ToString().ToLower();
		}
		catch (Exception ex)
		{
			Debug.LogError("CalculateMD5Hash error:\n" + ex.StackTrace);
		}
		finally
		{
		}
		return stringBuilder.ToString();
	}
}
