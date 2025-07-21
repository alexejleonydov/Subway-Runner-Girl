using UnityEngine;

public class UISlideInErrorMessage : UISlideIn
{
	[SerializeField]
	private UILabel messageLabel;

	public void SetupErrorMessage(string message)
	{
		base.gameObject.SetActive(true);
		messageLabel.text = message;
		SlideIn(null);
	}
}
