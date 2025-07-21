using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RewardManager
{
	public static bool canShowMultipleQueuedCelebrations;

	public static int rewardsToUnlockCount
	{
		get
		{
			return PlayerInfo.Instance.pendingRewards.Count;
		}
	}

	private static void AddChestToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		PlayerInfo instance = PlayerInfo.Instance;
		bool flag = instance.pendingRewards.Exists((CelebrationReward mb) => mb.Uid == reward.Uid);
		if (reward.Uid <= 0 || !flag)
		{
			reward.Uid = DateTime.Now.Ticks + UnityEngine.Random.Range(0, int.MaxValue);
		}
		if (!flag)
		{
			instance.AddPendingReward(reward);
			if (shouldSaveToDisk)
			{
				instance.SaveIfDirty();
			}
		}
	}

	public static void AddRewardToUnlock(CelebrationRewardOrigin origin)
	{
		CelebrationReward celebrationReward = null;
		if (origin != CelebrationRewardOrigin.Chest && origin != CelebrationRewardOrigin.SuperChest && origin != CelebrationRewardOrigin.ChestMini)
		{
			Debug.LogWarning("RewardManger Use this method for unknown prize only: MB, SMB,WCMB,WCSMB and MMB.");
		}
		if (celebrationReward != null)
		{
			celebrationReward.CelebrationRewardOrigin = origin;
			AddChestToUnlock(celebrationReward);
		}
	}

	public static void AddRewardToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		if (reward.CelebrationRewardOrigin != 0 && reward.rewardType != 0)
		{
			AddChestToUnlock(reward);
		}
		else
		{
			Debug.LogWarning("RewardManager You can't add a not set reward to unlock");
		}
	}

	public static CelebrationReward[] GetRewardsToUnlockForCelebration()
	{
		if (PlayerInfo.Instance.lastAddedReward != null && !canShowMultipleQueuedCelebrations)
		{
			return new CelebrationReward[1] { PlayerInfo.Instance.lastAddedReward };
		}
		return PlayerInfo.Instance.pendingRewards.ToArray();
	}

	public static List<CelebrationReward> GetWeeklyHuntRewardsToUnlock()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		List<CelebrationReward> list = new List<CelebrationReward>();
		for (int i = 0; i < instance.pendingRewards.Count; i++)
		{
			CelebrationReward item = instance.pendingRewards[i];
			list.Add(item);
		}
		return list;
	}

	private static bool IsRewardChest(CelebrationReward reward)
	{
		return reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Chest || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperChest;
	}

	public static void PayoutNonChestRewards()
	{
		List<CelebrationReward> source = new List<CelebrationReward>(PlayerInfo.Instance.pendingRewards);
		source = source.Where((CelebrationReward celebrationReward) => !IsRewardChest(celebrationReward)).ToList();
		int i = 0;
		for (int count = source.Count; i < count; i++)
		{
			UIScreenController.Instance.PayoutCelebrationReward(source[i]);
		}
	}

	public static void RewardPayedOut(CelebrationReward celebrationReward)
	{
		PlayerInfo.Instance.RemovePendingReward(celebrationReward);
	}
}
