using System;
using System.Collections.Generic;
using System.Text;
using Network;

public class IvyApp
{
	public enum RequestCommand
	{
		Statistics = 0,
		Redcode = 1,
		Other = 2
	}

	private string trackEventUrl = "http://sanxiao.iibingo.com/api/Dot/OnEventVer";

	private string url = "http://sanxiao.iibingo.com/api/Redcode/authRedCode";

	private static IvyApp _instance;

	public static IvyApp Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new IvyApp();
			}
			return _instance;
		}
	}

	private string GetUrl(RequestCommand command)
	{
		if (command == RequestCommand.Statistics)
		{
			return trackEventUrl;
		}
		return url;
	}

	public void Recode(string recode, Action<string> listener)
	{
		//Dictionary<string, string> dictionary = new Dictionary<string, string>();
		//dictionary.Add("appid", NetworkConnect.Instance.AppId());
		//dictionary.Add("uid", NetworkConnect.Instance.Uid());
		//dictionary.Add("redcode", recode);
		//IvyHttpHelper.Instance.MakeRequest(GetUrl(RequestCommand.Redcode), listener, dictionary);
	}

	public void Statistics(string event1, string event2, string event3, int version, int number = 0, Action<string> listener = null)
	{
		/*Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("appid", NetworkConnect.Instance.AppId());
		dictionary.Add("uid", NetworkConnect.Instance.Uid());
		if (!string.IsNullOrEmpty(event1))
		{
			dictionary.Add("event1", event1);
		}
		else if (!string.IsNullOrEmpty(event2))
		{
			dictionary.Add("event2", event2);
		}
		else if (!string.IsNullOrEmpty(event3))
		{
			dictionary.Add("event3", event3);
		}
		dictionary["version"] = get_uft8(RiseSdk.Instance.GetConfig(9));
		dictionary["num_count"] = number.ToString();
		IvyHttpHelper.Instance.GetRequest(GetUrl(RequestCommand.Statistics), listener, dictionary);*/
	}

	public string get_uft8(string unicodeString)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(unicodeString);
		return uTF8Encoding.GetString(bytes);
	}
}
