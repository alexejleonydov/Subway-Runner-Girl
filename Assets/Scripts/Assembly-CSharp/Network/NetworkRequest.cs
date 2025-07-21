using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class NetworkRequest : MonoBehaviour
	{
		[Serializable]
		public class RequestData
		{
			public NetworkConnect.RequestCommand command;

			public Dictionary<string, string> dataDic;

			public Action<string> listener;

			public int order;
		}

		private static NetworkRequest _instance;

		public Dictionary<int, Queue<RequestData>> commandQueue;

		public static NetworkRequest Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject("NetworkRequest").AddComponent<NetworkRequest>();
				}
				return _instance;
			}
		}

		private void Start()
		{
			commandQueue = new Dictionary<int, Queue<RequestData>>();
		}

		public void Request(NetworkConnect.RequestCommand command, Dictionary<string, string> dict, Action<string> onCompleted)
		{
			try
			{
				switch (command)
				{
				case NetworkConnect.RequestCommand.GetCountryCode:
					StartCoroutine(RequestCountryCode(null, onCompleted));
					break;
				default:
				{
					string url = NetworkConnect.Instance.Url(command);
					StartCoroutine(PostRequest(url, dict, onCompleted));
					break;
				}
				case NetworkConnect.RequestCommand.UploadFile:
				case NetworkConnect.RequestCommand.GetFileUrl:
					break;
				}
			}
			catch
			{
				Debug.Log("Request Error!");
			}
		}

		private IEnumerator PostRequest(string url, Dictionary<string, string> dict, Action<string> onCompleted)
		{
			WWWForm form = null;
			if (dict != null && dict.Count > 0)
			{
				form = new WWWForm();
				foreach (KeyValuePair<string, string> item in dict)
				{
					if (!string.IsNullOrEmpty(item.Key))
					{
						form.AddField(item.Key, item.Value);
					}
				}
			}
			WWW www2 = null;
			www2 = ((form != null) ? new WWW(url, form) : new WWW(url));
			yield return www2;
			if (onCompleted != null)
			{
				if (string.IsNullOrEmpty(www2.error))
				{
					onCompleted(www2.text);
				}
				else
				{
					onCompleted(null);
				}
			}
		}

		public IEnumerator RequestCountryCode(string ipStr, Action<string> listener)
		{
			WWW www = new WWW("http://ip-api.com/json/" + ipStr);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && listener != null)
			{
				listener(www.text);
			}
		}

		public IEnumerator RequestIpAddress(Action<string> listener)
		{
			WWW www = new WWW("http://whatismyip.akamai.com/");
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
			string obj = null;
			try
			{
				obj = www.text;
			}
			finally
			{
				if (listener != null)
				{
					listener(obj);
				}
			}
		}
	}
}
