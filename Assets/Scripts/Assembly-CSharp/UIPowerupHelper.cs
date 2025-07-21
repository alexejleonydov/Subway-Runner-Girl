using UnityEngine;

public class UIPowerupHelper : MonoBehaviour
{
	private enum FadeTarget
	{
		white = 0,
		green = 1,
		darkgreen = 2,
		none = 3
	}

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite sliderBG;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite iconBG;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private UIAnchor anchor;

	private IngameScreen _ingameScreen;

	private ActiveProp _powerup;

	private Color32[] colorTargets = new Color32[3]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(149, 192, 34, byte.MaxValue),
		new Color32(84, 118, 64, byte.MaxValue)
	};

	private FadeTarget currentFadeTarget = FadeTarget.none;

	private bool inverse;

	private float lerpTime = 1f;

	private FadeTarget oldFadeTarget = FadeTarget.none;

	private Color originalColor;

	private float sliderSteps;

	private Color storedColor;

	public UIAnchor Anchor
	{
		get
		{
			return anchor;
		}
	}

	private IngameScreen ingameScreen
	{
		get
		{
			if (_ingameScreen == null)
			{
				_ingameScreen = Object.FindObjectOfType(typeof(IngameScreen)) as IngameScreen;
			}
			return _ingameScreen;
		}
	}

	private void Awake()
	{
		originalColor = ingameScreen.MULTIPLIER_LABEL_ORIGINAL_COLOR;
	}

	private void FadingToWhiteToGreenToDarkGreen(bool sendInverse)
	{
		if (sendInverse)
		{
			if (lerpTime <= 1f)
			{
				currentFadeTarget = FadeTarget.green;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime);
			}
			else if (lerpTime <= 2f)
			{
				currentFadeTarget = FadeTarget.white;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime - 1f);
			}
			else
			{
				currentFadeTarget = FadeTarget.none;
				inverse = false;
				lerpTime = 0f;
			}
		}
		else if (lerpTime <= 1f)
		{
			currentFadeTarget = FadeTarget.green;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime);
		}
		else if (lerpTime <= 2f)
		{
			currentFadeTarget = FadeTarget.darkgreen;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, colorTargets[(int)currentFadeTarget], lerpTime - 1f);
		}
		else
		{
			currentFadeTarget = FadeTarget.none;
			inverse = true;
			lerpTime = 0f;
		}
		lerpTime += Time.deltaTime * 8.2343f;
		oldFadeTarget = currentFadeTarget;
	}

	public float getSliderHeight()
	{
		return iconBG.height;
	}

	public int getSliderWidth()
	{
		return sliderBG.width + iconBG.width / 2;
	}

	public int getHalfIconBGWidth()
	{
		return iconBG.width / 2;
	}

	public void HidePowerupSlot()
	{
		if (_powerup != null && _powerup.type == PropType.doubleMultiplier)
		{
			ReturnToNormal();
		}
	}

	private void OnDisable()
	{
		ReturnToNormal();
	}

	private void OnEnable()
	{
		sliderSteps = (int)slider.foregroundWidget.localSize.x;
	}

	private void ReturnToNormal()
	{
		lerpTime = 1f;
		inverse = false;
		if (GameStats.Instance.scoreBooster5Activated)
		{
			ingameScreen.multiplierLabel.color = ingameScreen.SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (ingameScreen.multiplierLabel.color != originalColor)
		{
			ingameScreen.multiplierLabel.color = originalColor;
		}
	}

	public void setContainerPosition(float x, float y, float z)
	{
		container.transform.localPosition = new Vector3(x, y, z);
	}

	public void SetPowerupSlot(ActiveProp powerup)
	{
		_powerup = powerup;
		Upgrade upgrade = Upgrades.upgrades[powerup.type];
		icon.spriteName = upgrade.iconName;
		float num = powerup.timeLeft / PlayerInfo.Instance.GetPowerupDuration(powerup.type);
		if (sliderSteps != 0f)
		{
			slider.value = num * sliderSteps / sliderSteps;
		}
		else
		{
			sliderSteps = slider.foregroundWidget.localSize.x;
		}
		if (powerup.type == PropType.doubleMultiplier)
		{
			FadingToWhiteToGreenToDarkGreen(inverse);
		}
		if (powerup.timeLeft < 0f)
		{
			if (slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, false);
			}
			icon.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * 3.141593f * 4f));
			iconBG.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * 3.141593f * 4f));
		}
		else
		{
			if (!slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, true);
			}
			icon.color = Color.white;
			iconBG.color = Color.white;
		}
	}
}
