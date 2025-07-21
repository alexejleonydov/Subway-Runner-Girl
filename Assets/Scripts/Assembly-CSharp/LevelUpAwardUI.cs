using UnityEngine;

public class LevelUpAwardUI : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel bottom;

	[SerializeField]
	private UISprite icon;

	public void InitInfo(LevelUpPopUp.LevelUpAwardType upAwardType)
	{
		switch (upAwardType)
		{
		case LevelUpPopUp.LevelUpAwardType.newChar:
			title.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_NEW);
			bottom.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_ROLE);
			icon.spriteName = Characters.characterData[LevelUpPopUp.instance.canUnlockCharType].buttonIconSpriteName;
			break;
		case LevelUpPopUp.LevelUpAwardType.newScreen:
			title.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_SCREEN);
			bottom.text = Strings.Get(LevelUpPopUp.instance.newSceneName);
			icon.spriteName = "AX_new_scene_icon";
			break;
		case LevelUpPopUp.LevelUpAwardType.scoreMultiple:
			title.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_MULTIPLE);
			bottom.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_SCORE);
			icon.spriteName = "A_new_level_score_icon";
			break;
		case LevelUpPopUp.LevelUpAwardType.newHelmet:
			title.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_NEW);
			bottom.text = Strings.Get(LanguageKey.UI_POPUP_LEVEL_UP_HELMET);
			icon.spriteName = "icon_upgrades_helmet";
			break;
		}
	}
}
