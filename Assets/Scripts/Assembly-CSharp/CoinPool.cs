using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
	public GameObject coinPrefab;

	public int numDeletedPerCleanup = 4;

	public float cleanupIntervalInSeconds = 5f;

	private List<PickupRotate> activeRotatePickups = new List<PickupRotate>();

	private List<TrackObject> coins;

	private static CoinPool instance;

	private int numberOfActiveCoins;

	private int numberOfActiveCoins_high;

	private Vector3 spawnPoint = -1000f * Vector3.up;

	private Vector3 spawnSpacing = -20f * Vector3.right;

	public static CoinPool Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(CoinPool)) as CoinPool;
			}
			return instance;
		}
	}

	public void Awake()
	{
		coins = new List<TrackObject>();
		GetCoins();
	}

	private IEnumerator CleanUpCoins()
	{
		while (base.enabled)
		{
			int numDeletes = ((coins.Count > numDeletedPerCleanup) ? numDeletedPerCleanup : coins.Count);
			int lastIndex = Mathf.Max(0, coins.Count - (numDeletes + 1));
			for (int i = coins.Count - 1; i >= lastIndex; i--)
			{
				Object.Destroy(coins[i].gameObject);
				coins.RemoveAt(i);
			}
			yield return new WaitForSeconds(cleanupIntervalInSeconds);
		}
	}

	public TrackObject GetCoin(string from)
	{
		TrackObject trackObject = ((coins.Count <= 0) ? MakeNewCoin(coins.Count) : coins[0]);
		coins.Remove(trackObject);
		Transform transform = trackObject.transform;
		if (!transform.gameObject.activeInHierarchy)
		{
			transform.gameObject.SetActive(true);
		}
		numberOfActiveCoins++;
		numberOfActiveCoins_high = Mathf.Max(numberOfActiveCoins_high, numberOfActiveCoins);
		return trackObject;
	}

	private void GetCoins()
	{
		IEnumerator enumerator = base.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Transform transform = (Transform)enumerator.Current;
			TrackObject component = transform.GetComponent<TrackObject>();
			if (component != null)
			{
				coins.Add(component);
			}
		}
	}

	private TrackObject MakeNewCoin(int coinIndex)
	{
		Vector3 position = spawnPoint + spawnSpacing * coinIndex;
		GameObject gameObject = Object.Instantiate(coinPrefab, position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		TrackObject component = gameObject.GetComponent<TrackObject>();
		coins.Add(component);
		return component;
	}

	private void OnEnable()
	{
		StartCoroutine(CleanUpCoins());
	}

	public void Put(List<TrackObject> coins)
	{
		int i = 0;
		for (int count = coins.Count; i < count; i++)
		{
			Put(coins[i]);
		}
	}

	public void Put(TrackObject coin)
	{
		coin.transform.parent = base.transform;
		Vector3 position = coin.transform.position;
		position.y = -1000f;
		coin.transform.position = position;
		coins.Add(coin);
		numberOfActiveCoins--;
	}

	private void Update()
	{
		int i = 0;
		for (int count = activeRotatePickups.Count; i < count; i++)
		{
			if (activeRotatePickups[i].enabled)
			{
				if (activeRotatePickups[i].Z + 50f < Character.Instance.z)
				{
					activeRotatePickups[i].enabled = false;
				}
				else
				{
					activeRotatePickups[i].PhasedRotate();
				}
			}
		}
	}

	public void AddActiveRotatePickups(PickupRotate rotate)
	{
		if (!activeRotatePickups.Contains(rotate))
		{
			activeRotatePickups.Add(rotate);
		}
	}

	public void RemoveActiveRotatePickups(PickupRotate rotate)
	{
		if (activeRotatePickups.Contains(rotate))
		{
			activeRotatePickups.Remove(rotate);
		}
	}

	public static string FindPath(Transform child)
	{
		string text = string.Empty;
		Transform parent = child.parent;
		while (parent != null)
		{
			text = parent.name + '/' + text;
			parent = parent.parent;
		}
		return text;
	}
}
