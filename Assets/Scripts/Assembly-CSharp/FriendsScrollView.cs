using System.Collections.Generic;
using Network;
using UnityEngine;

public class FriendsScrollView : MonoBehaviour
{
	[SerializeField]
	private GameObject rankcellPrefab;

	[SerializeField]
	private UITable table;

	[SerializeField]
	private GameObject _separateGo;

	private List<TopRunCell> _cacheCells;

	private PullListResult result = PullListResult.None;

	private float pullListTime;

	private List<TopRun> topRunList;

	private bool isShow;

	private Coroutine closeLoadingCoroutine;

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
		if (!SecondManager.Instance.facebook)
		{
			return;
		}
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
			string value = FacebookManger.Instance.FriendsIds();
			if (!string.IsNullOrEmpty(value))
			{
				ScoreRankRequest.Instance.GetFriendRankList("2213_facebook", PlatFormType.facebook, FacebookManger.Instance.FriendsIds(), RankOrder.Desc, PullFriendsListListener);
			}
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

	public void PullFriendsListListener(int status, object obj)
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
			if (!(enumerator.Current.Value is bool))
			{
				TopRun topRun = ServerManager.Instance.GetFriendTopRun(enumerator.Current.Key);
				if (topRun == null)
				{
					TopRun topRun2 = new TopRun();
					topRun2.userId = enumerator.Current.Key;
					topRun = topRun2;
					ServerManager.Instance.AddFriendTopRun(topRun);
				}
				topRun.highestScore = (int)(long)enumerator.Current.Value;
				topRun.rank = num + 1;
				topRunList.Add(topRun);
				if (enumerator.Current.Key.Equals(SecondManager.Instance.userId))
				{
					ServerManager.Instance.SynchroniseScoreGlobal(topRun.highestScore);
					ServerManager.Instance.SynchroniseRankIDGlobal(topRun.rank);
				}
				list.Add(topRun.userId);
				num++;
			}
		}
		result = PullListResult.Success;
		if (isShow)
		{
			UISliderInController.Instance.OnGetSuccessPickedUp();
		}
		if (list.Count > 0)
		{
			string userId = string.Join(",", list.ToArray());
			StringKeyValueRequest.Instance.GetStringData(userId, "facebookID", GetFacebookIDDataCallback);
			StringKeyValueRequest.Instance.GetStringData(userId, "subscription", GetSubscriptionDataCallback);
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
				if (_cacheCells[i] != null)
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
				topRunInfo.isVip = empty;
				if (_cacheCells[i] != null)
				{
					_cacheCells[i].RefreshVIP();
				}
			}
		}
	}

	private void GetFacebookIDDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = topRunList.Count;
		string empty = string.Empty;
		string text = null;
		TopRunInfo topRunInfo = null;
		for (int i = 0; i < count; i++)
		{
			text = topRunList[i].userId;
			if (!dictionary.ContainsKey(text) || dictionary[text] is bool)
			{
				continue;
			}
			empty = (string)dictionary[topRunList[i].userId];
			topRunInfo = ServerManager.Instance.GetTopRunInfo(text);
			topRunInfo.facebookName = FacebookManger.Instance.GetNameAccrodingID(empty);
			topRunInfo.pictureUrl = FacebookManger.Instance.GetPictureAccrodingID(empty);
			if (!(_cacheCells[i] != null))
			{
				continue;
			}
			_cacheCells[i].RefreshPlayerName();
			if (ImageManager.Instance.ContainsKey(topRunInfo.pictureUrl))
			{
				if (_cacheCells[i] != null)
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
			int index = (int)loader.cookie;
			if (_cacheCells[index] != null)
			{
				_cacheCells[index].RefreshImage();
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
		if (_cacheCells == null)
		{
			_cacheCells = new List<TopRunCell>();
		}
		int i = 0;
		int count = topRunList.Count;
		_separateGo.SetActive(count > 3);
		int num = 0;
		TopRunCell topRunCell = null;
		for (; count > i; i++)
		{
			if (_cacheCells.Count > i && _cacheCells[i] != null)
			{
				_cacheCells[i].gameObject.SetActive(true);
			}
			else
			{
				topRunCell = InstantCell<TopRunCell>(rankcellPrefab, table.gameObject);
				_cacheCells.Add(topRunCell);
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
