using UnityEngine;

[ExecuteInEditMode]
public class UISpriteShadowHelper : UIShadowHelper<UISprite>
{
	public Color frontColor = Color.white;

	public Color shadowColor = Color.magenta;

	protected override void withUpdate()
	{
		if (_front.color != frontColor)
		{
			_front.color = frontColor;
		}
		if (shadow.color != shadowColor)
		{
			shadow.color = shadowColor;
		}
		if (shadow.spriteName != _front.spriteName)
		{
			shadow.spriteName = _front.spriteName;
		}
	}
}
