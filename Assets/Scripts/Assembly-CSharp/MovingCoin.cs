using System;
using UnityEngine;

[RequireComponent(typeof(TrackObject))]
public class MovingCoin : MonoBehaviour
{
	private Vector3 originePos;

	private Transform coin;

	private Game game;

	public void Awake()
	{
		game = Game.Instance;
		if (game != null)
		{
			if (base.transform.childCount == 0)
			{
				Debug.Log("No coin child");
			}
			coin = base.transform.GetChild(0);
			TrackObject component = GetComponent<TrackObject>();
			component.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(component.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
			component.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(component.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
			base.enabled = false;
		}
		originePos = base.transform.localPosition;
	}

	public void OnActivate()
	{
		base.enabled = true;
		base.transform.localPosition = originePos;
		coin.localPosition = Vector3.zero;
	}

	public void OnDeactivate()
	{
		base.enabled = false;
	}

	public void OnDrawGizmos()
	{
		if (coin != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(coin.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(coin.position, 5f);
		}
	}
}
