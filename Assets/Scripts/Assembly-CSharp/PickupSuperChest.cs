public class PickupSuperChest : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
			GameStats.Instance.superChestPickups++;
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
