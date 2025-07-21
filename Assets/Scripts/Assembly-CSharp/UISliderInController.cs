using System;
using System.Collections.Generic;
using UnityEngine;

public class UISliderInController : MonoBehaviour
{
	public class SlideIn
	{
		private string _payload;

		private int _payloadInt;

		private SlideInType _type;

		public string payload
		{
			get
			{
				return _payload;
			}
		}

		public int payloadInt
		{
			get
			{
				return _payloadInt;
			}
		}

		public SlideInType type
		{
			get
			{
				return _type;
			}
		}

		public SlideIn(SlideInType type)
		{
			_payload = string.Empty;
			_type = type;
		}

		public SlideIn(SlideInType type, int PayLoadInt)
		{
			_payload = string.Empty;
			_type = type;
			_payloadInt = PayLoadInt;
		}

		public SlideIn(SlideInType type, string payload)
		{
			_payloadInt = 0;
			_type = type;
			_payload = payload;
		}
	}

	public enum SlideInType
	{
		TopRunTip = 0,
		Task = 1,
		TaskSet = 2,
		Achievement = 3,
		Unlock = 4,
		ErrorMessage = 5
	}

	public static UISliderInController Instance;

	[SerializeField]
	private UIMessageHelper messageHelper;

	[SerializeField]
	private UISlideInTaskHelper taskHelperSlide;

	public UISlideInTaskSetHelper taskSetHelperSlide;

	[SerializeField]
	private UISlideInUnlock unlockSlide;

	[SerializeField]
	private UISlideInErrorMessage errorMessageSlide;

	[SerializeField]
	private UISlideInTopRunTip topRunTipSlide;

	private bool slideInActive;

	private bool messageIsShowing;

	private bool stopping;

	private List<SlideIn> numOfTaskSetSlide = new List<SlideIn>();

	private List<SlideIn> queueSliderIn = new List<SlideIn>();

	private Queue<string> queueMessage = new Queue<string>();

	private bool hasPreloadAllSlide;

	public bool Stop
	{
		get
		{
			return stopping;
		}
		set
		{
			stopping = value;
			if (!stopping)
			{
				ReadyForNextSlide();
			}
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			UIAnchor component = base.transform.GetComponent<UIAnchor>();
			component.pixelOffset = new Vector2(0f, -132f);
		}
		else
		{
			UIScreenController.Instance.OnChangedScreen = (Action<string>)Delegate.Combine(UIScreenController.Instance.OnChangedScreen, new Action<string>(OnChangedScreen));
		}
		stopping = false;
		slideInActive = false;
		TasksManager.Instance.onTaskComplete = (TasksManager.TaskCompleteHandler)Delegate.Combine(TasksManager.Instance.onTaskComplete, new TasksManager.TaskCompleteHandler(OnTaskCompleted));
		TasksManager.Instance.onTaskSetComplete = (TasksManager.TaskSetCompleteHandler)Delegate.Combine(TasksManager.Instance.onTaskSetComplete, new TasksManager.TaskSetCompleteHandler(OnTaskSetCompleted));
		TasksManager.Instance.onAchievementComplete = (TasksManager.AchievementCompleteHandler)Delegate.Combine(TasksManager.Instance.onAchievementComplete, new TasksManager.AchievementCompleteHandler(OnAchievementCompleted));
		PlayerInfo.Instance.OnSymbolCollected = (Action<Characters.CharacterType>)Delegate.Combine(PlayerInfo.Instance.OnSymbolCollected, new Action<Characters.CharacterType>(OnSymbolPickUp));
		PreloadAllSlide();
	}

	private void OnChangedScreen(string screenName)
	{
		UIAnchor component = base.transform.GetComponent<UIAnchor>();
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			component.pixelOffset = new Vector2(0f, 0f - UIScreenController.Instance.bannerHeight);
		}
		else
		{
			component.pixelOffset = Vector2.zero;
		}
	}

	private void PreloadAllSlide()
	{
		if (!hasPreloadAllSlide)
		{
			hasPreloadAllSlide = true;
			StartCoroutine(taskHelperSlide.PreloadSlideIn());
			StartCoroutine(taskSetHelperSlide.PreloadSlideIn());
			StartCoroutine(unlockSlide.PreloadSlideIn());
			StartCoroutine(errorMessageSlide.PreloadSlideIn());
			StartCoroutine(topRunTipSlide.PreloadSlideIn());
		}
	}

	private void OnTaskCompleted(string message)
	{
		QueueSlideIn(new SlideIn(SlideInType.Task, message));
	}

	private void OnAchievementCompleted(string message)
	{
		QueueSlideIn(new SlideIn(SlideInType.Achievement, message));
	}

	private void OnTaskSetCompleted()
	{
		QueueSlideIn(new SlideIn(SlideInType.TaskSet));
	}

	private void OnSymbolPickUp(Characters.CharacterType type)
	{
		Characters.Model model = Characters.characterData[type];
		if (model.Price <= PlayerInfo.Instance.GetCollectedSymbols(type))
		{
			PlayerInfo.Instance.stats[Stat.OwnCharacters]++;
		}
	}

	public void OnNetErrorPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_NET_ERROR)));
		}
	}

	public void OnDataErrorPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_NET_ERROR)));
		}
	}

	public void OnGetSuccessPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_GOT_SUCCESSFULLY)));
		}
	}

	public void OnTrialFinished()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_FILL_COMPLETED)));
		}
	}

	public void OnRecodeStatusPickedUp(bool success)
	{
		if (success)
		{
			QueueSlideIn(new SlideIn(SlideInType.TopRunTip, Strings.Get(LanguageKey.UI_TOP_TIP_REDEEM_SUCCESS)));
		}
		else
		{
			QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_REDEEM_CODE_ERROR)));
		}
	}

	public void OnNeedEnoughLevel(int level)
	{
		string payload = string.Format(Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_LEVELLOCK), level);
		QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, payload));
	}

	public void OnNeedEnoughSymbol(string symbol)
	{
		string payload = string.Format(Strings.Get(LanguageKey.NOT_ENOUGH_UNLOCK_TIP), symbol);
		QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, payload));
	}

	public void OnErrorMessage(string description)
	{
		QueueSlideIn(new SlideIn(SlideInType.ErrorMessage, description));
	}

	private void ShowSlideIn()
	{
		if (queueSliderIn.Count > 0)
		{
			SlideIn slideIn = queueSliderIn[0];
			queueSliderIn.RemoveAt(0);
			if (slideIn.type == SlideInType.TopRunTip)
			{
				topRunTipSlide.SetupSlideTopRunTip(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.Task)
			{
				taskHelperSlide.SetupSlideInTask(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.Achievement)
			{
				taskHelperSlide.SetupSlideInAchievement(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.TaskSet)
			{
				taskSetHelperSlide.SetupSlideInTaskSet(PlayerInfo.Instance.rawMultiplier);
			}
			else if (slideIn.type == SlideInType.Unlock)
			{
				unlockSlide.SetupSlideInUnlock(slideIn.payload);
			}
			else if (slideIn.type == SlideInType.ErrorMessage)
			{
				errorMessageSlide.SetupErrorMessage(slideIn.payload);
			}
			slideInActive = true;
		}
	}

	public void QueueSlideIn(SlideIn slideIn)
	{
		if (queueSliderIn != null && queueSliderIn.Count > 0)
		{
			SlideIn slideIn2 = queueSliderIn[queueSliderIn.Count - 1];
			if (slideIn2.type == slideIn.type && slideIn2.payload == slideIn.payload && slideIn2.payloadInt == slideIn2.payloadInt)
			{
				return;
			}
		}
		queueSliderIn.Add(slideIn);
		if (!stopping && !slideInActive)
		{
			ShowSlideIn();
		}
	}

	public void ReadyForNextSlide()
	{
		slideInActive = false;
		if (!stopping && !slideInActive)
		{
			ShowSlideIn();
		}
	}

	public int NumberOfTaskSetSlideIns()
	{
		numOfTaskSetSlide = queueSliderIn.FindAll((SlideIn c) => c.type == SlideInType.TaskSet);
		return numOfTaskSetSlide.Count;
	}

	public void QueueMessage(string message)
	{
		_QueueMessage(message);
	}

	private void _QueueMessage(string message)
	{
		queueMessage.Enqueue(message);
		if (!messageIsShowing)
		{
			ShowNextMessage();
		}
		if (!slideInActive)
		{
			ShowSlideIn();
		}
	}

	private void ShowNextMessage()
	{
		if (queueMessage.Count > 0)
		{
			string message = queueMessage.Dequeue();
			messageHelper.ShowMessage(message);
			messageIsShowing = true;
		}
	}

	public void ReadyForNextMessage()
	{
		messageIsShowing = false;
		ShowNextMessage();
	}

	private void OnDestroy()
	{
		TasksManager.Instance.onTaskComplete = (TasksManager.TaskCompleteHandler)Delegate.Remove(TasksManager.Instance.onTaskComplete, new TasksManager.TaskCompleteHandler(OnTaskCompleted));
		TasksManager.Instance.onTaskSetComplete = (TasksManager.TaskSetCompleteHandler)Delegate.Remove(TasksManager.Instance.onTaskSetComplete, new TasksManager.TaskSetCompleteHandler(OnTaskSetCompleted));
		PlayerInfo instance = PlayerInfo.Instance;
		PlayerInfo.Instance.OnSymbolCollected = (Action<Characters.CharacterType>)Delegate.Remove(PlayerInfo.Instance.OnSymbolCollected, new Action<Characters.CharacterType>(OnSymbolPickUp));
	}
}
