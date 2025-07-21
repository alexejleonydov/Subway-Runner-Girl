using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestModelFactory : MonoBehaviour
{
	[Serializable]
	public class ChestSelection
	{
		public ChestType chestType;

		public GameObject chestPrefabInCell;

		public GameObject chestPrefabForOpen;
	}

	[SerializeField]
	private ChestSelection[] chestSelections;

	private Dictionary<ChestType, GameObject> chestInCellThatMatchChestType = new Dictionary<ChestType, GameObject>();

	private Dictionary<ChestType, GameObject> chestForOpenThatMatchChestType = new Dictionary<ChestType, GameObject>();

	private static ChestModelFactory _instance;

	public static ChestModelFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Utils.FindObject<ChestModelFactory>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		int i = 0;
		for (int num = chestSelections.Length; i < num; i++)
		{
			if (chestInCellThatMatchChestType.ContainsKey(chestSelections[i].chestType))
			{
				throw new Exception("There are more helmets assigned to the helmet selection");
			}
			chestInCellThatMatchChestType.Add(chestSelections[i].chestType, chestSelections[i].chestPrefabInCell);
			if (chestForOpenThatMatchChestType.ContainsKey(chestSelections[i].chestType))
			{
				throw new Exception("There are more helmets assigned to the helmet selection");
			}
			chestForOpenThatMatchChestType.Add(chestSelections[i].chestType, chestSelections[i].chestPrefabForOpen);
		}
	}

	public GameObject GetChestInCell(ChestType chestType)
	{
		GameObject value;
		if (chestInCellThatMatchChestType.TryGetValue(chestType, out value))
		{
			return value;
		}
		return null;
	}

	public GameObject GetChestForOpen(ChestType chestType)
	{
		GameObject value;
		if (chestForOpenThatMatchChestType.TryGetValue(chestType, out value))
		{
			return value;
		}
		return null;
	}
}
