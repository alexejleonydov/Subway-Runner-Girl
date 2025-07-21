using System;
using UnityEngine;

public class ShopManager
{
	private static ShopManager _instance;

	private TimeCoolDown _freeCoolDown;

	private ShopPopupData _popupData;

	public static ShopManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ShopManager();
			}
			return _instance;
		}
	}

	public int barIndex { get; set; }

	public ChestType chestType { get; set; }

	private ShopManager()
	{
		_freeCoolDown = new TimeCoolDown("FreeChestCoolingDown", 10800);
	}

	public bool IsCoolingDownOver()
	{
		return _freeCoolDown.IsCoolingDownOver();
	}

	public string GetCoolingDownTime()
	{
		return _freeCoolDown.GetCoolingDownTime3();
	}

	public void SetNewTime()
	{
		_freeCoolDown.SetFreeTime();
	}

	public void ForceCoolingDownOver()
	{
		_freeCoolDown.ForceCoolingDownOver();
	}

	public void SetShopType(LanguageKey title, string icon, int amount, int unlockType, string price, Action callback = null)
	{
		_popupData = new ShopPopupData();
		_popupData.title = title;
		_popupData.num = amount;
		_popupData.icon = icon;
		_popupData.unlockType = unlockType;
		_popupData.price = price;
		_popupData.buyCallback = callback;
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.PushPopup("BuyPopup");
		}
	}

	public ShopPopupData GetShopPopupData()
	{
		return _popupData;
	}
}
