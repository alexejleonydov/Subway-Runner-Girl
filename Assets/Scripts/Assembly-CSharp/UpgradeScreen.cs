using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : UIBaseScreen
{
	[SerializeField]
	private GameObject PermanentPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private GameObject listTitleComponent;

	[SerializeField]
	private PropType[] powerupPermanent = new PropType[4]
	{
		PropType.flypack,
		PropType.supershoes,
		PropType.coinmagnet,
		PropType.doubleMultiplier
	};

	private List<UpgradeHelper> cachedUpgradeHelpers;

	private PropType freeType;

	private bool _hasInited;

	private int numberOfObjects;

	private void FillTable()
	{
		if (!_hasInited)
		{
			numberOfObjects = 0;
			GameObject gameObject = NGUITools.AddChild(_table.gameObject, listTitleComponent);
			ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
			gameObject.name = string.Format("{0:000}", numberOfObjects);
			component.Setup(Strings.Get(LanguageKey.UPGRADES_TITLE), Strings.Get(LanguageKey.UPGRADES_DESCRIPTION));
			numberOfObjects++;
			int i = 0;
			for (int num = powerupPermanent.Length; i < num; i++)
			{
				MakeBuyable(powerupPermanent[i]);
				numberOfObjects++;
			}
			_hasInited = true;
		}
	}

	public override void Init()
	{
		base.Init();
		cachedUpgradeHelpers = new List<UpgradeHelper>(Upgrades.upgrades.Count);
		InitializeCoinbox(true, true, true, true);
	}

	private GameObject MakeBuyable(PropType powerupType)
	{
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, PermanentPrefab);
		gameObject.GetComponent<UpgradeHelper>().InitPermanent(powerupType);
		gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		NGUITools.AddWidgetCollider(gameObject);
		cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
		return gameObject;
	}

	public override void Show()
	{
		base.Show();
		FillTable();
		RefreshUpgrade();
	}

	private void OnDisable()
	{
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(OnUpgradePurchase);
		RiseSdkListener.OnAdEvent -= OnFreeRewardCallback;
	}

	private void OnEnable()
	{
		PurchaseHandler.Instance.AddOnUpgradePurchase(OnUpgradePurchase);
		RiseSdkListener.OnAdEvent -= OnFreeRewardCallback;
		RiseSdkListener.OnAdEvent += OnFreeRewardCallback;
	}

	private void OnUpgradePurchase()
	{
		freeType = PropType._notset;
		RefreshUpgrade();
	}

	private void RefreshUpgrade()
	{
		if (cachedUpgradeHelpers == null)
		{
			return;
		}
		if (freeType == PropType._notset)
		{
			List<PropType> list = new List<PropType>();
			Upgrade upgrade = null;
			int i = 0;
			for (int num = powerupPermanent.Length; i < num; i++)
			{
				upgrade = Upgrades.upgrades[powerupPermanent[i]];
				if (PlayerInfo.Instance.GetCurrentTier(powerupPermanent[i]) < 3)
				{
					list.Add(powerupPermanent[i]);
				}
			}
			if (list.Count == 0)
			{
				freeType = PropType._notset;
			}
			else
			{
				int index = Random.Range(0, list.Count);
				freeType = list[index];
			}
		}
		int j = 0;
		for (int count = cachedUpgradeHelpers.Count; j < count; j++)
		{
			cachedUpgradeHelpers[j].RefreshUpgrade(freeType);
		}
	}

	private void OnFreeRewardCallback(RiseSdk.AdEventType type, int id, string tag, int adType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 8)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_upgrades", 0);
			PlayerInfo.Instance.UseFreeUpgrade();
			PurchaseHandler.Instance.PurchaseUpgradeFree(freeType);
		}
	}
}
