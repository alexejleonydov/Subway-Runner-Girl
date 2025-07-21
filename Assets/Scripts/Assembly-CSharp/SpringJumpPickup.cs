using UnityEngine;

public class SpringJumpPickup : IPickup
{
	[SerializeField]
	private bool willShowPickup;

	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup && !Game.Instance.IsInFlypackMode)
		{
			Game.Instance.PickupPogostick(willShowPickup);
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
