using System.Collections.Generic;
using UnityEngine;

public class CoinLine : BaseO
{
	public float length = 100f;

	public float coinSpacing = 15f;

	private List<TrackObject> activeCoins;

	private CoinLineManager coinLineManager;

	private CoinPool coinPool;

	private float topLevelHight = 70f;

	protected override void Awake()
	{
		coinPool = CoinPool.Instance;
		coinLineManager = CoinLineManager.Instance;
		activeCoins = new List<TrackObject>();
		base.Awake();
	}

	public override void OnActivate()
	{
		for (float num = 0f; num < length; num += coinSpacing)
		{
			TrackObject coin = coinPool.GetCoin("CoinLine");
			coin.transform.parent = base.transform;
			coin.transform.position = base.transform.position + base.transform.forward * num;
			coin.transform.localScale = Vector3.one;
			if (coin != null)
			{
				coin.Activate();
			}
			activeCoins.Add(coin);
		}
		if (base.transform.position.y > topLevelHight)
		{
			coinLineManager.AddLine(this);
			if (Character.Instance.transform.position.y > topLevelHight && !Game.Instance.IsInFlypackMode)
			{
				ToggleCoinVisibility(true);
			}
			else
			{
				ToggleCoinVisibility(false);
			}
		}
	}

	public override void OnDeactivate()
	{
		RemoveCoins();
		if (base.transform.position.y > topLevelHight)
		{
			coinLineManager.RemoveLine(this);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * length);
		for (float num = 0f; num < length; num += coinSpacing)
		{
			Vector3 center = base.transform.position + base.transform.forward * num;
			Gizmos.DrawSphere(center, 2f);
		}
	}

	public void RemoveCoins()
	{
		int i = 0;
		for (int count = activeCoins.Count; i < count; i++)
		{
			activeCoins[i].Deactivate();
		}
		coinPool.Put(activeCoins);
		activeCoins.Clear();
	}

	public void ToggleCoinVisibility(bool active)
	{
		int i = 0;
		for (int count = activeCoins.Count; i < count; i++)
		{
			Coin component = activeCoins[i].GetComponent<Coin>();
			if (component == null)
			{
				break;
			}
			component.SetVisible(active);
		}
	}
}
