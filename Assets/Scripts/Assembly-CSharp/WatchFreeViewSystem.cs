using System.Collections.Generic;

public class WatchFreeViewSystem
{
	private static WatchFreeViewSystem _instance;

	public Dictionary<string, TimeCoolDown> times;

	public static WatchFreeViewSystem Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new WatchFreeViewSystem();
			}
			return _instance;
		}
	}

	private WatchFreeViewSystem()
	{
		times = new Dictionary<string, TimeCoolDown>();
	}

	public void AddNewTime(string key, int interval)
	{
		TimeCoolDown value = new TimeCoolDown(key, interval);
		if (!times.ContainsKey(key))
		{
			times.Add(key, value);
		}
	}

	public void RemoveTime(string key)
	{
		if (times.ContainsKey(key))
		{
			times.Remove(key);
		}
	}

	public void SetFreeTime(string key)
	{
		if (times.ContainsKey(key))
		{
			times[key].SetFreeTime();
		}
	}

	public string GetCoolingDownTime(string key)
	{
		if (times.ContainsKey(key))
		{
			return times[key].GetCoolingDownTime2();
		}
		return string.Empty;
	}

	public bool IsCoolingDownOver(string key)
	{
		if (times.ContainsKey(key))
		{
			return times[key].IsCoolingDownOver();
		}
		return false;
	}
}
