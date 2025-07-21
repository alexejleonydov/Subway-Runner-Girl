using UnityEngine;

public class UISlideInUnlock : UISlideIn
{
	public UILabel youUnlockLbl;

	public UILabel UnlockName;

	public void SetupSlideInUnlock(string message)
	{
		youUnlockLbl.text = Strings.Get(LanguageKey.UI_TOP_TIP_YOU_UNLOCK);
		base.gameObject.SetActive(true);
		UnlockName.text = message;
		SlideIn(null);
	}
}
