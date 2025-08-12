using System;
using UnityEngine;

public class UIBaseScreen : MonoBehaviour
{
	[SerializeField]
	protected GameObject[] dynamicallyLoadedElements;

	[OptionalField]
	[SerializeField]
	protected GameObject FooterPrefab;

	[SerializeField]
	protected int selectedFooterButton;

	[SerializeField]
	protected UIAnimatorController animatorCtrl;

	[SerializeField]
	protected UITweener tween;

	protected Collider[] colliders;

	protected UIButtonColor[] uiButtonColors;

	protected bool[] collidersActivate;

	protected UIFooterHandler _footerHandler;

	private bool _footerInited;

	protected bool _hasSetCollidersFalse;

	public Action Closed;

	public bool isActive { get; private set; }

	public string parentScreen { get; set; }

	public string screenName { get; set; }

	public virtual void GainFocus()
	{
		Collider(true);
	}

	public virtual void TryHide()
	{
		Collider(false);
		if (animatorCtrl == null)
		{
			if (tween != null)
			{
				PrepareHide();
				EventDelegate.Add(tween.onFinished, Hide, true);
				tween.PlayReverse();
				return;
			}
		}
		else if (animatorCtrl.OnExit())
		{
			return;
		}
		PrepareHide();
		Hide();
	}

	protected virtual void PrepareHide()
	{
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(false);
		isActive = false;
		if (Closed != null)
		{
			Closed();
			Closed = null;
		}
	}

	public virtual void Init()
	{
		for (int i = 0; i < dynamicallyLoadedElements.Length; i++)
		{
			NGUITools.AddChild(base.gameObject, dynamicallyLoadedElements[i]);
		}
		colliders = GetComponentsInChildren<Collider>();
		if (colliders != null)
		{
			uiButtonColors = new UIButtonColor[colliders.Length];
			collidersActivate = new bool[colliders.Length];
		}
		int j = 0;
		for (int num = colliders.Length; j < num; j++)
		{
			if (colliders[j] != null)
			{
				collidersActivate[j] = colliders[j].enabled;
				uiButtonColors[j] = colliders[j].GetComponent<UIButton>();
			}
			else
			{
				collidersActivate[j] = false;
			}
		}
		_hasSetCollidersFalse = false;
	}

	protected virtual void InitFooter()
	{
		if (FooterPrefab != null)
		{
			_footerHandler = NGUITools.AddChild(base.gameObject, FooterPrefab).GetComponent<UIFooterHandler>();
			//_footerHandler.OnButtonClick(selectedFooterButton);
			if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
			{
				_footerHandler.gameObject.GetComponent<UIAnchor>().pixelOffset.y = 102f;
			}
			else
			{
				_footerHandler.gameObject.GetComponent<UIAnchor>().pixelOffset.y = 0f;
			}
		}
	}

	public CoinBoxSizer InitializeCoinbox(bool fundsEnabled, bool coinsEnabled, bool keysEnabled, bool updateAutomatically)
	{
		GameObject prefab = Resources.Load("Prefabs/DynamicLoad/CoinboxQuick") as GameObject;
		GameObject gameObject = NGUITools.AddChild(base.gameObject, prefab);
		CoinBoxSizer componentInChildren = gameObject.GetComponentInChildren<CoinBoxSizer>();
		componentInChildren.Init(fundsEnabled, coinsEnabled, keysEnabled, updateAutomatically);
		UIAnchor[] componentsInChildren = gameObject.GetComponentsInChildren<UIAnchor>();
		int num = 0;
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			num = -132;
		}
		int i = 0;
		for (int num2 = componentsInChildren.Length; i < num2; i++)
		{
			componentsInChildren[i].pixelOffset.y = num;
		}
		return componentInChildren;
	}

	public static bool IsOutOfProportion()
	{
		return (float)Screen.height * 1f / ((float)Screen.width * 1f) > 1.7777778f;
	}

	public virtual void LooseFocus()
	{
		Collider(false);
	}

	public virtual void Show()
	{
		Collider(true);
		base.gameObject.SetActive(true);
		isActive = true;
		if (animatorCtrl != null)
		{
			animatorCtrl.OnEnter(AfterShow, PrepareHide, Hide);
		}
		if (tween != null)
		{
			EventDelegate.Add(tween.onFinished, AfterShow, true);
			tween.PlayForward();
		}
		if (!_footerInited)
		{
			InitFooter();
			_footerInited = true;
		}
	}

	protected virtual void AfterShow()
	{
	}

	public void Collider(bool active)
	{
		if (colliders == null || colliders.Length <= 0)
		{
			return;
		}
		int i = 0;
		for (int num = colliders.Length; i < num; i++)
		{
			if (!(colliders[i] != null))
			{
				continue;
			}
			if (active)
			{
				colliders[i].enabled = collidersActivate[i];
				if (uiButtonColors[i] != null && collidersActivate[i])
				{
					uiButtonColors[i].SetState(UIButtonColor.State.Normal, true);
				}
			}
			else if (!_hasSetCollidersFalse)
			{
				collidersActivate[i] = colliders[i].enabled;
				colliders[i].enabled = false;
			}
		}
		if (active)
		{
			_hasSetCollidersFalse = false;
		}
		else if (!_hasSetCollidersFalse)
		{
			_hasSetCollidersFalse = true;
		}
	}
}
