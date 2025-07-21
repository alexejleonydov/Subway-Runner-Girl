using UnityEngine;

public class NotificationController : MonoBehaviour
{
	private void Awake()
	{
		NotificationsObserver instance = NotificationsObserver.Instance;
		instance.RegisterUpdateNotificationValue(NotificationType.AchiementFinished, PlayerInfo.Instance.GetAllAchievementAward);
		instance.RegisterUpdateNotificationValue(NotificationType.CharacterCanUnlock, CanShowCharacterTip);
		instance.RegisterUpdateNotificationValue(NotificationType.TopRunUp, TopRunHasUp);
	}

	public bool DailyLandingPayOut()
	{
		bool Istoday;
		PlayerInfo.Instance.GetDailyLandingDaysInRow(out Istoday);
		return PlayerInfo.Instance.DailyLandingPayOut();
	}

	public bool CanShowCharacterTip()
	{
		if (PlayerInfo.Instance.CanUnlockCharacter() || PlayerInfo.Instance.CanUnlockHelm() || PlayerInfo.Instance.CanIncreasePowerup())
		{
			return true;
		}
		return false;
	}

	private bool TopRunHasUp()
	{
		return false;
	}

	private bool UpgradeCanBuy()
	{
		return false;
	}

	private bool Lucky()
	{
		return PlayerInfo.Instance.lotteryFreeRemainCount > 0;
	}

	private bool Online()
	{
		OnlineZone[] zones = OnlineRewardManager.Instance.Zones;
		int i = 0;
		for (int num = zones.Length; i < num; i++)
		{
			if (zones[i].deadline <= PlayerInfo.Instance.GetOnlineTime() && !PlayerInfo.Instance.GetOnlineZonePayedOut(i))
			{
				return true;
			}
		}
		return false;
	}
}
