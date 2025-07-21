using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class ScoreRankRequest
	{
		public class RankData
		{
			public string userId;

			public string key;

			public Action<int, object> onRespondSuccessed;

			public RankData(string userId, string key, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				onRespondSuccessed = onSuccess;
			}

			public void UploadListener(string s)
			{
				Debug.Log("UploadScoreListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (onRespondSuccessed != null)
					{
						onRespondSuccessed(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)(long)dictionary["status"] == 0)
				{
					if (onRespondSuccessed != null)
					{
						onRespondSuccessed(0, dictionary["msg"]);
					}
				}
				else if (onRespondSuccessed != null)
				{
					onRespondSuccessed(1, null);
				}
			}

			public void GetListener(string s)
			{
				Debug.Log("GetListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (onRespondSuccessed != null)
					{
						onRespondSuccessed(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)(long)dictionary["status"] == 0)
				{
					if (onRespondSuccessed != null)
					{
						onRespondSuccessed(0, dictionary["msg"]);
					}
				}
				else if (!dictionary.ContainsKey("data"))
				{
					if (onRespondSuccessed != null)
					{
						onRespondSuccessed(-1, "no data");
					}
				}
				else if (onRespondSuccessed != null)
				{
					onRespondSuccessed(1, dictionary["data"]);
				}
			}
		}

		private static ScoreRankRequest _instance;

		public static ScoreRankRequest Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ScoreRankRequest();
				}
				return _instance;
			}
		}

		public void UploadScoreFixedLength(string key, float score, Action<int, object> handle = null)
		{
			RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> submitScoreFixedLengthDict = NetworkConnect.Instance.GetSubmitScoreFixedLengthDict(SecondManager.Instance.userId, key, score);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadScoreFixedLength, submitScoreFixedLengthDict, @object.UploadListener);
		}

		public void UploadScoreExpire(string key, float score, Action<int, object> handle = null)
		{
			RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> submitScoreExpireDict = NetworkConnect.Instance.GetSubmitScoreExpireDict(SecondManager.Instance.userId, key, score, 1);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadScoreExpire, submitScoreExpireDict, @object.UploadListener);
		}

		public void GetUserScore(string key, Action<int, object> handle = null)
		{
			RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestUserScoreDict = NetworkConnect.Instance.GetRequestUserScoreDict(SecondManager.Instance.userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUserScore, requestUserScoreDict, @object.GetListener);
		}

		public void GetUserRankIdx(string key, Action<int, object> handle = null)
		{
			RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestUserRankIdxDict = NetworkConnect.Instance.GetRequestUserRankIdxDict(SecondManager.Instance.userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUserRankIdx, requestUserRankIdxDict, @object.GetListener);
		}

		public void GetRankList(string key, int rankCount, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (SecondManager.Instance.hasInited && ServeTimeUpdate.Instance.ServerTimeValid())
			{
				RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
				Dictionary<string, string> requestRankListDict = NetworkConnect.Instance.GetRequestRankListDict(SecondManager.Instance.userId, key, order, rankCount);
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetRankList, requestRankListDict, @object.GetListener);
			}
		}

		public void GetRequestRankListAroundUser(string key, int start, int end, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (SecondManager.Instance.hasInited && ServeTimeUpdate.Instance.ServerTimeValid())
			{
				RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
				Dictionary<string, string> requestRankListAroundUserDict = NetworkConnect.Instance.GetRequestRankListAroundUserDict(SecondManager.Instance.userId, key, start, end, order);
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetDynamicRankList, requestRankListAroundUserDict, @object.GetListener);
			}
		}

		public void GetFriendRankList(string key, PlatFormType platType, string platIds, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (SecondManager.Instance.hasInited && ServeTimeUpdate.Instance.ServerTimeValid())
			{
				RankData @object = new RankData(SecondManager.Instance.userId, key, handle);
				Dictionary<string, string> requestFriendRankListDict = NetworkConnect.Instance.GetRequestFriendRankListDict(SecondManager.Instance.userId, key, order, platIds, platType);
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetFriendRankList, requestFriendRankListDict, @object.GetListener);
			}
		}
	}
}
