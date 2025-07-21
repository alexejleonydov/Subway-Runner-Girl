using Network;
using UnityEngine;

public class PlayerLevelPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel tip;

	[SerializeField]
	private UILabel scoretip;

	[SerializeField]
	private UILabel nowLevel;

	[SerializeField]
	private UILabel expLabel;

	[SerializeField]
	private UILabel expDescripe;

	[SerializeField]
	private UILabel lockDescripe;

	[SerializeField]
	private UILabel unlockDescripe;

	[SerializeField]
	private UISlider expSlider;

	[SerializeField]
	private UISprite[] scoreMultiple = new UISprite[3];

	[SerializeField]
	private UITexture headTexture;

	private TaskInfo[] _currentTasks;

	private void OnEnable()
	{
		RefreshInfo();
		City city = TrackController.Instance.NextTaskSetWithUnlockCity(PlayerInfo.Instance.amountOfLevel);
		tip.text = Strings.Get(LanguageKey.UI_POPUP_PLAYER_LEVEL_INFO);
		scoretip.text = Strings.Get(LanguageKey.UI_POPUP_PLAYER_LEVEL_SCORE_MULTIPLE);
		expDescripe.text = Strings.Get(LanguageKey.UI_POPUP_PLAYER_LEVEL_EXP_DESCRIPTION);
		lockDescripe.text = string.Format(Strings.Get(LanguageKey.TASK_POPUP_LOCK_LABEL), Strings.Get(city.cityName));
		unlockDescripe.text = string.Format(Strings.Get(LanguageKey.TASK_POPUP_UNLOCK_LABEL), Strings.Get(city.cityName));
		if (city != null)
		{
			if (city.lockedTaskSet > PlayerInfo.Instance.amountOfLevel)
			{
				lockDescripe.transform.parent.gameObject.SetActive(true);
				unlockDescripe.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				lockDescripe.transform.parent.gameObject.SetActive(false);
				unlockDescripe.transform.parent.gameObject.SetActive(true);
			}
		}
		else
		{
			lockDescripe.transform.parent.gameObject.SetActive(false);
			unlockDescripe.transform.parent.gameObject.SetActive(false);
		}
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	public override void Show()
	{
		base.Show();
		RefreshHeadUI();
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}

	private void RefreshInfo()
	{
		float num = Game.Instance.GetExpCoefficient() * (float)(PlayerInfo.Instance.amountOfLevel - 1) + 8f;
		expSlider.value = (float)PlayerInfo.Instance.amountOfExp / num;
		expLabel.text = PlayerInfo.Instance.amountOfExp + "/" + num;
		nowLevel.text = "LV. " + PlayerInfo.Instance.amountOfLevel;
		if (PlayerInfo.Instance.amountOfLevel < 10)
		{
			scoreMultiple[0].enabled = true;
			scoreMultiple[0].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel;
			scoreMultiple[1].enabled = false;
			scoreMultiple[2].enabled = false;
		}
		else if (PlayerInfo.Instance.amountOfLevel < 100)
		{
			scoreMultiple[0].enabled = true;
			scoreMultiple[0].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel / 10;
			scoreMultiple[1].enabled = true;
			scoreMultiple[1].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel % 10;
			scoreMultiple[2].enabled = false;
		}
		else
		{
			scoreMultiple[0].enabled = true;
			scoreMultiple[0].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel / 100;
			scoreMultiple[1].enabled = true;
			scoreMultiple[1].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel % 100 / 10;
			scoreMultiple[2].enabled = true;
			scoreMultiple[2].spriteName = "A_sz" + PlayerInfo.Instance.amountOfLevel % 100 % 10;
		}
	}
}
