using System;
using UnityEngine;

public class MenuSlider : MonoBehaviour
{
	[Serializable]
	public class MenuTween
	{
		public Transform tween;

		public Vector3 showLocalPos;

		public Vector3 hideLocalPos;
	}

	[SerializeField]
	private ScrollDirection direction = ScrollDirection.Down;

	[SerializeField]
	private UISprite menuBg;

	[SerializeField]
	private UISprite menuArrow;

	[SerializeField]
	private BoxCollider _menuMask;

	[SerializeField]
	private GameObject menuTip;

	[SerializeField]
	private MenuTween[] tweens;

	[SerializeField]
	private float menuBgHideValue = 2f;

	[SerializeField]
	private float duration = 2f;

	private int menuBgInitialValue;

	private void Awake()
	{
		_menuMask.enabled = false;
		for (int i = 0; i < tweens.Length; i++)
		{
			MenuTween menuTween = tweens[i];
			menuTween.showLocalPos = menuTween.tween.localPosition;
			menuTween.hideLocalPos = Vector3.zero;
		}
		switch (direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			menuBgInitialValue = menuBg.height;
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			menuBgInitialValue = menuBg.width;
			break;
		}
	}

	private void Start()
	{
		if (!PlayerInfo.Instance.menuSliderShow)
		{
			TweenIn(1f);
		}
		else
		{
			TweenOut(1f);
		}
	}

	private void OnDisable()
	{
		if (!PlayerInfo.Instance.menuSliderShow)
		{
			TweenIn(1f);
		}
		else
		{
			TweenOut(1f);
		}
	}

	public void Show()
	{
		if (PlayerInfo.Instance.menuSliderShow)
		{
			_menuMask.enabled = true;
			StartCoroutine(myTween.To(duration, TweenIn));
			AudioPlayer.Instance.PlaySound("chilun", true);
			PlayerInfo.Instance.menuSliderShow = false;
		}
		else
		{
			_menuMask.enabled = true;
			StartCoroutine(myTween.To(duration, TweenOut));
			AudioPlayer.Instance.PlaySound("chilun", true);
			PlayerInfo.Instance.menuSliderShow = true;
		}
	}

	private void TweenIn(float t)
	{
		MenuTween menuTween = null;
		for (int i = 0; i < tweens.Length; i++)
		{
			menuTween = tweens[i];
			menuTween.tween.localPosition = Vector3.Lerp(menuTween.showLocalPos, menuTween.hideLocalPos, t);
		}
		switch (direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			menuBg.height = (int)Mathf.Lerp(menuBgInitialValue, menuBgHideValue, t);
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			menuBg.width = (int)Mathf.Lerp(menuBgInitialValue, menuBgHideValue, t);
			break;
		}
		menuArrow.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(180f, 0f, t));
		if (t >= 1f)
		{
			_menuMask.enabled = false;
			if (menuTip != null && !menuTip.activeInHierarchy && PlayerInfo.Instance.GetAllAchievementAward())
			{
				menuTip.SetActive(true);
			}
		}
	}

	private void TweenOut(float t)
	{
		for (int i = 0; i < tweens.Length; i++)
		{
			MenuTween menuTween = tweens[i];
			menuTween.tween.localPosition = Vector3.Lerp(menuTween.hideLocalPos, menuTween.showLocalPos, t);
		}
		switch (direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			menuBg.height = (int)Mathf.Lerp(menuBgHideValue, menuBgInitialValue, t);
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			menuBg.width = (int)Mathf.Lerp(menuBgHideValue, menuBgInitialValue, t);
			break;
		}
		menuArrow.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 180f, t));
		if (t >= 1f)
		{
			_menuMask.enabled = false;
			if (menuTip != null && menuTip.activeInHierarchy)
			{
				menuTip.SetActive(false);
			}
		}
	}
}
