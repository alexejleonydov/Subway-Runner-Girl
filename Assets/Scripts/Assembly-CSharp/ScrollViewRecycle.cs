using UnityEngine;

public class ScrollViewRecycle<T, V> : UIScrollView where T : MonoBehaviour
{
	[SerializeField]
	protected int maxCellCount;

	[SerializeField]
	protected int maxCount;

	[SerializeField]
	protected int cellHeight;

	protected T[] monos;

	protected Vector3[] _positions;

	protected Vector3 _originPos;

	protected int _lastIndex;

	public override Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mTrans = base.transform;
				if (Application.isPlaying)
				{
					mBounds = new Bounds((positions[positions.Length - 2] + positions[0]) * 0.5f, positions[0] - positions[positions.Length - 2] + Vector3.one * cellHeight);
				}
			}
			return mBounds;
		}
	}

	private Vector3[] positions
	{
		get
		{
			if (_positions == null)
			{
				_positions = new Vector3[maxCount + 1];
				for (int i = 0; i < maxCount + 1; i++)
				{
					_positions[i] = Vector3.down * ((float)(i * cellHeight) - 0.5f * (float)cellHeight);
				}
			}
			return _positions;
		}
	}

	public Transform[] cells { get; private set; }

	public V[] orders { get; private set; }

	public int MaxCellCount
	{
		get
		{
			return maxCellCount;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		cells = new Transform[maxCellCount];
		monos = new T[maxCellCount];
		_positions = new Vector3[maxCount + 1];
		orders = new V[maxCount];
		for (int i = 0; i < maxCount + 1; i++)
		{
			_positions[i] = Vector3.down * ((float)(i * cellHeight) - 0.5f * (float)cellHeight);
		}
	}

	public void Init(int count)
	{
		orders = new V[count];
		_positions = new Vector3[count + 1];
		for (int i = 0; i < count + 1; i++)
		{
			_positions[i] = Vector3.down * ((float)(i * cellHeight) - 0.5f * (float)cellHeight);
		}
	}

	protected override void Start()
	{
		base.Start();
		_originPos = mTrans.localPosition;
		_lastIndex = 0;
	}

	public void SetCell(T cell, int i)
	{
		cells[i] = cell.transform;
		monos[i] = cell;
		cell.transform.localPosition = _positions[i];
	}

	public override void MoveRelative(Vector3 relative, bool constraint = false)
	{
		base.MoveRelative(relative);
		int num = (int)((mTrans.localPosition.y - _originPos.y) / (float)cellHeight);
		if (num < 0 || _lastIndex == num)
		{
			return;
		}
		_lastIndex = num;
		int num2 = 0;
		while (num2 < maxCellCount)
		{
			if (num + num2 >= _positions.Length - 1)
			{
				if (cells[(num + num2) % maxCellCount] != null)
				{
					cells[(num + num2) % maxCellCount].localPosition = _positions[_positions.Length - 1];
				}
				num2++;
			}
			else
			{
				cells[(num + num2) % maxCellCount].localPosition = _positions[num + num2];
				RefreshUI((num + num2) % maxCellCount, orders[num + num2]);
				num2++;
			}
		}
	}

	protected virtual void RefreshUI(int cellIndex, V index)
	{
	}

	public void ResetCell()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].localPosition = _positions[i];
			RefreshUI(i, orders[i]);
		}
	}
}
