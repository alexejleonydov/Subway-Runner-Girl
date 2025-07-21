using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class StringKeyValueRequest
	{
		public class StringData
		{
			public string userId;

			public string key;

			public string value;

			private Action<int, object> onRespondResult;

			public StringData(string userId, string key, string value, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				this.value = value;
				onRespondResult = onSuccess;
			}

			public void UploadStringDataListener(string s)
			{
				Debug.Log("Key:" + key + ", UploadStringDataListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (onRespondResult != null)
					{
						onRespondResult(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)(long)dictionary["status"] == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						Debug.Log(dictionary["msg"]);
					}
					if (onRespondResult != null)
					{
						onRespondResult(0, dictionary["msg"]);
					}
				}
				else if (onRespondResult != null)
				{
					onRespondResult(1, null);
				}
			}

			public void GetStringDataListener(string s)
			{
				Debug.Log("Key:" + key + ", GetStringDataListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (onRespondResult != null)
					{
						onRespondResult(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)(long)dictionary["status"] == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						Debug.Log(dictionary["msg"]);
					}
					if (onRespondResult != null)
					{
						onRespondResult(0, dictionary["msg"]);
					}
				}
				else if (!dictionary.ContainsKey("data"))
				{
					if (onRespondResult != null)
					{
						onRespondResult(-1, "error message");
					}
				}
				else if (onRespondResult != null)
				{
					onRespondResult(1, dictionary["data"]);
				}
			}
		}

		private static StringKeyValueRequest _instance;

		public static StringKeyValueRequest Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StringKeyValueRequest();
				}
				return _instance;
			}
		}

		public void UploadStringData(string userId, string key, string value, Action<int, object> handle = null)
		{
			StringData @object = new StringData(userId, key, value, handle);
			Dictionary<string, string> uploadStringDataDict = NetworkConnect.Instance.GetUploadStringDataDict(userId, key, value);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadStringData, uploadStringDataDict, @object.UploadStringDataListener);
		}

		public void GetStringData(string userId, string key, Action<int, object> handle = null)
		{
			StringData @object = new StringData(userId, key, null, handle);
			Dictionary<string, string> requestStringDataDict = NetworkConnect.Instance.GetRequestStringDataDict(userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetStringData, requestStringDataDict, @object.GetStringDataListener);
		}
	}
}
