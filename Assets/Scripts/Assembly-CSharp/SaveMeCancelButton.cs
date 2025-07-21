using UnityEngine;

public class SaveMeCancelButton : MonoBehaviour
{
	public SaveMePopup saveMePopup;

	public static void CloseSaveMe()
	{
		UIScreenController.Instance.ClosePopup(null);
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
	}

	private void OnClick()
	{
		if (saveMePopup != null && saveMePopup.getAnimationTimeLeft() < saveMePopup.getAnimationDuration() - 1f)
		{
			CloseSaveMe();
		}
	}
}
