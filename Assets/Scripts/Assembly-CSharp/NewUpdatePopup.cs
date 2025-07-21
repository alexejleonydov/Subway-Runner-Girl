using UnityEngine;

public class NewUpdatePopup : UIBaseScreen
{
	public static bool ShowUpdate;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel contentLbl;

	[SerializeField]
	private UILabel updateLbl;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UIRewardHelper uiReward1;

	[SerializeField]
	private UIRewardHelper uiReward2;

	[SerializeField]
	private GameObject updateGo;

	[SerializeField]
	private GameObject getRewardGo;

	[SerializeField]
	private ParticleSystem getRewardPs;

	private int index;

	private UpdateReward updateReward1;

	private UpdateReward updateReward2;

	public override void Init()
	{
		updateReward1 = new UpdateReward();
		updateReward2 = new UpdateReward();
		UpdateRewardManager.Instance.GetUpdateRewardInfo(ref updateReward1, ref updateReward2);
		base.Init();
	}

	private void RefreshLabel()
	{
		updateLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_BUTTON_UPDATEB);
		rewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_GET_FREE_REWARD_TITLE);
	}

	public override void Show()
	{
		base.Show();
		index = UpdateRewardManager.Instance.GetUpdateRewardInfo(ref updateReward1, ref updateReward2);
		uiReward1.RefreshUI(updateReward1);
		uiReward2.RefreshUI(updateReward2);
		if (ShowUpdate)
		{
			titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_TITLE);
			contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_EXPLAIN);
			updateGo.SetActive(true);
			getRewardGo.SetActive(false);
		}
		else
		{
			titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_REWARD_TITLE);
			contentLbl.text = Strings.Get(LanguageKey.UI_POPUP_NEWUPDATE_REWARD_EXPLAIN);
			updateGo.SetActive(false);
			getRewardGo.SetActive(true);
		}
		RefreshLabel();
	}

	public void GetApp()
	{
		RiseSdk.Instance.GetApp(RiseSdk.Instance.GetConfig(10));
	}

	public void GetReward()
	{
		UpdateRewardManager.Instance.GetReward(updateReward1, 1);
		UpdateRewardManager.Instance.GetReward(updateReward2, 1);
		PlayerInfo.Instance.updateRewardIndex = index;
		getRewardPs.Play();
		UIScreenController.Instance.ClosePopup(null);
	}
}
