using UnityEngine;

public class HelmetThemeButton : MonoBehaviour
{
	public delegate void OnPressEvent(int index);

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private GameObject selectedSprite;

	[SerializeField]
	private GameObject ownedSprite;

	private int _index;

	public int ThemeIndex
	{
		get
		{
			return _index;
		}
	}

	public event OnPressEvent OnPress;

	private void ButtonPressed()
	{
		OnPressEvent onPress = this.OnPress;
		if (onPress != null)
		{
			onPress(_index);
		}
	}

	public void InitButton(string iconName, Color32 bgColor, Color32 iconColor, int index)
	{
		if (!string.IsNullOrEmpty(iconName))
		{
			_icon.spriteName = iconName;
		}
		_icon.color = iconColor;
		_background.color = bgColor;
		_index = index;
	}

	public void SetButtonState(int newState)
	{
		switch (newState)
		{
		case 0:
			selectedSprite.SetActive(false);
			ownedSprite.SetActive(false);
			break;
		case 1:
			selectedSprite.SetActive(false);
			ownedSprite.SetActive(true);
			break;
		case 2:
			selectedSprite.SetActive(true);
			ownedSprite.SetActive(false);
			break;
		}
	}
}
