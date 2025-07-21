public class SuperChestPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
			GameStats.Instance.superChestPickups++;
			particles.PickedupPowerUp();
			canPickup = false;
		}
	}
}
