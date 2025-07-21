using System.Collections;

namespace Network
{
	public class ServerData
	{
		private string userId;

		public PlayerName playerName;

		public FacebookID facebookID;

		public CountryCode countryCode;

		public Subscription subscription;

		public PlayerLevel playerLevel;

		public Score score_week;

		public Score score_vip;

		public ScoreFixed score_global;

		public RankID rankID_week;

		public RankID rankID_vip;

		public RankIDFixed rankID_global;

		public PictureUrl pictureUrl;

		public ServerData(string key)
		{
			userId = key;
		}

		public IEnumerator RequestGuest()
		{
			playerName = new PlayerName(userId);
			yield return null;
			countryCode = new CountryCode(userId);
			yield return null;
			subscription = new Subscription(userId);
			yield return null;
			playerLevel = new PlayerLevel(userId);
			yield return null;
			score_week = new Score(userId, "_week");
			yield return null;
			score_vip = new Score(userId, "_vip");
			yield return null;
			score_global = new ScoreFixed(userId, "2213_facebook");
			yield return null;
			rankID_week = new RankID(userId, "_week");
			yield return null;
			rankID_vip = new RankID(userId, "_vip");
			yield return null;
			rankID_global = new RankIDFixed(userId, "2213_facebook");
			yield return null;
			pictureUrl = new PictureUrl(userId);
			yield return null;
			HighestScoreSystem.Instance.Request();
		}

		public IEnumerator RequestFacebook()
		{
			playerName = new PlayerName(userId, FacebookManger.Instance.me.name);
			yield return null;
			countryCode = new CountryCode(userId);
			yield return null;
			subscription = new Subscription(userId);
			yield return null;
			playerLevel = new PlayerLevel(userId);
			yield return null;
			score_week = new Score(userId, "_week");
			yield return null;
			score_vip = new Score(userId, "_vip");
			yield return null;
			score_global = new ScoreFixed(userId, "2213_facebook");
			yield return null;
			rankID_week = new RankID(userId, "_week");
			yield return null;
			rankID_vip = new RankID(userId, "_vip");
			yield return null;
			rankID_global = new RankIDFixed(userId, "2213_facebook");
			yield return null;
			pictureUrl = new PictureUrl(userId, FacebookManger.Instance.me.picture);
			yield return null;
			HighestScoreSystem.Instance.Request();
		}

		public bool CanUploadPlayerName()
		{
			if (playerName == null)
			{
				return false;
			}
			return !SecondManager.Instance.facebook && SecondManager.Instance.hasInited && playerName.rule.hasGotInitValue;
		}

		public void RequestPlayerName()
		{
			if (playerName != null && playerName.rule.Check())
			{
				playerName.GetStringValue();
			}
		}

		public void SynchronisePlayerName(string playerName)
		{
			if (this.playerName != null)
			{
				this.playerName.Synchronise(playerName);
			}
		}

		public void UploadPlayerName_Strict(string playerName)
		{
			if (this.playerName != null)
			{
				this.playerName.UploadKeyValue_Strict(playerName);
			}
		}

		public void RequestScore()
		{
			if (score_week != null && score_week.rule.Check())
			{
				score_week.GetScore();
			}
		}

		public void UploadScore(int score)
		{
			if (score_week != null)
			{
				score_week.UploadScore(score);
			}
		}

		public void SynchroniseScore(int score)
		{
			if (score_week != null)
			{
				score_week.Synchronise(score);
			}
		}

		public void RequestScoreVIP()
		{
			if (score_vip != null && score_vip.rule.Check())
			{
				score_vip.GetScore();
			}
		}

		public void UploadScoreVip(int score)
		{
			if (score_vip != null)
			{
				score_vip.UploadScore(score);
			}
		}

		public void SynchroniseScoreVIP(int score)
		{
			if (score_vip != null)
			{
				score_vip.Synchronise(score);
			}
		}

		public void RequestScoreGlobal()
		{
			if (score_global != null && score_global.rule.Check())
			{
				score_global.GetScore();
			}
		}

		public void UploadScoreGlobal(int score)
		{
			if (score_global != null)
			{
				score_global.UploadScore(score);
			}
		}

		public void SynchroniseScoreGlobal(int score)
		{
			if (score_global != null)
			{
				score_global.Synchronise(score);
			}
		}

		public void RequestRankID()
		{
			if (rankID_week != null && rankID_week.rule.Check())
			{
				rankID_week.GetRankID();
			}
		}

		public void SynchroniseRankID(int rankId)
		{
			if (rankID_week != null)
			{
				rankID_week.Synchronise(rankId);
			}
		}

		public void SynchroniseRankIDVIP(int rankId)
		{
			if (rankID_vip != null)
			{
				rankID_vip.Synchronise(rankId);
			}
		}

		public void SynchroniseRankIDGlobal(int rankId)
		{
			if (rankID_global != null)
			{
				rankID_global.Synchronise(rankId);
			}
		}

		public void RequestCountryCode()
		{
			if (countryCode != null && countryCode.rule.Check())
			{
				countryCode.GetStringValue();
			}
		}

		public void SynchroniseCountryCode(string countryCode)
		{
			if (this.countryCode != null)
			{
				this.countryCode.Synchronise(countryCode);
			}
		}

		public void UploadSubscription(string value)
		{
			if (subscription != null)
			{
				subscription.UploadKeyValue_Force(value);
			}
		}

		public void UploadPlayerLevel(string value)
		{
			if (playerLevel != null)
			{
				playerLevel.UploadKeyValue_Force(value);
			}
		}

		public void RequestPictrueUrl()
		{
			if (pictureUrl != null && pictureUrl.rule.Check())
			{
				pictureUrl.GetStringValue();
			}
		}

		public void SynchronisePictrueUrl(string url)
		{
			if (pictureUrl != null)
			{
				pictureUrl.Synchronise(url);
			}
		}
	}
}
