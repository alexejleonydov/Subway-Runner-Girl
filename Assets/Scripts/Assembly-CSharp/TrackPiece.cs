using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
	[Serializable]
	public class TrackCheckPoint
	{
		public float y;

		public float Z;
	}

	public int subscene;

	public TrackPieceType trackPieceType;

	public float zSize = 40f;

	public int probability = 1;

	public float zMinimum;

	public bool zMaximumActive;

	public float zMaximum;

	public List<TrackCheckPoint> CheckPoints;

	public TrackObject[] objects;

	public bool hasSorted;

	public int nextCityOrder = -1;

	private Dictionary<Transform, Vector3> hiddenObstacles = new Dictionary<Transform, Vector3>();

	public void Awake()
	{
		objects = GetComponentsInChildren<TrackObject>(true);
		if (!zMaximumActive)
		{
			zMaximum = float.MaxValue;
		}
		Track component = base.transform.root.GetComponent<Track>();
		component.AddToChunks(this);
		Randomizer[] componentsInChildren = GetComponentsInChildren<Randomizer>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].InitializeRandomizer();
		}
		if (CheckPoints != null && CheckPoints.Count > 0)
		{
			CheckPoints.Sort((TrackCheckPoint x, TrackCheckPoint y) => x.Z.CompareTo(y.Z));
		}
		if (!hasSorted)
		{
			Array.Sort(objects, (TrackObject to1, TrackObject to2) => to1.transform.position.z.CompareTo(to2.transform.position.z));
			hasSorted = true;
		}
	}

	public void Deactivate()
	{
		int i = 0;
		for (int num = objects.Length; i < num; i++)
		{
			objects[i].Deactivate();
		}
	}

	public void DeactivateObstacles(float maxZ)
	{
		IEnumerator enumerator = base.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Transform target = (Transform)enumerator.Current;
			DeactiveObstaclesRecursive(target, maxZ);
		}
	}

	private void DeactiveObstaclesRecursive(Transform target, float maxZ)
	{
		float num = ((!(target.GetComponent<Collider>() == null)) ? target.GetComponent<Collider>().bounds.min.z : target.transform.position.z);
		if (target.GetComponent<FlagObject>() == null)
		{
			IEnumerator enumerator = target.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Transform target2 = (Transform)enumerator.Current;
				DeactiveObstaclesRecursive(target2, maxZ);
			}
		}
		else if (num < maxZ && target.gameObject.layer != 16)
		{
			Vector3 localPosition = target.localPosition;
			if (!hiddenObstacles.ContainsKey(target))
			{
				hiddenObstacles.Add(target, localPosition);
			}
			target.localPosition = new Vector3(localPosition.x, -1000f, localPosition.z);
		}
	}

	private void DrawCheckPointGizmos()
	{
		if (CheckPoints != null && CheckPoints.Count > 0)
		{
			int i = 0;
			for (int count = CheckPoints.Count; i < count; i++)
			{
				Vector3 position = base.transform.position;
				position.z = CheckPoints[i].Z;
				Gizmos.DrawSphere(position + Vector3.up * 5f, 5f);
			}
		}
	}

	public float GetLastCheckPoint(float characterZ)
	{
		int index = 0;
		for (int num = CheckPoints.Count - 1; num > 0; num--)
		{
			if (characterZ >= CheckPoints[num].Z)
			{
				index = num;
				break;
			}
		}
		return CheckPoints[index].Z;
	}

	public TrackCheckPoint GetNextCheckPoint(float characterZ)
	{
		TrackCheckPoint trackCheckPoint = null;
		for (int i = 0; i < CheckPoints.Count; i++)
		{
			if (characterZ <= CheckPoints[i].Z + base.transform.position.z)
			{
				trackCheckPoint = new TrackCheckPoint();
				trackCheckPoint.y = CheckPoints[i].y;
				trackCheckPoint.Z = CheckPoints[i].Z + base.transform.position.z;
				break;
			}
		}
		return trackCheckPoint;
	}

	public void OnDrawGizmos()
	{
		DrawCheckPointGizmos();
	}

	public void RestoreHiddenObstacles()
	{
		foreach (KeyValuePair<Transform, Vector3> hiddenObstacle in hiddenObstacles)
		{
			if (hiddenObstacle.Key != null)
			{
				hiddenObstacle.Key.localPosition = hiddenObstacle.Value;
			}
		}
		hiddenObstacles.Clear();
	}
}
