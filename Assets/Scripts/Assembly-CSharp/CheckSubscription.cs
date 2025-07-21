using Network;
using UnityEngine;

public class CheckSubscription : MonoBehaviour
{
	public static bool isSubscriptionActive;

	public static Characters.CharacterType subscriptionCharacterType = Characters.CharacterType.strong;

	public int duration = 300;

	private bool isWaiting;

	private int frame;

	private bool hasPaid_1;

	private NetworkReachability lastFrameNetworkReachability;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		frame = 0;
	}

	private void Check()
	{
		isSubscriptionActive = false;
		RiseSdkListener.OnPaymentEvent -= onCheckSubscriptionResult;
		RiseSdkListener.OnPaymentEvent += onCheckSubscriptionResult;
		frame = 0;
		if (!hasPaid_1)
		{
			hasPaid_1 = true;
			isWaiting = false;
			RiseSdk.Instance.HasPaid(-1);
		}
		else
		{
			isWaiting = true;
			RiseSdk.Instance.HasPaid(13);
		}
	}

	private void onCheckSubscriptionResult(RiseSdk.PaymentResult result, int id)
	{
		if (id != 13)
		{
			return;
		}
		isWaiting = false;
		if (result == RiseSdk.PaymentResult.Success)
		{
			PlayerInfo.Instance.hasSubscribed = true;
			ServerManager.Instance.UploadSubscription("yes");
		}
		else
		{
			PlayerInfo.Instance.hasSubscribed = false;
			ServerManager.Instance.UploadSubscription("no");
			if (PlayerInfo.Instance.currentCharacter == (int)subscriptionCharacterType)
			{
				UIModelController.Instance.SelectCharacterForPlay(Characters.CharacterType.slick, 0);
			}
		}
		isSubscriptionActive = true;
		RiseSdkListener.OnPaymentEvent -= onCheckSubscriptionResult;
		if (isSubscriptionActive)
		{
			RiseSdkListener.OnPaymentEvent -= PayResult;
			RiseSdkListener.OnPaymentEvent += PayResult;
		}
	}

	public void PayResult(RiseSdk.PaymentResult result, int billId)
	{
		if (billId == 13 && result == RiseSdk.PaymentResult.Success)
		{
			ServerManager.Instance.UploadSubscription("yes");
			UIModelController.Instance.SelectCharacterForPlay(subscriptionCharacterType, 0);
			PlayerInfo.Instance.hasSubscribed = true;
			UISliderInController.SlideIn slideIn = new UISliderInController.SlideIn(UISliderInController.SlideInType.Unlock, Strings.Get(LanguageKey.UI_TOP_TIP_SUBSCRIBE_TO_SUCCESS));
			UISliderInController.Instance.QueueSlideIn(slideIn);
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	private void Update()
	{
		if (isSubscriptionActive)
		{
			base.enabled = false;
		}
		else
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}
			if (lastFrameNetworkReachability == NetworkReachability.NotReachable)
			{
				lastFrameNetworkReachability = Application.internetReachability;
				Check();
			}
			else if (!isWaiting)
			{
				if (frame < duration)
				{
					frame++;
					return;
				}
				Check();
				frame = 0;
			}
		}
	}
}
