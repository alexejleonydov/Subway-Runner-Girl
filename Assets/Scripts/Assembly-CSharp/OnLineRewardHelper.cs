using UnityEngine;

public class OnLineRewardHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel number;

	public void SetIcomAndNumber(string icon, int number)
	{
		this.icon.spriteName = icon;
		this.number.text = number.ToString();
	}
}
