using System;
using System.Collections;
using UnityEngine;

public class DelayInvoke : MonoBehaviour
{
	private static bool hasInit;

	private static DelayInvoke instance;

	private void Awake()
	{
		base.enabled = false;
		instance = this;
	}

	private static void init()
	{
		if (!hasInit)
		{
			hasInit = true;
			GameObject gameObject = new GameObject("DelayInvoke");
			gameObject.AddComponent<DelayInvoke>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static Coroutine delayDo(Action action, float delayTime, bool ignoreTimeScale = false)
	{
		init();
		if (ignoreTimeScale)
		{
			return instance.StartCoroutine(startIgnoreTimeScale(action, delayTime));
		}
		return instance.StartCoroutine(start(action, delayTime));
	}

	public static IEnumerator start(Action action, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		if (action != null)
		{
			action();
		}
	}

	public static IEnumerator startIgnoreTimeScale(Action action, float delayTime)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + delayTime)
		{
			yield return null;
		}
		if (action != null)
		{
			action();
		}
	}

	public static void stopCoroutine(Coroutine cor)
	{
		init();
		if (cor != null)
		{
			instance.StopCoroutine(cor);
		}
	}
}
