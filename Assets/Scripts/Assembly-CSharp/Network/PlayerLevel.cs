using UnityEngine;

namespace Network
{
	public class PlayerLevel : StringKeyValue
	{
		public PlayerLevel(string key)
		{
			_key = "playerLevel";
			_playerPrefsKey = "Network_PlayerLevel_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, string.Empty);
		}
	}
}
