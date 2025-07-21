using UnityEngine;

namespace Network
{
	public class PlayerName : StringKeyValue
	{
		public PlayerName(string key)
		{
			_key = "playerName";
			_playerPrefsKey = "Network_PlayerName_" + key;
			base.Value = PlayerPrefs.GetString(_playerPrefsKey, "player");
			GetStringValue();
		}

		public PlayerName(string key, string value)
		{
			_key = "playerName";
			_playerPrefsKey = "Network_PlayerName_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, "player");
			UploadKeyValue_Force(value);
		}

		protected override void UploadWithLocalValueByExpire()
		{
			UploadKeyValue_Force(_value);
		}

		public override void OnValueChange()
		{
			base.OnValueChange();
			ServerManager.Instance.OnPlayerNameChange();
		}
	}
}
