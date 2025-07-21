using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public abstract class StringKeyValue
	{
		protected string _key;

		protected string _value;

		protected string _valueTmp;

		protected string _playerPrefsKey;

		protected SynchroniseRule _rule;

		public Action onValueChange;

		protected bool forceUpload;

		public virtual string Value
		{
			get
			{
				return _value;
			}
			protected set
			{
				if (!value.Equals(_value))
				{
					_value = value;
					OnValueChange();
				}
			}
		}

		public SynchroniseRule rule
		{
			get
			{
				return _rule;
			}
		}

		public StringKeyValue()
		{
			_rule = new SynchroniseRule(60f);
		}

		public void GetStringValue()
		{
			if (SecondManager.Instance.hasInited && ServeTimeUpdate.Instance.ServerTimeValid())
			{
				StringKeyValueRequest.Instance.GetStringData(SecondManager.Instance.userId, _key, GetValueCallback);
				_rule.Request();
			}
		}

		private void GetValueCallback(int status, object obj)
		{
			_rule.Respond();
			if (status == -1 || status == 0 || forceUpload)
			{
				return;
			}
			string userId = SecondManager.Instance.userId;
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey(userId))
			{
				if (dictionary[userId] is string)
				{
					_rule.hasGotInitValue = true;
					Value = (string)dictionary[userId];
				}
				else if (dictionary[userId] is bool)
				{
					UploadWithLocalValueByExpire();
				}
			}
		}

		protected virtual void UploadWithLocalValueByExpire()
		{
		}

		public void UploadKeyValue_Strict(string value)
		{
			if (SecondManager.Instance.hasInited && _rule.hasGotInitValue && !string.IsNullOrEmpty(value) && !value.Equals(_value))
			{
				_valueTmp = value;
				StringKeyValueRequest.Instance.UploadStringData(SecondManager.Instance.userId, _key, _valueTmp, UploadCallback);
			}
		}

		public void UploadKeyValue_Force(string value)
		{
			if (SecondManager.Instance.hasInited && !string.IsNullOrEmpty(value))
			{
				_valueTmp = value;
				StringKeyValueRequest.Instance.UploadStringData(SecondManager.Instance.userId, _key, _valueTmp, UploadCallback);
			}
		}

		protected virtual void UploadCallback(int status, object obj)
		{
			if (status != -1 && status != 0)
			{
				_rule.hasGotInitValue = true;
				forceUpload = true;
				Value = _valueTmp;
			}
		}

		public void Synchronise(string value)
		{
			if (SecondManager.Instance.hasInited)
			{
				_rule.hasGotInitValue = true;
				Value = value;
			}
		}

		public virtual void OnValueChange()
		{
			PlayerPrefs.SetString(_playerPrefsKey, _value);
			if (onValueChange != null)
			{
				onValueChange();
			}
		}
	}
}
