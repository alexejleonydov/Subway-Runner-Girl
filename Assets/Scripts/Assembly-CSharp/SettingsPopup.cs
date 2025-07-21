using System;
using Network;
using UnityEngine;

public class SettingsPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel nameLbl;

	[SerializeField]
	private UILabel redeemLbl;

	[SerializeField]
	private UILabel followLbl;

	[SerializeField]
	private UILabel enterNameLbl;

	[SerializeField]
	private UILabel submitLbl;

	[SerializeField]
	private UILabel redeemCodeLbl;

	[SerializeField]
	private UILabel connectLbl;

	[SerializeField]
	private UILabel loginedLbl;

	[SerializeField]
	private UILabel followUsLbl;

	[SerializeField]
	private UILabel followUsRedeemLbl;

	[SerializeField]
	private UILabel okLbl;

	[SerializeField]
	private UITexture headTexture;

	[SerializeField]
	private UILabel playerNameLbl;

	[SerializeField]
	private UISprite[] changeNameSprs;

	[SerializeField]
	private BoxCollider changeNameCollider;

	[SerializeField]
	private GameObject changeNamePanel;

	[SerializeField]
	private UIInput inputField;

	[SerializeField]
	private GameObject sendRecodePanel;

	[SerializeField]
	private UIInput inputRecodeField;

	[SerializeField]
	private GameObject loginFacebookGo;

	[SerializeField]
	private GameObject hasLoginFacebookGo;

	private string inputName;

	public static bool IsInGameOver;

	public static bool IsInPause;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(changeNameCollider.gameObject);
		uIEventListener.onClick = OnChangeNameClick;
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_TITLE);
		nameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_NAME);
		redeemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_REDEEM);
		followLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_FOLLOW);
		enterNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_TITLE);
		submitLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_NAME_SUBMIT);
		redeemCodeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_TITLE);
		followUsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_FOLLOW_US);
		followUsRedeemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_FOLLOW_REDEEM);
		okLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_INPUT_REDEEM_OK);
		connectLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_CONNECT);
		loginedLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_LGINED);
	}

	public override void Show()
	{
		base.Show();
		RefreshPlayerNameUI();
		RefreshLabel();
		RefreshHeadUI();
		if (FacebookManger.Instance.facebookHasLogin)
		{
			if (loginFacebookGo.activeInHierarchy)
			{
				loginFacebookGo.SetActive(false);
			}
			if (!hasLoginFacebookGo.activeInHierarchy)
			{
				hasLoginFacebookGo.SetActive(true);
			}
		}
		else
		{
			if (!loginFacebookGo.activeInHierarchy)
			{
				loginFacebookGo.SetActive(true);
			}
			if (hasLoginFacebookGo.activeInHierarchy)
			{
				hasLoginFacebookGo.SetActive(false);
			}
		}
	}

	public void OnSendGiftClick()
	{
		sendRecodePanel.SetActive(true);
		inputRecodeField.value = string.Empty;
	}

	public void CloseSendRecodePanel()
	{
		sendRecodePanel.SetActive(false);
	}

	public void SendGiftRecode()
	{
		sendRecodePanel.SetActive(false);
		RecodeManager.Instance.SendRecode(inputRecodeField.value.ToString());
	}

	public void OnChangeNameClick(GameObject go)
	{
		changeNamePanel.SetActive(true);
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null)
		{
			inputField.value = playerName.Value;
		}
	}

	public void OnChangeNameCloseClick()
	{
		changeNamePanel.SetActive(false);
	}

	public void SubmitNameClick()
	{
		changeNamePanel.SetActive(false);
		inputName = inputField.value;
		inputName = inputName.Trim();
		if (!string.IsNullOrEmpty(inputName))
		{
			ServerManager.Instance.UploadPlayerName(inputName);
		}
	}

	private void OnEnable()
	{
		ServerManager.Instance.RegisterOnPlayerNameChange(RefreshPlayerNameUI);
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPlayerNameChange(RefreshPlayerNameUI);
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void RefreshPlayerNameUI()
	{
		if (SecondManager.Instance.facebook)
		{
			playerNameLbl.text = FacebookManger.Instance.me.name;
		}
		else
		{
			PlayerName playerName = ServerManager.Instance.PlayerName;
			if (playerName != null)
			{
				playerNameLbl.text = playerName.Value;
			}
		}
		if (ServerManager.Instance.CanUploadPlayerName())
		{
			int i = 0;
			for (int num = changeNameSprs.Length; i < num; i++)
			{
				changeNameSprs[i].color = Color.white;
			}
			changeNameCollider.enabled = true;
		}
		else
		{
			int j = 0;
			for (int num2 = changeNameSprs.Length; j < num2; j++)
			{
				changeNameSprs[j].color = Color.cyan;
			}
			changeNameCollider.enabled = false;
		}
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}

	public void URL()
	{
		FacebookManger.Instance.URL();
	}

	public void OnFacebookLoginClick()
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
			if (loginFacebookGo.activeInHierarchy)
			{
				loginFacebookGo.SetActive(false);
			}
			if (!hasLoginFacebookGo.activeInHierarchy)
			{
				hasLoginFacebookGo.SetActive(true);
			}
			int i = 0;
			for (int num = changeNameSprs.Length; i < num; i++)
			{
				changeNameSprs[i].color = Color.cyan;
			}
			changeNameCollider.enabled = false;
		}
	}
}
