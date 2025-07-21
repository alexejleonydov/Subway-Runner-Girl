using UnityEngine;

public class UIRewardHelper : MonoBehaviour
{
	public UIReward uiReward;

	private UpdateReward reward;

	public void RefreshUI(UpdateReward reward)
	{
		if (reward != null)
		{
			this.reward = reward;
			if (uiReward.Icon != null)
			{
				uiReward.Icon.spriteName = reward.icon;
			}
			if (uiReward.Number != null)
			{
				uiReward.Number.text = "X " + reward.number;
				uiReward.Number.color = reward.color;
			}
		}
	}

	public void GetReward(int multiple = 1)
	{
		if (reward != null)
		{
			UpdateRewardManager.Instance.GetReward(reward, multiple);
		}
	}
}
