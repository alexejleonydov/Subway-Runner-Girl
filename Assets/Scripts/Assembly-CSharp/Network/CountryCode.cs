using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class CountryCode : StringKeyValue
	{
		public override string Value
		{
			get
			{
				if ("NotSet".Equals(_value))
				{
					GetStringValue();
				}
				return _value;
			}
			protected set
			{
				base.Value = value;
			}
		}

		public CountryCode(string key)
		{
			_key = "countryCode";
			_playerPrefsKey = "Network_CountryCode_" + key;
			_value = PlayerPrefs.GetString(_playerPrefsKey, "NotSet");
			GetStringValue();
		}

		protected override void UploadWithLocalValueByExpire()
		{
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetCountryCode, null, GetCountryCodeListener);
		}

		private void GetCountryCodeListener(string s)
		{
			Debug.Log("GetCountryCodeListener:" + s);
			if (!string.IsNullOrEmpty(s))
			{
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if (dictionary != null && dictionary.ContainsKey("countryCode"))
				{
					UploadKeyValue_Force((string)dictionary["countryCode"]);
				}
			}
		}

		public override void OnValueChange()
		{
			base.OnValueChange();
			ServerManager.Instance.OnCountryCodeChange();
		}
	}
}
