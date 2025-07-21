using System;
using Network;
using UnityEngine;

public class RankScreen : UIBaseScreen
{
	public enum RankPopupType
	{
		HighScore = 0,
		VIP = 1,
		Friends = 2,
		None = 3
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel globalLbl;

	[SerializeField]
	private UILabel vipLbl;

	[SerializeField]
	private UILabel friendLbl;

	[SerializeField]
	private UILabel changeNameLbl;

	[SerializeField]
	private UILabel submitLbl;

	[SerializeField]
	private UILabel connectLbl;

	[SerializeField]
	private MyRankCell myRankCell;

	[SerializeField]
	private TimeLeft timeLeft;

	[SerializeField]
	private GlobalScrollView highScore;

	[SerializeField]
	private VipScrollView vip;

	[SerializeField]
	private FriendsScrollView friends;

	[SerializeField]
	private RankToggles toggles;

	[SerializeField]
	private BoxCollider connectCollider;

	private RankPopupType currentType;

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_HIGHSCORE_TITLE);
		globalLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_GLOBAL_TITLE);
		vipLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_VIP_TITLE);
		friendLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_FRIEND_TITLE);
		changeNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_TITLE);
		submitLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_SUBMIT);
	}

	public override void Show()
	{
		base.Show();
		RefreshLabel();
		RefreshFacebook();
		if (Application.internetReachability == NetworkReachability.NotReachable || !SecondManager.Instance.hasInited || !ServeTimeUpdate.Instance.ServerTimeValid())
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
			return;
		}
		toggles.Global();
		ChangeState(RankPopupType.HighScore);
		myRankCell.Show();
	}

	public override void Hide()
	{
		if (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.CloseAllPopups();
		}
		currentType = RankPopupType.None;
		highScore.Hide();
		vip.Hide();
		friends.Hide();
		base.Hide();
	}

	private void RefreshFacebook()
	{
		if (SecondManager.Instance.facebook)
		{
			connectCollider.enabled = false;
			connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
		}
		else
		{
			connectCollider.enabled = true;
			connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_CONNECT);
		}
	}

	public void ChangeState(RankPopupType type)
	{
		currentType = type;
		switch (type)
		{
		case RankPopupType.HighScore:
			NGUITools.SetActive(highScore.gameObject, true);
			NGUITools.SetActive(vip.gameObject, false);
			NGUITools.SetActive(friends.gameObject, false);
			NGUITools.SetActive(timeLeft.gameObject, true);
			highScore.Show();
			break;
		case RankPopupType.VIP:
			NGUITools.SetActive(highScore.gameObject, false);
			NGUITools.SetActive(vip.gameObject, true);
			NGUITools.SetActive(friends.gameObject, false);
			NGUITools.SetActive(timeLeft.gameObject, false);
			vip.Show();
			break;
		case RankPopupType.Friends:
			NGUITools.SetActive(highScore.gameObject, false);
			NGUITools.SetActive(vip.gameObject, false);
			NGUITools.SetActive(friends.gameObject, true);
			NGUITools.SetActive(timeLeft.gameObject, false);
			friends.Show();
			break;
		default:
			NGUITools.SetActive(highScore.gameObject, false);
			NGUITools.SetActive(vip.gameObject, false);
			NGUITools.SetActive(friends.gameObject, false);
			NGUITools.SetActive(timeLeft.gameObject, false);
			break;
		}
		myRankCell.UpdateUI(type);
	}

	public void ToggleGlobalScrollView()
	{
		toggles.Global();
		ChangeState(RankPopupType.HighScore);
	}

	public void ToggleVIPScrollView()
	{
		toggles.Vip();
		ChangeState(RankPopupType.VIP);
	}

	public void ToggleFriendScrollView()
	{
		toggles.Friend();
		ChangeState(RankPopupType.Friends);
	}

	public void OnConnectClick()
	{
		FacebookManger instance = FacebookManger.Instance;
		instance.OnFacebookLoginResult = (Action<bool>)Delegate.Remove(instance.OnFacebookLoginResult, new Action<bool>(OnLoginFacebookResult));
		FacebookManger instance2 = FacebookManger.Instance;
		instance2.OnFacebookLoginResult = (Action<bool>)Delegate.Combine(instance2.OnFacebookLoginResult, new Action<bool>(OnLoginFacebookResult));
		FacebookManger.Instance.LoginFacebook();
	}

	private void OnLoginFacebookResult(bool result)
	{
		FacebookManger instance = FacebookManger.Instance;
		instance.OnFacebookLoginResult = (Action<bool>)Delegate.Remove(instance.OnFacebookLoginResult, new Action<bool>(OnLoginFacebookResult));
		if (result)
		{
			connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
			connectCollider.enabled = false;
		}
	}
}
