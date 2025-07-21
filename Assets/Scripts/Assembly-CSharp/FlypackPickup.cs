public class FlypackPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup)
		{
			Game.Instance.PickupFlypack();
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
