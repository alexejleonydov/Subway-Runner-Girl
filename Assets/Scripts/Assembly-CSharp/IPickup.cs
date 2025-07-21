using UnityEngine;

public class IPickup : BaseO
{
	public Collider pickupCollider;

	public MeshRenderer meshRenderer;

	public Glow glow;

	protected bool canPickup;

	public virtual void SetVisible(bool visible)
	{
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = visible;
		}
		if ((bool)glow)
		{
			glow.SetVisible(visible);
		}
	}

	public virtual void Activate()
	{
		base.gameObject.SetActive(true);
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = true;
		}
		SetVisible(true);
		canPickup = true;
	}

	public virtual void Deactivate()
	{
		SetVisible(false);
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = false;
		}
		canPickup = false;
		base.gameObject.SetActive(false);
	}

	public override void OnActivate()
	{
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = true;
		}
		SetVisible(true);
		canPickup = true;
	}

	public override void OnDeactivate()
	{
		SetVisible(false);
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = false;
		}
		canPickup = false;
	}

	public virtual void NotifyPickup(PickupParticles pickupParticles)
	{
		if ((bool)pickupCollider)
		{
			pickupCollider.enabled = false;
		}
		SetVisible(false);
		canPickup = false;
	}
}
