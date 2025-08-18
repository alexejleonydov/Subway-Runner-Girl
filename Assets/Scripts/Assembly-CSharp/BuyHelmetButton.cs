using UnityEngine;
using UnityEngine.InputSystem;

public class BuyHelmetButton : MonoBehaviour, IPurchaseHandler
{
	public int number;

	[SerializeField]
	private UISprite[] freeViewSprs;

	[SerializeField]
	private UILabel freeViewLbl;

	[SerializeField]
	private UILabel priceLabel;

	private bool _purchaseInProgress;

	private Color _freeViewLblColor;

	private int _freeState;
	private InputActions inputActions;



	public void OnBuyClick()
	{
		if (!_purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseHelmet(1, this);
		}
	}

	public void OnFreeViewClick()
	{
		if (_freeState == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_buy_helmet", 0);
			RiseSdk.Instance.TrackEvent("click_video_buy_helmet", "default,default");
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 12;
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
		else if (_freeState == 2)
		{
			if (UIScreenController.Instance.CheckNetwork())
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
			else
			{
				UIScreenController.Instance.PushPopup("NoNetworkPopup");
			}
		}
	}

	private void Awake()
	{
		inputActions = new InputActions();

		Upgrade upgrade = Upgrades.upgrades[PropType.helmet];
		priceLabel.text = (upgrade.getPrice(0) * number).ToString();
		_freeViewLblColor = freeViewLbl.color;
	}

	private void OnEnable()
	{


		inputActions.Enable();
		inputActions.Play.PlayGame.performed += OnBuyPerformed;

		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
		if (RiseSdk.Instance.HasRewardAd())
		{
			_freeState = 1;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_buy_helmet", 0);
			int i = 0;
			for (int num = freeViewSprs.Length; i < num; i++)
			{
				freeViewSprs[i].color = Color.white;
			}
			freeViewLbl.color = _freeViewLblColor;
		}
		else
		{
			_freeState = 2;
			int j = 0;
			for (int num2 = freeViewSprs.Length; j < num2; j++)
			{
				freeViewSprs[j].color = Color.cyan;
			}
			freeViewLbl.color = Color.cyan;
		}
	}

	private void OnDisable()
	{
		inputActions.Disable();
		inputActions.Play.PlayGame.performed -= OnBuyPerformed;

		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	private void OnBuyPerformed(InputAction.CallbackContext context)
	{
		GameObject HelmetPopup = GameObject.Find("HelmetPopup(Clone)");
		if (HelmetPopup != null && HelmetPopup.activeInHierarchy)
		{
			OnBuyClick();
			Debug.Log("BUY is performed!");
		}
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, number);
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 12)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_buy_helmet", 0);
			FreeRewardManager.Instance.SetFreeRewardType(RewardType.helmet, 3);
		}
	}
}
