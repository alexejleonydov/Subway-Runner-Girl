using System.Collections.Generic;
using UnityEngine;

public class TrackPieceController
{
	private List<TrackPiece> activeTrackPieces = new List<TrackPiece>();

	private int lastAddedIndex = -1;

	private List<int> randomSpace = new List<int>();

	private Dictionary<TrackPieceType, List<TrackPiece>> trackPieces;

	public Dictionary<TrackPieceType, List<TrackPiece>> TrackPieces
	{
		get
		{
			return trackPieces;
		}
	}

	public void SetTrackPieces(SubScene sub)
	{
		if (sub != null)
		{
			trackPieces = sub.TrackPieces;
		}
	}

	public bool CanDeliver()
	{
		return randomSpace.Count > 0;
	}

	public TrackPiece GetJetPakPiece(int index)
	{
		if ((float)index < 0f || index >= trackPieces[TrackPieceType.Jetpack].Count)
		{
			Debug.Log(string.Concat(new object[2] { "Illegal TrackChunk used in jetpack mode. Index=", index }));
			Debug.Break();
			return null;
		}
		return trackPieces[TrackPieceType.Jetpack][index];
	}

	public TrackPiece GetTransitionPiece(int index)
	{
		if ((float)index < 0f || index >= trackPieces[TrackPieceType.Transition].Count)
		{
			Debug.Log(string.Concat(new object[2] { "Illegal TrackChunk used in Transition mode. Index=", index }));
			Debug.Break();
			return null;
		}
		return trackPieces[TrackPieceType.Transition][index];
	}

	public TrackPiece GetPieceBySpecialTrackPieceType(TrackPieceType type)
	{
		activeTrackPieces.RemoveAll((TrackPiece piece) => piece.trackPieceType != TrackPieceType.Normal);
		activeTrackPieces.AddRange(trackPieces[type]);
		Recalculate(true);
		return GetRandomActive();
	}

	public TrackPiece GetRandomActive()
	{
		int index = Random.Range(0, randomSpace.Count);
		int index2 = randomSpace[index];
		return activeTrackPieces[index2];
	}

	public void Initialize(float z)
	{
		activeTrackPieces.Clear();
		lastAddedIndex = -1;
		List<TrackPiece> list = trackPieces[TrackPieceType.Normal];
		for (int i = 0; i < list.Count; i++)
		{
			TrackPiece trackPiece = list[i];
			if (trackPiece.zMinimum <= z && z < trackPiece.zMaximum)
			{
				activeTrackPieces.Add(trackPiece);
				lastAddedIndex = i;
			}
		}
		Recalculate(false);
	}

	public void MoveForward(float z, bool forcePutChest, bool changeEnvEnable, bool forceChangeEnv, bool changeCityEnable, bool forceChangeCity)
	{
		int num = 0;
		activeTrackPieces.RemoveAll((TrackPiece piece) => piece.trackPieceType != TrackPieceType.Normal);
		List<TrackPieceType> list = new List<TrackPieceType>(trackPieces.Keys);
		int i = 0;
		for (int count = trackPieces.Count; i < count; i++)
		{
			TrackPieceType trackPieceType = list[i];
			if (trackPieceType == TrackPieceType.Tutorial || trackPieceType == TrackPieceType.Jetpack || trackPieceType == TrackPieceType.Jetpacklanding || (forcePutChest && trackPieceType != TrackPieceType.Chest) || (!forcePutChest && trackPieceType == TrackPieceType.Chest) || (forceChangeEnv && trackPieceType != TrackPieceType.SubsceneTransition) || (forceChangeCity && trackPieceType != TrackPieceType.CityTransition) || (!changeCityEnable && trackPieceType == TrackPieceType.CityTransition) || (!changeEnvEnable && trackPieceType == TrackPieceType.SubsceneTransition))
			{
				continue;
			}
			if (trackPieceType == TrackPieceType.Normal)
			{
				int j = lastAddedIndex + 1;
				for (int count2 = trackPieces[TrackPieceType.Normal].Count; j < count2; j++)
				{
					TrackPiece trackPiece = trackPieces[TrackPieceType.Normal][j];
					if (trackPiece.zMinimum > z)
					{
						break;
					}
					activeTrackPieces.Add(trackPiece);
					num++;
					lastAddedIndex = j;
				}
			}
			else
			{
				activeTrackPieces.AddRange(trackPieces[trackPieceType]);
			}
		}
		activeTrackPieces.RemoveAll(delegate(TrackPiece piece)
		{
			if (piece.trackPieceType == TrackPieceType.CityTransition)
			{
				if (TrackController.Instance.forceNextCityOrder > 0)
				{
					return piece.nextCityOrder != TrackController.Instance.forceNextCityOrder;
				}
				City city = TrackController.Instance.GetCity(piece.nextCityOrder);
				if (city == null)
				{
					return true;
				}
				return city.lockedTaskSet > TrackController.Instance.taskSet;
			}
			return piece.trackPieceType == TrackPieceType.Normal && piece.zMaximum < z;
		});
		Recalculate(forcePutChest || forceChangeEnv || forceChangeCity);
	}

	private void Recalculate(bool onlySpecial)
	{
		randomSpace.Clear();
		for (int i = 0; i < activeTrackPieces.Count; i++)
		{
			TrackPiece trackPiece = activeTrackPieces[i];
			if (!onlySpecial || trackPiece.trackPieceType != 0)
			{
				for (int j = 0; j < trackPiece.probability; j++)
				{
					randomSpace.Add(i);
				}
			}
		}
	}
}
