using UnityEngine;

public class ChestPickup : BaseO
{
	[SerializeField]
	private Collider pickupCollider;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private GameObject lightCurtain;

	private bool canPickup;

	public override void OnActivate()
	{
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = true;
		}
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = true;
		}
		if ((bool)lightCurtain)
		{
			lightCurtain.SetActive(true);
		}
		canPickup = true;
	}

	public override void OnDeactivate()
	{
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = false;
		}
		if ((bool)lightCurtain)
		{
			lightCurtain.SetActive(false);
		}
		canPickup = false;
	}

	public void NotifyPickup(PickupParticles particles)
	{
		if (canPickup)
		{
			GameStats.Instance.chestPickups += 1 << TrackController.Instance.nextChestIndex - 1;
			particles.PickedupPowerUp();
			GameStats.Instance.AddScoreForPickup(PropType.chest);
			OnDeactivate();
		}
	}
}
