using UnityEngine;

public class UpdateRewardManager : MonoBehaviour
{
	private static UpdateRewardManager _instance;

	[SerializeField]
	private UpdateReward[] updateRewards;

	public static UpdateRewardManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Utils.FindObject<UpdateRewardManager>();
			}
			return _instance;
		}
	}

	public int GetUpdateRewardInfo(ref UpdateReward one, ref UpdateReward two)
	{
		int updateRewardIndex = PlayerInfo.Instance.updateRewardIndex;
		updateRewardIndex %= updateRewards.Length;
		one = updateRewards[updateRewardIndex];
		updateRewardIndex = (updateRewardIndex + 1) % updateRewards.Length;
		two = updateRewards[updateRewardIndex];
		return updateRewardIndex;
	}

	public void GetReward(UpdateReward reward, int multiple)
	{
		if (reward != null)
		{
			int num = reward.number * multiple;
			switch (reward.rewardType)
			{
			case UpdateRewardType.Chest:
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.chest, num);
				break;
			case UpdateRewardType.Helmet:
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, num);
				break;
			case UpdateRewardType.HeadSprint:
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, num);
				break;
			case UpdateRewardType.ScoreBooster:
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, num);
				break;
			case UpdateRewardType.LeeToken:
				PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.lee, num);
				break;
			case UpdateRewardType.TurtlefokToken:
				PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.turtlefok, num);
				break;
			case UpdateRewardType.Coins:
				PlayerInfo.Instance.amountOfCoins += num;
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, num);
				break;
			case UpdateRewardType.Keys:
				PlayerInfo.Instance.amountOfKeys += num;
				break;
			}
		}
	}
}
