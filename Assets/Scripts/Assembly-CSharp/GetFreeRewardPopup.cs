using UnityEngine;

public class GetFreeRewardPopup : UIBaseScreen
{
	[SerializeField]
	private GameObject hideGo;

	[SerializeField]
	private GameObject ItemGo;

	[SerializeField]
	private UILabel getLbl;

	[SerializeField]
	private FlyHelper flyHelper;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UISprite keyIcon;

	[SerializeField]
	private UISprite headstartIcon;

	[SerializeField]
	private UISprite scoreboosterIcon;

	[SerializeField]
	private UISprite leeIcon;

	[SerializeField]
	private UISprite turtlefokIcon;

	[SerializeField]
	private UISprite helmetIcon;

	[SerializeField]
	private UILabel amountOfItemLbl;

	[SerializeField]
	private ParticleSystem bgPs;

	[SerializeField]
	private UIAnchor anchor;

	[SerializeField]
	private ParticleSystem coinPs;

	[SerializeField]
	private ParticleSystem keyPs;

	[SerializeField]
	private ParticleSystem propPs;

	[SerializeField]
	private ParticleSystem coinBoomPs;

	[SerializeField]
	private ParticleSystem keyBoomPs;

	[SerializeField]
	private ParticleSystem propBoomPs;

	[SerializeField]
	private float fly_duration = 1f;

	private bool isRewardShowing;

	private FreeRewardPopupData popupData;

	public override void Init()
	{
		base.Init();
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			anchor.pixelOffset.y = -132f;
		}
		else
		{
			anchor.pixelOffset.y = 0f;
		}
	}

	public override void Show()
	{
		base.Show();
		ShowRewardPanel();
		RefreshLbal();
	}

	public override void Hide()
	{
		isRewardShowing = false;
		popupData = null;
		base.Hide();
	}

	private void ShowRewardPanel()
	{
		isRewardShowing = true;
		AudioPlayer.Instance.PlaySound("Get_reward_sfx", true);
		popupData = FreeRewardManager.Instance.GetRewardPopupData();
		hideGo.SetActive(true);
		bgPs.Play();
		coinIcon.enabled = false;
		keyIcon.enabled = false;
		headstartIcon.enabled = false;
		scoreboosterIcon.enabled = false;
		leeIcon.enabled = false;
		turtlefokIcon.enabled = false;
		helmetIcon.enabled = false;
		switch (popupData.rewardType)
		{
		case RewardType.coins:
		case RewardType.viewcoins:
		case RewardType.dailycoins:
		case RewardType.doublecoins:
			coinIcon.enabled = true;
			flyHelper.Selecte(coinPs, coinBoomPs);
			break;
		case RewardType.keys:
		case RewardType.viewkeys:
		case RewardType.dailykeys:
			keyIcon.enabled = true;
			flyHelper.Selecte(keyPs, keyBoomPs);
			break;
		case RewardType.headstart2000:
			headstartIcon.enabled = true;
			flyHelper.Selecte(propPs, propBoomPs);
			break;
		case RewardType.scorebooster:
			scoreboosterIcon.enabled = true;
			flyHelper.Selecte(propPs, propBoomPs);
			break;
		case RewardType.leeSymbol:
			leeIcon.enabled = true;
			flyHelper.Selecte(propPs, propBoomPs);
			break;
		case RewardType.turtlefokSymbol:
			turtlefokIcon.enabled = true;
			flyHelper.Selecte(propPs, propBoomPs);
			break;
		case RewardType.helmet:
			helmetIcon.enabled = true;
			flyHelper.Selecte(propPs, propBoomPs);
			break;
		}
		amountOfItemLbl.text = "x" + popupData.num;
	}

	private void RefreshLbal()
	{
		getLbl.text = Strings.Get(LanguageKey.UI_POPUP_GET_FREE_REWARD_BUTTON_GET);
	}

	public void GetBtnOnClick()
	{
		if (!isRewardShowing)
		{
			return;
		}
		if (popupData.payReward)
		{
			PayReward();
		}
		bgPs.Stop();
		bgPs.Clear();
		hideGo.SetActive(false);
		flyHelper.Flying(fly_duration, delegate
		{
			if (popupData.getCallback != null)
			{
				popupData.getCallback();
			}
			isRewardShowing = false;
			if (UIScreenController.isInstanced)
			{
				UIScreenController.Instance.ClosePopup(null);
			}
		});
	}

	private void PayReward()
	{
		switch (popupData.rewardType)
		{
		case RewardType.coins:
		case RewardType.viewcoins:
		case RewardType.doublecoins:
			PlayerInfo.Instance.amountOfCoins += popupData.num;
			TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, popupData.num);
			break;
		case RewardType.dailycoins:
			PlayerInfo.Instance.amountOfCoins += popupData.num;
			TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, popupData.num);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, popupData.num);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_daily", 0, popupData.num);
			break;
		case RewardType.keys:
		case RewardType.viewkeys:
			PlayerInfo.Instance.amountOfKeys += popupData.num;
			break;
		case RewardType.dailykeys:
			PlayerInfo.Instance.amountOfKeys += popupData.num;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, popupData.num);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_menu_daily", 0, popupData.num);
			break;
		case RewardType.headstart2000:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, popupData.num);
			break;
		case RewardType.scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, popupData.num);
			break;
		case RewardType.leeSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.lee, popupData.num);
			break;
		case RewardType.turtlefokSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.turtlefok, popupData.num);
			break;
		case RewardType.helmet:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, popupData.num);
			break;
		}
	}
}
