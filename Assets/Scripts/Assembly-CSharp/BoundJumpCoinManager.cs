using System.Collections.Generic;
using UnityEngine;

public class BoundJumpCoinManager : MonoBehaviour
{
	private static BoundJumpCoinManager instance;

	private List<BoundJumpCoins> topLevelPlaced = new List<BoundJumpCoins>();

	public static BoundJumpCoinManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(BoundJumpCoinManager)) as BoundJumpCoinManager;
			}
			return instance;
		}
	}

	public void Add(BoundJumpCoins line)
	{
		topLevelPlaced.Add(line);
	}

	public void ClearTopLevelLines()
	{
		int i = 0;
		for (int count = topLevelPlaced.Count; i < count; i++)
		{
			topLevelPlaced[i].RemoveCoins();
		}
		Reset();
	}

	public void Remove(BoundJumpCoins line)
	{
		line.ToggleCoinVisibility(true);
		topLevelPlaced.Remove(line);
	}

	public void Reset()
	{
		topLevelPlaced.Clear();
	}
}
