using UnityEngine;

namespace Network
{
	public class Subscription : StringKeyValue
	{
		public Subscription(string key)
		{
			_key = "subscription";
			_playerPrefsKey = "Network_Subscription_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, string.Empty);
		}
	}
}
