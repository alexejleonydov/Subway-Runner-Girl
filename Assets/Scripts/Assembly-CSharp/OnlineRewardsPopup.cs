using UnityEngine;

public class OnlineRewardsPopup : UIBaseScreen
{
	[SerializeField]
	private OnlineRewardLine[] rewardLines;

	[SerializeField]
	private UILabel time;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel timeLbl;

	private int onlineTime;

	private OnlineRewardLine line;

	private int lastIndex;

	public override void Init()
	{
		int i = 0;
		for (int num = rewardLines.Length; i < num; i++)
		{
			rewardLines[i].Init();
		}
		lastIndex = 0;
		for (int j = 0; j < OnlineRewardManager.Instance.Zones.Length; j++)
		{
			if (onlineTime >= OnlineRewardManager.Instance.Zones[j].deadline)
			{
				lastIndex++;
			}
		}
		base.Init();
	}

	private void OnEnable()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_TITLE);
		timeLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_ONLINE_TIME);
	}

	public override void Show()
	{
		base.Show();
		int i = 0;
		for (int num = rewardLines.Length; i < num; i++)
		{
			rewardLines[i].Show();
		}
	}

	private void Update()
	{
		onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (onlineTime > 3600)
		{
			time.text = string.Format("{0:D2}:{1:D2}:{2:D2}", onlineTime / 3600, onlineTime / 60 % 60, onlineTime % 60);
		}
		else
		{
			time.text = string.Format("{0:D2}:{1:D2}", onlineTime / 60, onlineTime % 60);
		}
		if (lastIndex < rewardLines.Length)
		{
			line = rewardLines[lastIndex];
			line.RefreshSlider();
			if (onlineTime >= OnlineRewardManager.Instance.Zones[lastIndex].deadline)
			{
				line.RefreshButton();
				line.RefreshSign();
				lastIndex++;
			}
		}
	}
}
