using UnityEngine;

[AddComponentMenu("GUI/Interaction/Change Screen Button")]
public class UIButtonChangeScreen : UIBasicButton
{
	public enum ScreenChangeType
	{
		PushScreen = 0,
		SwitchScreen = 1,
		None = 2,
		QueuePopup = 3,
		ClosePopup = 4,
		PushPopup = 5
	}

	public ScreenChangeType screenChangeType;

	public string ScreenNameToOpen;

	private bool useSend = true;

	protected override void Send()
	{
		if (!useSend)
		{
			return;
		}
		if (ScreenNameToOpen == "PauseUI")
		{
			Game.Instance.wasButtonClicked = true;
		}
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (string.IsNullOrEmpty(ScreenNameToOpen) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
		{
			Debug.LogError(base.name + " tried to send an empty Change Screen message");
		}
		UIScreenController instance = UIScreenController.Instance;
		if (!(instance == null))
		{
			if (screenChangeType == ScreenChangeType.PushScreen)
			{
				instance.PushScreen(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.SwitchScreen)
			{
				instance.SwitchScreen(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.QueuePopup)
			{
				instance.QueuePopup(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.PushPopup)
			{
				instance.PushPopup(ScreenNameToOpen);
			}
			if (screenChangeType == ScreenChangeType.ClosePopup)
			{
				instance.ClosePopup(ScreenNameToOpen);
			}
		}
	}
}
