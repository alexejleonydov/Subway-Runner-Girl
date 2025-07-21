using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsObserver
{
	private static NotificationsObserver _instance;

	private Dictionary<NotificationType, NotifucationInfo> notificationData;

	public List<Notification> notifications;

	public static NotificationsObserver Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NotificationsObserver();
			}
			return _instance;
		}
	}

	private NotificationsObserver()
	{
		if (_instance == null)
		{
			_instance = this;
			notifications = new List<Notification>();
			notificationData = new Dictionary<NotificationType, NotifucationInfo>();
			int i = 0;
			for (int num = 4; i < num; i++)
			{
				NotificationType key = (NotificationType)i;
				NotifucationInfo value = new NotifucationInfo();
				notificationData.Add(key, value);
			}
		}
		else
		{
			Debug.LogError("There is more than one NotificationsObserver in the scene!");
		}
	}

	public void RegisterUpdateNotificationValue(NotificationType type, Func<bool> func)
	{
		if (notificationData.ContainsKey(type))
		{
			notificationData[type].updateNotificationValue = func;
		}
	}

	public void RegisterNotificationAction(GameObject go)
	{
		if (!(go == null))
		{
			Notification component = go.GetComponent<Notification>();
			if (!(component == null))
			{
				notifications.Add(component);
			}
		}
	}

	public void NotifyNotificationDataChange(NotificationType type)
	{
		RefreshNotificationData(type);
		RefreshNotificationListShow(type);
	}

	public void NotifyNotificationDataChange(NotificationType type, bool value)
	{
		SetNotificationInfoValue(type, value);
		RefreshNotificationListShow(type);
	}

	public void NotifyNotificationDataChange()
	{
		int i = 0;
		for (int num = 4; i < num; i++)
		{
			NotificationType type = (NotificationType)i;
			RefreshNotificationData(type);
		}
		RefreshNotificationListShow();
	}

	private void RefreshNotificationListShow()
	{
		if (notifications != null && notifications.Count > 0)
		{
			int i = 0;
			for (int count = notifications.Count; i < count; i++)
			{
				RefreshOneNotificationShow(notifications[i]);
			}
		}
	}

	private void RefreshNotificationListShow(NotificationType type)
	{
		if (notifications != null && notifications.Count > 0)
		{
			int i = 0;
			for (int count = notifications.Count; i < count; i++)
			{
				RefreshOneNotificationShow(type, notifications[i]);
			}
		}
	}

	private void RefreshOneNotificationShow(Notification notificaiton)
	{
		int[] ids = notificaiton.GetIds();
		for (int i = 0; i < ids.Length; i++)
		{
			SetNotificationID(notificaiton, ids[i], i);
		}
	}

	private void RefreshOneNotificationShow(NotificationType type, Notification notificaiton)
	{
		int[] ids = notificaiton.GetIds();
		int i = 0;
		for (int num = ids.Length; i < num; i++)
		{
			int num2 = 1 << (int)type;
			if ((ids[i] & num2) != 0)
			{
				SetNotificationID(notificaiton, ids[i], i);
			}
		}
	}

	private void SetNotificationID(Notification notificaiton, int id, int order)
	{
		bool flag = false;
		int num = 1;
		for (int i = 0; i < 4; i++)
		{
			if ((id & num) != 0)
			{
				flag = flag || GetValueByIndex(i);
			}
			num <<= 1;
		}
		notificaiton.SetNotification(order, flag);
	}

	private bool GetValueByIndex(int index)
	{
		return GetValueByNotificationType((NotificationType)index);
	}

	private bool GetValueByNotificationType(NotificationType type)
	{
		if (notificationData.ContainsKey(type))
		{
			return notificationData[type].value;
		}
		return false;
	}

	private void RefreshNotificationData(NotificationType type)
	{
		if (notificationData.ContainsKey(type))
		{
			notificationData[type].SetValue();
		}
	}

	private void SetNotificationInfoValue(NotificationType type, bool value)
	{
		if (notificationData.ContainsKey(type))
		{
			notificationData[type].value = value;
		}
	}
}
