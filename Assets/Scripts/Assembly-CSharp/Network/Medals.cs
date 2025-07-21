using System.Collections.Generic;

namespace Network
{
	public class Medals : KeyJsonValue
	{
		private const string Format = "{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}";

		public int _gold;

		public int _sliver;

		public int _copper;

		private string _rankTag;

		private int _goldTmp;

		private int _sliverTmp;

		private int _copperTmp;

		private string _rankTagTmp;

		public Medals()
		{
			_key = "medals";
			_gold = 0;
			_sliver = 0;
			_copper = 0;
			_rankTag = "2013_201917_week";
		}

		public override void Reset()
		{
			base.Reset();
			_gold = 0;
			_sliver = 0;
			_copper = 0;
			_rankTag = "2013_201917_week";
		}

		protected override void Parse(object obj)
		{
			if (obj == null)
			{
				_goldTmp = PlayerInfo.Instance.TopRunData.hasGoldMedal;
				_sliverTmp = PlayerInfo.Instance.TopRunData.hasSliverMedal;
				_copperTmp = PlayerInfo.Instance.TopRunData.hasBronzeMedal;
				_rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
				UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", _goldTmp, _sliverTmp, _copperTmp, _rankTagTmp));
				return;
			}
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey("g"))
			{
				_gold = (int)(long)dictionary["g"];
			}
			if (dictionary.ContainsKey("s"))
			{
				_sliver = (int)(long)dictionary["s"];
			}
			if (dictionary.ContainsKey("c"))
			{
				_copper = (int)(long)dictionary["c"];
			}
			if (dictionary.ContainsKey("w"))
			{
				_rankTag = (string)dictionary["w"];
			}
			if (!_rankTag.Equals(ServeTimeUpdate.Instance.RankWeekString))
			{
				ServerManager.Instance.SynchroniseScore(0);
				ScoreRankRequest.Instance.GetUserRankIdx(_rankTag, GetUserRankIdxHandle);
			}
		}

		private void GetUserRankIdxHandle(int status, object obj)
		{
			switch (status)
			{
			case -1:
				return;
			case 0:
				_rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
				_goldTmp = _gold;
				_sliverTmp = _sliver;
				_copperTmp = _copper;
				UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", _goldTmp, _sliverTmp, _copperTmp, _rankTagTmp));
				return;
			}
			_goldTmp = _gold;
			_sliverTmp = _sliver;
			_copperTmp = _copper;
			int num = (int)(long)obj;
			if (num <= 3)
			{
				_goldTmp++;
			}
			else if (num <= 10)
			{
				_sliverTmp++;
			}
			else if (num <= 100)
			{
				_copperTmp++;
			}
			_rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
			UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", _goldTmp, _sliverTmp, _copperTmp, _rankTagTmp));
		}

		protected override void UploadCallback(int status, object obj)
		{
			if (status != -1 && status != 0)
			{
				_gold = _goldTmp;
				_sliver = _sliverTmp;
				_copper = _copperTmp;
				_rankTag = _rankTagTmp;
				_rule.hasGotInitValue = true;
			}
		}

		protected override void OnValueChange()
		{
			PlayerInfo.Instance.TopRunData.hasGoldMedal = _gold;
			PlayerInfo.Instance.TopRunData.hasSliverMedal = _sliver;
			PlayerInfo.Instance.TopRunData.hasBronzeMedal = _copper;
			PlayerInfo.Instance.TopRunData.weekstring = _rankTag;
			base.OnValueChange();
		}

		protected override string ToJson()
		{
			return string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", _gold, _sliver, _copper, _rankTag);
		}
	}
}
