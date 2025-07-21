using UnityEngine;

public class Settings : MonoBehaviour
{
	private static bool _optionsLoaded;

	private static bool _optionSound;

	private const int OPTION_SOUND_DEFAULT = 1;

	private const string OPTION_SOUND_KEY = "OPTION_SOUND";

	public static bool optionSound
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionSound;
		}
		set
		{
			_optionSound = value;
			AudioListener.volume = (_optionSound ? 1f : 0f);
			PlayerPrefs.SetInt("OPTION_SOUND", _optionSound ? 1 : 0);
		}
	}

	private void Awake()
	{
		LoadOptionsIfNeeded();
	}

	private static void LoadOptionsIfNeeded()
	{
		if (!_optionsLoaded)
		{
			_optionSound = PlayerPrefs.GetInt("OPTION_SOUND", 1) != 0;
			AudioListener.volume = (_optionSound ? 1f : 0f);
			_optionsLoaded = true;
		}
	}
}
