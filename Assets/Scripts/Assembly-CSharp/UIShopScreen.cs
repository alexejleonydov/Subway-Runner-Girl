using System;
using UnityEngine;

public class UIShopScreen : UIBaseScreen
{
	[SerializeField]
	private UIScrollView scrollView;

	[SerializeField]
	private float[] absolutes;

	[SerializeField]
	private UITable table;

	[SerializeField]
	private ListTitleComponentHelper restorePurchaseTitle;

	[SerializeField]
	private PromotionManager promotionManager;

	[SerializeField]
	private Transform chestParent;

	[SerializeField]
	private ListTitleComponentHelper chestTitle;

	[SerializeField]
	private UIChestButton[] uiChestButtons;

	[SerializeField]
	private Transform uiFreeChest;

	[SerializeField]
	private Transform coinParent;

	[SerializeField]
	private ListTitleComponentHelper coinTitle;

	[SerializeField]
	private Transform keyParent;

	[SerializeField]
	private ListTitleComponentHelper keyTitle;

	[SerializeField]
	private CoinButtonHelper[] coinButtonHelpers;

	[SerializeField]
	private ListTitleComponentHelper upgradeTitle;

	[SerializeField]
	private UIUpgradeButton[] uiUpgradeButtons;

	[SerializeField]
	private float strength = 10f;

	private bool autoMove;

	private Vector3 current;

	private Vector3 to;

	private float origin;

	private void OnEnable()
	{
		restorePurchaseTitle.Setup(Strings.Get(LanguageKey.ListTitle_label_01), string.Empty);
		chestTitle.Setup(Strings.Get(LanguageKey.ListTitle_label_02), string.Empty);
		coinTitle.Setup(Strings.Get(LanguageKey.ListTitle_label_03), string.Empty);
		keyTitle.Setup(Strings.Get(LanguageKey.ListTitle_label_04), string.Empty);
		upgradeTitle.Setup(Strings.Get(LanguageKey.ListTitle_label_05), Strings.Get(LanguageKey.PROP_DESCRIPTION));
	}

	private void Calc()
	{
		table.transform.localPosition = Vector3.zero;
		absolutes[0] = 0f - origin;
		absolutes[1] = 0f - origin - coinParent.localPosition.y - 50f;
		absolutes[2] = 0f - origin - keyParent.localPosition.y - 50f;
		absolutes[3] = 0f - origin - chestParent.localPosition.y - 50f;
	}

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(true, true, true, true);
		AfterWhile();
	}

	private void AfterWhile()
	{
		scrollView.SetDragAmount(0f, 0f, false);
		float activeHeight = UIScreenController.Instance.root.activeHeight;
		int i = 0;
		for (int num = uiChestButtons.Length; i < num; i++)
		{
			GameObject gameObject = uiChestButtons[i].Init();
			InitAssets.Instance.SetChestBox(gameObject.GetComponentsInChildren<MeshRenderer>(), scrollView.panel.baseClipRegion, activeHeight);
			uiChestButtons[i].RegisterOnClickEvent(OnButtonClick);
		}
		int j = 0;
		for (int num2 = coinButtonHelpers.Length; j < num2; j++)
		{
			coinButtonHelpers[j].Init();
		}
		int k = 0;
		for (int num3 = uiUpgradeButtons.Length; k < num3; k++)
		{
			uiUpgradeButtons[k].Init();
		}
		if (PlayerInfo.Instance.tutorialStep < 2)
		{
			ShopManager.Instance.SetNewTime();
		}
		origin = scrollView.panel.clipOffset.y;
		table.onReposition = (UITable.OnReposition)Delegate.Combine(table.onReposition, new UITable.OnReposition(Calc));
		promotionManager.RefreshPayHelps();
	}

	public override void Show()
	{
		if (PlayerInfo.Instance.tutorialStep == 2)
		{
			ShopManager.Instance.ForceCoolingDownOver();
			UIScreenController.Instance.ReadyTutorial();
		}
		base.Show();
		int i = 0;
		for (int num = uiChestButtons.Length; i < num; i++)
		{
			uiChestButtons[i].Refresh();
		}
		int j = 0;
		for (int num2 = uiUpgradeButtons.Length; j < num2; j++)
		{
			uiUpgradeButtons[j].RefreshUpgrade();
		}
		ResetScrollViewImmediately();
	}

	protected override void AfterShow()
	{
		if (PlayerInfo.Instance.tutorialStep == 2)
		{
			ShopManager.Instance.barIndex = 3;
			ResetScrollViewImmediately();
			GameObject gameObject = UnityEngine.Object.Instantiate(uiFreeChest.gameObject);
			gameObject.transform.parent = uiFreeChest.parent;
			gameObject.transform.localPosition = Vector3.up * uiFreeChest.localPosition.y;
			gameObject.transform.localScale = Vector3.one;
			UIDragScrollView component = gameObject.GetComponent<UIDragScrollView>();
			if ((bool)component)
			{
				UnityEngine.Object.Destroy(component);
			}
			UIChestButton component2 = gameObject.GetComponent<UIChestButton>();
			component2.SetChestType(ChestType.Free);
			component2.RegisterOnClickEvent(delegate
			{
				UIScreenController.Instance.HideTutorial();
				OnButtonClick();
			});
			UIScreenController.Instance.ShowTutorial(gameObject, UIPosScalesAndNGUIAtlas.Instance.shopScreenFreeFingerOffset / UIScreenController.Instance.root.activeHeight * 2f, UIPosScalesAndNGUIAtlas.Instance.shopScreenFreeFingerRotZ);
		}
	}

	private void ResetScrollViewImmediately()
	{
		float num = absolutes[ShopManager.Instance.barIndex];
		num -= scrollView.transform.localPosition.y;
		scrollView.MoveRelative(new Vector3(0f, num, 0f));
	}

	public void ResetScrollViewSmooth()
	{
		to = new Vector3(0f, absolutes[ShopManager.Instance.barIndex] - scrollView.transform.localPosition.y, 0f);
		current = Vector3.zero;
		autoMove = true;
	}

	public override void Hide()
	{
		ShopManager.Instance.barIndex = 0;
		base.Hide();
	}

	private void OnButtonClick()
	{
		UIScreenController.Instance.PushPopup("BoxPreviewPopup");
	}

	private void Update()
	{
		if (autoMove)
		{
			current = NGUIMath.SpringLerp(current, to, strength, Time.deltaTime);
			scrollView.MoveRelative(current);
			to -= current;
			current = Vector3.zero;
			if (Mathf.Abs(to.y) <= float.Epsilon)
			{
				autoMove = false;
			}
		}
	}
}
