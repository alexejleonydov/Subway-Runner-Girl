using UnityEngine;

public class UIButtonOverlayOff : MonoBehaviour
{
	public enum ButtonType
	{
		Custom = 0,
		Primary = 1,
		Tertiary = 2,
		Footer_shop = 3
	}

	public GameObject fillWithPrimeColor;

	public UISprite overlay;

	public ButtonType buttonType;

	private bool _buttonStateLocked;

	protected bool _initDone;

	private ButtonColorScheme buttonColorScheme;

	private float DURATION;

	private UIWidget fillSprite;

	private Color32 originalFillColor;

	private void Awake()
	{
		if (!_initDone)
		{
			Init();
		}
	}

	public void ButtonPressed(bool isPressed)
	{
		if (base.enabled)
		{
			if (overlay != null && buttonColorScheme.light.HasValue)
			{
				Color32 value = buttonColorScheme.light.Value;
				TweenColor.Begin(overlay.gameObject, DURATION, isPressed ? ((Color)new Color32(value.r, value.g, value.b, 0)) : ((Color)value));
			}
			if (isPressed)
			{
				byte r = (byte)Mathf.Clamp(originalFillColor.r - 30, 0, 255);
				byte g = (byte)Mathf.Clamp(originalFillColor.g - 30, 0, 255);
				byte b = (byte)Mathf.Clamp(originalFillColor.b - 30, 0, 255);
				TweenColor.Begin(fillSprite.gameObject, DURATION, new Color32(r, g, b, originalFillColor.a));
			}
			else
			{
				TweenColor.Begin(fillSprite.gameObject, DURATION, originalFillColor);
			}
		}
	}

	public void GoToNormalState()
	{
		if (!_buttonStateLocked)
		{
			if (GetComponent<BoxCollider>() != null)
			{
				GetComponent<BoxCollider>().enabled = true;
			}
			fillSprite.color = originalFillColor;
			if (buttonColorScheme.light.HasValue)
			{
			}
			if (overlay != null)
			{
				overlay.enabled = true;
			}
		}
	}

	public void GoToNotAvailableState()
	{
		if (!_buttonStateLocked)
		{
			if (GetComponent<BoxCollider>() != null)
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			if (buttonColorScheme.unavailable.HasValue)
			{
				fillSprite.color = buttonColorScheme.unavailable.Value;
			}
		}
	}

	public void GoToSelectedState()
	{
		if (!_buttonStateLocked)
		{
			if (GetComponent<BoxCollider>() != null)
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			if (buttonColorScheme.selected.HasValue)
			{
				fillSprite.color = buttonColorScheme.selected.Value;
			}
			else
			{
				Debug.LogWarning("No 'selected' color found for button type: " + buttonType, base.gameObject);
			}
		}
	}

	protected void Init()
	{
		_initDone = true;
		if (fillSprite == null)
		{
			fillSprite = fillWithPrimeColor.GetComponent<UIWidget>();
		}
		buttonColorScheme = GlobalColors.GetButtonColorScheme(buttonType, base.gameObject);
		if (buttonColorScheme.original.HasValue)
		{
			fillSprite.color = buttonColorScheme.original.Value;
		}
		originalFillColor = fillSprite.color;
		if (overlay != null)
		{
			overlay.enabled = true;
			if (buttonColorScheme.light.HasValue)
			{
				overlay.color = buttonColorScheme.light.Value;
			}
		}
	}

	public void LockState(bool locked)
	{
		_buttonStateLocked = locked;
	}

	private void OnDisable()
	{
		if (fillSprite.color != originalFillColor)
		{
			GoToNormalState();
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		ButtonPressed(isPressed);
	}

	public void ResetTweens()
	{
		if (base.enabled)
		{
			if (overlay != null && buttonColorScheme.light.HasValue)
			{
				TweenColor.Begin(overlay.gameObject, DURATION, buttonColorScheme.light.Value);
			}
			TweenColor.Begin(fillSprite.gameObject, DURATION, originalFillColor);
		}
	}

	public void SetButtonType(ButtonType type)
	{
		buttonType = type;
		Init();
		ButtonPressed(false);
	}
}
