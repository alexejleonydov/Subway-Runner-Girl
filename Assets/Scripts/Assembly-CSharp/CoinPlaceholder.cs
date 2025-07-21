using UnityEngine;

public class CoinPlaceholder : BaseO
{
	private TrackObject coin;

	private CoinPool coinPool;

	protected override void Awake()
	{
		coinPool = CoinPool.Instance;
		base.Awake();
	}

	public override void OnActivate()
	{
		coin = coinPool.GetCoin("CoinPlaceholder");
		coin.transform.parent = base.transform;
		coin.transform.position = base.transform.position;
		coin.transform.localScale = Vector3.one;
		coin.Activate();
	}

	public override void OnDeactivate()
	{
		if (coin != null)
		{
			coin.Deactivate();
			coinPool.Put(coin);
			coin = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 2f);
	}
}
