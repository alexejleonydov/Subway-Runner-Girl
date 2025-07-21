using System;

namespace Network
{
	public class RankIDInFriends
	{
		private int _rankId;

		public Action onValueChange;

		public int rankID
		{
			get
			{
				return _rankId;
			}
			set
			{
				if (_rankId != value)
				{
					_rankId = value;
					if (onValueChange != null)
					{
						onValueChange();
					}
				}
			}
		}

		public RankIDInFriends()
		{
			_rankId = -1;
		}

		public void Synchronise(int rankId)
		{
			rankID = rankId;
		}
	}
}
