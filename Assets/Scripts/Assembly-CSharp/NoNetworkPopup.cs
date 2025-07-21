using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NoNetworkPopup : UIBaseScreen
{
	public TweenAlpha tweenAlpha;

	public UISprite background;

	public float duartion = 1.5f;

	public override void Show()
	{
		base.Show();
		StartCoroutine(ShowPopup());
	}

	public override void Hide()
	{
		StartCoroutine(HidePopup());
	}

	public override void Init()
	{
		base.Init();
		if (tweenAlpha == null)
		{
			tweenAlpha = GetComponentInChildren<TweenAlpha>();
		}
		tweenAlpha.GetComponent<UISprite>().alpha = tweenAlpha.from;
	}

	private IEnumerator ShowPopup()
	{
		tweenAlpha.PlayForward();
		float t2 = Time.realtimeSinceStartup;
		float start = Time.realtimeSinceStartup;
		while (t2 < start + tweenAlpha.duration)
		{
			background.alpha = Mathf.Lerp(0.1f, 0.8f, (t2 - start) / tweenAlpha.duration);
			yield return null;
			t2 = Time.realtimeSinceStartup;
		}
		for (t2 = Time.realtimeSinceStartup; t2 < start + duartion; t2 = Time.realtimeSinceStartup)
		{
			yield return null;
		}
		if ((bool)UIScreenController.Instance)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	private IEnumerator HidePopup()
	{
		float t = Time.realtimeSinceStartup;
		float start = Time.realtimeSinceStartup;
		while (t < start + tweenAlpha.duration)
		{
			background.alpha = Mathf.Lerp(0.8f, 0.1f, (t - start) / tweenAlpha.duration);
			yield return null;
			t = Time.realtimeSinceStartup;
		}
		tweenAlpha.Stop();
		_003CHide_003E__BaseCallProxy0();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CHide_003E__BaseCallProxy0()
	{
		base.Hide();
	}
}
