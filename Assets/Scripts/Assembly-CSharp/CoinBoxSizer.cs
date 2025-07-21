using System;
using Network;
using UnityEngine;

public class CoinBoxSizer : MonoBehaviour
{
	[SerializeField]
	private GameObject _coinsParent;

	[SerializeField]
	private GameObject _keysParent;

	[SerializeField]
	private GameObject _levelParent;

	[SerializeField]
	private GameObject _closeParent;

	[SerializeField]
	private UISprite coinAdd;

	[SerializeField]
	private UISprite keyAdd;

	[SerializeField]
	private BoxCollider levelCollider;

	[SerializeField]
	private UILabel coinAmountLabel;

	[SerializeField]
	private UILabel keyAmountLabel;

	[SerializeField]
	private UILabel levelAmountLabel;

	[SerializeField]
	private UILabel expAmountLabel;

	[SerializeField]
	private UISlider expSlider;

	[SerializeField]
	private UITexture headTexture;

	private int _originCoinAmount;

	private int _originKeyAmount;

	private int _originExpAmount;

	private int _originLevelAmount;

	private float _depthOffset_default = -1f;

	private bool _updateAutomatically = true;

	private bool isInitialized;

	private UIAnchor parentAnchor;

	public bool updateAutomatically
	{
		get
		{
			return _updateAutomatically;
		}
		set
		{
			_updateAutomatically = value;
			if (value)
			{
				OnCoinsChanged();
				OnKeysChanged();
				OnExpChanged();
				OnLevelChanged();
			}
		}
	}

	private void EnableElements(bool fundsEnabled, bool coinsEnabled, bool keysEnabled)
	{
		_closeParent.SetActive(fundsEnabled);
		_coinsParent.SetActive(coinsEnabled);
		_keysParent.SetActive(keysEnabled);
		coinAdd.enabled = fundsEnabled;
		keyAdd.enabled = fundsEnabled;
		levelCollider.enabled = fundsEnabled;
	}

	public void Init(bool fundsEnabled, bool coinsEnabled, bool keysEnabled, bool updateAutomatically)
	{
		EnableElements(fundsEnabled, coinsEnabled, keysEnabled);
		_updateAutomatically = updateAutomatically;
		parentAnchor = base.gameObject.transform.parent.GetComponent<UIAnchor>();
		parentAnchor.transform.localPosition = new Vector3(parentAnchor.transform.localPosition.x, parentAnchor.transform.localPosition.y, _depthOffset_default);
		if (!isInitialized)
		{
			Refresh();
			isInitialized = true;
		}
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Remove(instance.onCoinsChanged, new Action(OnCoinsChanged));
		instance.onKeysChanged = (Action)Delegate.Remove(instance.onKeysChanged, new Action(OnKeysChanged));
		instance.onLevelChanged = (Action)Delegate.Remove(instance.onLevelChanged, new Action(OnLevelChanged));
		instance.onExpChanged = (Action)Delegate.Remove(instance.onExpChanged, new Action(OnExpChanged));
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(OnCoinsChanged));
		instance.onKeysChanged = (Action)Delegate.Combine(instance.onKeysChanged, new Action(OnKeysChanged));
		instance.onLevelChanged = (Action)Delegate.Combine(instance.onLevelChanged, new Action(OnLevelChanged));
		instance.onExpChanged = (Action)Delegate.Combine(instance.onExpChanged, new Action(OnExpChanged));
		RefreshHeadUI();
		if (isInitialized)
		{
			Refresh();
		}
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}

	private void Refresh()
	{
		_originCoinAmount = PlayerInfo.Instance.amountOfCoins;
		_originKeyAmount = PlayerInfo.Instance.amountOfKeys;
		_originExpAmount = PlayerInfo.Instance.amountOfExp;
		_originLevelAmount = PlayerInfo.Instance.amountOfLevel;
		RefreshCoins(_originCoinAmount);
		RefreshKeys(_originKeyAmount);
		RefreshLevel(_originLevelAmount);
		RefreshExp(_originLevelAmount, _originExpAmount);
	}

	private void RefreshCoins(int amount)
	{
		coinAmountLabel.text = amount.ToString();
	}

	private void RefreshKeys(int amount)
	{
		keyAmountLabel.text = amount.ToString();
	}

	private void RefreshExp(int level, int amount)
	{
		float num = Game.Instance.GetExpCoefficient(level) * (float)(level - 1) + 8f;
		expAmountLabel.text = amount + "/" + num;
		expSlider.value = (float)amount / num;
	}

	private void RefreshLevel(int amount)
	{
		levelAmountLabel.text = "LV. " + amount;
	}

	private void OnCoinsChanged()
	{
		if (updateAutomatically && _coinsParent.activeSelf)
		{
			RefreshCoins(PlayerInfo.Instance.amountOfCoins);
		}
	}

	private void OnKeysChanged()
	{
		if (updateAutomatically && _keysParent.activeSelf)
		{
			RefreshKeys(PlayerInfo.Instance.amountOfKeys);
		}
	}

	private void OnExpChanged()
	{
		if (updateAutomatically && _levelParent.activeSelf)
		{
			RefreshExp(PlayerInfo.Instance.amountOfLevel, PlayerInfo.Instance.amountOfExp);
		}
	}

	private void OnLevelChanged()
	{
		if (updateAutomatically && _levelParent.activeSelf)
		{
			RefreshLevel(PlayerInfo.Instance.amountOfLevel);
		}
	}

	public void AddCoins(int add)
	{
		if (!updateAutomatically && _coinsParent.activeSelf)
		{
			RefreshCoins(_originCoinAmount + add);
		}
	}

	public void AddKeys(int add)
	{
		if (!updateAutomatically && _keysParent.activeSelf)
		{
			RefreshKeys(_originKeyAmount + add);
		}
	}

	public void AddExps(int add)
	{
		if (!updateAutomatically && _levelParent.activeSelf)
		{
			int originLevelAmount = _originLevelAmount;
			int num = _originExpAmount + add;
			float num2 = Game.Instance.GetExpCoefficient(originLevelAmount) * (float)(originLevelAmount - 1) + 8f;
			if ((float)num >= num2)
			{
				num -= (int)num2;
				AddLevel(1);
			}
			RefreshExp(_originLevelAmount, num);
		}
	}

	public void AddLevel(int add)
	{
		if (!updateAutomatically && _levelParent.activeSelf)
		{
			RefreshLevel(_originLevelAmount + add);
		}
	}

	public void AddOriginCoins(int add)
	{
		if (!updateAutomatically && _coinsParent.activeSelf)
		{
			RefreshCoins(_originCoinAmount + add);
			_originCoinAmount += add;
		}
	}

	public void AddOriginKeys(int add)
	{
		if (!updateAutomatically && _keysParent.activeSelf)
		{
			RefreshKeys(_originKeyAmount + add);
			_originKeyAmount += add;
		}
	}

	public void AddOriginExps(int add)
	{
		if (!updateAutomatically && _levelParent.activeSelf)
		{
			int originLevelAmount = _originLevelAmount;
			int num = _originExpAmount + add;
			float num2 = Game.Instance.GetExpCoefficient(originLevelAmount) * (float)(originLevelAmount - 1) + 8f;
			if ((float)num >= num2)
			{
				num -= (int)num2;
				AddOriginLevel(1);
			}
			RefreshExp(_originLevelAmount, num);
			_originExpAmount = num;
		}
	}

	public void AddOriginLevel(int add)
	{
		if (!updateAutomatically && _levelParent.activeSelf)
		{
			RefreshLevel(_originLevelAmount + add);
			_originLevelAmount += add;
		}
	}

	public void OnCoinPlusClick()
	{
		ShopManager.Instance.barIndex = 1;
		if ("CoinsUI_shop".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("CoinsUI_shop") as UIShopScreen).ResetScrollViewSmooth();
		}
		else
		{
			UIScreenController.Instance.SwitchScreen("CoinsUI_shop");
		}
	}

	public void OnKeyPlusClick()
	{
		ShopManager.Instance.barIndex = 2;
		if ("CoinsUI_shop".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("CoinsUI_shop") as UIShopScreen).ResetScrollViewSmooth();
		}
		else
		{
			UIScreenController.Instance.SwitchScreen("CoinsUI_shop");
		}
	}
}
