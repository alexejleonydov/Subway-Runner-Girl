using System.Collections.Generic;
using UnityEngine;

public class BoundJumpCoins : BaseO
{
	public string poolName = "Coin";

	private CoinPool pool;

	public float deltaX;

	public int rows = 18;

	public int startRowPosition = 3;

	public int endRowPosition = 10;

	public int randomPickupPosition;

	public AnimationCurve coinX;

	public float jumpHeight = 70f;

	public float jumpDistance = 400f;

	private List<TrackObject> activeobjs = new List<TrackObject>();

	private BoundJumpCoinManager boundJumpCoinManager;

	protected override void Awake()
	{
		pool = CoinPool.Instance;
		if (pool == null)
		{
			Debug.LogWarning("The PoolManager has not a " + poolName + " spawnPool!!!");
			return;
		}
		boundJumpCoinManager = BoundJumpCoinManager.Instance;
		base.Awake();
	}

	public override void OnActivate()
	{
		Vector3 localPosition = Vector3.zero;
		int num = startRowPosition;
		float num2 = 0f;
		while (num <= endRowPosition)
		{
			if (num == randomPickupPosition)
			{
				num++;
				continue;
			}
			num2 = (float)num / (float)rows;
			localPosition = new Vector3(coinX.Evaluate(num2) * deltaX, ObliqueMotion.CalcHeight(num2) * jumpHeight + base.transform.localPosition.y, jumpDistance * num2);
			TrackObject coin = pool.GetCoin("BoundJumpCoins");
			Transform transform = coin.transform;
			transform.parent = base.transform;
			transform.localPosition = localPosition;
			transform.localScale = Vector3.one;
			coin.Activate();
			activeobjs.Add(coin);
			num++;
		}
		boundJumpCoinManager.Add(this);
	}

	public override void OnDeactivate()
	{
		boundJumpCoinManager.Remove(this);
		RemoveCoins();
	}

	public void RemoveCoins()
	{
		int i = 0;
		for (int count = activeobjs.Count; i < count; i++)
		{
			activeobjs[i].Deactivate();
		}
		pool.Put(activeobjs);
		activeobjs.Clear();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		float num = startRowPosition;
		Vector3 center = Vector3.zero;
		float num2 = 0f;
		while (num <= (float)endRowPosition)
		{
			if (num == (float)randomPickupPosition)
			{
				num += 1f;
				continue;
			}
			num2 = num / (float)rows;
			center = new Vector3(base.transform.position.x + coinX.Evaluate(num2) * deltaX, ObliqueMotion.CalcHeight(num2) * jumpHeight + base.transform.position.y, jumpDistance * num2 + base.transform.position.z);
			Gizmos.DrawSphere(center, 2f);
			num += 1f;
		}
	}

	public void ToggleCoinVisibility(bool active)
	{
		int i = 0;
		for (int count = activeobjs.Count; i < count; i++)
		{
			if (!active)
			{
				activeobjs[i].Deactivate();
			}
			else
			{
				activeobjs[i].Activate();
			}
		}
	}
}
