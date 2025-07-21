using UnityEngine;

public class CelebrationPopupLabelTemplate : MonoBehaviour
{
	public UILabel bigLabel;

	public UILabel subLabel;

	[SerializeField]
	private GameObject normalStateGO;

	public float Alpha
	{
		get
		{
			return bigLabel.alpha;
		}
		set
		{
			bigLabel.alpha = Mathf.Clamp01(value);
		}
	}

	private string _GetCoinsLabel(int amount)
	{
		return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COINS_AMOUNT), string.Format("{0:#,###0}", amount));
	}

	private string _GetKeysLabel(int amount)
	{
		if (amount > 1)
		{
			return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_KEYS_AMOUNT), amount.ToString());
		}
		return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_KEY_AMOUNT), amount.ToString());
	}

	private string _GetPowerupLabel(PropType type, int amount)
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		return string.Empty + amount + Strings.Get(LanguageKey.UI_CELEBRATION_GE) + Strings.Get(upgrade.GetName());
	}

	public void Init(int backgroundDepth)
	{
		base.gameObject.GetComponent<UIPanel>().depth = backgroundDepth + 1;
	}

	private void OnEnable()
	{
		normalStateGO.SetActive(false);
	}

	public void SetDoubleCions(int amount)
	{
		normalStateGO.SetActive(true);
		bigLabel.text = _GetCoinsLabel(amount);
		subLabel.text = string.Empty;
	}

	public void SetupDoubleKeys(int amount)
	{
		normalStateGO.SetActive(true);
		bigLabel.text = _GetKeysLabel(amount);
		subLabel.text = string.Empty;
	}

	public void SetDoublePowerup(PropType powerup, int amount)
	{
		normalStateGO.SetActive(true);
		bigLabel.text = _GetPowerupLabel(powerup, amount);
		Upgrade upgrade = Upgrades.upgrades[powerup];
		subLabel.text = Strings.Get(upgrade.mysteryBoxDescription);
		ResetSubLabelPosition();
	}

	public void SetupDoubleSymbol(Characters.CharacterType characterType, int amount)
	{
		normalStateGO.SetActive(true);
		Characters.Model model = Characters.characterData[characterType];
		bigLabel.text = amount * 2 + Strings.Get(LanguageKey.UI_CELEBRATION_GE) + Strings.Get(model.symbolName);
		int num = model.Price - PlayerInfo.Instance.GetCollectedSymbols(characterType) - amount;
		if (num > 0)
		{
			subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), num, Strings.Get(model.name));
		}
		else
		{
			subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
		}
	}

	public void SetupCharacter(string charName)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_CHARACTER_UNLOCK), charName);
		subLabel.text = string.Empty;
	}

	public void SetupCoins()
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Empty;
		subLabel.text = string.Empty;
	}

	public void SetupEventSpecialHelm(string helmName)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_EVENT_BOARD_TRYOUT), helmName);
		subLabel.text = string.Empty;
	}

	public void SetupKeys(int amount)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = _GetKeysLabel(amount);
		subLabel.text = string.Empty;
	}

	public void SetupPowerup(PropType powerup, int amount)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = _GetPowerupLabel(powerup, amount);
		Upgrade upgrade = Upgrades.upgrades[powerup];
		subLabel.text = Strings.Get(upgrade.mysteryBoxDescription);
		ResetSubLabelPosition();
	}

	public void SetupSymbol(Characters.CharacterType characterType, int amount)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		Characters.Model model = Characters.characterData[characterType];
		bigLabel.text = amount + Strings.Get(LanguageKey.UI_CELEBRATION_GE) + Strings.Get(model.symbolName);
		int num = model.Price - PlayerInfo.Instance.GetCollectedSymbols(characterType) - amount;
		if (num > 0)
		{
			subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), num, Strings.Get(model.name));
		}
		else
		{
			subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
		}
	}

	public void UpdateCoins(int amount)
	{
		normalStateGO.SetActive(true);
		bigLabel.text = _GetCoinsLabel(amount);
		subLabel.text = string.Empty;
		ResetSubLabelPosition();
	}

	private void ResetSubLabelPosition()
	{
		subLabel.transform.localPosition = bigLabel.transform.localPosition - new Vector3(0f, (float)bigLabel.height * 0.5f + (float)subLabel.height * 0.5f + 5f, 0f);
	}
}
