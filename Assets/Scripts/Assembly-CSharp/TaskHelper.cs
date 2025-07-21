using System;
using UnityEngine;

public class TaskHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite tip;

	[SerializeField]
	private Animation anim;

	private void OnEnable()
	{
		CheckTipShow();
		PlayerInfo.Instance.onLevelChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onLevelChanged, new Action(Play));
	}

	public void CheckTipShow()
	{
		if (ShowGetRewardTip())
		{
			tip.enabled = true;
		}
		else if (LevelExpManager.Instance.IsCoolingDownOver() && PlayerPrefs.GetInt("TipTaskSet") != PlayerInfo.Instance.currentTaskSet)
		{
			tip.enabled = true;
		}
		else
		{
			tip.enabled = false;
		}
	}

	public bool ShowGetRewardTip()
	{
		bool result = false;
		for (int i = 0; i < 3; i++)
		{
			if (TasksManager.Instance.GetTaskInfo()[i].complete && !PlayerInfo.Instance.GetIndexTaskRewardPayedOut(i))
			{
				return true;
			}
			result = false;
		}
		if (PlayerInfo.Instance.TaskRewardAllPayed())
		{
			result = false;
		}
		return result;
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.onLevelChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onLevelChanged, new Action(Play));
	}

	private void Play()
	{
		if (anim != null)
		{
			anim.Play();
		}
	}

	public void OnClick()
	{
		if (LevelExpManager.Instance.IsCoolingDownOver())
		{
			PlayerPrefs.SetInt("TipTaskSet", PlayerInfo.Instance.currentTaskSet);
		}
		if (!ShowGetRewardTip())
		{
			tip.enabled = false;
		}
		if (anim != null)
		{
			anim.Stop();
		}
		UIScreenController.Instance.PushPopup("Task_popup");
	}
}
