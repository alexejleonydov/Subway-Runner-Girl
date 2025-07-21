using UnityEngine;

public class SoundButton : MonoBehaviour
{
	[SerializeField]
	private UILabel musicLbl;

	[SerializeField]
	private UILabel onLbl;

	[SerializeField]
	private UILabel offLbl;

	[SerializeField]
	private GameObject on;

	[SerializeField]
	private GameObject off;

	private void Awake()
	{
		SetActive();
	}

	private void OnEnable()
	{
		musicLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC);
		onLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC_OPEN);
		offLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC_CLOSE);
	}

	private void SetActive()
	{
		on.SetActive(Settings.optionSound);
		off.SetActive(!Settings.optionSound);
	}

	public void ClickON()
	{
		if (!Settings.optionSound)
		{
			Settings.optionSound = true;
		}
		SetActive();
	}

	public void ClickOFF()
	{
		if (Settings.optionSound)
		{
			Settings.optionSound = false;
		}
		SetActive();
	}
}
