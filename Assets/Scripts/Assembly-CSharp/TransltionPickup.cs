using UnityEngine;

public class TransltionPickup : IPickup
{
	private int nextTrackOrder;

	private void Start()
	{
		TrackPiece trackPiece = null;
		Transform parent = base.transform;
		while ((trackPiece = parent.GetComponent<TrackPiece>()) == null)
		{
			parent = parent.parent;
		}
		if (trackPiece == null)
		{
			Debug.LogError("Transltion is not under TrackPiece!");
		}
		else
		{
			nextTrackOrder = trackPiece.nextCityOrder;
		}
	}

	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (canPickup)
		{
			TrackController.Instance.nextTrackOrder = nextTrackOrder;
			Game.Instance.PickupTransition();
			base.NotifyPickup(pickupParticles);
		}
	}
}
