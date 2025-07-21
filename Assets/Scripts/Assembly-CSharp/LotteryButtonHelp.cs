using System;
using UnityEngine;

public class LotteryButtonHelp : MonoBehaviour
{
	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UILabel nextLbl;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private string freeFillSpriteName;

	[SerializeField]
	private string payFillSpriteName;

	[SerializeField]
	private GameObject nextFreeTimeGo;

	[SerializeField]
	private GameObject watchVideo;

	[SerializeField]
	private GameObject usePay;

	[SerializeField]
	private GameObject free;

	[SerializeField]
	private UILabel usecoinAmound;

	[SerializeField]
	private int amount;

	[SerializeField]
	private UILabel freeLabel;

	[SerializeField]
	private UILabel freeVideoLabel;

	[SerializeField]
	private Color freeVideoLabelColor;

	[SerializeField]
	private UISprite[] adSprits;

	[SerializeField]
	private BoxCollider buttonCollider;

	public Action OnButtonClick;

	private bool force;

	private int type;

	private bool isActive;

	private void Awake()
	{
		usecoinAmound.text = amount.ToString();
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= RewardAdSuc;
		RiseSdkListener.OnAdEvent += RewardAdSuc;
		nextLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_NEXT_FREE_SPIN_TIME);
		adLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_AD_SPIN_TIP);
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= RewardAdSuc;
	}

	public void RewardAdSuc(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 7)
		{
			if (force)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_lottery_endless", 0);
				freeVideoLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE_AGAIN);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_lottery", 0);
				PlayerInfo.Instance.UseLotteryWatchView();
			}
			if (OnButtonClick != null)
			{
				OnButtonClick();
			}
		}
	}

	public void OnClick()
	{
		if (type == 0)
		{
			if (PlayerInfo.Instance.amountOfCoins >= amount)
			{
				PlayerInfo.Instance.amountOfCoins -= amount;
				TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, amount);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, amount);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_menu_lottery", 0, amount);
				if (OnButtonClick != null)
				{
					OnButtonClick();
				}
			}
			else
			{
				PurchaseHandler.Instance.PurchaseCoinsIfNeeded(amount);
			}
		}
		else if (type == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			if (force)
			{
				RiseSdk.Instance.TrackEvent("click_video_lottery_endless", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_lottery_endless", 0);
			}
			else
			{
				RiseSdk.Instance.TrackEvent("click_video_lottery", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_lottery", 0);
			}
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 7;
					UIScreenController.Instance.PushPopup("VideoLoadingPopup");
				}
				else
				{
					UISliderInController.Instance.OnNetErrorPickedUp();
				}
			}
			else
			{
				UIScreenController.Instance.PushPopup("NoNetworkPopup");
			}
		}
		else if (type == 2)
		{
			if (OnButtonClick != null)
			{
				OnButtonClick();
			}
			PlayerInfo.Instance.UseLotteryFree();
		}
	}

	public void Reload(bool force)
	{
		this.force = force;
		if (force)
		{
			type = 0;
			if (GameStats.Instance.gameOverPlayLotteryCount < 2)
			{
				if (GameStats.Instance.gameOverPlayLotteryCount == 1)
				{
					freeVideoLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE_AGAIN);
				}
				if (RiseSdk.Instance.HasRewardAd())
				{
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_lottery_endless", 0);
					buttonCollider.enabled = true;
					type = 1;
					watchVideo.SetActive(true);
					free.SetActive(false);
					usePay.SetActive(false);
					fill.enabled = true;
					fill.spriteName = freeFillSpriteName;
					fill.color = Color.white;
					for (int i = 0; i < adSprits.Length; i++)
					{
						adSprits[i].color = Color.white;
					}
					freeVideoLabel.color = freeVideoLabelColor;
				}
				else
				{
					buttonCollider.enabled = false;
					type = 1;
					watchVideo.SetActive(true);
					free.SetActive(false);
					usePay.SetActive(false);
					fill.enabled = true;
					fill.spriteName = freeFillSpriteName;
					fill.color = Color.cyan;
					for (int j = 0; j < adSprits.Length; j++)
					{
						adSprits[j].color = Color.cyan;
					}
					freeVideoLabel.color = Color.gray;
				}
			}
			else
			{
				watchVideo.SetActive(false);
				free.SetActive(false);
				usePay.SetActive(false);
				fill.enabled = false;
				type = -1;
			}
			if (nextFreeTimeGo.activeInHierarchy)
			{
				nextFreeTimeGo.SetActive(false);
			}
			isActive = false;
			return;
		}
		isActive = true;
		if (PlayerInfo.Instance.CheckIfLotteryCanFree())
		{
			free.SetActive(true);
			watchVideo.SetActive(false);
			usePay.SetActive(false);
			fill.enabled = true;
			fill.spriteName = freeFillSpriteName;
			fill.color = Color.white;
			freeLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE);
			type = 2;
		}
		else if (PlayerInfo.Instance.CheckIfLotteryCanWatchFreeView())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_lottery", 0);
				watchVideo.SetActive(true);
				free.SetActive(false);
				usePay.SetActive(false);
				fill.enabled = true;
				fill.spriteName = freeFillSpriteName;
				fill.color = Color.white;
				type = 1;
			}
			else
			{
				watchVideo.SetActive(false);
				free.SetActive(false);
				usePay.SetActive(true);
				fill.enabled = true;
				fill.color = Color.white;
				fill.spriteName = payFillSpriteName;
				type = 0;
			}
		}
		else
		{
			watchVideo.SetActive(false);
			free.SetActive(false);
			usePay.SetActive(true);
			fill.enabled = true;
			fill.color = Color.white;
			fill.spriteName = payFillSpriteName;
			type = 0;
		}
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (PlayerInfo.Instance.CheckIfLotteryCanFree())
		{
			Reload(force);
			if (nextFreeTimeGo.activeInHierarchy)
			{
				nextFreeTimeGo.SetActive(false);
			}
		}
		else
		{
			if (!nextFreeTimeGo.activeInHierarchy)
			{
				nextFreeTimeGo.SetActive(true);
			}
			timeLbl.text = PlayerInfo.Instance.LotteryFreeTimeSpan();
		}
	}
}
