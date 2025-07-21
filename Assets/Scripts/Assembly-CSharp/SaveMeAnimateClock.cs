using UnityEngine;

public class SaveMeAnimateClock : MonoBehaviour
{
	public UISprite clockSprite;

	public UILabel timeLabel;

	public void FillSpriteAmount(float amount)
	{
		if (clockSprite != null)
		{
			clockSprite.fillAmount = amount;
			timeLabel.text = ((int)(amount * 10f)).ToString();
		}
		else
		{
			Debug.Log("ClockSprite == NULL");
		}
	}
}
