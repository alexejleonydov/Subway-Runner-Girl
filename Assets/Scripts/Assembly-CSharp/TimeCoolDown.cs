using System;
using UnityEngine;

public class TimeCoolDown
{
	public string prefsKey;

	public int interval;

	private DateTime NextFreeDTime;

	public string NextFreeTimeStr
	{
		get
		{
			return PlayerPrefs.GetString(prefsKey, DateTime.UtcNow.ToString());
		}
		set
		{
			PlayerPrefs.SetString(prefsKey, value);
		}
	}

	public TimeCoolDown(string key, int interval)
	{
		prefsKey = key;
		this.interval = interval;
		NextFreeDTime = DateTime.Parse(NextFreeTimeStr);
	}

	public bool IsCoolingDownOver()
	{
		if (DateTime.Compare(DateTime.UtcNow, NextFreeDTime) > 0)
		{
			return true;
		}
		return false;
	}

	public string GetCoolingDownTime2()
	{
		TimeSpan timeSpan = NextFreeDTime - DateTime.UtcNow;
		return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
	}

	public string GetCoolingDownTime3()
	{
		TimeSpan timeSpan = NextFreeDTime - DateTime.UtcNow;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public void SetFreeTime()
	{
		NextFreeDTime = DateTime.UtcNow.AddSeconds(interval);
		NextFreeTimeStr = NextFreeDTime.ToString();
	}

	public void ForceCoolingDownOver()
	{
		NextFreeDTime = DateTime.UtcNow.AddSeconds(-1.0);
		NextFreeTimeStr = NextFreeDTime.ToString();
	}
}
