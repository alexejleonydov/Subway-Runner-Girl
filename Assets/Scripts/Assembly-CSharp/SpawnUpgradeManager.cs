using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUpgradeManager
{
	private class PickupType
	{
		public Func<SpawnUpgrade, GameObject> ExtractGameObject;

		public int spawnProbability;

		public float spawnDistanceMin;

		public float spawnZ;
	}

	private PickupType doubleScoreMultiplier;

	private static SpawnUpgradeManager instance;

	private PickupType flypackPickup;

	private PickupType superShoes;

	private PickupType magnetBooster;

	private List<PickupType> pickups;

	private System.Random randomGen = new System.Random();

	private PickupType gem;

	private float spawnSpacing;

	private float spawnZ;

	private int flypackSpawnProbability;

	public static SpawnUpgradeManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SpawnUpgradeManager();
			}
			return instance;
		}
	}

	public SpawnUpgradeManager()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		Upgrade upgrade = Upgrades.upgrades[PropType.doubleMultiplier];
		doubleScoreMultiplier = new PickupType();
		doubleScoreMultiplier.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		doubleScoreMultiplier.spawnProbability = upgrade.spawnProbability;
		doubleScoreMultiplier.ExtractGameObject = (SpawnUpgrade spawnPoint) => spawnPoint.doubleScoreMultiplier;
		upgrade = Upgrades.upgrades[PropType.flypack];
		flypackPickup = new PickupType();
		flypackPickup.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		flypackPickup.spawnProbability = upgrade.spawnProbability;
		flypackPickup.ExtractGameObject = (SpawnUpgrade spawnPoint) => spawnPoint.flypackPickup;
		upgrade = Upgrades.upgrades[PropType.supershoes];
		superShoes = new PickupType();
		superShoes.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		superShoes.spawnProbability = upgrade.spawnProbability;
		superShoes.ExtractGameObject = (SpawnUpgrade spawnPoint) => spawnPoint.superShoes;
		upgrade = Upgrades.upgrades[PropType.coinmagnet];
		magnetBooster = new PickupType();
		magnetBooster.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		magnetBooster.spawnProbability = upgrade.spawnProbability;
		magnetBooster.ExtractGameObject = (SpawnUpgrade spawnPoint) => spawnPoint.magnetBooster;
		upgrade = Upgrades.upgrades[PropType.gem];
		gem = new PickupType();
		gem.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		gem.spawnProbability = upgrade.spawnProbability;
		gem.ExtractGameObject = (SpawnUpgrade spawnPoint) => spawnPoint.gem;
		pickups = new List<PickupType> { doubleScoreMultiplier, flypackPickup, superShoes, magnetBooster, gem };
		flypackSpawnProbability = flypackPickup.spawnProbability;
	}

	public bool CanSpawnPickup(float z)
	{
		return z > spawnZ;
	}

	private void CheckFlypackSpawnRate()
	{
		if (!TrackController.Instance.AllowFlypack())
		{
			if (flypackPickup.spawnProbability != 0)
			{
				flypackPickup.spawnProbability = 0;
			}
		}
		else if (flypackPickup.spawnProbability != flypackSpawnProbability)
		{
			flypackPickup.spawnProbability = flypackSpawnProbability;
		}
	}

	public void PerformSelection(SpawnUpgrade spawn, List<GameObject> objectsToVisit)
	{
		float z = spawn.transform.position.z;
		PickupType pickupType = null;
		CheckFlypackSpawnRate();
		if (CanSpawnPickup(z))
		{
			List<PickupType> list = pickups.FindAll((PickupType p) => p.spawnZ < z);
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					num = (array[i] = num + list[i].spawnProbability);
				}
				int num2 = randomGen.Next(0, num + 1);
				for (int j = 0; j < array.Length; j++)
				{
					if (num2 <= array[j])
					{
						pickupType = list[j];
						pickupType.spawnZ = z + pickupType.spawnDistanceMin;
						break;
					}
				}
				SetNextSpawnPositionZ(z);
			}
		}
		if (pickupType == flypackPickup)
		{
			TrackController.Instance.LastFlypackSpawnZ = z;
		}
		for (int k = 0; k < pickups.Count; k++)
		{
			GameObject gameObject = pickups[k].ExtractGameObject(spawn);
			if (pickups[k] == pickupType)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void Restart()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		spawnZ = 250f * distancePerMeter;
		spawnSpacing = 300f * distancePerMeter;
		int i = 0;
		for (int count = pickups.Count; i < count; i++)
		{
			pickups[i].spawnZ = float.MinValue;
		}
	}

	public void SetNextSpawnPositionZ(float z)
	{
		spawnZ = z + spawnSpacing;
	}
}
