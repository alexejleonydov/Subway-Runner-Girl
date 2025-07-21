using UnityEngine;

public class TutorialButton : MonoBehaviour
{
	public enum ButtonAction
	{
		Ok = 0,
		Cancel = 1
	}

	public enum TutorialPopupType
	{
		_notSet = 0,
		Tasks1 = 1,
		Tasks2 = 2,
		Facebook = 3,
		CollectFromFriends = 4,
		Helmets = 5,
		ChangeLog = 6,
		ChangeLogEndGame = 7
	}

	[SerializeField]
	private ButtonAction buttonAction;

	private const int NUMBER_OF_FREE_HOVERBOARDS = 3;

	public TutorialPopupType tutorialType;

	private void OnClick()
	{
		if (buttonAction == ButtonAction.Ok)
		{
			if (tutorialType == TutorialPopupType.Tasks1)
			{
				UIScreenController.Instance.QueuePopup("Task_popup");
			}
			else if (tutorialType == TutorialPopupType.Tasks2)
			{
				UIScreenController.Instance.QueuePopup("Task_popup");
			}
			else if (tutorialType == TutorialPopupType.Facebook)
			{
				UIScreenController.Instance.PushScreen("FriendsUI");
			}
			else if (tutorialType == TutorialPopupType.CollectFromFriends)
			{
				UIScreenController.Instance.PushScreen("FriendsUI");
			}
			else if (tutorialType == TutorialPopupType.Helmets)
			{
				UIScreenController.Instance.QueuePopup("HelmetPopup");
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, 3);
			}
			else if (tutorialType != TutorialPopupType.ChangeLog)
			{
				if (tutorialType == TutorialPopupType.ChangeLogEndGame)
				{
					UIScreenController.Instance.QueuePopup("Task_popup");
				}
				else
				{
					Debug.LogError("tutorialType was not defined in " + base.gameObject.name, base.gameObject);
				}
			}
		}
		SendFlurryEvent();
		UIScreenController.Instance.ClosePopup(null);
	}

	private void SendFlurryEvent()
	{
	}
}
