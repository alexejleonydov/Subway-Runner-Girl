using System;
using UnityEngine;

public class SubscriptionPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel trialLbl;

	[SerializeField]
	private UILabel trialDescripeLbl;

	[SerializeField]
	private UILabel reminderLbl;

	[SerializeField]
	private UILabel subscribeInfoLbl;

	[SerializeField]
	private UILabel removeLbl;

	[SerializeField]
	private UILabel adsLbl;

	[SerializeField]
	private UILabel gemLbl;

	[SerializeField]
	private UILabel roleLbl;

	[SerializeField]
	private UILabel roleTypeLbl;

	[SerializeField]
	private UILabel doubleCoinsLbl;

	[SerializeField]
	private UILabel tipLbl;

	[SerializeField]
	private UILabel termsLbl;

	[SerializeField]
	private UILabel policyLbl;

	[SerializeField]
	private GameObject purchaseBtn;

	[SerializeField]
	private UIToggle toggle;

	[SerializeField]
	private UILabel btnNameLbl;

	private const string termsUrlFormat = "[url=https://sites.google.com/site/huskaimmcomtermsofuse/][u]{0}[/u][/url]";

	private const string policyUrlFormat = "[url=https://sites.google.com/site/riooprivacypolicy/][u]{0}[/u][/url]";

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(purchaseBtn);
		uIEventListener.onClick = Pay;
	}

	public override void Show()
	{
		base.Show();
		Refresh();
		RefreshLabel();
	}

	private void Refresh()
	{
		Debug.Log("Subscription: hasSubscribed" + PlayerInfo.Instance.hasSubscribed + " isSubscriptionActive:" + CheckSubscription.isSubscriptionActive);
		if (PlayerInfo.Instance.hasSubscribed)
		{
			purchaseBtn.GetComponent<TweenScale>().enabled = false;
			purchaseBtn.GetComponent<BoxCollider>().enabled = false;
			btnNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_PURCHASED);
			trialDescripeLbl.enabled = false;
		}
		else
		{
			purchaseBtn.GetComponent<TweenScale>().enabled = true;
			purchaseBtn.GetComponent<BoxCollider>().enabled = true;
			btnNameLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_TRIAL);
			trialDescripeLbl.enabled = true;
		}
		toggle.value = PlayerInfo.Instance.ignoreSubscriptionPopup;
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_TRIAL_LABEL);
		trialDescripeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_BUTTON_TRIAL_TIP);
		reminderLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_NO_REMINDER);
		removeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_REMOVE);
		adsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_ADS);
		gemLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_DISCOUNT_GEMS);
		roleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_EXCLUSIVE_ROLE);
		roleTypeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_MONK);
		doubleCoinsLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_DOUBLE_COINS);
		tipLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_SUBSCRIBE_INFO);
		termsLbl.text = string.Format("[url=https://sites.google.com/site/huskaimmcomtermsofuse/][u]{0}[/u][/url]", Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_URL_TERMS_OF_USE));
		policyLbl.text = string.Format("[url=https://sites.google.com/site/riooprivacypolicy/][u]{0}[/u][/url]", Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_URL_PRIVACY_POLICY));
	}

	public void Pay(GameObject go)
	{
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.Pay(13);
		}
	}

	public void OnToggleValue()
	{
		if (UIToggle.current.value)
		{
			PlayerInfo.Instance.ignoreSubscriptionNextTime = DateTime.UtcNow.AddDays(1.0);
			PlayerInfo.Instance.ignoreSubscriptionPopup = true;
		}
		else
		{
			PlayerInfo.Instance.ignoreSubscriptionPopup = false;
		}
	}
}
