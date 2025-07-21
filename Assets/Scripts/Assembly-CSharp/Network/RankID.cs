using System;

namespace Network
{
	public class RankID
	{
		private int _rankId;

		private string _rankKey;

		private string _suffix;

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

		public RankID(string key, string suffix = null)
		{
			rule = new SynchroniseRule(60f);
			_key = key;
			_rankKey = ServeTimeUpdate.Instance.RankWeekString;
			_suffix = suffix;
			_rankId = -1;
			GetRankID();
		}

		public void GetRankID()
		{
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited)
			{
				_rankKey = ServeTimeUpdate.Instance.RankWeekString;
				ScoreRankRequest.Instance.GetUserRankIdx(_rankKey + _suffix, OnGetRankIDCallback);
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
