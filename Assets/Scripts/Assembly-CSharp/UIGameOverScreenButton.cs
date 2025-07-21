using UnityEngine;

public class UIGameOverScreenButton : UIButtonGame
{
	protected override void Send()
	{
		if (messageType != GameMessage.StartNewRun)
		{
			return;
		}
		if (Game.Instance != null)
		{
			TrialManager.Instance.preUseTryRole = false;
			if (!TrialManager.Instance.nothingElse && TrialManager.Instance.currentTrialInfo != null && TrialManager.Instance.CheckOnMainScreen() && PlayerInfo.Instance.showTrialPopupCount <= 4 && PlayerInfo.Instance.gameOverFullAdCount >= 4)
			{
				TrialPopup.startNewGame = true;
				PlayerInfo.Instance.showTrialPopupCount++;
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "Try_popup_auto", 0);
				UIScreenController.Instance.QueuePopup("TryHoverboardPopup");
				PlayerInfo.Instance.gameOverFullAdCount = 0;
			}
			else
			{
				SaveMeManager.ResetSaveMeForNewRun();
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
			}
		}
		else
		{
			UIScreenController.Instance.PushScreen("GameoverUI");
			Debug.LogError("UIGameOverScreenButton:Send");
		}
	}
}
