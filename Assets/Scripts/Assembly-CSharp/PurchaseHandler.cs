using System;
using UnityEngine;

public class PurchaseHandler
{
	private static PurchaseHandler _instance;

	private Action _onUpgradePurchase;

	public static PurchaseHandler Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new PurchaseHandler();
			}
			return _instance;
		}
	}

	public void AddOnUpgradePurchase(Action handler)
	{
		_onUpgradePurchase = (Action)Delegate.Combine(_onUpgradePurchase, handler);
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, int themeIndex, bool isPopup, Action buySuccessCallBack = null)
	{
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		Characters.Model model = Characters.characterData[characterType];
		CharacterTheme themeForCharacter = CharacterThemes.GetThemeForCharacter(characterType, themeIndex);
		bool flag = themeForCharacter != null;
		Characters.UnlockType unlockType = model.unlockType;
		int price = model.Price;
		if (flag)
		{
			CharacterTheme characterTheme = themeForCharacter;
			unlockType = characterTheme.unlockType;
			price = characterTheme.price;
		}
		if (unlockType != Characters.UnlockType.coins && unlockType != Characters.UnlockType.keys)
		{
			Debug.LogWarning("Cannot buy character with unlocktype: " + model.unlockType);
			instance.CharacterPurchaseFailure();
			return;
		}
		int num = PlayerInfo.Instance.amountOfCoins;
		InAppData.DataType type = InAppData.DataType.Coin;
		if (unlockType == Characters.UnlockType.keys)
		{
			num = PlayerInfo.Instance.amountOfKeys;
			type = InAppData.DataType.Key;
		}
		if (num < price)
		{
			InAppManager.instance.SetupNativePopup(price, isPopup, type);
			instance.CharacterPurchaseFailure();
			return;
		}
		if (buySuccessCallBack != null)
		{
			buySuccessCallBack();
		}
		switch (unlockType)
		{
		case Characters.UnlockType.coins:
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_roles", 0, price);
			Characters.Model model2 = Characters.characterData[characterType];
			if (model2.taskTargetKey != 0)
			{
				Characters.Model model3 = Characters.characterData[characterType];
				TasksManager.Instance.PlayerDidThis(model3.taskTargetKey);
			}
			break;
		}
		case Characters.UnlockType.keys:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price);
			PlayerInfo.Instance.amountOfKeys -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_buy_roles", 0, price);
			break;
		}
		if (flag)
		{
			PlayerInfo.Instance.UnlockTheme(characterType, themeIndex);
		}
		else
		{
			PlayerInfo.Instance.CollectSymbol(characterType, price);
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters);
		}
		NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.CharacterCanUnlock);
		instance.CharacterPurchaseSuccessful(characterType, themeIndex);
	}

	public void PurchaseHelmetTheme(Helmets.HelmType helmetType, IPurchaseHandler handler)
	{
		Helmets.Helm helm = Helmets.helmData[helmetType];
		Helmets.UnlockType unlockType = helm.unlockType;
		int price = helm.price;
		if (unlockType != Helmets.UnlockType.coins && unlockType != Helmets.UnlockType.keys)
		{
			Debug.LogWarning("Cannot buy helmet with unlocktype: " + helm.unlockType);
			handler.PurchaseFailure();
			return;
		}
		int num = PlayerInfo.Instance.amountOfCoins;
		InAppData.DataType type = InAppData.DataType.Coin;
		if (unlockType == Helmets.UnlockType.keys)
		{
			num = PlayerInfo.Instance.amountOfKeys;
			type = InAppData.DataType.Key;
		}
		if (num < price)
		{
			InAppManager.instance.SetupNativePopup(price, false, type);
			handler.PurchaseFailure();
			return;
		}
		switch (unlockType)
		{
		case Helmets.UnlockType.coins:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_hoverboards", 0, price);
			break;
		case Helmets.UnlockType.keys:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price);
			PlayerInfo.Instance.amountOfKeys -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_buy_hoverboards", 0, price);
			break;
		}
		PlayerInfo.Instance.UnlockHelmet(helmetType);
		handler.PurchaseSuccessful();
	}

	public void PurchaseHelmet(int number, IPurchaseHandler handler)
	{
		int num = Upgrades.upgrades[PropType.helmet].pricesRaw[0];
		int amountOfCoins = PlayerInfo.Instance.amountOfCoins;
		InAppData.DataType type = InAppData.DataType.Coin;
		if (amountOfCoins < number * num)
		{
			handler.PurchaseFailure();
			InAppManager.instance.SetupNativePopup(number * num, true, type);
			return;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, number * num);
		PlayerInfo.Instance.amountOfCoins -= number * num;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, number * num);
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_props", 0, number * num);
		handler.PurchaseSuccessful();
	}

	public void PurchaseHelmet(Helmets.HelmType helmetType, IPurchaseHandler handler)
	{
		Helmets.Helm helm = Helmets.helmData[helmetType];
		Helmets.UnlockType unlockType = helm.unlockType;
		int price = helm.price;
		switch (unlockType)
		{
		case Helmets.UnlockType.coins:
		{
			int amountOfCoins = PlayerInfo.Instance.amountOfCoins;
			if (amountOfCoins < price)
			{
				InAppManager.instance.SetupNativePopup(price, true, InAppData.DataType.Coin);
				handler.PurchaseFailure();
				return;
			}
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_hoverboards", 0, price);
			break;
		}
		case Helmets.UnlockType.keys:
		{
			int amountOfKeys = PlayerInfo.Instance.amountOfKeys;
			if (amountOfKeys < price)
			{
				InAppManager.instance.SetupNativePopup(price, true, InAppData.DataType.Key);
				handler.PurchaseFailure();
				return;
			}
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, price);
			PlayerInfo.Instance.amountOfKeys -= price;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_buy_hoverboards", 0, price);
			break;
		}
		}
		PlayerInfo.Instance.UnlockHelmet(helmetType);
		handler.PurchaseSuccessful();
	}

	public void PurchaseKeysIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SetupNativePopup(amountToSaveMe, true, InAppData.DataType.Key);
	}

	public void PurchaseCoinsIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SetupNativePopup(amountToSaveMe, true, InAppData.DataType.Coin);
	}

	public void PurchaseUpgrade(PropType type, bool isPopup, IPurchaseHandler sender)
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		int num = ((upgrade.numberOfTiers != 0) ? Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1) : Upgrades.upgrades[type].getPrice(0));
		if (PlayerInfo.Instance.amountOfCoins < num)
		{
			InAppManager.instance.SetupNativePopup(num, isPopup, InAppData.DataType.Coin);
			sender.PurchaseFailure();
			return;
		}
		switch (type)
		{
		case PropType.helmet:
		case PropType.headstart500:
		case PropType.headstart2000:
		case PropType.scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(type);
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num);
			if (type == PropType.headstart2000)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveHeadStartLarge);
			}
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_props", 0, num);
			break;
		case PropType.chest:
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.Chest);
			UIScreenController.Instance.QueueChest();
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.BuyMysterybox);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_buy_props", 0, num);
			break;
		case PropType.flypack:
		case PropType.supershoes:
		case PropType.coinmagnet:
		case PropType.letters:
		case PropType.doubleMultiplier:
			PlayerInfo.Instance.IncreasePowerupTier(type);
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveUpgrades);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_upgrade_props", 0, num);
			break;
		case PropType.skiptask1:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num, 0);
			break;
		case PropType.skiptask2:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num, 1);
			break;
		case PropType.skiptask3:
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, num, 2);
			break;
		}
		PlayerInfo.Instance.amountOfCoins -= num;
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, num);
		sender.PurchaseSuccessful();
		if (_onUpgradePurchase != null)
		{
			_onUpgradePurchase();
		}
	}

	public void PurchaseUpgradeFree(PropType type)
	{
		switch (type)
		{
		case PropType.helmet:
		case PropType.headstart500:
		case PropType.headstart2000:
		case PropType.scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(type);
			if (type == PropType.headstart2000)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.HaveHeadStartLarge);
			}
			break;
		case PropType.chest:
			TasksManager.Instance.PlayerDidThis(TaskTarget.BuyMysterybox);
			break;
		case PropType.flypack:
		case PropType.supershoes:
		case PropType.coinmagnet:
		case PropType.letters:
		case PropType.doubleMultiplier:
			PlayerInfo.Instance.IncreasePowerupTier(type);
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveUpgrades);
			break;
		}
		if (_onUpgradePurchase != null)
		{
			_onUpgradePurchase();
		}
	}

	public void RemoveOnUpgradePurchase(Action handler)
	{
		if (_onUpgradePurchase != null)
		{
			_onUpgradePurchase = (Action)Delegate.Remove(_onUpgradePurchase, handler);
		}
	}

	public void PurchaseChest(ChestType chestType, IPurchaseHandler sender)
	{
		Chest chest = ChestsData.GetChest(chestType);
		if (chest == null)
		{
			sender.PurchaseFailure();
			return;
		}
		if (chest.unlockType == UnlockType.coin)
		{
			if (PlayerInfo.Instance.amountOfCoins < chest.price)
			{
				InAppManager.instance.SetupNativePopup(chest.price, true, InAppData.DataType.Coin);
				sender.PurchaseFailure();
				return;
			}
			PlayerInfo.Instance.amountOfCoins -= chest.price;
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendCoin, chest.price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_total", 0, chest.price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_coins_shop_buy_chest", 0, chest.price);
		}
		else if (chest.unlockType == UnlockType.key)
		{
			if (PlayerInfo.Instance.amountOfKeys < chest.price)
			{
				InAppManager.instance.SetupNativePopup(chest.price, true, InAppData.DataType.Key);
				sender.PurchaseFailure();
				return;
			}
			PlayerInfo.Instance.amountOfKeys -= chest.price;
			TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, chest.price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_total", 0, chest.price);
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "spend_gems_shop_buy_chest", 0, chest.price);
		}
		sender.PurchaseSuccessful();
	}
}
