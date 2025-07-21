using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class KeyJsonValueRequest
	{
		public class JsonData
		{
			public string userId;

			public string key;

			public string json;

			public Action<int, object> onRespondResult;

			public JsonData(string userId, string key, string json, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				this.json = json;
				onRespondResult = onSuccess;
			}

			public void UploadDataListener(string s)
			{
				Debug.Log("Key:" + key + ", UploadDataListener:" + s);
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

			public void GetDataListener(string s)
			{
				Debug.Log("Key:" + key + ", GetDataListener:" + s);
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
						onRespondResult(-1, "Is not contains data!");
					}
				}
				else if (onRespondResult != null)
				{
					onRespondResult(1, dictionary["data"]);
				}
			}
		}

		private static KeyJsonValueRequest _instance;

		private Dictionary<string, JsonData> jsonKeyValueList;

		public static KeyJsonValueRequest Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new KeyJsonValueRequest();
				}
				return _instance;
			}
		}

		private KeyJsonValueRequest()
		{
			jsonKeyValueList = new Dictionary<string, JsonData>();
		}

		public void UploadJsonData(string userId, string key, string json, Action<int, object> handle = null)
		{
			if (!jsonKeyValueList.ContainsKey(key))
			{
				jsonKeyValueList.Add(key, new JsonData(userId, key, json, handle));
			}
			JsonData @object = new JsonData(userId, key, json, handle);
			Dictionary<string, string> uploadJsonDataDict = NetworkConnect.Instance.GetUploadJsonDataDict(userId, key, json);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadJsonData, uploadJsonDataDict, @object.UploadDataListener);
		}

		public void GetJsonData(string userId, string key, string jsonKey, Action<int, object> handle = null)
		{
			if (!jsonKeyValueList.ContainsKey(key))
			{
				jsonKeyValueList.Add(key, new JsonData(userId, key, null, handle));
			}
			JsonData @object = jsonKeyValueList[key];
			Dictionary<string, string> requestJsonDataDict = NetworkConnect.Instance.GetRequestJsonDataDict(userId, key, jsonKey);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetJsonData, requestJsonDataDict, @object.GetDataListener);
		}

		public void GetAllJsonData(string userId, string key, Action<int, object> handle = null)
		{
			if (!jsonKeyValueList.ContainsKey(key))
			{
				jsonKeyValueList.Add(key, new JsonData(userId, key, null, handle));
			}
			JsonData @object = jsonKeyValueList[key];
			Dictionary<string, string> requestAllJsonDataDict = NetworkConnect.Instance.GetRequestAllJsonDataDict(userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetAllInnerJsonData, requestAllJsonDataDict, @object.GetDataListener);
		}
	}
}
