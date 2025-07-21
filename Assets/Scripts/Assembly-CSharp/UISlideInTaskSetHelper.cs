using UnityEngine;

public class UISlideInTaskSetHelper : UISlideIn
{
	[SerializeField]
	private UILabel line1;

	[SerializeField]
	private UILabel lineReward;

	[SerializeField]
	private UILabel lineRewardShadow;

	[SerializeField]
	private UISprite superChest;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UILabel coinLabel;

	private int _displayedMultiplier;

	private int _tasksInStoryLineSet;

	private bool _multiplierIsIncrementing;

	private int _queuedTaskSetSlideIns;

	private void Awake()
	{
		_tasksInStoryLineSet = TasksManager.Instance.taskSetStoryCount + 1;
	}

	private int CalculateTheDisplayedMultiplierNumber(int multiplier)
	{
		if (multiplier < _tasksInStoryLineSet)
		{
			_queuedTaskSetSlideIns = UISliderInController.Instance.NumberOfTaskSetSlideIns();
			_multiplierIsIncrementing = true;
		}
		else if (_displayedMultiplier == 0)
		{
			_queuedTaskSetSlideIns = 0;
		}
		else
		{
			_queuedTaskSetSlideIns = _tasksInStoryLineSet - (_displayedMultiplier + 1);
		}
		return multiplier - _queuedTaskSetSlideIns;
	}

	public void EnableCoinLabel(bool enable)
	{
		coinIcon.gameObject.SetActive(enable);
		coinLabel.gameObject.SetActive(enable);
	}

	public void SetupSlideInTaskSet(int multiplier)
	{
		base.gameObject.SetActive(true);
		if (_displayedMultiplier == _tasksInStoryLineSet)
		{
			_multiplierIsIncrementing = false;
		}
		if (PlayerInfo.Instance.taskCompletedSum > _tasksInStoryLineSet && !_multiplierIsIncrementing)
		{
			lineReward.enabled = true;
			lineRewardShadow.enabled = true;
			_displayedMultiplier = CalculateTheDisplayedMultiplierNumber(multiplier);
			lineReward.text = "lV." + PlayerInfo.Instance.amountOfLevel;
			superChest.enabled = false;
		}
		else
		{
			lineReward.enabled = true;
			lineRewardShadow.enabled = true;
			_displayedMultiplier = CalculateTheDisplayedMultiplierNumber(multiplier);
			lineReward.text = "lV." + PlayerInfo.Instance.amountOfLevel;
			superChest.enabled = false;
		}
		line1.text = Strings.Get(LanguageKey.UI_TOP_TIP_TASK_SET_COMPLETE);
		EnableCoinLabel(false);
		SlideIn(null);
	}
}
