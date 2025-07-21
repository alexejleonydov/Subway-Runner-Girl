using System;
using UnityEngine;

public class RiseSdkListener : MonoBehaviour
{
	private static RiseSdkListener _instance;

	public static RiseSdkListener Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(RiseSdkListener)) as RiseSdkListener;
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("RiseSdkListener");
					_instance = gameObject.AddComponent<RiseSdkListener>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			return _instance;
		}
	}

	public static event Action<RiseSdk.PaymentResult, int> OnPaymentEvent;

	public static event Action<RiseSdk.SnsEventType, int> OnSNSEvent;

	public static event Action<bool, int, string> OnCacheUrlResult;

	public static event Action<bool, bool, string, string> OnLeaderBoardEvent;

	public static event Action<int, bool, string> OnReceiveServerResult;

	public static event Action<string> OnReceivePaymentsPrice;

	public static event Action<string> OnReceiveServerExtra;

	public static event Action<string> OnReceiveNotificationData;

	public static event Action<RiseSdk.AdEventType, int, string, int> OnAdEvent;

	public static event Action OnResumeAdEvent;

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			RiseSdk.Instance.OnPause();
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus)
		{
			RiseSdk.Instance.OnResume();
		}
	}

	private void OnApplicationQuit()
	{
		RiseSdk.Instance.OnStop();
		RiseSdk.Instance.OnDestroy();
	}

	private void Awake()
	{
		RiseSdk.Instance.OnStart();
	}

	public void OnResumeAd()
	{
		if (RiseSdkListener.OnResumeAdEvent != null)
		{
			RiseSdkListener.OnResumeAdEvent();
		}
	}

	public void onPaymentSuccess(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Success, arg);
		}
	}

	public void onPaymentFail(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Failed, arg);
		}
	}

	public void onPaymentCanceled(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Cancel, arg);
		}
	}

	public void onPaymentSystemError(string data)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			RiseSdk.Instance.SetPaymentSystemValid(false);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.PaymentSystemError, -1);
		}
	}

	public void onPaymentSystemValid(string data)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			RiseSdk.Instance.SetPaymentSystemValid(true);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.PaymentSystemValid, -1);
		}
	}

	public void onReceiveBillPrices(string data)
	{
		if (RiseSdkListener.OnReceivePaymentsPrice != null && RiseSdkListener.OnReceivePaymentsPrice.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceivePaymentsPrice(data);
		}
	}

	public void onReceiveLoginResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num == 0) ? RiseSdk.SnsEventType.LoginSuccess : RiseSdk.SnsEventType.LoginFailed, 0);
		}
	}

	public void onReceiveInviteResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num != 0) ? RiseSdk.SnsEventType.InviteFailed : RiseSdk.SnsEventType.InviteSuccess, 0);
		}
	}

	public void onReceiveLikeResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num != 0) ? RiseSdk.SnsEventType.LikeFailed : RiseSdk.SnsEventType.LikeSuccess, 0);
		}
	}

	public void onReceiveChallengeResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num <= 0) ? RiseSdk.SnsEventType.ChallengeFailed : RiseSdk.SnsEventType.ChallengeSuccess, num);
		}
	}

	public void onSubmitSuccess(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(true, true, leaderBoardTag, string.Empty);
		}
	}

	public void onSubmitFailure(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(true, false, leaderBoardTag, string.Empty);
		}
	}

	public void onLoadSuccess(string data)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			string[] array = data.Split('|');
			RiseSdkListener.OnLeaderBoardEvent(false, true, array[0], array[1]);
		}
	}

	public void onLoadFailure(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(false, false, leaderBoardTag, string.Empty);
		}
	}

	public void onServerResult(string data)
	{
		if (RiseSdkListener.OnReceiveServerResult != null && RiseSdkListener.OnReceiveServerResult.GetInvocationList().Length > 0)
		{
			string[] array = data.Split('|');
			int arg = int.Parse(array[0]);
			bool arg2 = int.Parse(array[1]) == 0;
			RiseSdkListener.OnReceiveServerResult(arg, arg2, array[2]);
		}
	}

	public void onCacheUrlResult(string data)
	{
		if (RiseSdkListener.OnCacheUrlResult != null && RiseSdkListener.OnCacheUrlResult.GetInvocationList().Length > 0)
		{
			string[] array = data.Split('|');
			int arg = int.Parse(array[0]);
			if (int.Parse(array[1]) == 0)
			{
				RiseSdkListener.OnCacheUrlResult(true, arg, array[2]);
			}
			else
			{
				RiseSdkListener.OnCacheUrlResult(false, arg, string.Empty);
			}
		}
	}

	public void onReceiveServerExtra(string data)
	{
		if (RiseSdkListener.OnReceiveServerExtra != null && RiseSdkListener.OnReceiveServerExtra.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceiveServerExtra(data);
		}
	}

	public void onReceiveNotificationData(string data)
	{
		if (RiseSdkListener.OnReceiveNotificationData != null && RiseSdkListener.OnReceiveNotificationData.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceiveNotificationData(data);
		}
	}

	public void onReceiveReward(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		bool flag = false;
		int arg = -1;
		string arg2 = "Default";
		bool flag2 = false;
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 1)
			{
				flag = int.Parse(array[0]) == 0;
				arg = int.Parse(array[1]);
				if (array.Length > 2)
				{
					arg2 = array[2];
					if (array.Length > 3)
					{
						flag2 = int.Parse(array[3]) == 0;
					}
				}
			}
		}
		if (flag)
		{
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdShowFinished, arg, arg2, 2);
		}
		else
		{
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdShowFailed, arg, arg2, 2);
		}
	}

	public void onFullAdClosed(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 0)
			{
				arg = array[0];
			}
		}
		RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.FullAdClosed, -1, arg, 1);
	}

	public void onFullAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 0)
			{
				arg = array[0];
			}
		}
		RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.FullAdClicked, -1, arg, 1);
	}

	public void onAdShow(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		int result = 1;
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 1)
			{
				int.TryParse(array[0], out result);
				arg = array[1];
			}
		}
		RiseSdk.AdEventType arg2 = RiseSdk.AdEventType.FullAdClicked;
		switch (result)
		{
		case 1:
			arg2 = RiseSdk.AdEventType.FullAdShown;
			break;
		case 2:
			arg2 = RiseSdk.AdEventType.RewardAdShowStart;
			break;
		case 3:
		case 4:
		case 5:
			arg2 = RiseSdk.AdEventType.AdShown;
			break;
		}
		RiseSdkListener.OnAdEvent(arg2, -1, arg, result);
	}

	public void onAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		int result = 1;
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 1)
			{
				int.TryParse(array[0], out result);
				arg = array[1];
			}
		}
		RiseSdk.AdEventType arg2 = RiseSdk.AdEventType.FullAdClicked;
		switch (result)
		{
		case 1:
			arg2 = RiseSdk.AdEventType.FullAdClicked;
			break;
		case 2:
			arg2 = RiseSdk.AdEventType.VideoAdClicked;
			break;
		case 3:
			arg2 = RiseSdk.AdEventType.BannerAdClicked;
			break;
		case 4:
			arg2 = RiseSdk.AdEventType.IconAdClicked;
			break;
		case 5:
			arg2 = RiseSdk.AdEventType.NativeAdClicked;
			break;
		}
		RiseSdkListener.OnAdEvent(arg2, -1, arg, result);
	}

	public void onVideoAdClosed(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 0)
			{
				arg = array[0];
			}
		}
		RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdClosed, -1, arg, 2);
	}

	public void onBannerAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 0)
			{
				arg = array[0];
			}
		}
		RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.BannerAdClicked, -1, arg, 3);
	}

	public void onCrossAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent == null || RiseSdkListener.OnAdEvent.GetInvocationList().Length <= 0)
		{
			return;
		}
		string arg = "Default";
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = data.Split('|');
			if (array != null && array.Length > 0)
			{
				arg = array[0];
			}
		}
		RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.CrossAdClicked, -1, arg, -1);
	}
}
