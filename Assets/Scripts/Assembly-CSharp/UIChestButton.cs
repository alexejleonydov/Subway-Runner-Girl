using System;
using UnityEngine;

public class UIChestButton : MonoBehaviour
{
	public delegate void OnClickEvent();

	[SerializeField]
	private ChestType _chestType;

	[SerializeField]
	private GameObject chestModelParent;

	[SerializeField]
	private BoxCollider boxCollider;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private GameObject buyGo;

	[SerializeField]
	private GameObject freeGo;

	[SerializeField]
	private GameObject viewGo;

	[SerializeField]
	private UISprite buyIcon;

	[SerializeField]
	private UILabel buyPrice;

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private UILabel viewLbl;

	[SerializeField]
	private TweenPosition tipTween;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite[] viewSprs;

	public Transform button;

	private OnClickEvent onClick;

	private GameObject currentChest;

	private int _type;

	public int type
	{
		get
		{
			return _type;
		}
	}

	public bool isActive { get; set; }

	public void SetChestType(ChestType type)
	{
		_chestType = type;
	}

	public GameObject Init()
	{
		SetChestModel();
		return currentChest;
	}

	private void SetChestModel()
	{
		if (currentChest != null)
		{
			UnityEngine.Object.Destroy(currentChest);
		}
		currentChest = UnityEngine.Object.Instantiate(ChestModelFactory.Instance.GetChestInCell(_chestType));
		currentChest.transform.parent = chestModelParent.transform;
		currentChest.transform.localPosition = Vector3.zero;
		currentChest.transform.localRotation = Quaternion.identity;
		currentChest.transform.localScale = Vector3.one;
		Utility.SetLayerRecursively(currentChest.transform, chestModelParent.layer);
	}

	public void ShowChestModel()
	{
		chestModelParent.SetActive(true);
	}

	public void HideChestModel()
	{
		chestModelParent.SetActive(false);
	}

	public void Refresh()
	{
		Chest chest = ChestsData.GetChest(_chestType);
		if (chest == null)
		{
			Debug.LogError("There is not chest of " + _chestType);
			return;
		}
		if (chest.unlockType == UnlockType.coin || chest.unlockType == UnlockType.key)
		{
			buyGo.SetActive(true);
			freeGo.SetActive(false);
			viewGo.SetActive(false);
			if (chest.unlockType == UnlockType.coin)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			else if (chest.unlockType == UnlockType.key)
			{
				buyIcon.spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
			}
			buyPrice.text = chest.price.ToString();
			boxCollider.enabled = true;
			base.enabled = false;
			_type = 1;
		}
		else if (chest.unlockType == UnlockType.view)
		{
			viewGo.SetActive(true);
			buyGo.SetActive(false);
			freeGo.SetActive(false);
			base.enabled = false;
			boxCollider.enabled = true;
			bool flag = RiseSdk.Instance.HasRewardAd();
			if (viewSprs != null && viewSprs.Length > 0)
			{
				viewLbl.text = Strings.Get(LanguageKey.UI_SCREEN_CHARACTER_SELECT_BUTTON_WATCH);
				int i = 0;
				for (int num = viewSprs.Length; i < num; i++)
				{
					viewSprs[i].color = ((!flag) ? Color.cyan : Color.white);
				}
				_type = ((!flag) ? 3 : 2);
				if (flag)
				{
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_all_success", 0);
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "show_video_shop_free_box", 0);
				}
			}
			else
			{
				viewLbl.text = Strings.Get(LanguageKey.SHOP_BOX_LABEL_FERR);
				if ((bool)tipTween)
				{
					tipTween.gameObject.SetActive(flag);
				}
				_type = 2;
			}
		}
		else if (chest.unlockType == UnlockType.free)
		{
			freeGo.SetActive(true);
			buyGo.SetActive(false);
			viewGo.SetActive(false);
			freeLbl.text = string.Empty;
			timeLbl.text = string.Empty;
			base.enabled = true;
			_type = 5;
		}
		ChestTemplate chestTemplate = ChestsData.GetChestTemplate(_chestType);
		if (chestTemplate != null)
		{
			description.text = Strings.Get(chestTemplate.description);
		}
	}

	private void Update()
	{
		if (!ShopManager.Instance.IsCoolingDownOver())
		{
			freeLbl.text = string.Empty;
			if (fillSpr != null)
			{
				fillSpr.enabled = false;
				boxCollider.enabled = false;
			}
			if ((bool)tipTween)
			{
				tipTween.gameObject.SetActive(false);
			}
			timeLbl.text = ShopManager.Instance.GetCoolingDownTime();
			_type = 5;
		}
		else
		{
			freeLbl.text = Strings.Get(LanguageKey.SHOP_BOX_LABEL_FERR);
			if (fillSpr != null)
			{
				fillSpr.enabled = true;
				boxCollider.enabled = true;
			}
			if ((bool)tipTween)
			{
				tipTween.gameObject.SetActive(true);
			}
			timeLbl.text = string.Empty;
			_type = 4;
		}
	}

	public void RegisterOnClickEvent(OnClickEvent onClick)
	{
		this.onClick = (OnClickEvent)Delegate.Combine(this.onClick, onClick);
	}

	public void UnregisterOnClickEvent(OnClickEvent onClick)
	{
		this.onClick = (OnClickEvent)Delegate.Remove(this.onClick, onClick);
	}

	public void OnClick()
	{
		ShopManager.Instance.chestType = _chestType;
		if (onClick != null)
		{
			onClick();
		}
	}
}
