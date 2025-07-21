using System.Collections.Generic;
using UnityEngine;

public class CoinLineManager : MonoBehaviour
{
	private static CoinLineManager instance;

	private List<CoinLine> topLevelPlaced = new List<CoinLine>();

	public static CoinLineManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(CoinLineManager)) as CoinLineManager;
			}
			return instance;
		}
	}

	public void AddLine(CoinLine line)
	{
		topLevelPlaced.Add(line);
	}

	public void ClearTopLevelLines()
	{
		int i = 0;
		for (int count = topLevelPlaced.Count; i < count; i++)
		{
			topLevelPlaced[i].ToggleCoinVisibility(true);
			topLevelPlaced[i].RemoveCoins();
		}
		Reset();
	}

	public void RemoveLine(CoinLine line)
	{
		line.ToggleCoinVisibility(true);
		topLevelPlaced.Remove(line);
	}

	public void Reset()
	{
		topLevelPlaced.Clear();
	}

	public void ToggleLines(bool active)
	{
		int i = 0;
		for (int count = topLevelPlaced.Count; i < count; i++)
		{
			topLevelPlaced[i].ToggleCoinVisibility(active);
		}
	}
}
