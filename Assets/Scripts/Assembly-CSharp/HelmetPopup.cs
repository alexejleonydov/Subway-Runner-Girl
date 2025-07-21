using System;
using UnityEngine;

public class HelmetPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel helmetAmountLabel;

	[SerializeField]
	private UILabel helmetDescription1;

	[SerializeField]
	private UILabel helmetDescription2;

	[SerializeField]
	private UILabel adLbl;

	[SerializeField]
	private UILabel coinsLabel;

	[SerializeField]
	private UILabel gemsLabel;

	[SerializeField]
	private UILabel getLbl;

	public override void Hide()
	{
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom");
		}
		base.Hide();
	}

	public override void Show()
	{
		base.Show();
		UpdateCoinsUI();
		UpdateGemsUI();
		UpdateLabels();
		RiseSdk.Instance.enableBackHomeAd(false, "custom");
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(UpdateCoinsUI));
		instance.onKeysChanged = (Action)Delegate.Combine(instance.onKeysChanged, new Action(UpdateGemsUI));
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		popupTitle.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP);
		helmetDescription1.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP_DESCRIPTION_1);
		helmetDescription2.text = Strings.Get(LanguageKey.HOVERBOARD_POPUP_DESCRIPTION_2);
		getLbl.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_GET) + " 3";
		adLbl.text = Strings.Get(LanguageKey.UI_POPUP_HELMET_AD_GET_TIP);
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.onCoinsChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onCoinsChanged, new Action(UpdateCoinsUI));
		PlayerInfo.Instance.onKeysChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onKeysChanged, new Action(UpdateGemsUI));
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void UpdateCoinsUI()
	{
		coinsLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
	}

	public void UpdateGemsUI()
	{
		gemsLabel.text = PlayerInfo.Instance.amountOfKeys.ToString();
	}

	private void UpdateLabels()
	{
		helmetAmountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet).ToString();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
		}
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		RiseSdk.Instance.CloseNativeAd("loading");
	}
}
