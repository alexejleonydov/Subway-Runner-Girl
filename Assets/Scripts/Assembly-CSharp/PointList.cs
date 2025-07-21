using UnityEngine;

public class PointList : MonoBehaviour
{
	[SerializeField]
	private Point[] wayPoints;

	private int currentPointId;

	[SerializeField]
	private int showgroup;

	public int CurrentId
	{
		get
		{
			return currentPointId;
		}
	}

	public Point CurrentPoint
	{
		get
		{
			return wayPoints[currentPointId];
		}
	}

	public Point this[int index]
	{
		get
		{
			if (wayPoints != null && index >= 0 && index < Count)
			{
				return wayPoints[index];
			}
			return null;
		}
	}

	public int Count
	{
		get
		{
			if (wayPoints == null)
			{
				return 0;
			}
			return wayPoints.Length;
		}
	}

	public void Play()
	{
		currentPointId = 0;
	}

	public int OnBreak()
	{
		if (currentPointId == -1)
		{
			return -1;
		}
		int i;
		for (i = currentPointId; i < Count && wayPoints[i].group != -1; i++)
		{
		}
		if (i >= Count)
		{
			return currentPointId = -1;
		}
		return currentPointId = i;
	}

	private void Reset()
	{
		wayPoints = new Point[base.transform.childCount];
		int i = 0;
		for (int childCount = base.transform.childCount; i < childCount; i++)
		{
			wayPoints[i] = base.transform.GetChild(i).GetComponent<Point>();
			base.transform.GetChild(i).gameObject.name = string.Format("{0:00}", i);
		}
	}

	private void OnDrawGizmos()
	{
		if (wayPoints == null)
		{
			return;
		}
		Gizmos.color = Color.blue;
		for (int i = 0; i < Count - 1; i++)
		{
			if (!(wayPoints[i] == null) && (wayPoints[i].group == 0 || wayPoints[i].group == showgroup))
			{
				int num = i;
				do
				{
					num++;
				}
				while (num < Count && wayPoints[i] != null && wayPoints[num].group != 0 && wayPoints[num].group != showgroup);
				if (num < Count)
				{
					Gizmos.DrawLine(wayPoints[i].transform.position, wayPoints[num].transform.position);
				}
			}
		}
		Gizmos.color = Color.red;
		Point[] array = wayPoints;
		foreach (Point point in array)
		{
			Gizmos.DrawSphere(point.transform.position, 1f);
		}
	}

	public int WaitNext(PointsManager manager)
	{
		if (currentPointId == -1)
		{
			return -1;
		}
		int i;
		for (i = currentPointId; i < Count && !(wayPoints[i] is Wait); i++)
		{
			wayPoints[i].OnImility(manager);
		}
		if (i >= Count - 1)
		{
			return -1;
		}
		return currentPointId = i + 1;
	}

	public int CalcNext()
	{
		if (currentPointId == -1)
		{
			return -1;
		}
		int num = currentPointId;
		int group = wayPoints[currentPointId].group;
		if (group != 0)
		{
			if (group % 2 == 0)
			{
				do
				{
					num++;
					num %= Count;
				}
				while (wayPoints[num].group != group);
				return num;
			}
			do
			{
				num++;
			}
			while (num < Count && wayPoints[num].group != group);
			if (num >= Count)
			{
				return -1;
			}
			return num;
		}
		num++;
		if (num >= Count)
		{
			return -1;
		}
		return num;
	}

	public int Next()
	{
		return currentPointId = CalcNext();
	}
}
