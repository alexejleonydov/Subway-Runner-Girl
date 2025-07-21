using System;
using UnityEngine;

public class UIUpgradeButton : MonoBehaviour, IPurchaseHandler
{
	[SerializeField]
	private PropType type;

	[SerializeField]
	private UISprite powerupIcon;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel haveLbl;

	private bool _purchaseInProgress;

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(RefreshUpgrade));
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(RefreshUpgrade));
	}

	public void Init()
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		powerupIcon.spriteName = upgrade.iconName;
		titleLbl.text = Strings.Get(upgrade.name).ToUpper();
		priceLbl.text = string.Empty + upgrade.getPrice(0);
		haveLbl.text = Strings.Get(LanguageKey.UPGRADES_HELPER_YOU_HAVE) + " " + PlayerInfo.Instance.GetUpgradeAmount(type);
	}

	public void RefreshUpgrade()
	{
		haveLbl.text = Strings.Get(LanguageKey.UPGRADES_HELPER_YOU_HAVE) + " " + PlayerInfo.Instance.GetUpgradeAmount(type);
	}

	private void OnClick()
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		powerupIcon.spriteName = upgrade.iconName;
		ShopManager.Instance.SetShopType(upgrade.name, upgrade.iconName, 1, 1, string.Empty + upgrade.getPrice(0), Buy);
	}

	private void Buy()
	{
		_purchaseInProgress = true;
		PurchaseHandler.Instance.PurchaseUpgrade(type, true, this);
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		UIScreenController.Instance.ClosePopup(null);
	}
}
