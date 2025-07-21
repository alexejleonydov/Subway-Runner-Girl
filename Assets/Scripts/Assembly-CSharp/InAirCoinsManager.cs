using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirCoinsManager : MonoBehaviour
{
	public GameObject coinPrefab;

	public int numberOfCoins = 200;

	public float stayInTrackDistance = 60f;

	public float coinDistance = 30f;

	private CoinPool coinPool;

	private List<TrackObject> coins = new List<TrackObject>();

	private AnimationCurve curve;

	private Flypack flypack;

	private TrackController trackController;

	public void Awake()
	{
		flypack = Flypack.Instance;
		trackController = TrackController.Instance;
		coinPool = CoinPool.Instance;
	}

	private IEnumerator MoveCoins(float StartZ, float length, float height)
	{
		float z = StartZ;
		while (z < StartZ + length)
		{
			TrackObject coin = coinPool.GetCoin("InAirCoinsManager");
			coin.transform.position = Vector3.up * height + trackController.GetPosition(curve.Evaluate(z), z);
			coin.transform.localScale = Vector3.one;
			coin.Activate();
			z += coinDistance;
			coins.Add(coin);
			yield return null;
		}
	}

	public void ReleaseCoins()
	{
		int i = 0;
		for (int count = coins.Count; i < count; i++)
		{
			coins[i].Deactivate();
		}
		coinPool.Put(coins);
		coins.Clear();
	}

	public void Spawn(float startZ, float length, float height)
	{
		curve = new AnimationCurve();
		int num = 1;
		for (float num2 = startZ; num2 < startZ + length; num2 += flypack.characterChangeTrackLength + stayInTrackDistance)
		{
			curve.AddKey(new Keyframe(num2, trackController.GetTrackX(num)));
			curve.AddKey(new Keyframe(num2 + stayInTrackDistance, trackController.GetTrackX(num)));
			num = Mathf.Clamp(num + Random.Range(-1, 2), 0, trackController.NumberOfTracks - 1);
			curve.AddKey(new Keyframe(num2 + stayInTrackDistance + flypack.characterChangeTrackLength, trackController.GetTrackX(num)));
		}
		StartCoroutine(MoveCoins(startZ, length, height));
	}
}
