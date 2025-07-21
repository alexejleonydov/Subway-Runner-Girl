using UnityEngine;

public class CheckPay : MonoBehaviour
{
	public int billId;

	private InAppProfile appProfile;

	private void Start()
	{
	}

	private void OnEnable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
		RiseSdkListener.OnPaymentEvent += PayResult;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
	}

	public void PayResult(RiseSdk.PaymentResult result, int billId)
	{
		if (billId == this.billId && result == RiseSdk.PaymentResult.Success)
		{
			appProfile = InAppData.inAppData[billId];
			PlayerInfo.Instance.amountOfKeys += appProfile.amountOfKeys;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, appProfile.amountOfKeys);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_buy", 0, appProfile.amountOfKeys);
			PlayerInfo.Instance.amountOfCoins += appProfile.amountOfCoins;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, appProfile.amountOfCoins);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_buy", 0, appProfile.amountOfCoins);
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, appProfile.amountOfHeadstarts);
			if (appProfile.removeAd)
			{
				PlayerInfo.Instance.hasRemoveAd = true;
			}
			UIScreenController.Instance.ClosePopup(null);
		}
	}
}
