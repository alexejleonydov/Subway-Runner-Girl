using UnityEngine;

public class Version : MonoBehaviour
{
	[SerializeField]
	private GameObject child;

	private void Awake()
	{
		StartCoroutine(DelayInvoke.start(delegate
		{
			Check();
		}, 3f));
		child.SetActive(false);
	}

	private void Check()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			CancelUpdate();
		}
		else
		{
			CheckUpdate();
		}
	}

	private void CancelUpdate()
	{
		Object.Destroy(base.gameObject);
	}

	private void CheckUpdate()
	{
		string remoteConfigString = RiseSdk.Instance.GetRemoteConfigString("data");
		if (string.IsNullOrEmpty(remoteConfigString))
		{
			CancelUpdate();
		}
		else if (Application.version.Equals(remoteConfigString))
		{
			if (PlayerInfo.Instance.updateFromLastApp)
			{
				GetReward();
			}
			PlayerInfo.Instance.updateFromLastApp = false;
			CancelUpdate();
		}
		else
		{
			PlayerInfo.Instance.updateFromLastApp = true;
			child.SetActive(true);
		}
	}

	public void ShowUpdate()
	{
		NewUpdatePopup.ShowUpdate = true;
		UIScreenController.Instance.PushPopup("NewUpdatePopup");
	}

	public void GetReward()
	{
		NewUpdatePopup.ShowUpdate = false;
		UIScreenController.Instance.PushPopup("NewUpdatePopup");
	}
}
