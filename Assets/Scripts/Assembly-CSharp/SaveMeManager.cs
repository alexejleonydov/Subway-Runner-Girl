using UnityEngine;

public static class SaveMeManager
{
	public static int _numberOfUsedKeysInCurrentRun = 2;

	public static bool IS_PURCHASE_MADE_FROM_INGAME;

	public static bool IS_PURCHASE_RUNNING_INGAME;

	public static int GetNumberOfKeysToSaveMe()
	{
		return _numberOfUsedKeysInCurrentRun;
	}

	public static void HasShownKeysPopup()
	{
		PlayerInfo.Instance.hasShownKeysPopup = true;
	}

	public static void IncrementNumberOfUsedKeys()
	{
		if (_numberOfUsedKeysInCurrentRun <= 0)
		{
			_numberOfUsedKeysInCurrentRun = 2;
			return;
		}
		_numberOfUsedKeysInCurrentRun *= 2;
		if (_numberOfUsedKeysInCurrentRun >= int.MaxValue)
		{
			_numberOfUsedKeysInCurrentRun = int.MaxValue;
		}
	}

	public static void ResetSaveMeForNewRun()
	{
		_numberOfUsedKeysInCurrentRun = 2;
	}

	public static void SendReviveIfPurchaseSucceeded()
	{
		if (UIScreenController.isInstanced)
		{
			if (UIScreenController.Instance.GetTopScreenName().Equals("IngameUI") && IS_PURCHASE_MADE_FROM_INGAME)
			{
				if (PlayerInfo.Instance.amountOfKeys - GetNumberOfKeysToSaveMe() >= 0)
				{
					PlayerInfo.Instance.amountOfKeys -= GetNumberOfKeysToSaveMe();
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, GetNumberOfKeysToSaveMe());
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_int_game_revive", 0, GetNumberOfKeysToSaveMe());
					TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, GetNumberOfKeysToSaveMe());
				}
				IS_PURCHASE_MADE_FROM_INGAME = false;
				IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance = Revive.Instance;
				if (instance != null)
				{
					instance.SendRevive();
				}
				else
				{
					Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
				IncrementNumberOfUsedKeys();
				UIScreenController instance2 = UIScreenController.Instance;
				IngameScreen ingameScreen = instance2.GetScreenFromCache(instance2.GetTopScreenName()) as IngameScreen;
				if (ingameScreen == null)
				{
					Debug.LogError("IngameScreen == NULL");
				}
				else
				{
					ingameScreen.SetPauseButtonVisibility(true);
				}
			}
		}
		else
		{
			Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}

	public static void SkipReviveIfPurchaseFailed()
	{
		if (UIScreenController.isInstanced)
		{
			if (UIScreenController.Instance.GetTopScreenName() == "IngameUI" && IS_PURCHASE_MADE_FROM_INGAME)
			{
				IS_PURCHASE_MADE_FROM_INGAME = false;
				IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance = Revive.Instance;
				if (instance != null)
				{
					instance.SendSkipRevive();
				}
				else
				{
					Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
			}
			Time.timeScale = 1f;
		}
		else
		{
			Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}
}
