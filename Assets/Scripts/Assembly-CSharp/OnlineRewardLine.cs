using UnityEngine;

public class OnlineRewardLine : MonoBehaviour
{
	private OnlineZone zone;

	private OnlineZone lastZone;

	[SerializeField]
	private int index;

	[SerializeField]
	private UISprite lockedSpr;

	[SerializeField]
	private UISprite openningSpr;

	[SerializeField]
	private UISprite gotSpr;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private OnLineRewardHelper[] rewardHelpers;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private UISprite maskSpr;

	[SerializeField]
	private GameObject getBtn;

	[SerializeField]
	private GameObject unGetBtn;

	[SerializeField]
	private ParticleSystem getParticleSystem;

	public void Init()
	{
		zone = OnlineRewardManager.Instance.Zones[index];
		if (index - 1 >= 0 && index < OnlineRewardManager.Instance.Zones.Length)
		{
			lastZone = OnlineRewardManager.Instance.Zones[index - 1];
		}
		else
		{
			lastZone = null;
		}
		int num = zone.rewards.Length;
		OnlineReward onlineReward = null;
		for (int i = 0; i < num; i++)
		{
			onlineReward = zone.rewards[i];
			rewardHelpers[i].SetIcomAndNumber(zone.rewards[i].icon, zone.rewards[i].number);
		}
	}

	public void Show()
	{
		getLbl.text = Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_BUTTON_GET);
		timeLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_ONLINE_REWARD_INTERVAL_TIME), zone.deadline / 60);
		RefreshSign();
		RefreshSlider();
		RefreshButton();
	}

	public void RefreshSlider()
	{
		int onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (zone.deadline > onlineTime)
		{
			if (lastZone == null)
			{
				slider.value = (float)onlineTime / (float)zone.deadline;
			}
			else if (onlineTime < lastZone.deadline)
			{
				slider.value = 0f;
			}
			else
			{
				slider.value = (float)(onlineTime - lastZone.deadline) / (float)(zone.deadline - lastZone.deadline);
			}
		}
		else
		{
			slider.value = 1f;
		}
	}

	public void RefreshSign()
	{
		if (zone.deadline > PlayerInfo.Instance.GetOnlineTime())
		{
			lockedSpr.enabled = true;
			gotSpr.enabled = false;
			openningSpr.enabled = false;
		}
		else if (!PlayerInfo.Instance.GetOnlineZonePayedOut(index))
		{
			gotSpr.enabled = false;
			openningSpr.enabled = true;
			lockedSpr.enabled = false;
		}
		else
		{
			gotSpr.enabled = true;
			openningSpr.enabled = false;
			lockedSpr.enabled = false;
		}
	}

	public void RefreshButton()
	{
		if (zone.deadline > PlayerInfo.Instance.GetOnlineTime())
		{
			maskSpr.enabled = false;
			getBtn.SetActive(false);
			unGetBtn.SetActive(true);
		}
		else if (!PlayerInfo.Instance.GetOnlineZonePayedOut(index))
		{
			maskSpr.enabled = false;
			getBtn.SetActive(true);
			unGetBtn.SetActive(false);
		}
		else
		{
			maskSpr.enabled = true;
			getBtn.SetActive(false);
			unGetBtn.SetActive(false);
		}
	}

	public void OnGetClick()
	{
		if (zone == null)
		{
			return;
		}
		OnlineReward onlineReward = null;
		int i = 0;
		for (int num = zone.rewards.Length; i < num; i++)
		{
			onlineReward = zone.rewards[i];
			if (onlineReward != null)
			{
				switch (onlineReward.rewardType)
				{
				case OnlineRewardType.Coins:
					PlayerInfo.Instance.amountOfCoins += onlineReward.number;
					TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, onlineReward.number);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, onlineReward.number);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_menu_online", 0, onlineReward.number);
					break;
				case OnlineRewardType.Keys:
					PlayerInfo.Instance.amountOfKeys += onlineReward.number;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, onlineReward.number);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_menu_online", 0, onlineReward.number);
					break;
				case OnlineRewardType.HeadSprint:
					PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, onlineReward.number);
					break;
				case OnlineRewardType.ScoreBooster:
					PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, onlineReward.number);
					break;
				}
			}
		}
		PlayerInfo.Instance.SetOnlineZonePayedOut(index, true);
		OnlineRewardManager.Instance.PayedOut--;
		if (getParticleSystem != null)
		{
			getParticleSystem.Play();
		}
		RefreshButton();
		RefreshSign();
	}
}
