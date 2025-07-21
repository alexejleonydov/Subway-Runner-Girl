using System;
using UnityEngine;

[Serializable]
public class PrizeEntryTemplate
{
	public LanguageKey description;

	public string bg_small;

	public string bg_large;

	public string icon;

	public bool isSymbol;

	public Characters.CharacterType characterType;

	public bool isProp;

	public PropType propType;

	public bool isExp;

	public int ExpPoint;

	public bool UseSlider()
	{
		return isSymbol;
	}

	public int Total()
	{
		if (isSymbol)
		{
			return Characters.characterData[characterType].Price;
		}
		return -1;
	}

	public int Have()
	{
		if (isSymbol)
		{
			return PlayerInfo.Instance.GetCollectedSymbols(characterType);
		}
		return -1;
	}

	public void PayoutReward(int amount)
	{
		if (isSymbol)
		{
			PlayerInfo.Instance.CollectSymbol(characterType, amount);
		}
		else if (isProp)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(propType, amount);
		}
		else if (isExp)
		{
			PlayerInfo.Instance.amountOfExp += amount * ExpPoint;
			TasksManager.Instance.CheckPlayerLevel();
		}
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public PrizeEntryTemplate Parse(string json)
	{
		JsonUtility.FromJsonOverwrite(json, this);
		return this;
	}
}
