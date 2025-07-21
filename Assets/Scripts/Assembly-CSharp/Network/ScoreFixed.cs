using System;
using UnityEngine;

namespace Network
{
	public class ScoreFixed
	{
		private int _score;

		private int _scoreTmp;

		private string _rankKey;

		private string _key;

		public Action onValueChange;

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

		public ScoreFixed(string key, string rankKey)
		{
			rule = new SynchroniseRule(60f);
			_key = key;
			_rankKey = rankKey;
			_score = PlayerPrefs.GetInt("Network_Score_" + _rankKey + _key, 0);
			_scoreTmp = _score;
			GetScore();
		}

		public void GetScore()
		{
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited)
			{
				ScoreRankRequest.Instance.GetUserScore(_rankKey, OnGetScoreCallback);
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
			if (ServeTimeUpdate.Instance.ServerTimeValid() && SecondManager.Instance.hasInited && (_scoreTmp < score || _scoreTmp > _score))
			{
				_scoreTmp = score;
				ScoreRankRequest.Instance.UploadScoreExpire(_rankKey, score, OnUploadScoreCallback);
			}
		}

		private void OnUploadScoreCallback(int status, object obj)
		{
			if (status != -1 && status != 0)
			{
				rule.hasGotInitValue = true;
				score = _scoreTmp;
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
			PlayerPrefs.SetInt("Network_Score_" + _rankKey + _key, _score);
			if (onValueChange != null)
			{
				onValueChange();
			}
			ServerManager.Instance.OnScoreChange();
		}
	}
}
