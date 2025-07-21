using UnityEngine;

public class UpgradeHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite powerupIcon;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel decriptionLbl;

	[SerializeField]
	private BuyButtonIngame button;

	[SerializeField]
	private UITierHelper tierHelper;

	private bool _hasInited;

	private PropType _type;

	public void InitPermanent(PropType type)
	{
		_type = type;
		Upgrade upgrade = Upgrades.upgrades[type];
		powerupIcon.spriteName = upgrade.iconName;
		titleLbl.text = Strings.Get(upgrade.name).ToUpper();
		decriptionLbl.text = Strings.Get(upgrade.description);
		if (tierHelper != null)
		{
			tierHelper.SetupTiers(type);
		}
		if (button != null)
		{
			button.initBuyButton(type);
		}
		_hasInited = true;
	}

	public void RefreshUpgrade(PropType type)
	{
		if (_hasInited)
		{
			if (tierHelper != null)
			{
				tierHelper.ResetTiers();
			}
			if (button != null)
			{
				button.Reload(_type != type);
			}
		}
	}
}
