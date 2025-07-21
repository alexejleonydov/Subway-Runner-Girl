using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class IvyHttpHelper : MonoBehaviour
{
	private class PostRequestData
	{
		public string url;

		public Dictionary<string, string> post;

		public Action<string> callback;

		public PostRequestData(string url, Dictionary<string, string> post, Action<string> callback)
		{
			this.url = url;
			this.post = post;
			this.callback = callback;
		}
	}

	private static IvyHttpHelper _Instance;

	public static IvyHttpHelper Instance
	{
		get
		{
			object obj = _Instance;
			if (obj == null)
			{
				obj = UnityEngine.Object.FindObjectOfType<IvyHttpHelper>() ?? new GameObject("HttpHelper").AddComponent<IvyHttpHelper>();
				_Instance = (IvyHttpHelper)obj;
			}
			return (IvyHttpHelper)obj;
		}
	}

	private void Awake()
	{
		if (_Instance == null)
		{
			_Instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public Coroutine MakeRequest(string url, Action<string> callback, Dictionary<string, string> param)
	{
		try
		{
			return StartCoroutine("PostRequest", new PostRequestData(url, param, callback));
		}
		catch
		{
			return null;
		}
	}

	private IEnumerator PostRequest(PostRequestData data)
	{
		WWWForm form = null;
		if (data != null && data.post != null && data.post.Count > 0)
		{
			form = new WWWForm();
			foreach (KeyValuePair<string, string> item in data.post)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					form.AddField(item.Key, item.Value);
				}
			}
		}
		WWW www2 = null;
		www2 = ((form != null) ? new WWW(data.url, form) : new WWW(data.url));
		yield return www2;
		if (data.callback != null)
		{
			if (string.IsNullOrEmpty(www2.error))
			{
				data.callback(www2.text);
			}
			else
			{
				data.callback(null);
			}
		}
	}

	public Coroutine GetRequest(string url, Action<string> callback, Dictionary<string, string> param)
	{
		try
		{
			return StartCoroutine("GetRequest_C", new PostRequestData(url, param, callback));
		}
		catch
		{
			return null;
		}
	}

	private IEnumerator GetRequest_C(PostRequestData data)
	{
		StringBuilder parameters = new StringBuilder(data.url);
		if (data != null && data.post != null && data.post.Count > 0 && data.post != null && data.post.Count > 0)
		{
			parameters.Append("?");
			foreach (KeyValuePair<string, string> item in data.post)
			{
				parameters.Append(item.Key).Append('=').Append(item.Value)
					.Append('&');
			}
			parameters.Remove(parameters.Length - 1, 1);
		}
		WWW www = new WWW(parameters.ToString());
		while (!www.isDone)
		{
			yield return null;
		}
		if (data.callback != null)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				data.callback(www.text);
			}
			else
			{
				data.callback(null);
			}
		}
	}

	public float GetProgress(WWW www)
	{
		float num = 0f;
		if (www != null && www.isDone && string.IsNullOrEmpty(www.error))
		{
			return 1f;
		}
		if (www != null && www.isDone)
		{
			return 0.9f;
		}
		if (www != null)
		{
			return www.progress;
		}
		return 0f;
	}
}
