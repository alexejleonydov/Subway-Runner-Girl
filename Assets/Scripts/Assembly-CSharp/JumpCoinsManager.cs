using System.Collections.Generic;
using UnityEngine;

public class JumpCoinsManager
{
	private CoinPool coinPool;

	private List<TrackObject> coins = new List<TrackObject>();

	private TrackController trackController;

	private static JumpCoinsManager _instance;

	public static JumpCoinsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new JumpCoinsManager();
			}
			return _instance;
		}
	}

	public JumpCoinsManager()
	{
		trackController = TrackController.Instance;
		coinPool = CoinPool.Instance;
	}

	private void MoveCoin(Vector3 position)
	{
		TrackObject coin = coinPool.GetCoin("JumpCoinsManage");
		coin.transform.position = position;
		coin.transform.localScale = Vector3.one;
		coin.Activate();
		coins.Add(coin);
	}

	public void placeRow(float z, float height)
	{
		MoveCoin(trackController.GetPosition(0f, z) + Vector3.up * height);
		MoveCoin(trackController.GetPosition(20f, z) + Vector3.up * height);
		MoveCoin(trackController.GetPosition(40f, z) + Vector3.up * height);
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
}
