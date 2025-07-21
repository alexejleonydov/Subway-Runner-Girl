using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubScene
{
	public string mainMusic;

	public int minLength;

	public int maxLength;

	public bool allowCityEnd;

	public bool allowFlypack;

	public int order;

	private Dictionary<TrackPieceType, List<TrackPiece>> trackPieces;

	public Dictionary<TrackPieceType, List<TrackPiece>> TrackPieces
	{
		get
		{
			return trackPieces;
		}
	}

	public float LastZ { get; set; }

	public SubScene()
	{
		trackPieces = new Dictionary<TrackPieceType, List<TrackPiece>>();
		LastZ = 0f;
	}

	public void AddToDict(TrackPiece tp)
	{
		if (!trackPieces.ContainsKey(tp.trackPieceType))
		{
			List<TrackPiece> list = new List<TrackPiece>();
			list.Add(tp);
			trackPieces.Add(tp.trackPieceType, list);
			return;
		}
		List<TrackPiece> list2 = trackPieces[tp.trackPieceType];
		int num = 0;
		int count = list2.Count;
		while (list2[num].zMinimum < tp.zMinimum)
		{
			num++;
			if (num == count)
			{
				break;
			}
		}
		list2.Insert(num, tp);
	}

	public void Restart()
	{
		List<TrackPiece> list = null;
		Vector3 zero = Vector3.zero;
		foreach (KeyValuePair<TrackPieceType, List<TrackPiece>> trackPiece in trackPieces)
		{
			list = trackPiece.Value;
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				zero = list[i].transform.position;
				zero.y = -1000f;
				list[i].transform.position = zero;
			}
		}
		LastZ = 0f;
	}
}
