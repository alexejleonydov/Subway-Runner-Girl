using System;
using System.Collections.Generic;

namespace Network
{
	public abstract class KeyJsonValue
	{
		protected string _key;

		protected SynchroniseRule _rule;

		public Action onValueChange;

		public SynchroniseRule rule
		{
			get
			{
				return _rule;
			}
		}

		public KeyJsonValue()
		{
			_rule = new SynchroniseRule(60f);
		}

		public virtual void Reset()
		{
			_rule.Reset();
		}

		public void GetJsonData()
		{
			if (SecondManager.Instance.hasInited && ServeTimeUpdate.Instance.ServerTimeValid())
			{
				KeyJsonValueRequest.Instance.GetAllJsonData(SecondManager.Instance.userId, _key, GetJsonDataCallback);
				_rule.Request();
			}
		}

		private void GetJsonDataCallback(int status, object obj)
		{
			_rule.Respond();
			if (status == -1 || status == 0)
			{
				return;
			}
			_rule.hasGotInitValue = true;
			string userId = SecondManager.Instance.userId;
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey(userId))
			{
				if (dictionary[userId] is bool)
				{
					Parse(null);
				}
				else
				{
					Parse(dictionary[userId]);
				}
			}
		}

		protected virtual void Parse(object obj)
		{
		}

		protected virtual string ToJson()
		{
			return null;
		}

		public void UploadJson(string json)
		{
			if (SecondManager.Instance.hasInited)
			{
				KeyJsonValueRequest.Instance.UploadJsonData(SecondManager.Instance.userId, _key, json, UploadCallback);
			}
		}

		protected virtual void UploadCallback(int status, object obj)
		{
		}

		protected virtual void OnValueChange()
		{
			if (onValueChange != null)
			{
				onValueChange();
			}
		}
	}
}
