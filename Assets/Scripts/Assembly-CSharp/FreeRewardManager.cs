using System;
using UnityEngine;

public class FreeRewardManager : MonoBehaviour
{
	private static FreeRewardManager _instance;

	public FreeRewardPopupData _popupData;

	public static FreeRewardManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "FreeRewardManager";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_instance = gameObject.AddComponent<FreeRewardManager>();
			}
			return _instance;
		}
	}

	public FreeRewardPopupData GetRewardPopupData()
	{
		return _popupData;
	}

	public void SetFreeViewReward(RewardType type, Action callback = null, int rewardNum = 0)
	{
		_popupData = new FreeRewardPopupData();
		_popupData.rewardType = type;
		_popupData.payReward = true;
		_popupData.getCallback = callback;
		switch (type)
		{
		case RewardType.viewcoins:
			_popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewCoinReward;
			_popupData.popupDescription = string.Format("{0} Coins Reward", _popupData.num);
			break;
		case RewardType.viewkeys:
			_popupData.num = UIPosScalesAndNGUIAtlas.Instance.freeViewGemReward;
			_popupData.popupDescription = string.Format("{0} Gems Reward", _popupData.num);
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(RewardType type, int amount, Action callback = null)
	{
		_popupData = new FreeRewardPopupData();
		_popupData.rewardType = type;
		_popupData.payReward = true;
		_popupData.getCallback = callback;
		switch (type)
		{
		case RewardType.coins:
			_popupData.num = amount;
			_popupData.popupDescription = string.Format("{0} Coins Reward", amount);
			break;
		case RewardType.keys:
			_popupData.num = amount;
			_popupData.popupDescription = string.Format("{0} Gems Reward", amount);
			break;
		case RewardType.doublecoins:
			if (amount != 0)
			{
				_popupData.num = amount;
			}
			else
			{
				_popupData.num = GameStats.Instance.coins;
			}
			_popupData.popupDescription = string.Format("{0} Coins Reward", _popupData.num);
			break;
		case RewardType.headstart2000:
			_popupData.popupDescription = string.Format("{0} headstart2000s Reward", amount);
			_popupData.num = amount;
			break;
		case RewardType.scorebooster:
			_popupData.popupDescription = string.Format("{0} scoreboosters Reward", amount);
			_popupData.num = amount;
			break;
		case RewardType.helmet:
			_popupData.popupDescription = string.Format("{0} helmets Reward", amount);
			_popupData.num = amount;
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(DailyLandingAward award, Action callback = null)
	{
		_popupData = new FreeRewardPopupData();
		_popupData.payReward = true;
		_popupData.getCallback = callback;
		switch (award.type)
		{
		case DailyLandingAward.DailyLandingRewardType.Coins:
			_popupData.rewardType = RewardType.dailycoins;
			_popupData.num = award.Amount;
			_popupData.popupDescription = string.Format(" {0} Coins Reward", _popupData.num);
			break;
		case DailyLandingAward.DailyLandingRewardType.Keys:
			_popupData.rewardType = RewardType.dailykeys;
			_popupData.num = award.Amount;
			_popupData.popupDescription = string.Format(" {0} Gems Reward", _popupData.num);
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}

	public void SetFreeRewardType(WheelReward reward, Action callback = null)
	{
		_popupData = new FreeRewardPopupData();
		_popupData.payReward = false;
		_popupData.getCallback = callback;
		switch (reward.type)
		{
		case WheelRewardType.Coin:
			_popupData.rewardType = RewardType.coins;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Coins Reward", reward.count);
			break;
		case WheelRewardType.Key:
			_popupData.rewardType = RewardType.keys;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Gems Reward", reward.count);
			break;
		case WheelRewardType.Headstart:
			_popupData.rewardType = RewardType.headstart2000;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Headstarts Reward", reward.count);
			break;
		case WheelRewardType.Scorebooster:
			_popupData.rewardType = RewardType.scorebooster;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Scoreboosters Reward", reward.count);
			break;
		case WheelRewardType.LeeSymbol:
			_popupData.rewardType = RewardType.leeSymbol;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Symbols Of Lee Reward", reward.count);
			break;
		case WheelRewardType.TurtlefokSymbol:
			_popupData.rewardType = RewardType.turtlefokSymbol;
			_popupData.num = reward.count;
			_popupData.popupDescription = string.Format("{0} Symbols Of Turtlefok Reward", reward.count);
			break;
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("GetFreeRewardPopup");
		}
	}
}
