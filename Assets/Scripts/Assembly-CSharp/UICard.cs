using UnityEngine;

public class UICard : MonoBehaviour
{
	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UISprite icon;

	public void Set(string bg, string icon)
	{
		this.bg.spriteName = bg;
		this.icon.spriteName = icon;
	}
}
