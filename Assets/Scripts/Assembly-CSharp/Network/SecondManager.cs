using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class SecondManager : MonoBehaviour
	{
		public class LoginIn
		{
			private PlatFormType _platType;

			public string platId { get; set; }

			public bool hasInited { get; private set; }

			public string userId { get; private set; }

			public LoginIn(PlatFormType type)
			{
				_platType = type;
			}

			public void GetUserIDListener(string s)
			{
				Debug.Log("GetUserIDListener: " + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if (dictionary == null || !dictionary.ContainsKey("status"))
				{
					return;
				}
				int num = (int)(long)dictionary["status"];
				if (num == -1 || num == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						Debug.Log(dictionary["msg"]);
					}
				}
				else if (dictionary.ContainsKey("data"))
				{
					userId = (string)dictionary["data"];
					hasInited = true;
				}
			}
		}

		private static SecondManager _instance;

		public int _min_interval = 300;

		public int _max_interval = 60000;

		private int _factor;

		private int _interval;

		private LoginIn _guestLoginIn;

		private LoginIn _facebookLoginIn;

		public static SecondManager Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("SecondManager");
					_instance = gameObject.AddComponent<SecondManager>();
				}
				return _instance;
			}
		}

		public PlatFormType platType { get; private set; }

		public string userId
		{
			get
			{
				if (platType == PlatFormType.facebook)
				{
					return _facebookLoginIn.userId;
				}
				return _guestLoginIn.userId;
			}
		}

		public bool hasInited
		{
			get
			{
				if (platType == PlatFormType.facebook)
				{
					return _facebookLoginIn.hasInited;
				}
				return _guestLoginIn.hasInited;
			}
		}

		public bool facebook
		{
			get
			{
				return platType == PlatFormType.facebook && _facebookLoginIn.hasInited;
			}
		}

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			_interval = _min_interval;
			_guestLoginIn = new LoginIn(PlatFormType.guest);
			_facebookLoginIn = new LoginIn(PlatFormType.facebook);
			platType = PlatFormType.guest;
		}

		public void RequestGuestUserID()
		{
			ResetTime();
			if (Application.internetReachability != 0)
			{
				Dictionary<string, string> userLoginDict = NetworkConnect.Instance.GetUserLoginDict();
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUniqueUid, userLoginDict, _guestLoginIn.GetUserIDListener);
			}
		}

		public void RequestFacebookUserID(string platId)
		{
			_facebookLoginIn.platId = platId;
			platType = PlatFormType.facebook;
			RequestFacebookUserID();
		}

		private void RequestFacebookUserID()
		{
			ResetTime();
			if (Application.internetReachability != 0)
			{
				Dictionary<string, string> userLoginDict = NetworkConnect.Instance.GetUserLoginDict(PlatFormType.facebook, _facebookLoginIn.platId);
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUniqueUid, userLoginDict, _facebookLoginIn.GetUserIDListener);
			}
		}

		private void ResetTime()
		{
			_factor = 0;
			base.enabled = true;
		}

		private void Update()
		{
			/*if (platType == PlatFormType.guest && _guestLoginIn.hasInited)
			{
				base.enabled = false;
				_interval = _min_interval;
				ServerManager.Instance.RequestUserInformation(platType, _guestLoginIn.userId);
				return;
			}
			if (platType == PlatFormType.facebook && _facebookLoginIn.hasInited)
			{
				base.enabled = false;
				_interval = _min_interval;
				ServerManager.Instance.RequestUserInformation(platType, _facebookLoginIn.userId);
				return;
			}
			if (_factor < _interval)
			{
				_factor++;
				return;
			}
			_factor = 0;
			if (_interval < _max_interval)
			{
				_interval *= 2;
			}
			else
			{
				_interval = _max_interval;
			}
			if (platType == PlatFormType.guest)
			{
				RequestGuestUserID();
				_interval = _min_interval;
			}
			else if (platType == PlatFormType.facebook)
			{
				RequestFacebookUserID();
				_interval = _min_interval;
			}*/
		}
	}
}
