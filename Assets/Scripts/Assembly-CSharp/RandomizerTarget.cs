using System.Collections.Generic;
using UnityEngine;

public class RandomizerTarget : Randomizer
{
	public List<GameObject> Targets;

	public override void PerformRandomizer(List<GameObject> objects)
	{
		int num = Random.Range(0, Targets.Count);
		for (int i = 0; i < Targets.Count; i++)
		{
			if (i == num)
			{
				objects.Add(Targets[i]);
			}
			else
			{
				Targets[i].SetActive(false);
			}
		}
	}
}
