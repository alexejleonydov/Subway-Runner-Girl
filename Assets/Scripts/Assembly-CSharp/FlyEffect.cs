using System;
using System.Collections;
using UnityEngine;

public class FlyEffect : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] particleSystems;

	[SerializeField]
	private TrailRenderer[] trailRenderers;

	private Vector2 start;

	private Vector2 end;

	private float duration;

	private void Start()
	{
		int i = 0;
		for (int num = trailRenderers.Length; i < num; i++)
		{
			trailRenderers[i].enabled = false;
		}
	}

	private void Fly()
	{
		int i = 0;
		for (int num = particleSystems.Length; i < num; i++)
		{
			particleSystems[i].Play();
		}
		int j = 0;
		for (int num2 = trailRenderers.Length; j < num2; j++)
		{
			trailRenderers[j].enabled = true;
		}
	}

	private void End()
	{
		int i = 0;
		for (int num = particleSystems.Length; i < num; i++)
		{
			particleSystems[i].Stop();
		}
		int j = 0;
		for (int num2 = trailRenderers.Length; j < num2; j++)
		{
			trailRenderers[j].enabled = false;
		}
	}

	public void Flying(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		this.start = start;
		this.end = end;
		this.duration = duration;
		StartCoroutine(Flying_C(action));
	}

	private IEnumerator Flying_C(Action action = null)
	{
		base.transform.position = start;
		Fly();
		float factor2 = particleSystems[0].main.startLifetimeMultiplier * 0.3f;
		while (factor2 > 0f)
		{
			factor2 -= Time.deltaTime;
			yield return null;
		}
		factor2 = 0f;
		yield return null;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			base.transform.position = Vector2.Lerp(start, end, factor2);
			yield return null;
		}
		base.transform.position = end;
		End();
		if (action != null)
		{
			action();
		}
	}
}
