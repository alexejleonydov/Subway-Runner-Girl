using UnityEngine;

[ExecuteInEditMode]
public class UITextShadowHelper : UIShadowHelper<UILabel>
{
	public Color frontColor = Color.white;

	public Color shadowColor = Color.magenta;

	protected override void withUpdate()
	{
		if (_front.color != frontColor)
		{
			_front.color = frontColor;
		}
		if (_front.effectColor != shadowColor)
		{
			_front.effectColor = shadowColor;
		}
		if (shadow.text != _front.text)
		{
			shadow.text = _front.text;
		}
		if (shadow.color != shadowColor)
		{
			shadow.color = shadowColor;
		}
	}
}
