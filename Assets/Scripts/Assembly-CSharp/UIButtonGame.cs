using UnityEngine;

public class UIButtonGame : UIBasicButton
{
	public enum GameMessage
	{
		_notSet = 0,
		StartNewRun = 1,
		RestartFromPause = 2
	}

	public GameMessage messageType;

	public override void Send()
	{
		if (messageType == GameMessage.StartNewRun)
		{
			if (Game.Instance != null)
			{
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
				SaveMeManager.ResetSaveMeForNewRun();
			}
			else
			{
				UIScreenController.Instance.PushScreen("GameoverUI");
				Debug.LogError("UIButtonGame:Send");
			}
		}
		else if (messageType != GameMessage.RestartFromPause)
		{
		}
	}
}
