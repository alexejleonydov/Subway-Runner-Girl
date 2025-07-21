using System;
using UnityEngine;

namespace Network
{
	public class Score
	{
		private int _score;

		private int _scoreTmp;

		private string _rankKey;

		private string _suffix;

		public Action onValueChange;

		private string _key;

		public int score
		{
			get
			{
				return _score;
			}
			private set
			{
				if (_score != value)
				{
					_score = value;
					OnValueChange();
				}
			}
		}

		public SynchroniseRule rule { get; private set; }

		public Score(string key, string suffix = null)
		{
			rule = new SynchroniseRule(60f);
			_key = key;
			_rankKey = ServeTimeUpdate.Instance.RankWeekString;
			_suffix = suffix;
			if (_rankKey.Equals(PlayerPrefs.GetString("Network_Score_" + _suffix, string.Empty)))
			{
				_score = PlayerPrefs.GetInt("Network_Score_" + _key + _suffix, 0);
			}
			else
			{
				_score = 0;
				PlayerPrefs.SetString("Network_Score_" + _suffix, _rankKey);
				PlayerPrefs.SetInt("Network_Score_" + _key + _suffix, 0);
			}
			_scoreTmp = _score;
			GetScore();
		}

		public void GetScore()
		{
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited)
			{
				_rankKey = ServeTimeUpdate.Instance.RankWeekString;
				ScoreRankRequest.Instance.GetUserScore(_rankKey + _suffix, OnGetScoreCallback);
				rule.Request();
			}
		}

		private void OnGetScoreCallback(int status, object obj)
		{
			if (status != -1 && status != 0)
			{
				score = (int)(long)obj;
				_scoreTmp = _score;
				rule.hasGotInitValue = true;
			}
		}

		public void UploadScore(int score)
		{
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited)
			{
				if (!ServeTimeUpdate.Instance.RankWeekString.Equals(_rankKey))
				{
					this.score = 0;
					_scoreTmp = 0;
					_rankKey = ServeTimeUpdate.Instance.RankWeekString;
				}
				if (_scoreTmp < score || _scoreTmp > _score)
				{
					_scoreTmp = score;
					ScoreRankRequest.Instance.UploadScoreExpire(_rankKey + _suffix, score, OnUploadScoreCallback);
				}
			}
		}

		private void OnUploadScoreCallback(int status, object obj)
		{
			if (status == -1 || status == 0)
			{
				return;
			}
			rule.hasGotInitValue = true;
			score = _scoreTmp;
			if (string.IsNullOrEmpty(_suffix))
			{
				RankID rankID_Week = ServerManager.Instance.RankID_Week;
				if (rankID_Week != null)
				{
					rankID_Week.GetRankID();
				}
				HighestScoreSystem.Instance.Request();
			}
		}

		public void Synchronise(int score)
		{
			if (SecondManager.Instance.hasInited)
			{
				rule.hasGotInitValue = true;
				this.score = score;
			}
		}

		private void OnValueChange()
		{
			PlayerPrefs.SetInt("Network_Score_" + _key + _suffix, _score);
			if (onValueChange != null)
			{
				onValueChange();
			}
			ServerManager.Instance.OnScoreChange();
		}
	}
}
