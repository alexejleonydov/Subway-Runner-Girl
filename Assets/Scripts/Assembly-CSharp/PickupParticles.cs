using System;
using System.Collections;
using UnityEngine;

public class PickupParticles : MonoBehaviour
{
	[Serializable]
	private class EffectDetails
	{
		public Transform target;

		public float duration = 0.05f;

		public AnimationCurve scaleCurve;
	}

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	public AnimationCurve compressCurve;

	[SerializeField]
	private EffectDetails coinEffect;

	[SerializeField]
	private EffectDetails powerupEffect;

	[SerializeField]
	private ParticleSystem coinPs;

	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	private int coinStairway;

	private int flyCount;

	private Flypack flypack;

	private int[] slendro = new int[17]
	{
		12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
		22, 23, 24, 25, 26, 27, 28
	};

	private void Start()
	{
		flypack = Flypack.Instance;
		flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(flypack.OnStop, new Flypack.OnStopDelegate(OnFlypackStop));
	}

	private IEnumerator EffectCoroutine(EffectDetails details)
	{
		details.target.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Renderer render = details.target.GetComponent<Renderer>();
		Material material = render.sharedMaterial;
		yield return StartCoroutine(myTween.To(details.duration, delegate(float t)
		{
			render.enabled = true;
			material.SetColor(Shaders.Instance.MainColor, Color.Lerp(Color.white, Color.black, t));
			details.target.localScale = Vector3.one * details.scaleCurve.Evaluate(t);
		}));
		render.enabled = false;
	}

	private void OnDestroy()
	{
		if (!(flypack == null))
		{
			flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Remove(flypack.OnStop, new Flypack.OnStopDelegate(OnFlypackStop));
		}
	}

	private void OnFlypackStop()
	{
		flyCount = 0;
	}

	public void PickedupCoin(IPickup pickup)
	{
		float pitch = AudioPlayer.Instance.GetAudioSource("leyou_Hr_coin").pitch;
		if (80f < pickup.transform.position.y)
		{
			coinStairway = 0;
			pitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyCount / 48f));
			flyCount++;
		}
		else if (pickup.transform.position.y < 0.1f || (8.795f < pickup.transform.position.y && pickup.transform.position.y < 8.805f) || (9.95f < pickup.transform.position.y && pickup.transform.position.y < 10.05f) || (28.95f < pickup.transform.position.y && pickup.transform.position.y < 29.05f) || (34.95f < pickup.transform.position.y && pickup.transform.position.y < 35.05f))
		{
			flyCount = 0;
			coinStairway = 0;
			pitch = Mathf.Pow(2f, (float)slendro[coinStairway % slendro.Length] / 12f) * 0.5f;
		}
		else
		{
			flyCount = 0;
			if (coinStairway < slendro.Length - 1)
			{
				coinStairway++;
			}
			pitch = Mathf.Pow(2f, (float)slendro[coinStairway % slendro.Length] / 12f) * 0.5f;
		}
		AudioPlayer.Instance.PlaySound("leyou_Hr_coin", pitch, pickup.transform.position);
		coinPs.Play();
	}

	public void PickedupDefaultPowerUp()
	{
		StartCoroutine(EffectCoroutine(powerupEffect));
	}

	public void PickedupPowerUp()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
		PickedupDefaultPowerUp();
	}
}
