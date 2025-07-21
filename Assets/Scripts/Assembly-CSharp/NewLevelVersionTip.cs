using Network;
using UnityEngine;

public class NewLevelVersionTip : UIBaseScreen
{
	[SerializeField]
	private UITexture headTexture;

	[SerializeField]
	public UILabel levelLabel;

	[SerializeField]
	public UILabel expLabel;

	[SerializeField]
	public UISlider expSlider;

	[SerializeField]
	public UILabel expPlanLabel;

	[SerializeField]
	public UILabel titleLabel;

	[SerializeField]
	public UILabel scoreMulite;

	[SerializeField]
	public NumberSprite scoreMultiple;

	public override void Show()
	{
		base.Show();
		RefreshHeadUI();
		levelLabel.text = "LV. " + PlayerInfo.Instance.amountOfLevel;
		float num = Game.Instance.GetExpCoefficient() * (float)(PlayerInfo.Instance.amountOfLevel - 1) + 8f;
		float num2 = 0f;
		expLabel.text = num2.ToString() + "/" + num;
		expSlider.value = num2 / num;
		expPlanLabel.text = Strings.Get(LanguageKey.UI_POPUP_NEWVERSIONTIP_DESCRIPTION);
		titleLabel.text = Strings.Get(LanguageKey.UI_POPUP_NEWVERSIONTIP_TITLE);
		scoreMultiple.SetLevelNumber(PlayerInfo.Instance.amountOfLevel);
		scoreMulite.text = Strings.Get(LanguageKey.UI_POPUP_PLAYER_LEVEL_SCORE_MULTIPLE);
	}

	private void OnEnable()
	{
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}
}
