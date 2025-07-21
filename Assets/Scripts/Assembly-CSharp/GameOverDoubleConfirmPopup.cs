using UnityEngine;

public class GameOverDoubleConfirmPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel descriptionLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel reminderLbl;

	[SerializeField]
	private GameObject confirmGo;

	[SerializeField]
	private UIToggle toggle;

	public override void Init()
	{
		base.Init();
		UIEventListener uIEventListener = UIEventListener.Get(confirmGo);
		uIEventListener.onClick = OnDoubleClick;
	}

	public override void Show()
	{
		base.Show();
		if (PlayerInfo.Instance.firstGameOverNoRemind)
		{
			PlayerInfo.Instance.firstGameOverNoRemind = false;
			PlayerInfo.Instance.gameOverDoubleConfirmNoRemind = true;
		}
		toggle.value = PlayerInfo.Instance.gameOverDoubleConfirmNoRemind;
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_TITLE);
		descriptionLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_CONTENT);
		watchLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_BUTTON_WATCH);
		reminderLbl.text = Strings.Get(LanguageKey.UI_POPUP_CONFIRM_NO_REMINDER);
	}

	private void OnDoubleClick(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				VideoLoadingPopup.adType = 2;
				VideoLoadingPopup.rewardId = 4;
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

	public void OnToggleChange()
	{
		PlayerInfo.Instance.gameOverDoubleConfirmNoRemind = UIToggle.current.value;
	}
}
