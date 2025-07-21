public class GemPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup)
		{
			PlayerInfo.Instance.AddSaveGemToUnlock();
			GameStats.Instance.saveMeSymbolPickup++;
			particles.PickedupPowerUp();
			GameStats.Instance.AddScoreForPickup(PropType.gem);
			PlayerInfo.Instance.stats[Stat.KeysCollected]++;
			base.NotifyPickup(particles);
		}
	}
}
