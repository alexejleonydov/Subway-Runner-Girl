using System.Collections.Generic;
using UnityEngine;

public class BoxPreviewPopup : UIBaseScreen, IPurchaseHandler
{
	[SerializeField]
	private UIScrollView scoreView;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private GameObject chestModelParent;

	[SerializeField]
	private UISprite oneSpr;

	[SerializeField]
	private UILabel oneLbl;

	[SerializeField]
	private UISprite twoSpr;

	[SerializeField]
	private UILabel twoLbl;

	[SerializeField]
	private Transform featureParent;

	[SerializeField]
	private UICard defaultFeature;

	[SerializeField]
	private UICard cusomFeature;

	[SerializeField]
	private UIChestButton chestButton;

	private List<UICard> features;

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private ChestType _chestType;

	private ChestTemplate _template;

	private bool _purchaseInProgress;

	private void Awake()
	{
		Vector3 localPosition = defaultFeature.transform.localPosition;
		Vector3 localPosition2 = cusomFeature.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		features = new List<UICard>();
		features.Add(defaultFeature);
		features.Add(cusomFeature);
	}

	public override void Show()
	{
		_chestType = ShopManager.Instance.chestType;
		_template = ChestsData.GetChestTemplate(_chestType);
		if (_template == null)
		{
			Debug.LogError("There is not ChestTemplate of " + _chestType);
			return;
		}
		base.Show();
		if (PlayerInfo.Instance.tutorialStep == 2)
		{
			UIScreenController.Instance.ReadyTutorial();
		}
		chestButton.SetChestType(_chestType);
		GameObject gameObject = chestButton.Init();
		InitAssets.Instance.SetChestBox(gameObject.GetComponentsInChildren<MeshRenderer>(), new Vector4(0f, 0f, 1f, 1f), 1f);
		chestButton.Refresh();
		chestButton.RegisterOnClickEvent(OnButtonClick);
		scoreView.ResetPosition();
		oneSpr.spriteName = _template.one.icon;
		oneLbl.text = _template.one.min + "-" + _template.one.max;
		twoSpr.spriteName = _template.two.icon;
		twoLbl.text = _template.two.min + "-" + _template.two.max;
		int i = 0;
		for (int count = features.Count; i < count; i++)
		{
			features[i].gameObject.SetActive(false);
		}
		int num = _template.features.Length;
		for (int j = 0; j < num; j++)
		{
			if (j < features.Count)
			{
				UICard uICard = features[j];
				PrizeEntryTemplate prizeEntryTemplate = ChestsData.GetPrizeEntryTemplate(_template.features[j]);
				uICard.gameObject.SetActive(true);
				uICard.Set(prizeEntryTemplate.bg_small, prizeEntryTemplate.icon);
				continue;
			}
			UICard component = Object.Instantiate(cusomFeature.gameObject).GetComponent<UICard>();
			if (component != null)
			{
				component.transform.parent = featureParent;
				component.transform.localPosition = cusomFeature.transform.localPosition + _distanceBetweenButtons * (j - 1);
				component.transform.localScale = Vector3.one;
				component.name = j + 1 + "Card";
				PrizeEntryTemplate prizeEntryTemplate2 = ChestsData.GetPrizeEntryTemplate(_template.features[j]);
				component.Set(prizeEntryTemplate2.bg_small, prizeEntryTemplate2.icon);
				features.Add(component);
			}
		}
	}

	protected override void AfterShow()
	{
		if (PlayerInfo.Instance.tutorialStep == 2)
		{
			GameObject gameObject = Object.Instantiate(chestButton.button.gameObject);
			gameObject.transform.parent = chestButton.button.parent;
			gameObject.transform.localPosition = Vector3.up * chestButton.button.localPosition.y;
			gameObject.transform.localScale = Vector3.one;
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(320f, 120f, 0f);
			UIButtonMessage uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = chestButton.gameObject;
			uIButtonMessage.functionName = "OnClick";
			UIScreenController.Instance.ShowTutorial(gameObject, UIPosScalesAndNGUIAtlas.Instance.chestPopupButtoneFingerOffset / UIScreenController.Instance.root.activeHeight * 2f, UIPosScalesAndNGUIAtlas.Instance.chestPopupButtoneFingerRotZ);
		}
	}

	public override void Hide()
	{
		chestButton.UnregisterOnClickEvent(OnButtonClick);
		base.Hide();
	}

	private void OnEnable()
	{
		rewardLbl.text = Strings.Get(LanguageKey.BOX_PREVIEW_POPUP_REWARD_LABEL);
		RiseSdkListener.OnAdEvent -= OnFreeReward;
		RiseSdkListener.OnAdEvent += OnFreeReward;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= OnFreeReward;
	}

	public void OnFreeReward(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 16)
		{
			OpenChest();
		}
	}

	private void OnFreeViewClick(int type)
	{
		switch (type)
		{
		case 2:
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_shop_chest", 0);
			RiseSdk.Instance.TrackEvent("click_video_shop_chest", "default,default");
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (RiseSdk.Instance.HasRewardAd())
				{
					IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_shop_chest", 0);
					VideoLoadingPopup.adType = 2;
					VideoLoadingPopup.rewardId = 16;
					UIScreenController.Instance.PushPopup("VideoLoadingPopup");
				}
				else
				{
					UISliderInController.Instance.OnNetErrorPickedUp();
				}
			}
			else
			{
				UIScreenController.Instance.PushPopup("NoNetworkPopup");
			}
			break;
		case 3:
			if (UIScreenController.Instance.CheckNetwork())
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
			else
			{
				UIScreenController.Instance.PushPopup("NoNetworkPopup");
			}
			break;
		}
	}

	public override void GainFocus()
	{
		base.GainFocus();
		if (chestButton != null)
		{
			chestButton.ShowChestModel();
		}
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		if (chestButton != null)
		{
			chestButton.HideChestModel();
		}
	}

	private void OnButtonClick()
	{
		int type = chestButton.type;
		switch (type)
		{
		case 1:
			if (!_purchaseInProgress)
			{
				_purchaseInProgress = true;
				PurchaseHandler.Instance.PurchaseChest(_chestType, this);
			}
			break;
		case 2:
		case 3:
			OnFreeViewClick(type);
			break;
		case 4:
			if (ShopManager.Instance.IsCoolingDownOver())
			{
				if (PlayerInfo.Instance.tutorialStep == 2)
				{
					PlayerInfo.Instance.tutorialStep++;
					UIScreenController.Instance.HideTutorial();
				}
				OpenChest();
				ShopManager.Instance.SetNewTime();
			}
			break;
		}
	}

	private void OpenChest()
	{
		UIScreenController.Instance.ClosePopup(null);
		UIScreenController.Instance.PushPopup("Box_Open");
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
		OpenChest();
	}
}
