using System.Collections;
using UnityEngine;

public class ResumeButtonHelper : MonoBehaviour
{
	private UIButtonOverlayOff _cachedOverlayHelper;

	private bool buttonEnabled = true;

	public bool isButtonEnabled
	{
		get
		{
			return buttonEnabled;
		}
	}

	private UIButtonOverlayOff overlayHelper
	{
		get
		{
			if (_cachedOverlayHelper == null)
			{
				_cachedOverlayHelper = base.gameObject.GetComponent<UIButtonOverlayOff>();
			}
			return _cachedOverlayHelper;
		}
	}

	public void DisableButton()
	{
		if (buttonEnabled)
		{
			buttonEnabled = false;
		}
	}

	public void EnableButton()
	{
		if (!buttonEnabled)
		{
			buttonEnabled = true;
		}
	}

	private IEnumerator EnableButtonWhenReady()
	{
		float startTime = Time.realtimeSinceStartup;
		float timeWaited = 0f;
		while (timeWaited < 1f)
		{
			timeWaited = Time.realtimeSinceStartup - startTime;
			yield return new WaitForEndOfFrame();
		}
		EnableButton();
	}

	private void OnApplicationPause(bool pause)
	{
		DisableButton();
		if (!pause)
		{
			StartCoroutine(EnableButtonWhenReady());
		}
	}
}
