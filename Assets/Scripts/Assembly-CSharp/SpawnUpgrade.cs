using System.Collections.Generic;
using UnityEngine;

public class SpawnUpgrade : Randomizer
{
	public GameObject doubleScoreMultiplier;

	public GameObject flypackPickup;

	public GameObject superShoes;

	public GameObject magnetBooster;

	public GameObject gem;

	public override void PerformRandomizer(List<GameObject> objects)
	{
		SpawnUpgradeManager.Instance.PerformSelection(this, objects);
	}
}
