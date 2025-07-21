using UnityEngine;

public class Coin : IPickup
{
	private Character character;

	private GameStats gameStats;

	private Vector3 initialGlowPosition;

	private Vector3 initialPivotPosition;

	private Transform pivot;

	public Transform PivotTransform
	{
		get
		{
			return pivot;
		}
	}

	protected override void Awake()
	{
		character = Character.Instance;
		gameStats = GameStats.Instance;
		pivot = base.transform.GetChild(0);
		initialPivotPosition = pivot.localPosition;
		base.Awake();
		if (glow != null)
		{
			initialGlowPosition = glow.transform.localPosition;
		}
	}

	private void Start()
	{
		if (glow.meshRenderer == null)
		{
			InitAssets.Instance.NotifyInitMaterials(new Renderer[1] { meshRenderer });
		}
		else
		{
			InitAssets.Instance.NotifyInitMaterials(new Renderer[2] { meshRenderer, glow.meshRenderer });
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		pivot.localPosition = initialPivotPosition;
		if (glow != null)
		{
			glow.transform.localPosition = initialGlowPosition;
		}
	}

	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (canPickup)
		{
			int num = 1;
			if (PlayerInfo.Instance.hasDoubleCoins)
			{
				num = 2;
			}
			gameStats.coins += num;
			if (Helmet.Instance.IsActive)
			{
				gameStats.coinsWithHelmet++;
			}
			if (character.IsAboveGround)
			{
				gameStats.coinsInAir++;
			}
			if (Flypack.Instance.isActive)
			{
				gameStats.coinsWithFlypack++;
			}
			if (SpringJump.Instance.isActive)
			{
				gameStats.coinsWithSpringJump++;
			}
			if (character.trackIndex == 0)
			{
				gameStats.coinsCollectedOnLeftTrack++;
			}
			else if (character.trackIndex == 1)
			{
				gameStats.coinsCollectedOnCenterTrack++;
			}
			else if (character.trackIndex == 2)
			{
				gameStats.coinsCollectedOnRightTrack++;
			}
			pickupParticles.PickedupCoin(this);
			base.SetVisible(false);
			canPickup = false;
		}
	}
}
