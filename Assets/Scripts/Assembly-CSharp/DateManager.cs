using System;
using System.Text;
using Network;
using UnityEngine;

public class DateManager
{
	public static int TheDayAfterTomorrow()
	{
		DateTime utcNow = DateTime.UtcNow;
		return (int)(utcNow.Date.AddDays(2.0) - utcNow).TotalSeconds;
	}

	public static int NextSaturday()
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow;
		int dayOfWeek = (int)utcNow.DayOfWeek;
		return (int)(((dayOfWeek != 6) ? utcNow.Date.AddDays(6 - dayOfWeek) : ((utcNow.Hour >= 12) ? utcNow.Date.AddDays(7.0) : utcNow.Date)).AddHours(12.0).AddMinutes(30.0) - utcNow).TotalSeconds;
	}

	public static double CalcDeltaTime(long timeL)
	{
		return CalcDeltaTime(TranslateServeTimeStampToDateTime(timeL));
	}

	public static double CalcDeltaTime(DateTime now)
	{
		return (new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1.0) - now).TotalSeconds;
	}

	public static string CalcWeekRankString(long time)
	{
		DateTime now = TranslateServeTimeStampToDateTime(time);
		return CalcWeekRankString(now);
	}

	public static string CalcWeekRankString(DateTime now)
	{
		StringBuilder stringBuilder = new StringBuilder();
		DateTime dateTime = new DateTime(now.Year + 1, 1, 1).AddDays(-1.0);
		DayOfWeek dayOfWeek = dateTime.DayOfWeek;
		stringBuilder.Append(NetworkConnect.Instance.AppId());
		stringBuilder.Append("_");
		if ((dateTime - now).Days < (int)dayOfWeek)
		{
			stringBuilder.Append(now.Year + 1);
			stringBuilder.Append("01");
			return stringBuilder.ToString();
		}
		stringBuilder.Append(now.Year);
		stringBuilder.AppendFormat("{0:D2}", CalcWeekOfYear(now, false));
		return stringBuilder.ToString();
	}

	private static int CalcWeekOfYear(DateTime date, bool fremdness)
	{
		DayOfWeek dayOfWeek = new DateTime(date.Year, 1, 1).DayOfWeek;
		int num = 0;
		num = ((!fremdness) ? (date.DayOfYear + (int)(dayOfWeek + 6) % 7) : ((int)(date.DayOfYear + dayOfWeek)));
		int num2 = num / 7;
		if (num % 7 > 0)
		{
			num2++;
		}
		return num2;
	}

	public static DateTime TranslateServeTimeStampToDateTime(long timeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timeStamp);
	}

	public static int TranslateDateTimeToTimeStamp(DateTime time)
	{
		return (int)(time - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
	}

	private static TimeSpan CalcRemainTimeToWeekend(DateTime date, bool fremdness)
	{
		int dayOfWeek = (int)date.DayOfWeek;
		int num = 0;
		num = ((!fremdness) ? (7 - (dayOfWeek + 6) % 7) : (7 - dayOfWeek));
		return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).AddDays(num) - date;
	}

	public static TimeSpan CalcRemainTimeToWeekend(long ticks, float time)
	{
		return CalcRemainTimeToWeekend(TranslateServeTimeStampToDateTime(ticks).AddSeconds(RealTimeTracker.time - time), false);
	}

	public static string TopRunRemainTimeToString(long ticks, float time)
	{
		TimeSpan timeSpan = CalcRemainTimeToWeekend(ticks, time);
		if (timeSpan.Seconds < 0 || timeSpan.Minutes < 0 || timeSpan.Hours < 0 || timeSpan.Days < 0)
		{
			return string.Empty;
		}
		return string.Format("{0}:{1:D2}:{2:D2} {3}", timeSpan.Days * 24 + timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, Strings.Get(LanguageKey.UI_SCREEN_RANK_LEFT));
	}
}
