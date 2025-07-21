public class RaftPickup : IPickup
{
	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (canPickup)
		{
			Game.Instance.Attachment.Add(Game.Instance.Raft);
			if ((bool)pickupCollider)
			{
				pickupCollider.enabled = false;
			}
			canPickup = false;
		}
	}
}
