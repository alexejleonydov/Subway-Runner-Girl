using System;
using System.Collections;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
	private Action _onDidShowCallback;

	private float _readyForNextTimer = 1f;

	private float _slideOutTimer = 3f;

	private bool _triggerReadyForNext;

	private bool _triggerSlideOut;

	protected Vector3 posOff = new Vector3(0f, 250f, 0f);

	protected Vector3 posOn = new Vector3(0f, 0f, 0f);

	private IEnumerator InvokeDidShowCallback(float waitTime)
	{
		float startPoint = 0f;
		while (startPoint < waitTime)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		Action handler = _onDidShowCallback;
		if (handler != null)
		{
			handler();
		}
	}

	public IEnumerator PreloadSlideIn()
	{
		base.gameObject.SetActive(true);
		yield return null;
		yield return null;
		base.gameObject.SetActive(false);
	}

	protected virtual void ReadyForNewMessage()
	{
		base.gameObject.SetActive(false);
		UISliderInController.Instance.ReadyForNextSlide();
	}

	public void SetupSlideIn()
	{
		base.gameObject.SetActive(true);
		SlideIn(null);
	}

	protected virtual void SlideIn(Action onDidShowCallback)
	{
		_onDidShowCallback = onDidShowCallback;
		SpringPosition.Begin(base.gameObject, posOn, 10f).ignoreTimeScale = true;
		_slideOutTimer = 1.5f;
		_readyForNextTimer = 0.5f;
		StartCoroutine("InvokeDidShowCallback", 0.5f);
		_triggerSlideOut = true;
	}

	protected virtual void SlideOut()
	{
		SpringPosition.Begin(base.gameObject, posOff, 10f).ignoreTimeScale = true;
		_triggerReadyForNext = true;
	}

	protected virtual void Start()
	{
		base.transform.localPosition = posOff;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!_triggerReadyForNext && !_triggerSlideOut)
		{
			return;
		}
		float deltaTime = RealTimeTracker.deltaTime;
		if (_triggerSlideOut)
		{
			_slideOutTimer -= deltaTime;
			if (_slideOutTimer <= 0f)
			{
				SlideOut();
				_triggerSlideOut = false;
			}
		}
		if (_triggerReadyForNext)
		{
			_readyForNextTimer -= deltaTime;
			if (_readyForNextTimer <= 0f)
			{
				_triggerReadyForNext = false;
				ReadyForNewMessage();
			}
		}
	}
}
