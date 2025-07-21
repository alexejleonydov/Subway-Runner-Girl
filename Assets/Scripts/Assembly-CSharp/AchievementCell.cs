using UnityEngine;

public class AchievementCell : MonoBehaviour
{
	[SerializeField]
	private UILabel getRewardLbl;

	[SerializeField]
	private UISprite fillWhilte;

	[SerializeField]
	private UISprite fillGreen;

	[SerializeField]
	private UISprite iconSpr;

	[SerializeField]
	private UILabel context;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UISprite checkMark;

	[SerializeField]
	private GameObject getRewardBtn;

	[SerializeField]
	private GameObject hasGetReward;

	[SerializeField]
	private UILabel hasGetRewardLbl;

	[SerializeField]
	private UISprite rewardIconSpr;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UISlider progressSlider;

	[SerializeField]
	private Color unColor;

	[SerializeField]
	private Color getColor;

	[SerializeField]
	private Color unConcentColor;

	[SerializeField]
	private Color getContentColor;

	private TweenScale ts;

	private UISprite rewardFillSpr;

	private int index = -1;

	private void Awake()
	{
		if (getRewardBtn != null)
		{
			ts = getRewardBtn.GetComponent<TweenScale>();
			rewardFillSpr = getRewardBtn.GetComponent<UISprite>();
		}
	}

	public void RefreshUI(int index)
	{
		if (this.index == index)
		{
			return;
		}
		this.index = index;
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(index + 3);
		AchievementInfo achievementInfo = TasksManager.Instance.GetAchievementInfo(index + 3);
		if (achievementInfo != null)
		{
			iconSpr.spriteName = achievementInfo.icon;
			rewardLbl.text = achievementInfo.rewardAmount.ToString();
			string spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			switch (achievementInfo.rewardType)
			{
			case RewardType.coins:
				spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
				break;
			case RewardType.keys:
				spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
				break;
			}
			rewardIconSpr.spriteName = spriteName;
		}
		context.text = string.Format(Strings.Get(taskInfo.template.ultraShortDescription), taskInfo.task.aim);
		getRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_BUTTON_GET);
		progressLabel.text = taskInfo.progress.ToString() + "/" + taskInfo.task.aim;
		progressSlider.value = (float)taskInfo.progress / (float)taskInfo.task.aim;
		hasGetRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_COMPLETED_GOTTENLABEL);
		if (taskInfo.complete)
		{
			progressLabel.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_COMPLETED);
		}
		ResetBtn();
	}

	private void ResetBtn()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(index + 3);
		if (PlayerInfo.Instance.GetCurrentAchievementAward(index))
		{
			checkMark.enabled = true;
			ts.enabled = false;
			getRewardBtn.SetActive(false);
			hasGetReward.SetActive(true);
			fillGreen.enabled = true;
			fillWhilte.enabled = false;
			rewardIconSpr.enabled = false;
			rewardLbl.text = string.Empty;
			context.color = getContentColor;
			return;
		}
		checkMark.enabled = false;
		getRewardBtn.SetActive(true);
		hasGetReward.SetActive(false);
		rewardIconSpr.enabled = true;
		context.color = unConcentColor;
		if (taskInfo.complete)
		{
			rewardFillSpr.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.achievementCellFillSpriteName, "yellow");
			ts.enabled = true;
			ts.PlayForward();
			fillGreen.enabled = true;
			fillWhilte.enabled = false;
			getRewardLbl.color = getColor;
		}
		else
		{
			ts.enabled = false;
			ts.ResetToBeginning();
			rewardFillSpr.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.achievementCellFillSpriteName, "gray");
			getRewardLbl.color = unColor;
			fillWhilte.enabled = true;
			fillGreen.enabled = false;
		}
	}

	private void OnClick()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(index + 3);
		AchievementInfo achieveInfo = TasksManager.Instance.GetAchievementInfo(index + 3);
		if (!PlayerInfo.Instance.GetCurrentAchievementAward(index) && taskInfo.complete)
		{
			FreeRewardManager.Instance.SetFreeRewardType(achieveInfo.rewardType, achieveInfo.rewardAmount, delegate
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, achieveInfo.rewardAmount);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_achievement", 0, achieveInfo.rewardAmount);
				ResetBtn();
			});
			PlayerInfo.Instance.SetCurrentAchivementReward(index, true);
		}
	}
}
