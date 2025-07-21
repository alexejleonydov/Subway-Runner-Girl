using Network;
using UnityEngine;

public class TopRunCell : RankCell
{
	[SerializeField]
	private UISprite rankSpr;

	[SerializeField]
	private UILabel rankLbl;

	[SerializeField]
	private UISprite fillTopSpr;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite avatarTopSpr;

	[SerializeField]
	private UISprite avatarSpr;

	[SerializeField]
	private UITexture headTopTxt;

	[SerializeField]
	private UITexture headTxt;

	[SerializeField]
	private UILabel playerNameToplbl;

	[SerializeField]
	private UILabel playerNamelbl;

	[SerializeField]
	private UILabel scroeTopLbl;

	[SerializeField]
	private UILabel scroeLbl;

	[SerializeField]
	private UILabel playerLevelLbl;

	[SerializeField]
	private UISprite countrySpr;

	[SerializeField]
	private UISprite vipTip;

	private TopRun _data;

	private TopRunInfo _info;

	private const string goldSpriteName = "Rank_icon_place_first";

	private const string sliverSpriteName = "Rank_icon_place_second";

	private const string bronzeSpriteName = "Rank_icon_place_third";

	public override void RefreshUI(TopRun topRun)
	{
		if (_data == topRun)
		{
			return;
		}
		_data = topRun;
		_info = ServerManager.Instance.GetTopRunInfo(topRun);
		if (_data == null)
		{
			Object.Destroy(base.gameObject, 0.1f);
			return;
		}
		if (_data.rank <= 3)
		{
			rankSpr.enabled = true;
			rankLbl.enabled = false;
			if (_data.rank == 1)
			{
				rankSpr.spriteName = "Rank_icon_place_first";
			}
			if (_data.rank == 2)
			{
				rankSpr.spriteName = "Rank_icon_place_second";
			}
			if (_data.rank == 3)
			{
				rankSpr.spriteName = "Rank_icon_place_third";
			}
			scroeTopLbl.enabled = true;
			scroeTopLbl.text = _data.highestScore.ToString();
			scroeLbl.enabled = false;
			fillSpr.enabled = false;
			fillTopSpr.enabled = true;
		}
		else
		{
			rankSpr.enabled = false;
			rankLbl.enabled = true;
			rankLbl.text = _data.rank.ToString();
			scroeLbl.enabled = true;
			scroeLbl.text = _data.highestScore.ToString();
			scroeTopLbl.enabled = false;
			fillTopSpr.enabled = false;
			fillSpr.enabled = _data.rank % 2 == 0;
		}
		RefreshPlayerName();
		RefreshVIP();
		RefreshCoutryCode();
		RefreshImage();
		RefreshPlayrLevel();
	}

	public void RefreshPlayerName()
	{
		if (_data.rank <= 3)
		{
			playerNameToplbl.enabled = true;
			playerNameToplbl.text = _info.playerName;
			playerNamelbl.enabled = false;
		}
		else
		{
			playerNamelbl.enabled = true;
			playerNamelbl.text = _info.playerName;
			playerNameToplbl.enabled = false;
		}
	}

	public void RefreshVIP()
	{
		if (vipTip != null)
		{
			vipTip.enabled = "yes".Equals(_info.isVip);
		}
	}

	public void RefreshCoutryCode()
	{
		if (countrySpr != null)
		{
			countrySpr.spriteName = _info.countryCode + "@2x";
		}
	}

	public void RefreshImage()
	{
		if (_data.rank <= 3)
		{
			avatarTopSpr.enabled = true;
			avatarSpr.enabled = false;
			headTopTxt.enabled = true;
			headTopTxt.mainTexture = ImageManager.Instance.GetTexture(_info.pictureUrl);
			headTxt.enabled = false;
		}
		else
		{
			avatarTopSpr.enabled = false;
			avatarSpr.enabled = true;
			headTxt.enabled = true;
			headTxt.mainTexture = ImageManager.Instance.GetTexture(_info.pictureUrl);
			headTopTxt.enabled = false;
		}
	}

	public void RefreshPlayrLevel()
	{
		if (playerLevelLbl != null)
		{
			playerLevelLbl.text = string.Format(Strings.Get(LanguageKey.RANK_SCREEN_LV_LABEL), _info.playerLevel);
		}
	}
}
