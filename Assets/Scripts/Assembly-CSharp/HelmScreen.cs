using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmScreen : UIBaseScreen, IScrollClick
{
	private class HelmState
	{
		public Helmets.HelmType type;

		public Quaternion defaultRotation;

		public GameObject helmRoot;

		public Transform helmTransform;
	}

	[SerializeField]
	private GameObject scrollAnchor;

	[SerializeField]
	private UIGrid scrollGrid;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private GameObject skill1Go;

	[SerializeField]
	private UILabel skill1Title;

	[SerializeField]
	private UILabel skill2Title;

	[SerializeField]
	private UILabel skill1Label;

	[SerializeField]
	private GameObject skill2Go;

	[SerializeField]
	private UILabel skill2Label;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private HelmetSelectButton selectBtn;

	[SerializeField]
	private float minScale;

	[SerializeField]
	private float maxScale;

	[SerializeField]
	private UIDrag drag;

	public bool resetHelmAnimation;

	private float _cellWidth;

	private CenterOnChild _centerer;

	private Helmets.HelmType _currentHelmShown;

	private bool _hasInited;

	private bool _modelsEnabled;

	private bool _popupActive;

	private List<OverlayIndex> helmIndices = new List<OverlayIndex>();

	private List<KeyValuePair<Helmets.HelmType, Helmets.Helm>> helmList;

	private List<HelmState> scrollHelms = new List<HelmState>();

	private void OnEnable()
	{
		drag.onHandleDir = (UIDrag.OnSwipeDelegate)Delegate.Combine(drag.onHandleDir, new UIDrag.OnSwipeDelegate(OnHandleDir));
	}

	private void OnDisable()
	{
		drag.onHandleDir = (UIDrag.OnSwipeDelegate)Delegate.Remove(drag.onHandleDir, new UIDrag.OnSwipeDelegate(OnHandleDir));
	}

	private void OnHandleDir(SwipeDir dir)
	{
		int num = Helmets.helmOrder.IndexOf(_currentHelmShown);
		if (num != -1)
		{
			bool flag = false;
			if (dir == SwipeDir.Right && num > 0)
			{
				flag = true;
				num--;
			}
			if (dir == SwipeDir.Left && num < scrollHelms.Count - 1)
			{
				flag = true;
				num++;
			}
			if (flag)
			{
				_centerer.CenterOnTransform(helmIndices[num].transform, true);
				_centerer.Recenter();
			}
		}
	}

	private T ElementToCenterOn<T>(List<T> elements, Helmets.HelmType helm)
	{
		T result = default(T);
		int num = 0;
		int i = 0;
		for (int count = helmList.Count; i < count && helm != helmList[i].Key; i++)
		{
			num++;
		}
		if (num < elements.Count)
		{
			result = elements[num];
		}
		return result;
	}

	public override void Hide()
	{
		UIModelController.Instance.ClearModels();
		base.Hide();
	}

	public override void Init()
	{
		base.Init();
		selectBtn.InitButton(this);
		_cellWidth = scrollGrid.cellWidth;
		_centerer = scrollGrid.GetComponent<CenterOnChild>();
		InitHelms();
		_hasInited = true;
		InitializeCoinbox(true, true, true, true);
	}

	private void InitHelms()
	{
		helmList = new List<KeyValuePair<Helmets.HelmType, Helmets.Helm>>();
		foreach (Helmets.HelmType key in Helmets.helmData.Keys)
		{
			Helmets.Helm value = Helmets.helmData[key];
			if (HelmetManager.Instance.isHelmetUnlocked(key) || HelmetManager.Instance.isHelmActive(key))
			{
				helmList.Add(new KeyValuePair<Helmets.HelmType, Helmets.Helm>(key, value));
			}
		}
		int num = 0;
		int i = 0;
		for (int count = helmList.Count; i < count; i++)
		{
			HelmState helmState = new HelmState();
			helmState.type = helmList[i].Key;
			helmState.helmRoot = HelmetModelPreviewFactory.Instance.GetHelmetModelForScroll(helmList[i].Value.helmModelName, helmList[i].Key);
			if (helmState.helmRoot == null)
			{
				Debug.LogError("Helm: '" + helmList[i].Value.helmModelName + "' not found.");
				continue;
			}
			helmState.helmRoot.name = string.Format("{0:000}{1}", num, helmList[i].Value.name);
			helmState.helmTransform = helmState.helmRoot.transform.GetChild(0);
			helmState.defaultRotation = HelmetModelPreviewFactory.Instance.GetHelmetDefaultRotation(helmList[i].Value.helmModelName);
			scrollHelms.Add(helmState);
			Transform transform = helmState.helmRoot.transform;
			transform.parent = scrollAnchor.transform;
			transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
			transform.localScale = Vector3.one * 7f;
			transform.localEulerAngles = new Vector3(53f, 183f, 360f);
			GameObject gameObject = NGUITools.AddChild(scrollGrid.gameObject, dummyObject);
			helmIndices.Add(gameObject.AddComponent<OverlayIndex>());
			helmIndices[num].index = num;
			gameObject.name = string.Format("{0:000}{1}", num, helmList[i].Key.ToString());
			num++;
		}
		Utility.SetLayerRecursively(scrollAnchor.transform, 20);
		scrollGrid.Reposition();
		Helmets.HelmType currentHelmet = PlayerInfo.Instance.currentHelmet;
		OverlayIndex overlayIndex = ElementToCenterOn(helmIndices, currentHelmet);
		_centerer.CenterOnTransform(overlayIndex.transform, true);
		_currentHelmShown = currentHelmet;
		float num2 = Mathf.Abs(scrollAnchor.transform.localPosition.x);
		for (int j = 0; j < scrollHelms.Count; j++)
		{
			float num3 = Mathf.Abs(num2 - (float)j * _cellWidth);
			float num4 = 1.5f * _cellWidth;
			float num5 = Mathf.SmoothStep(maxScale, minScale, num3 / num4);
			scrollHelms[j].helmRoot.transform.localScale = Vector3.one * num5;
		}
		_modelsEnabled = true;
	}

	private void RefreshCurrentHelmAndUI()
	{
		_modelsEnabled = false;
		UIModelController.Instance.ActivateHelmetModel();
		ShowHelmetInMenu(true);
	}

	public override void GainFocus()
	{
		base.GainFocus();
		RefreshCurrentHelmAndUI();
	}

	public void ScrollClicked(Vector2 pos)
	{
		if (_centerer.CenterOnClosestChildAtPosition(pos) && HelmetManager.Instance.isHelmetUnlocked(_currentHelmShown))
		{
			SelectCurrentHelmShown();
			NGUITools.PlaySound(selectSound);


		}
	}

	private void SelectCurrentHelmShown()
	{
		PlayerInfo.Instance.currentHelmet = _currentHelmShown;
		UpdateButtons();
	}

	public override void Show()
	{
		base.Show();
		RefreshCurrentHelmAndUI();
	}

	private void Update()
	{
		if (!_hasInited || helmList == null)
		{
			return;
		}
		if (_centerer.centeredObject != null)
		{
			int index = _centerer.centeredObject.GetComponent<OverlayIndex>().index;
			Transform helmTransform = scrollHelms[index].helmTransform;
			helmTransform.Rotate(Vector3.up, 30f * Time.deltaTime);
			if (scrollHelms[index].type != _currentHelmShown)
			{
				_currentHelmShown = scrollHelms[index].type;
				ShowHelmetInMenu(true);
			}
			for (int i = 0; i < scrollHelms.Count; i++)
			{
				if (i != index)
				{
					HelmState helmState = scrollHelms[i];
					Quaternion b = helmState.helmTransform.parent.rotation * helmState.defaultRotation;
					helmState.helmTransform.rotation = Quaternion.Lerp(helmState.helmTransform.rotation, b, 10f * Time.deltaTime);
				}
			}
		}
		for (int j = 0; j < scrollHelms.Count; j++)
		{
			float num = Mathf.Abs(Mathf.Abs(scrollAnchor.transform.localPosition.x) - (float)j * _cellWidth);
			float num2 = 1.5f * _cellWidth;
			float num3 = Mathf.SmoothStep(maxScale, minScale, num / num2);
			scrollHelms[j].helmRoot.transform.localScale = Vector3.one * num3;
		}
		if (UIScreenController.Instance.isShowingPopup)
		{
			if (!_popupActive)
			{
				_popupActive = true;
			}
		}
		else if (_popupActive)
		{
			_popupActive = false;
		}
		if (_popupActive)
		{
			if (_modelsEnabled)
			{
				scrollAnchor.SetActive(false);
				_modelsEnabled = false;
			}
		}
		else if (!_modelsEnabled)
		{
			scrollAnchor.SetActive(true);
			ShowHelmetInMenu(true);
			_modelsEnabled = true;
		}
	}

	public void ShowHelmetInMenu(bool updateAnimation)
	{
		UIModelController.Instance.ShowHelmetMenuModel(_currentHelmShown, updateAnimation);
		UpdateNamesAndButtons();
	}

	private void UpdateNamesAndButtons()
	{
		UpdateNames();
		UpdateButtons();
	}

	private void UpdateNames()
	{
		Helmets.Helm helm = Helmets.helmData[_currentHelmShown];
		nameLabel.text = Strings.Get(helm.name);
		nameLabel.gameObject.SetActive(true);
		skill1Title.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_H);
		skill2Title.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_H);
		skill1Label.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_H);
		skill2Label.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL2_H);
		skill1Go.SetActive(helm.useMutiplier);
		skill2Go.SetActive(helm.useMagent);
	}

	private void UpdateButtons()
	{
		selectBtn.UpdateSelectState(_currentHelmShown);
		Debug.Log("Swipe on Button" + _currentHelmShown);
	}
}
