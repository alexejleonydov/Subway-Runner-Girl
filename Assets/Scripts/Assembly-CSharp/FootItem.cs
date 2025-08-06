using System;

[Serializable]
public class FootItem
{
	public UISprite fill;

	public UISprite arrow;

	public UISprite fillIcon;

	private bool isFilled = true;

	public void SetFill(bool isfill)
	{
		if (isFilled != isfill)
		{
			isFilled = isfill;
			fill.enabled = isfill;
			arrow.enabled = isfill;
			fillIcon.enabled = isfill;
		}
	}

	public UIButtonChangeScreen GetParentBuuton()
    {
		return fill.transform.parent.gameObject.GetComponent<UIButtonChangeScreen>();
    }
}
