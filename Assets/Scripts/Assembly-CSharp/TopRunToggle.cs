using UnityEngine;

public class TopRunToggle : MonoBehaviour
{
	[SerializeField]
	private UISprite on_bg;

	[SerializeField]
	private UISprite on_icon;

	[SerializeField]
	private UILabel titleLbl;

	private Color titleLblColor;

	private void Awake()
	{
		titleLblColor = titleLbl.color;
	}

	public void Toggle(bool value)
	{
		if (value)
		{
			on_bg.enabled = true;
			on_icon.enabled = true;
			titleLbl.color = titleLblColor;
		}
		else
		{
			on_bg.enabled = false;
			on_icon.enabled = false;
			titleLbl.color = Color.white;
		}
	}
}
