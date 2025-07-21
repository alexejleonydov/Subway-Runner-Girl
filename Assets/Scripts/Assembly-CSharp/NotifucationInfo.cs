using System;

public class NotifucationInfo
{
	public Func<bool> updateNotificationValue;

	public bool value;

	public NotifucationInfo()
	{
		updateNotificationValue = null;
		value = false;
	}

	public void SetValue()
	{
		if (updateNotificationValue != null)
		{
			value = updateNotificationValue();
		}
	}
}
