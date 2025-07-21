using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopUp : UIBaseScreen
{
	public enum LevelUpAwardType
	{
		newChar = 0,
		newScreen = 1,
		scoreMultiple = 2,
		newHelmet = 3
	}

	[SerializeField]
	private UILabel label_levelup;

	[SerializeField]
	private UILabel label_multiple;

	[SerializeField]
	private UILabel label_locktip;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UISprite[] scoreMultiple = new UISprite[3];

	[SerializeField]
	private LevelUpAwardUI[] levelUpAwards = new LevelUpAwardUI[3];

	[HideInInspector]
	public LanguageKey newSceneName;

	private Dictionary<LevelUpAwardType, int> levelAwardDic = new Dictionary<LevelUpAwardType, int>();

	public static LevelUpPopUp instance;

	[SerializeField]
	private AudioSource audioSource;

	public Characters.CharacterType canUnlockCharType;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void OnEnable()
	{
		audioSource.Play();
		RefreshInfo();
		label_levelup.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_TITILE);
		label_multiple.text = Strings.Get(LanguageKey.UI_POPUP_PLAYER_LEVEL_SCORE_MULTIPLE);
		label_locktip.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_YOU_UNLOCK);
	}

	protected override void AfterShow()
	{
		base.AfterShow();
		RefreshInfo();
	}

	public void ShowScoreMultiple()
	{
		level.text = PlayerInfo.Instance.amountOfLevel.ToString();
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

	private void RefreshInfo()
	{
		ShowScoreMultiple();
		levelAwardDic = new Dictionary<LevelUpAwardType, int>();
		City city = TrackController.Instance.NextTaskSetWithUnlockCity(PlayerInfo.Instance.amountOfLevel);
		levelAwardDic.Add(LevelUpAwardType.scoreMultiple, 1);
		canUnlockCharType = Characters.CharacterType.none;
		foreach (Characters.CharacterType value in Enum.GetValues(typeof(Characters.CharacterType)))
		{
			if (value != Characters.CharacterType.none && Characters.characterData[value].Level != 0 && PlayerInfo.Instance.amountOfLevel == Characters.characterData[value].Level)
			{
				canUnlockCharType = value;
				levelAwardDic.Add(LevelUpAwardType.newChar, 1);
			}
		}
		if (PlayerInfo.Instance.amountOfLevel == city.lockedTaskSet)
		{
			newSceneName = city.cityName;
			levelAwardDic.Add(LevelUpAwardType.newScreen, 1);
		}
		foreach (Helmets.HelmType value2 in Enum.GetValues(typeof(Helmets.HelmType)))
		{
			if (value2 != 0 && Helmets.helmData[value2].level != 0 && PlayerInfo.Instance.amountOfLevel == Helmets.helmData[value2].level)
			{
				levelAwardDic.Add(LevelUpAwardType.newHelmet, 1);
			}
		}
		switch (levelAwardDic.Count)
		{
		case 1:
			levelUpAwards[0].gameObject.SetActive(true);
			levelUpAwards[0].transform.localPosition = new Vector3(0f, -38f, 0f);
			levelUpAwards[1].gameObject.SetActive(false);
			levelUpAwards[2].gameObject.SetActive(false);
			break;
		case 2:
			levelUpAwards[0].gameObject.SetActive(true);
			levelUpAwards[0].transform.localPosition = new Vector3(-100f, -38f, 0f);
			levelUpAwards[1].gameObject.SetActive(true);
			levelUpAwards[1].transform.localPosition = new Vector3(100f, -38f, 0f);
			levelUpAwards[2].gameObject.SetActive(false);
			break;
		case 3:
			levelUpAwards[0].gameObject.SetActive(true);
			levelUpAwards[0].transform.localPosition = new Vector3(-180f, -38f, 0f);
			levelUpAwards[1].gameObject.SetActive(true);
			levelUpAwards[1].transform.localPosition = new Vector3(0f, -38f, 0f);
			levelUpAwards[2].gameObject.SetActive(true);
			levelUpAwards[2].transform.localPosition = new Vector3(180f, -38f, 0f);
			break;
		}
		int num = 0;
		foreach (KeyValuePair<LevelUpAwardType, int> item in levelAwardDic)
		{
			if (item.Value == 1)
			{
				levelUpAwards[num].InitInfo(item.Key);
				num++;
			}
		}
		levelAwardDic.Clear();
	}
}
