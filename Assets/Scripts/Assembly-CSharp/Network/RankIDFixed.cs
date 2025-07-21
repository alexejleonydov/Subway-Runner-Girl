using System;

namespace Network
{
	public class RankIDFixed
	{
		private int _rankId;

		private string _rankKey;

		public Action onValueChange;

		private string _key;

		public int rankID
		{
			get
			{
				return _rankId;
			}
			private set
			{
				if (_rankId != value)
				{
					_rankId = value;
					OnValueChange();
				}
			}
		}

		public SynchroniseRule rule { get; private set; }

		public RankIDFixed(string key, string rankKey)
		{
			rule = new SynchroniseRule(60f);
			_key = key;
			_rankKey = rankKey;
			_rankId = -1;
		}

		public void GetRankID()
		{
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited)
			{
				ScoreRankRequest.Instance.GetUserRankIdx(_rankKey, OnGetRankIDCallback);
				rule.Request();
			}
		}

		private void OnGetRankIDCallback(int status, object obj)
		{
			if (status != -1 && status != 0)
			{
				rule.hasGotInitValue = true;
				rankID = (int)(long)obj;
			}
		}

		public void Synchronise(int rankId)
		{
			if (SecondManager.Instance.hasInited)
			{
				rule.hasGotInitValue = true;
				rankID = rankId;
			}
		}

		private void OnValueChange()
		{
			if (onValueChange != null)
			{
				onValueChange();
			}
			ServerManager.Instance.OnRankIDChange();
		}
	}
}
