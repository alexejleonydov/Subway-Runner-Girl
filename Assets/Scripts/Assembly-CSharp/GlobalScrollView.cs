using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class GlobalScrollView : MonoBehaviour
{
	[SerializeField]
	private GameObject rankcellPrefab;

	[SerializeField]
	private UITable table;

	[SerializeField]
	private GameObject _separateGo;

	private TopRunCell[] _cacheCells;

	private PullListResult result = PullListResult.None;

	private float pullListTime;

	private List<TopRun> topRunList;

	private bool isShow;

	private Coroutine closeLoadingCoroutine;

	private void Awake()
	{
		_cacheCells = new TopRunCell[100];
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
			ScoreRankRequest.Instance.GetRankList(ServeTimeUpdate.Instance.RankWeekString + "_week", 1000, RankOrder.Desc, PullGlobalListListener);
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

	public void PullGlobalListListener(int status, object obj)
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
		HighestScoreSystem.Instance.Clear();
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
			topRunList.Add(topRun);
			HighestScoreSystem.Instance.Add(enumerator.Current.Key);
			if (enumerator.Current.Key.Equals(SecondManager.Instance.userId))
			{
				ServerManager.Instance.SynchroniseScore(topRun.highestScore);
				ServerManager.Instance.SynchroniseRankID(topRun.rank);
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
			StringKeyValueRequest.Instance.GetStringData(userId, "countryCode", GetCountryCodeDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "playerName", GetPlayerNameDataCallback);
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
			empty = topRunList[i].userId;
			if (dictionary.ContainsKey(empty) && !(dictionary[empty] is bool))
			{
				empty2 = (string)dictionary[empty];
				if (empty.Equals(SecondManager.Instance.userId))
				{
					ServerManager.Instance.SynchroniseCountryCode(empty2);
				}
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty);
				topRunInfo.countryCode = empty2;
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
			if (dictionary.ContainsKey(empty) && !(dictionary[topRunList[i].userId] is bool))
			{
				empty2 = (string)dictionary[empty];
				if (empty.Equals(SecondManager.Instance.userId))
				{
					ServerManager.Instance.SynchronisePlayerName(empty2);
				}
				topRunInfo = ServerManager.Instance.GetTopRunInfo(empty);
				topRunInfo.playerName = empty2;
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
			result = PullListResult.NoData;
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
			empty2 = topRunList[i].userId;
			if (!dictionary.ContainsKey(empty2) || dictionary[empty2] is bool)
			{
				continue;
			}
			empty = (string)dictionary[empty2];
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
				list.Add(topRunInfo.pictureUrl);
			}
		}
		if (list.Count > 0)
		{
			NetworkRequest.Instance.StartCoroutine(DownloadImage(list));
		}
	}

	private IEnumerator DownloadImage(List<string> urls)
	{
		int count = urls.Count;
		string pictureUrl2 = string.Empty;
		for (int i = 0; i < count; i++)
		{
			pictureUrl2 = urls[i];
			ImageDownloader loader = new ImageDownloader(pictureUrl2, OnComplete, 60f, i);
			NetworkRequest.Instance.StartCoroutine(loader.Download());
			yield return null;
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
		int num = ((topRunList.Count <= 100) ? topRunList.Count : 100);
		_separateGo.SetActive(num > 3);
		int num2 = 0;
		TopRunCell topRunCell = null;
		for (; num > i; i++)
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
			num2 = ((i < 3) ? i : (i + 1));
			_cacheCells[i].transform.SetSiblingIndex(num2);
			if (i == 2)
			{
				_separateGo.transform.SetSiblingIndex(3);
			}
		}
		table.Reposition();
	}

	private T InstantCell<T>(GameObject prefab, GameObject parent)
	{
		GameObject gameObject = null;
		gameObject = Object.Instantiate(prefab);
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<T>();
	}
}
