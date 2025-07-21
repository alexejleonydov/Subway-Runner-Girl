using UnityEngine;

public class OnlineButton : MonoBehaviour
{
	[SerializeField]
	private UISprite tip;

	private int onlineTime;

	private int lastIndex;

	private void OnEnable()
	{
		onlineTime = PlayerInfo.Instance.GetOnlineTime();
		int i = 0;
		lastIndex = 0;
		OnlineRewardManager.Instance.PayedOut = 0;
		for (; i < OnlineRewardManager.Instance.Zones.Length; i++)
		{
			if (onlineTime >= OnlineRewardManager.Instance.Zones[i].deadline)
			{
				lastIndex++;
			}
			if (onlineTime >= OnlineRewardManager.Instance.Zones[i].deadline && !PlayerInfo.Instance.GetOnlineZonePayedOut(i))
			{
				OnlineRewardManager.Instance.PayedOut++;
			}
		}
	}

	private void Update()
	{
		if (PlayerInfo.Instance.AllOnlineZonePayedOut())
		{
			base.gameObject.SetActive(false);
			return;
		}
		tip.enabled = OnlineRewardManager.Instance.PayedOut > 0;
		onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (lastIndex < OnlineRewardManager.Instance.Zones.Length && onlineTime >= OnlineRewardManager.Instance.Zones[lastIndex].deadline)
		{
			OnlineRewardManager.Instance.PayedOut++;
			lastIndex++;
		}
	}
}
