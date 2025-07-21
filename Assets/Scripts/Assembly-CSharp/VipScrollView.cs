using System.Collections.Generic;
using Network;
using UnityEngine;

public class VipScrollView : MonoBehaviour
{
	[SerializeField]
	private GameObject rankcellPrefab;

	[SerializeField]
	private UITable table;

	[SerializeField]
	private GameObject _separateGo;

	private TopRunCell[] _cacheCells = new TopRunCell[50];

	private PullListResult result = PullListResult.None;

	private float pullListTime;

	private List<TopRun> topRunList;

	private bool isShow;

	private Coroutine closeLoadingCoroutine;

	private void Awake()
	{
		_cacheCells = new TopRunCell[50];
	}

	public void Hide()
	{
		isShow = false;
	}

	public void Show()
	{
		if (isShow)
		{
			return;
		}
		isShow = true;
		if (result != PullListResult.Success || pullListTime + 60f < RealTimeTracker.time)
		{
			result = PullListResult.None;
			pullListTime = RealTimeTracker.time;
			UIScreenController.Instance.PushPopup("RankLoadingPopup");
			StopCloseLoadingCoroutine();
			closeLoadingCoroutine = StartCoroutine(DelayInvoke.start(delegate
			{
				CloseLoadingPopup();
			}, 30f));
			ScoreRankRequest.Instance.GetRankList(ServeTimeUpdate.Instance.RankWeekString + "_vip", 50, RankOrder.Desc, PullVipsListListener);
		}
		else
		{
			FillTable();
		}
	}

	private void StopCloseLoadingCoroutine()
	{
		if (closeLoadingCoroutine != null)
		{
			StopCoroutine(closeLoadingCoroutine);
			closeLoadingCoroutine = null;
		}
	}

	public void PullVipsListListener(int status, object obj)
	{
		StopCloseLoadingCoroutine();
		if (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.CloseAllPopups();
		}
		switch (status)
		{
		case -1:
			result = PullListResult.NetError;
			if (isShow)
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
			return;
		case 0:
			result = PullListResult.NoData;
			if (isShow)
			{
				UISliderInController.Instance.OnDataErrorPickedUp();
			}
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		if (topRunList == null)
		{
			topRunList = new List<TopRun>();
		}
		topRunList.Clear();
		int num = 0;
		List<string> list = new List<string>(dictionary.Count);
		IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TopRun topRun = ServerManager.Instance.GetVipTopRun(enumerator.Current.Key);
			if (topRun == null)
			{
				TopRun topRun2 = new TopRun();
				topRun2.userId = enumerator.Current.Key;
				topRun = topRun2;
				ServerManager.Instance.AddVipTopRun(topRun);
			}
			topRun.highestScore = (int)(long)enumerator.Current.Value;
			topRun.rank = num + 1;
			topRunList.Add(topRun);
			if (enumerator.Current.Key.Equals(SecondManager.Instance.userId))
			{
				ServerManager.Instance.SynchroniseScoreVIP(topRun.highestScore);
				ServerManager.Instance.SynchroniseRankIDVIP(topRun.rank);
			}
			list.Add(topRun.userId);
			num++;
		}
		result = PullListResult.Success;
		if (isShow)
		{
			UISliderInController.Instance.OnGetSuccessPickedUp();
		}
		if (list.Count > 0)
		{
			string userId = string.Join(",", list.ToArray());
			StringKeyValueRequest.Instance.GetStringData(userId, "subscription", GetSubscriptionDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "playerName", GetPlayerNameDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "countryCode", GetCountryCodeDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "pictureUrl", GePictureUrlDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "playerLevel", GetPlayerLevelDataCall);
		}
		FillTable();
	}

	private void GetPlayerLevelDataCall(int status, object obj)
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
			empty2 = topRunList[i].userId;
			if (dictionary.ContainsKey(empty2) && !(dictionary[empty2] is bool))
			{
				empty = (string)dictionary[topRunList[i].userId];
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty2);
				topRunInfo.playerLevel = empty;
				if (i < _cacheCells.Length && _cacheCells[i] != null)
				{
					_cacheCells[i].RefreshPlayrLevel();
				}
			}
		}
	}

	private void GetSubscriptionDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			result = PullListResult.NoData;
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = topRunList.Count;
		string empty = string.Empty;
		string empty2 = string.Empty;
		TopRunInfo topRunInfo = null;
		for (int i = 0; i < count; i++)
		{
			empty = topRunList[i].userId;
			if (dictionary.ContainsKey(empty) && !(dictionary[empty] is bool))
			{
				empty2 = (string)dictionary[empty];
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty);
				topRunInfo.isVip = empty2;
				if (i < _cacheCells.Length && _cacheCells[i] != null)
				{
					_cacheCells[i].RefreshVIP();
				}
			}
		}
	}

	private void GetCountryCodeDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			result = PullListResult.NoData;
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = topRunList.Count;
		string empty = string.Empty;
		string empty2 = string.Empty;
		TopRunInfo topRunInfo = null;
		for (int i = 0; i < count; i++)
		{
			empty2 = topRunList[i].userId;
			if (dictionary.ContainsKey(empty2) && !(dictionary[empty2] is bool))
			{
				empty = (string)dictionary[topRunList[i].userId];
				if (empty2.Equals(SecondManager.Instance.userId))
				{
					ServerManager.Instance.SynchroniseCountryCode(empty);
				}
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty2);
				topRunInfo.countryCode = empty;
				if (i < _cacheCells.Length && _cacheCells[i] != null)
				{
					_cacheCells[i].RefreshCoutryCode();
				}
			}
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
			empty2 = topRunList[i].userId;
			if (dictionary.ContainsKey(empty2) && !(dictionary[empty2] is bool))
			{
				empty = (string)dictionary[topRunList[i].userId];
				if (empty2.Equals(SecondManager.Instance.userId))
				{
					ServerManager.Instance.SynchronisePlayerName(empty);
				}
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty2);
				topRunInfo.playerName = empty;
				if (i < _cacheCells.Length && _cacheCells[i] != null)
				{
					_cacheCells[i].RefreshPlayerName();
				}
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
		for (int i = 0; i < count; i++)
		{
			empty2 = topRunList[i].userId;
			if (!dictionary.ContainsKey(empty2) || dictionary[empty2] is bool)
			{
				continue;
			}
			empty = (string)dictionary[topRunList[i].userId];
			if (empty2.Equals(SecondManager.Instance.userId))
			{
				ServerManager.Instance.SynchronisePictrueUrl(empty);
			}
			topRunInfo = ServerManager.Instance.GetTopRunInfo(empty2);
			topRunInfo.pictureUrl = empty;
			if (ImageManager.Instance.ContainsKey(topRunInfo.pictureUrl))
			{
				if (i < _cacheCells.Length && _cacheCells[i] != null)
				{
					_cacheCells[i].RefreshImage();
				}
			}
			else
			{
				ImageDownloader imageDownloader = new ImageDownloader(topRunInfo.pictureUrl, OnComplete, 60f, i);
				NetworkRequest.Instance.StartCoroutine(imageDownloader.Download());
			}
		}
	}

	private void OnComplete(bool result, ImageDownloader loader)
	{
		if (result)
		{
			int num = (int)loader.cookie;
			if (num < _cacheCells.Length && _cacheCells[num] != null)
			{
				_cacheCells[num].RefreshImage();
			}
		}
	}

	private void CloseLoadingPopup()
	{
		UIScreenController.Instance.ClosePopup(null);
	}

	public void FillTable()
	{
		if (topRunList == null || topRunList.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = topRunList.Count;
		_separateGo.SetActive(count > 3);
		int num = 0;
		TopRunCell topRunCell = null;
		for (; count > i; i++)
		{
			if (_cacheCells[i] != null)
			{
				_cacheCells[i].gameObject.SetActive(true);
			}
			else
			{
				topRunCell = InstantCell<TopRunCell>(rankcellPrefab, table.gameObject);
				_cacheCells[i] = topRunCell;
			}
			_cacheCells[i].RefreshUI(topRunList[i]);
			num = ((i < 3) ? i : (i + 1));
			_cacheCells[i].transform.SetSiblingIndex(num);
			if (i == 2)
			{
				_separateGo.transform.SetSiblingIndex(3);
			}
		}
		table.Reposition();
	}

	private TopRunCell InstantCell<T>(GameObject prefab, GameObject parent) where T : TopRunCell
	{
		GameObject gameObject = null;
		gameObject = Object.Instantiate(prefab);
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<T>();
	}
}
