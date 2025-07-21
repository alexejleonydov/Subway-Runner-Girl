using System;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager
{
	public delegate void AchievementCompleteHandler(string msg);

	public delegate void TaskCompleteHandler(string msg);

	public delegate void TaskSetCompleteHandler();

	public TaskSetCompleteHandler onTaskSetComplete;

	public TaskCompleteHandler onTaskComplete;

	public AchievementCompleteHandler onAchievementComplete;

	private Task[] _combinedArray;

	private int _currentTaskSetLoaded = -1;

	private int _currentTaskTemplateSetLoaded = -1;

	private int[] _currentRunProgress;

	private static TasksManager _instance;

	private Task[][] _tasks;

	private TaskTemplate[] _templates;

	private PlayerInfo playerinfo = PlayerInfo.Instance;

	private Task[] combinedArray
	{
		get
		{
			if (_currentTaskSetLoaded != playerinfo.currentTaskSet || _combinedArray == null)
			{
				if (_combinedArray == null)
				{
					_combinedArray = new Task[Achievements.NUMBER_OF_ACHIEVEMENTS + 3];
				}
				for (int i = 0; i < 3; i++)
				{
					_combinedArray[i] = tasks[playerinfo.currentTaskSet][i];
				}
				if (_currentTaskSetLoaded == -1)
				{
					for (int j = 0; j < Achievements.NUMBER_OF_ACHIEVEMENTS; j++)
					{
						_combinedArray[j + 3] = Achievements.achievementArray[j];
					}
				}
			}
			_currentTaskSetLoaded = currentTaskSet;
			return _combinedArray;
		}
	}

	public int currentTaskSet
	{
		get
		{
			return playerinfo.currentTaskSet;
		}
		set
		{
			if (value != playerinfo.currentTaskSet)
			{
				int num = Mathf.Clamp(value, 0, taskSetCount);
				playerinfo.InitCurrentTaskSet(num, tasks[num].Length, true);
			}
		}
	}

	public bool inRun
	{
		get
		{
			return _currentRunProgress != null;
		}
		set
		{
			if (value)
			{
				_currentRunProgress = new int[combinedArray.Length];
			}
			else
			{
				if (_currentRunProgress == null)
				{
					return;
				}
				if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
				{
					for (int i = 0; i < combinedArray.Length; i++)
					{
						if (templates[i].singleRun && templates[i].completeIfLess && _currentRunProgress[i] < combinedArray[i].aim)
						{
							playerinfo.SetCurrentTaskProgress(i, combinedArray[i].aim);
							Complete(i);
						}
					}
				}
				PlayerDidThis(TaskTarget.StayInOneLane, (int)(Time.time - Character.Instance.sameLaneTimeStamp));
				RemoveProgressForThis(TaskTarget.StayInOneLane);
				_currentRunProgress = null;
			}
		}
	}

	public static TasksManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TasksManager();
			}
			return _instance;
		}
	}

	private Task[][] tasks
	{
		get
		{
			if (_tasks == null)
			{
				Task[][] repeatableTasks = TasksData.repeatableTasks;
				Task[][] singleuseTasks = TasksData.singleuseTasks;
				_tasks = new Task[singleuseTasks.Length + repeatableTasks.Length][];
				for (int i = 0; i < singleuseTasks.Length; i++)
				{
					_tasks[i] = singleuseTasks[i];
				}
				for (int j = 0; j < repeatableTasks.Length; j++)
				{
					_tasks[singleuseTasks.Length + j] = repeatableTasks[j];
				}
			}
			return _tasks;
		}
	}

	public int taskSetCount
	{
		get
		{
			return tasks.Length;
		}
	}

	public int taskSetStoryCount
	{
		get
		{
			return tasks.Length - TasksData.repeatableTasks.Length;
		}
	}

	private TaskTemplate[] templates
	{
		get
		{
			if (_currentTaskTemplateSetLoaded != playerinfo.currentTaskSet || _templates == null)
			{
				Dictionary<TaskType, TaskTemplate> taskTemplates = TasksData.taskTemplates;
				if (_templates == null)
				{
					_templates = new TaskTemplate[Achievements.NUMBER_OF_ACHIEVEMENTS + 3];
				}
				for (int i = 0; i < 3; i++)
				{
					_templates[i] = taskTemplates[tasks[playerinfo.currentTaskSet][i].type];
				}
				if (_currentTaskTemplateSetLoaded == -1)
				{
					for (int j = 0; j < Achievements.NUMBER_OF_ACHIEVEMENTS; j++)
					{
						_templates[j + 3] = taskTemplates[Achievements.achievementArray[j].type];
					}
				}
				_currentTaskTemplateSetLoaded = playerinfo.currentTaskSet;
			}
			return _templates;
		}
	}

	private TasksManager()
	{
		if (playerinfo.currentTaskSet == -1)
		{
			playerinfo.InitCurrentTaskSet(0, tasks[0].Length, true);
		}
	}

	private void CheckAllCompleteAndIncrement()
	{
		for (int i = 3; i < combinedArray.Length; i++)
		{
			if (templates[i].singleRun && templates[i].completeIfLess)
			{
				if (_currentRunProgress[i] < combinedArray[i].aim)
				{
					playerinfo.SetCurrentTaskProgress(i, combinedArray[i].aim);
					Complete(i, 1f, true);
				}
			}
			else if (_currentRunProgress != null)
			{
				if ((float)_currentRunProgress[i] / (float)combinedArray[i].aim > (float)playerinfo.GetCurrentTaskProgress(i) / (float)combinedArray[i].aim)
				{
					Complete(i, (float)_currentRunProgress[i] / (float)combinedArray[i].aim, true);
				}
				else
				{
					Complete(i, (float)playerinfo.GetCurrentTaskProgress(i) / (float)combinedArray[i].aim, true);
				}
			}
			else
			{
				Complete(i, (float)playerinfo.GetCurrentTaskProgress(i) / (float)combinedArray[i].aim, true);
			}
		}
	}

	private void Complete(int task, float completedFactor = 1f, bool sendEvenIfInRun = false)
	{
		int num = 3;
		if (task >= num)
		{
			if (inRun && !sendEvenIfInRun)
			{
				return;
			}
			int num2 = task - num;
			if (num2 < 0 || num2 >= Achievements.NUMBER_OF_ACHIEVEMENTS)
			{
				LogError("Tasks.Complete: achievement index is out of bounds", null);
			}
			else if (completedFactor == 1f)
			{
				if (onAchievementComplete != null)
				{
					string format = Strings.Get(templates[task].ultraShortDescription);
					onAchievementComplete(string.Format(format, combinedArray[task].aim));
				}
				NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.AchiementFinished);
			}
		}
		else
		{
			if (completedFactor != 1f)
			{
				return;
			}
			TaskCompleteHandler taskCompleteHandler = onTaskComplete;
			if (taskCompleteHandler != null)
			{
				string format2 = Strings.Get(templates[task].ultraShortDescription);
				taskCompleteHandler(string.Format(format2, combinedArray[task].aim));
			}
			PlayerInfo.Instance.stats[Stat.TaskCompleted]++;
			bool flag = true;
			int num3 = 0;
			for (int i = 0; i < 3; i++)
			{
				int currentTaskProgress = playerinfo.GetCurrentTaskProgress(i);
				if (currentTaskProgress >= combinedArray[i].aim)
				{
					num3++;
				}
				if (currentTaskProgress < combinedArray[i].aim)
				{
					flag = false;
					break;
				}
			}
		}
	}

	public void CheckPlayerLevel()
	{
		float num = Game.Instance.GetExpCoefficient() * (float)(PlayerInfo.Instance.amountOfLevel - 1) + 8f;
		float num2 = 0f;
		num2 = PlayerInfo.Instance.amountOfExp;
		if (num2 >= num)
		{
			PlayerInfo.Instance.amountOfLevel++;
			PlayerInfo.Instance.amountOfExp = (int)(num2 - num);
			UIScreenController.Instance.QueuePopup("LevelUpPopup");
		}
	}

	public void CheckGetNextTask(Action<bool> callBack = null, Action refreshWait = null)
	{
		if (LevelExpManager.Instance.IsCoolingDownOver() && PlayerInfo.Instance.levelTaskComplete)
		{
			GetNewTaskSet();
			PlayerInfo.Instance.ResetTaskReward();
			PlayerInfo.Instance.levelTaskComplete = false;
			PlayerInfo.Instance.GetTaskPlayerLevel = PlayerInfo.Instance.amountOfLevel;
		}
		if (playerinfo.TaskRewardAllPayed())
		{
			PlayerInfo.Instance.levelTaskComplete = true;
			if (playerinfo.taskCompletedSum > Game.Instance.newPlayerSkipTaskNum)
			{
				if (callBack != null)
				{
					callBack(true);
				}
				if (refreshWait != null)
				{
					refreshWait();
				}
				return;
			}
			if (LevelExpManager.Instance.IsCoolingDownOver() && PlayerInfo.Instance.levelTaskComplete)
			{
				GetNewTaskSet();
				PlayerInfo.Instance.ResetTaskReward();
				PlayerInfo.Instance.levelTaskComplete = false;
			}
			if (callBack != null)
			{
				callBack(false);
			}
		}
		else if (callBack != null)
		{
			callBack(false);
		}
	}

	public void GetNewTaskSet()
	{
		PlayerDidThis(TaskTarget.TaskSet);
		if (_currentRunProgress != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if (_currentRunProgress[i] != 0)
				{
					_currentRunProgress[i] = 0;
				}
			}
		}
		if (currentTaskSet + 1 > taskSetStoryCount)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
			if (!inRun)
			{
				UIScreenController.Instance.QueueChest();
			}
		}
		int num = ((currentTaskSet < taskSetCount - 1) ? (playerinfo.currentTaskSet + 1) : (playerinfo.currentTaskSet - TasksData.repeatableTasks.Length + 1));
		playerinfo.taskCompletedSum++;
		int taskCount = 0;
		if (num < taskSetCount)
		{
			taskCount = tasks[num].Length;
		}
		playerinfo.InitCurrentTaskSet(num, taskCount, true);
		PlayerDidThis(TaskTarget.ReachTaskSet);
	}

	public TaskInfo[] GetTaskInfo()
	{
		TaskInfo[] array = new TaskInfo[combinedArray.Length];
		for (int i = 0; i < combinedArray.Length; i++)
		{
			GetTaskInfo(i, ref array[i]);
		}
		return array;
	}

	public TaskInfo GetTaskInfo(int taskNumber)
	{
		TaskInfo info = default(TaskInfo);
		GetTaskInfo(taskNumber, ref info);
		return info;
	}

	public AchievementInfo GetAchievementInfo(int taskNumber)
	{
		if (taskNumber < 3 || taskNumber >= combinedArray.Length)
		{
			return null;
		}
		return Achievements.achievementInfo[taskNumber - 3];
	}

	private void GetTaskInfo(int taskNumber, ref TaskInfo info)
	{
		info.task = combinedArray[taskNumber];
		info.template = templates[taskNumber];
		info.progress = playerinfo.GetCurrentTaskProgress(taskNumber);
		info.complete = info.progress >= info.task.aim;
		if (!info.complete && info.template.singleRun && inRun)
		{
			info.progress = _currentRunProgress[taskNumber];
		}
	}

	public bool IsTaskTargetActive(TaskTarget target)
	{
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (templates[i].taskTarget == target)
			{
				return playerinfo.GetCurrentTaskProgress(i) < combinedArray[i].aim;
			}
		}
		return false;
	}

	private static void LogError(string msg, UnityEngine.Object context)
	{
		Debug.LogError(msg, context);
	}

	public void OnChangeIsCharacterOnGround(Transform characterTransform)
	{
		if (characterTransform.localPosition.y < 1f)
		{
			RemoveProgressForThis(TaskTarget.EarnCoinWithoutTouchingGround);
		}
	}

	public void PlayerDidThis(TaskTarget myTask, int magnitude = 1, int taskToIgnore = -1)
	{
		if (templates == null)
		{
			LogError("currentTemplates == null", null);
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (i == taskToIgnore || (templates[i].singleRun && !inRun) || templates[i].taskTarget != myTask)
			{
				continue;
			}
			int num = playerinfo.GetCurrentTaskProgress(i);
			if (templates[i].singleRun && inRun && num < combinedArray[i].aim && _currentRunProgress != null)
			{
				num = _currentRunProgress[i];
			}
			int num2 = num + magnitude;
			if (templates[i].completeIfLess)
			{
				if (num2 > combinedArray[i].aim)
				{
					if (templates[i].singleRun)
					{
						if (_currentRunProgress != null)
						{
							_currentRunProgress[i] = num2;
						}
						else
						{
							LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
						}
					}
					else
					{
						playerinfo.SetCurrentTaskProgress(i, num2);
						Complete(i, (float)num2 / (float)combinedArray[i].aim);
					}
					continue;
				}
				if (templates[i].singleRun && inRun)
				{
					if (_currentRunProgress != null)
					{
						_currentRunProgress[i] = num2;
					}
					else
					{
						LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
					}
				}
				playerinfo.SetCurrentTaskProgress(i, combinedArray[i].aim);
				Complete(i);
			}
			else if (templates[i].completeIfEqual)
			{
				if (myTask == TaskTarget.GetExactlyAmountOfCoins)
				{
					num2 = magnitude;
				}
				if (num2 == combinedArray[i].aim)
				{
					playerinfo.SetCurrentTaskProgress(i, tasks[playerinfo.currentTaskSet][i].aim);
					Complete(i);
				}
			}
			else if (num2 < combinedArray[i].aim)
			{
				if (templates[i].singleRun)
				{
					if (_currentRunProgress != null)
					{
						_currentRunProgress[i] = num2;
					}
					else
					{
						LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
					}
					continue;
				}
				if (_currentRunProgress != null)
				{
					_currentRunProgress[i] = num2;
				}
				playerinfo.SetCurrentTaskProgress(i, num2);
				Complete(i, (float)num2 / (float)combinedArray[i].aim);
			}
			else
			{
				playerinfo.SetCurrentTaskProgress(i, combinedArray[i].aim);
				if (num < combinedArray[i].aim)
				{
					Complete(i);
				}
			}
		}
	}

	public void RemoveProgressForThis(TaskTarget myTask)
	{
		if (templates == null)
		{
			LogError("currentTemplates == null", null);
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (templates[i].taskTarget == myTask)
			{
				ResetProgressForTaskIndex(i);
			}
		}
	}

	private void ResetProgressForTaskIndex(int questIndex)
	{
		if (!GetTaskInfo(questIndex).complete)
		{
			playerinfo.SetCurrentTaskProgress(questIndex, 0);
			if (_currentRunProgress != null)
			{
				_currentRunProgress[questIndex] = 0;
			}
		}
	}

	public void SkipTask(int taskNumber)
	{
		playerinfo.SetCurrentTaskProgress(taskNumber, tasks[playerinfo.currentTaskSet][taskNumber].aim);
		Complete(taskNumber);
	}
}
