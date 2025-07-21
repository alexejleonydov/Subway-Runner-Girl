using System;
using System.Collections.Generic;

namespace Network
{
	public class ServerManager
	{
		private static ServerManager _instance;

		public Dictionary<string, ServerData> serverDatas = new Dictionary<string, ServerData>();

		public Dictionary<string, TopRunInfo> allTopRunDatas = new Dictionary<string, TopRunInfo>();

		public Dictionary<string, TopRun> globalDatas = new Dictionary<string, TopRun>();

		public Dictionary<string, TopRun> friendDatas = new Dictionary<string, TopRun>();

		public Dictionary<string, TopRun> vipDatas = new Dictionary<string, TopRun>();

		private ServerData currentServerData;

		public Action onPlayerNameChange;

		public Action onScoreChange;

		public Action onRankIDChange;

		public Action onCountryCodeChange;

		public Action onPictureURLChange;

		public static ServerManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ServerManager();
				}
				return _instance;
			}
		}

		public PlayerName PlayerName
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.playerName;
			}
		}

		public Score Score_Week
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.score_week;
			}
		}

		public Score Score_Vip
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.score_vip;
			}
		}

		public ScoreFixed Score_Global
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.score_global;
			}
		}

		public RankID RankID_Week
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.rankID_week;
			}
		}

		public RankID RankID_VIP
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.rankID_vip;
			}
		}

		public RankIDFixed RankID_Global
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.rankID_global;
			}
		}

		public CountryCode CountryCode
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.countryCode;
			}
		}

		public PictureUrl PictureUrl
		{
			get
			{
				if (currentServerData == null)
				{
					return null;
				}
				return currentServerData.pictureUrl;
			}
		}

		public TopRunInfo GetTopRunInfo(TopRun topRun)
		{
			return GetTopRunInfo(topRun.userId);
		}

		public TopRunInfo GetTopRunInfo(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return new TopRunInfo();
			}
			if (allTopRunDatas.ContainsKey(userId))
			{
				return allTopRunDatas[userId];
			}
			TopRunInfo topRunInfo = new TopRunInfo();
			allTopRunDatas.Add(userId, topRunInfo);
			return topRunInfo;
		}

		public TopRun GetGlobalTopRun(string userId)
		{
			if (globalDatas.ContainsKey(userId))
			{
				return globalDatas[userId];
			}
			return null;
		}

		public void AddGlobalTopRun(TopRun topRun)
		{
			if (topRun != null && !globalDatas.ContainsKey(topRun.userId))
			{
				globalDatas.Add(topRun.userId, topRun);
			}
		}

		public void AddFriendTopRun(TopRun topRun)
		{
			if (topRun != null && !friendDatas.ContainsKey(topRun.userId))
			{
				friendDatas.Add(topRun.userId, topRun);
			}
		}

		public TopRun GetFriendTopRun(string userId)
		{
			if (friendDatas.ContainsKey(userId))
			{
				return friendDatas[userId];
			}
			return null;
		}

		public void AddVipTopRun(TopRun topRun)
		{
			if (topRun != null && !vipDatas.ContainsKey(topRun.userId))
			{
				vipDatas.Add(topRun.userId, topRun);
			}
		}

		public TopRun GetVipTopRun(string userId)
		{
			if (vipDatas.ContainsKey(userId))
			{
				return vipDatas[userId];
			}
			return null;
		}

		public void RequestUserInformation(PlatFormType platForm, string userId)
		{
			if (!ServeTimeUpdate.Instance.ChechCanRequestDatas())
			{
				return;
			}
			if (serverDatas.ContainsKey(userId))
			{
				currentServerData = serverDatas[userId];
				return;
			}
			ServerData serverData = new ServerData(userId);
			serverDatas.Add(userId, serverData);
			currentServerData = serverData;
			switch (platForm)
			{
			case PlatFormType.guest:
				CoroutineC.Instance.StartCoroutine(serverData.RequestGuest());
				break;
			case PlatFormType.facebook:
				CoroutineC.Instance.StartCoroutine(serverData.RequestFacebook());
				break;
			}
		}

		public void OnPlayerNameChange()
		{
			if (onPlayerNameChange != null)
			{
				onPlayerNameChange();
			}
		}

		public void OnScoreChange()
		{
			if (onScoreChange != null)
			{
				onScoreChange();
			}
		}

		public void OnRankIDChange()
		{
			if (onRankIDChange != null)
			{
				onRankIDChange();
			}
		}

		public void OnCountryCodeChange()
		{
			if (onCountryCodeChange != null)
			{
				onCountryCodeChange();
			}
		}

		public void OnPictureURLChange()
		{
			if (onPictureURLChange != null)
			{
				onPictureURLChange();
			}
		}

		public bool CanUploadPlayerName()
		{
			if (currentServerData == null)
			{
				return false;
			}
			return currentServerData.CanUploadPlayerName();
		}

		public void RequestPlayerName()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestPlayerName();
			}
		}

		public void SynchronisePlayerName(string playerName)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchronisePlayerName(playerName);
			}
		}

		public void UploadPlayerName(string playerName)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadPlayerName_Strict(playerName);
			}
		}

		public void RegisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			this.onPlayerNameChange = (Action)Delegate.Remove(this.onPlayerNameChange, onPlayerNameChange);
			this.onPlayerNameChange = (Action)Delegate.Combine(this.onPlayerNameChange, onPlayerNameChange);
		}

		public void UnregisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			this.onPlayerNameChange = (Action)Delegate.Remove(this.onPlayerNameChange, onPlayerNameChange);
		}

		public void RequestScore()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestScore();
			}
		}

		public void UploadScore(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadScore(score);
			}
		}

		public void SynchroniseScore(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseScore(score);
			}
		}

		public void RegisterOnScoreChange(Action onScoreChange)
		{
			this.onScoreChange = (Action)Delegate.Remove(this.onScoreChange, onScoreChange);
			this.onScoreChange = (Action)Delegate.Combine(this.onScoreChange, onScoreChange);
		}

		public void UnregisterOnScoreChange(Action onScoreChange)
		{
			this.onScoreChange = (Action)Delegate.Remove(this.onScoreChange, onScoreChange);
		}

		public void RequestScoreVIP()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestScoreVIP();
			}
		}

		public void UploadScoreVip(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadScoreVip(score);
			}
		}

		public void SynchroniseScoreVIP(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseScoreVIP(score);
			}
		}

		public void RequestScoreGlobal()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestScoreGlobal();
			}
		}

		public void UploadScoreGlobal(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadScoreGlobal(score);
			}
		}

		public void SynchroniseScoreGlobal(int score)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseScoreGlobal(score);
			}
		}

		public void RegisterOnRankIDChange(Action onRankIDChange)
		{
			this.onRankIDChange = (Action)Delegate.Remove(this.onRankIDChange, onRankIDChange);
			this.onRankIDChange = (Action)Delegate.Combine(this.onRankIDChange, onRankIDChange);
		}

		public void UnregisterOnRankIDChange(Action onRankIDChange)
		{
			this.onRankIDChange = (Action)Delegate.Remove(this.onRankIDChange, onRankIDChange);
		}

		public void RequestRankID()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestRankID();
			}
		}

		public void SynchroniseRankID(int rankId)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseRankID(rankId);
			}
		}

		public void SynchroniseRankIDVIP(int rankId)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseRankIDVIP(rankId);
			}
		}

		public void SynchroniseRankIDGlobal(int rankId)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseRankIDGlobal(rankId);
			}
		}

		public void RequestCountryCode()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestCountryCode();
			}
		}

		public void SynchroniseCountryCode(string countryCode)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchroniseCountryCode(countryCode);
			}
		}

		public void RegisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			this.onCountryCodeChange = (Action)Delegate.Remove(this.onCountryCodeChange, onCountryCodeChange);
			this.onCountryCodeChange = (Action)Delegate.Combine(this.onCountryCodeChange, onCountryCodeChange);
		}

		public void UnregisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			this.onCountryCodeChange = (Action)Delegate.Remove(this.onCountryCodeChange, onCountryCodeChange);
		}

		public void UploadSubscription(string value)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadSubscription(value);
			}
		}

		public void UploadPlayerLevel(string value)
		{
			if (currentServerData != null)
			{
				currentServerData.UploadPlayerLevel(value);
			}
		}

		public void RequestPictrueUrl()
		{
			if (currentServerData != null)
			{
				currentServerData.RequestPictrueUrl();
			}
		}

		public void SynchronisePictrueUrl(string url)
		{
			if (currentServerData != null)
			{
				currentServerData.SynchronisePictrueUrl(url);
			}
		}

		public void RegisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			onPictureURLChange = (Action)Delegate.Remove(onPictureURLChange, onDownloadImageSuccess);
			onPictureURLChange = (Action)Delegate.Combine(onPictureURLChange, onDownloadImageSuccess);
		}

		public void UnregisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			onPictureURLChange = (Action)Delegate.Remove(onPictureURLChange, onDownloadImageSuccess);
		}
	}
}
