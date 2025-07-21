using System;
using UnityEngine;

namespace Network
{
	public class ServeTimeUpdate : MonoBehaviour
	{
		public int _interval = 60;

		public int _max_interval = 60000;

		private int _factor;

		private double _deltaTime = 60.0;

		private float _time = -1f;

		private long _serverTime = -1L;

		private bool _requestDatasAfter;

		private DateTime _utcServerTime;

		private static ServeTimeUpdate _instance;

		public static ServeTimeUpdate Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Utils.FindObject<ServeTimeUpdate>();
				}
				if (_instance == null)
				{
					_instance = new GameObject("ServeTimeUpdate").AddComponent<ServeTimeUpdate>();
				}
				return _instance;
			}
		}

		public string RankWeekString { get; private set; }

		public long ServerTime
		{
			get
			{
				return _serverTime;
			}
		}

		public float time
		{
			get
			{
				return _time;
			}
		}

		public bool IsActive { get; set; }

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Start()
		{
			RequestServerTime();
		}

		public void RequestServerTime()
		{
			if (Application.internetReachability != 0)
			{
				IsActive = false;
				NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetServerTime, null, GetServerTimeListener);
			}
		}

		private void GetServerTimeListener(string s)
		{
			Debug.Log("GetServerTimeListener:" + s);
			IsActive = true;
			long result;
			if (!string.IsNullOrEmpty(s) && long.TryParse(s, out result) && _serverTime < result)
			{
				_serverTime = result;
				_utcServerTime = DateTime.UtcNow;
				_time = RealTimeTracker.time;
				_deltaTime = DateManager.CalcDeltaTime(result);
				RankWeekString = DateManager.CalcWeekRankString(result);
				_interval *= 3;
				if (_requestDatasAfter)
				{
					ServerManager.Instance.RequestUserInformation(SecondManager.Instance.platType, SecondManager.Instance.userId);
				}
			}
		}

		private bool CheckForNewCalc()
		{
			return (double)_time + _deltaTime < (double)RealTimeTracker.time;
		}

		public bool ServerTimeValid()
		{
			return _serverTime != -1;
		}

		public bool ChechCanRequestDatas()
		{
			bool flag = ServerTimeValid();
			_requestDatasAfter = !flag;
			return flag;
		}

		public DateTime ServerDateTime()
		{
			return DateManager.TranslateServeTimeStampToDateTime(_serverTime) + (DateTime.UtcNow - _utcServerTime);
		}

		private void Update()
		{
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
			if (!IsActive)
			{
				IsActive = true;
			}
			else if (CheckForNewCalc())
			{
				RequestServerTime();
			}
		}
	}
}
