using UnityEngine;

public class UISlideInTaskHelper : UISlideIn
{
	public UILabel line1;

	public UILabel line2;

	public void SetupSlideInTask(string message)
	{
		base.gameObject.SetActive(true);
		line1.text = message;
		line2.text = Strings.Get(LanguageKey.UI_TOP_TIP_MISSION_COMPLETE);
		SlideIn(null);
	}

	public void SetupSlideInAchievement(string message)
	{
		base.gameObject.SetActive(true);
		line1.text = message;
		line2.text = Strings.Get(LanguageKey.UI_TOP_TIP_ACHIEVEMENT_COMPLETE);
		SlideIn(null);
	}
}
