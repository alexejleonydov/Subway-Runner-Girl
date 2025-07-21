using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class HighestScoreSystem : MonoBehaviour
{
	[SerializeField]
	private TopRun[] topRuns_AI;

	[SerializeField]
	private int minimumInterval = 100;

	private List<string> topRunList;

	private int rankId;

	private static HighestScoreSystem _instance;

	public static HighestScoreSystem Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Utils.FindObject<HighestScoreSystem>();
			}
			return _instance;
		}
	}

	public TopRun lastBeatenTopRun { get; set; }

	public bool isAI { get; private set; }

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		lastBeatenTopRun = null;
	}

	public void Request()
	{
		ScoreRankRequest.Instance.GetRankList(ServeTimeUpdate.Instance.RankWeekString + "_week", 1000, RankOrder.Desc, PullListListener);
	}

	public void PullListListener(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		Clear();
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int num = 0;
		List<string> list = new List<string>(dictionary.Count);
		IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TopRun topRun = ServerManager.Instance.GetGlobalTopRun(enumerator.Current.Key);
			if (topRun == null)
			{
				TopRun topRun2 = new TopRun();
				topRun2.userId = enumerator.Current.Key;
				topRun = topRun2;
				ServerManager.Instance.AddGlobalTopRun(topRun);
			}
			topRun.highestScore = (int)(long)enumerator.Current.Value;
			topRun.rank = num + 1;
			Add(enumerator.Current.Key);
			if (!enumerator.Current.Key.Equals(SecondManager.Instance.userId))
			{
				list.Add(enumerator.Current.Key);
			}
			num++;
		}
		if (list.Count > 0)
		{
			string userId = string.Join(",", list.ToArray());
			StringKeyValueRequest.Instance.GetStringData(userId, "playerName", GetPlayerNameDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "pictureUrl", GePictureUrlDataCallback);
		}
	}

	public void Clear()
	{
		if (topRunList == null)
		{
			topRunList = new List<string>();
		}
		topRunList.Clear();
	}

	public void Add(string userId)
	{
		if (!string.IsNullOrEmpty(userId))
		{
			topRunList.Add(userId);
		}
	}

	private void GetPlayerNameDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = topRunList.Count;
		string empty = string.Empty;
		string empty2 = string.Empty;
		TopRunInfo topRunInfo = null;
		for (int i = 0; i < count; i++)
		{
			empty = topRunList[i];
			if (dictionary.ContainsKey(empty) && !(dictionary[topRunList[i]] is bool))
			{
				empty2 = (string)dictionary[empty];
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty);
				topRunInfo.playerName = empty2;
			}
		}
	}

	private void GePictureUrlDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = topRunList.Count;
		string empty = string.Empty;
		string empty2 = string.Empty;
		TopRunInfo topRunInfo = null;
		List<string> list = new List<string>();
		for (int i = 0; i < count; i++)
		{
			empty2 = topRunList[i];
			if (dictionary.ContainsKey(empty2) && !(dictionary[empty2] is bool))
			{
				empty = (string)dictionary[empty2];
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty2);
				topRunInfo.pictureUrl = empty;
				if (!ImageManager.Instance.ContainsKey(topRunInfo.pictureUrl))
				{
					list.Add(topRunInfo.pictureUrl);
				}
			}
		}
		if (list.Count > 0)
		{
			StartCoroutine(DownloadImage(list));
		}
	}

	private IEnumerator DownloadImage(List<string> urls)
	{
		int count = urls.Count;
		string pictureUrl2 = string.Empty;
		for (int i = 0; i < count; i++)
		{
			pictureUrl2 = urls[i];
			ImageDownloader loader = new ImageDownloader(pictureUrl2, null, 60f, i);
			NetworkRequest.Instance.StartCoroutine(loader.Download());
			yield return null;
		}
	}

	public TopRun Init()
	{
		if (topRunList == null || !PlayerInfo.Instance.transcendByLeadborad)
		{
			isAI = true;
			rankId = topRuns_AI.Length - 1;
			return topRuns_AI[rankId];
		}
		isAI = false;
		int num = topRunList.IndexOf(SecondManager.Instance.userId);
		if (num == -1)
		{
			rankId = topRunList.Count - 1;
		}
		else
		{
			rankId = num - 1;
		}
		if (rankId < 0 || rankId >= topRunList.Count)
		{
			return null;
		}
		return ServerManager.Instance.GetGlobalTopRun(topRunList[rankId]);
	}

	public TopRun Next()
	{
		TopRun topRun = null;
		int score = GameStats.Instance.score;
		do
		{
			topRun = null;
			rankId--;
			if (rankId < 0)
			{
				break;
			}
			if (isAI)
			{
				topRun = topRuns_AI[rankId];
				if (rankId < topRuns_AI.Length - 3)
				{
					PlayerInfo.Instance.transcendByLeadborad = true;
				}
			}
			else
			{
				topRun = ServerManager.Instance.GetGlobalTopRun(topRunList[rankId]);
			}
			if (topRun.highestScore < score)
			{
				lastBeatenTopRun = topRun;
			}
		}
		while (topRun.highestScore < score + minimumInterval);
		return topRun;
	}

	public void End(int score)
	{
		if (lastBeatenTopRun == null)
		{
			return;
		}
		TopRun topRun = lastBeatenTopRun;
		while (topRun.highestScore < score)
		{
			lastBeatenTopRun = topRun;
			topRun = null;
			rankId--;
			if (rankId < 0)
			{
				break;
			}
			topRun = ((!isAI) ? ServerManager.Instance.GetGlobalTopRun(topRunList[rankId]) : topRuns_AI[rankId]);
		}
	}
}
