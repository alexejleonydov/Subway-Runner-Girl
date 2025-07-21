using Network;
using UnityEngine;

public class MyRankCell : MonoBehaviour
{
	[SerializeField]
	private UILabel rankLbl;

	[SerializeField]
	private UILabel scoreLbl;

	[SerializeField]
	private UILabel playerNameLbl;

	[SerializeField]
	private UILabel playerLevelLbl;

	[SerializeField]
	private UITexture headTxt;

	[SerializeField]
	private UISprite vipSpr;

	[SerializeField]
	private UISprite coutrySpr;

	[SerializeField]
	private GameObject changeNameBtn;

	[SerializeField]
	private GameObject inputGo;

	[SerializeField]
	private UIInput inputField;

	[SerializeField]
	private BoxCollider changeNameCollider;

	private RankScreen.RankPopupType rankType;

	private void Awake()
	{
		rankLbl.text = "--";
		scoreLbl.text = "-----";
		UIEventListener uIEventListener = UIEventListener.Get(changeNameBtn);
		uIEventListener.onClick = OnChangeNameClick;
		if (inputGo.activeInHierarchy)
		{
			inputGo.SetActive(false);
		}
	}

	private void OnEnable()
	{
		ServerManager.Instance.RegisterOnPlayerNameChange(RefreshPlayerName);
		ServerManager.Instance.RequestPlayerName();
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHead);
		ServerManager.Instance.RequestPictrueUrl();
		ServerManager.Instance.RegisterOnScoreChange(RefreshScore);
		ServerManager.Instance.RequestScore();
		ServerManager.Instance.RequestScoreVIP();
		ServerManager.Instance.RequestScoreGlobal();
		ServerManager.Instance.RegisterOnRankIDChange(RefreshRankID);
		ServerManager.Instance.RequestRankID();
		ServerManager.Instance.RegisterOnCountryCodeChange(RefreshCountryCode);
		ServerManager.Instance.RequestCountryCode();
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPlayerNameChange(RefreshPlayerName);
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHead);
		ServerManager.Instance.UnregisterOnScoreChange(RefreshScore);
		ServerManager.Instance.UnregisterOnRankIDChange(RefreshRankID);
		ServerManager.Instance.UnregisterOnCountryCodeChange(RefreshCountryCode);
	}

	public void OnChangeNameClick(GameObject go)
	{
		if (!inputGo.activeInHierarchy)
		{
			inputGo.SetActive(true);
		}
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null)
		{
			inputField.value = playerName.Value;
		}
	}

	public void SubmitNameClick()
	{
		if (inputGo.activeInHierarchy)
		{
			inputGo.SetActive(false);
		}
		string text = inputField.value.Trim();
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName != null && !string.IsNullOrEmpty(text) && !text.Equals(playerName.Value))
		{
			ServerManager.Instance.UploadPlayerName(text);
		}
	}

	public void OnChangeNameCloseClick()
	{
		if (inputGo.activeInHierarchy)
		{
			inputGo.SetActive(false);
		}
	}

	public void Show()
	{
		if (PlayerInfo.Instance.autoShowChangePlayerName && ServerManager.Instance.CanUploadPlayerName())
		{
			if (!inputGo.activeInHierarchy)
			{
				inputGo.SetActive(true);
			}
			PlayerName playerName = ServerManager.Instance.PlayerName;
			if (playerName != null)
			{
				inputField.value = playerName.Value;
			}
			PlayerInfo.Instance.autoShowChangePlayerName = false;
		}
	}

	public void UpdateUI(RankScreen.RankPopupType type)
	{
		rankType = type;
		RefreshPlayerName();
		RefreshPlayerLevel();
		RefreshScore();
		RefreshCountryCode();
		RefreshSubscription();
		RefreshRankID();
		RefreshHead();
	}

	private void RefreshRankID()
	{
		int num = -1;
		if (rankType == RankScreen.RankPopupType.HighScore)
		{
			RankID rankID_Week = ServerManager.Instance.RankID_Week;
			if (rankID_Week != null)
			{
				num = rankID_Week.rankID;
			}
		}
		else if (rankType == RankScreen.RankPopupType.VIP)
		{
			RankID rankID_VIP = ServerManager.Instance.RankID_VIP;
			if (rankID_VIP != null)
			{
				num = rankID_VIP.rankID;
			}
		}
		else if (rankType == RankScreen.RankPopupType.Friends)
		{
			RankIDFixed rankID_Global = ServerManager.Instance.RankID_Global;
			if (rankID_Global != null)
			{
				num = rankID_Global.rankID;
			}
		}
		if (num == -1)
		{
			rankLbl.text = "--";
		}
		else
		{
			rankLbl.text = num.ToString();
		}
	}

	private void RefreshScore()
	{
		if (rankType == RankScreen.RankPopupType.HighScore)
		{
			Score score_Week = ServerManager.Instance.Score_Week;
			if (score_Week != null)
			{
				scoreLbl.text = score_Week.score.ToString();
			}
		}
		else if (rankType == RankScreen.RankPopupType.VIP)
		{
			Score score_Vip = ServerManager.Instance.Score_Vip;
			if (score_Vip != null)
			{
				scoreLbl.text = score_Vip.score.ToString();
			}
		}
		else if (rankType == RankScreen.RankPopupType.Friends)
		{
			ScoreFixed score_Global = ServerManager.Instance.Score_Global;
			if (score_Global != null)
			{
				scoreLbl.text = score_Global.score.ToString();
			}
		}
	}

	private void RefreshCountryCode()
	{
		CountryCode countryCode = ServerManager.Instance.CountryCode;
		if (countryCode != null)
		{
			coutrySpr.spriteName = countryCode.Value + "@2x";
		}
	}

	private void RefreshSubscription()
	{
		vipSpr.enabled = PlayerInfo.Instance.hasSubscribed;
	}

	private void RefreshHead()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTxt.mainTexture = pictureUrl.Image;
		}
	}

	private void RefreshPlayerName()
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
		changeNameCollider.enabled = ServerManager.Instance.CanUploadPlayerName();
	}

	private void RefreshPlayerLevel()
	{
		playerLevelLbl.text = string.Format(Strings.Get(LanguageKey.RANK_SCREEN_LV_LABEL), PlayerInfo.Instance.amountOfLevel);
	}
}
