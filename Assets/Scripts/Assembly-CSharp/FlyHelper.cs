using System;
using System.Collections;
using UnityEngine;

public class FlyHelper : MonoBehaviour
{
	[SerializeField]
	private Transform item;

	[SerializeField]
	private Transform ps;

	[SerializeField]
	private Transform start;

	[SerializeField]
	private Transform end;

	[SerializeField]
	private AnimationCurve curveY;

	[SerializeField]
	private AnimationCurve curveScale;

	private ParticleSystem currentFlyPs;

	private ParticleSystem currentBoomPs;

	private float duration;

	public void Selecte(ParticleSystem flyPs, ParticleSystem boomPs)
	{
		currentFlyPs = flyPs;
		currentBoomPs = boomPs;
		item.position = start.position;
		item.localScale = Vector2.one;
		currentFlyPs.transform.position = item.position;
		item.gameObject.SetActive(true);
	}

	public void Flying(float duration, Action action = null)
	{
		this.duration = duration;
		StartCoroutine(Flying_C(action));
	}

	public void OpenBoxFlying(float duration, Action action = null)
	{
		this.duration = duration;
		StartCoroutine(Flying_OpenBox(action));
	}

	private IEnumerator Flying_OpenBox(Action action = null)
	{
		currentFlyPs.Play();
		AudioPlayer.Instance.PlaySound("prop_fly");
		yield return null;
		float factor = 0f;
		Vector2 startLPos = start.localPosition;
		Vector2 endLPos = base.transform.InverseTransformPoint(end.position);
		AudioPlayer.Instance.PlaySound("prop_boom");
		currentBoomPs.transform.position = start.position;
		currentBoomPs.Play();
		while (factor < 1f)
		{
			factor += Time.unscaledDeltaTime / duration;
			item.localPosition = new Vector3(Mathf.Lerp(startLPos.x, endLPos.x, factor), Mathf.Lerp(startLPos.y, endLPos.y, curveY.Evaluate(factor)), 0f);
			item.localScale = Vector2.one * curveScale.Evaluate(factor);
			currentFlyPs.transform.position = item.position;
			yield return null;
		}
		AudioPlayer.Instance.StopSound("prop_fly");
		item.localPosition = endLPos;
		item.gameObject.SetActive(false);
		currentFlyPs.Stop();
		currentFlyPs.transform.position = start.position;
		if (action != null)
		{
			action();
		}
	}

	private IEnumerator Flying_C(Action action = null)
	{
		currentFlyPs.Play();
		AudioPlayer.Instance.PlaySound("prop_fly");
		yield return null;
		float factor2 = 0f;
		Vector2 startLPos = start.localPosition;
		Vector2 endLPos = base.transform.InverseTransformPoint(end.position);
		while (factor2 < 1f)
		{
			factor2 += Time.unscaledDeltaTime / duration;
			item.localPosition = new Vector3(Mathf.Lerp(startLPos.x, endLPos.x, factor2), Mathf.Lerp(startLPos.y, endLPos.y, curveY.Evaluate(factor2)), 0f);
			item.localScale = Vector2.one * curveScale.Evaluate(factor2);
			currentFlyPs.transform.position = item.position;
			yield return null;
		}
		AudioPlayer.Instance.StopSound("prop_fly");
		item.localPosition = endLPos;
		item.gameObject.SetActive(false);
		currentBoomPs.transform.position = end.position;
		currentBoomPs.Play();
		factor2 = currentBoomPs.main.startLifetimeMultiplier;
		AudioPlayer.Instance.PlaySound("prop_boom");
		while (factor2 > 0f)
		{
			factor2 -= Time.unscaledDeltaTime;
			yield return null;
		}
		currentFlyPs.Stop();
		currentFlyPs.transform.position = start.position;
		if (action != null)
		{
			action();
		}
	}
}
