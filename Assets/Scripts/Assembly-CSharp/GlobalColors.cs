using System.Collections.Generic;
using UnityEngine;

public class GlobalColors
{
	public static readonly Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme> buttonColorData;

	public static readonly Color32 PRIMARY_ACTION_COLOR;

	private static readonly Color32 PRIMARY_ACTION_COLOR_LIGHT;

	private static readonly Color32 PRIMARY_ACTION_COLOR_SELECTED;

	public static readonly Color32 PRIMARY_ACTION_COLOR_UNAVAILABLE;

	public static readonly Color32 TERTIARY_ACTION_COLOR;

	private static readonly Color32 TERTIARY_ACTION_COLOR_LIGHT;

	private static readonly Color32 TERTIARY_ACTION_COLOR_SELECTED;

	public static readonly Color32 TERTIARY_ACTION_COLOR_UNAVAILABLE;

	public static readonly Color32 FOOTER_ACTION_COLOR;

	private static readonly Color32 FOOTER_ACTION_COLOR_LIGHT;

	private static readonly Color32 FOOTER_ACTION_COLOR_SELECTED;

	public static readonly Color32 FOOTER_ACTION_COLOR_UNAVAILABLE;

	static GlobalColors()
	{
		PRIMARY_ACTION_COLOR = new Color32(70, 157, 43, byte.MaxValue);
		PRIMARY_ACTION_COLOR_LIGHT = new Color32(139, 194, 62, byte.MaxValue);
		PRIMARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);
		PRIMARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);
		TERTIARY_ACTION_COLOR = new Color32(54, 137, 190, byte.MaxValue);
		TERTIARY_ACTION_COLOR_LIGHT = new Color32(183, 227, 245, 112);
		TERTIARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);
		TERTIARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);
		FOOTER_ACTION_COLOR = new Color32(239, 239, 239, byte.MaxValue);
		FOOTER_ACTION_COLOR_LIGHT = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		FOOTER_ACTION_COLOR_SELECTED = new Color32(byte.MaxValue, 174, 0, byte.MaxValue);
		FOOTER_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);
		buttonColorData = new Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme>
		{
			{
				UIButtonOverlayOff.ButtonType.Custom,
				default(ButtonColorScheme)
			},
			{
				UIButtonOverlayOff.ButtonType.Primary,
				new ButtonColorScheme
				{
					light = PRIMARY_ACTION_COLOR_LIGHT,
					selected = PRIMARY_ACTION_COLOR_SELECTED,
					unavailable = PRIMARY_ACTION_COLOR_UNAVAILABLE,
					original = PRIMARY_ACTION_COLOR
				}
			},
			{
				UIButtonOverlayOff.ButtonType.Tertiary,
				new ButtonColorScheme
				{
					light = TERTIARY_ACTION_COLOR_LIGHT,
					selected = TERTIARY_ACTION_COLOR_SELECTED,
					unavailable = TERTIARY_ACTION_COLOR_UNAVAILABLE,
					original = TERTIARY_ACTION_COLOR
				}
			},
			{
				UIButtonOverlayOff.ButtonType.Footer_shop,
				new ButtonColorScheme
				{
					light = FOOTER_ACTION_COLOR_LIGHT,
					selected = FOOTER_ACTION_COLOR_SELECTED,
					unavailable = FOOTER_ACTION_COLOR_UNAVAILABLE,
					original = FOOTER_ACTION_COLOR
				}
			}
		};
	}

	public static ButtonColorScheme GetButtonColorScheme(UIButtonOverlayOff.ButtonType buttonType, GameObject gameObject)
	{
		if (buttonColorData.ContainsKey(buttonType))
		{
			return buttonColorData[buttonType];
		}
		Debug.LogError("No color scheme for button type: " + buttonType.ToString() + ". GameObject: " + gameObject.name);
		return buttonColorData[UIButtonOverlayOff.ButtonType.Custom];
	}
}
