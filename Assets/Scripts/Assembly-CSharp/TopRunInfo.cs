using System;

[Serializable]
public class TopRunInfo
{
	public string playerName;

	public string facebookName;

	public string countryCode;

	public string isVip;

	public string playerLevel;

	public string pictureUrl;

	public TopRunInfo()
	{
		playerName = "----";
		facebookName = "----";
		playerLevel = "1";
		countryCode = "NotSet";
		isVip = "no";
		pictureUrl = null;
	}
}
