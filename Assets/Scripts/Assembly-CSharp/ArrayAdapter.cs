using System.Collections.Generic;

public class ArrayAdapter<T> : ListAdapter
{
	public List<T> _dataList;

	public ArrayAdapter(List<T> list)
	{
		_dataList = list;
	}

	public virtual void AddItem(T element)
	{
		_dataList.Add(element);
	}

	public virtual void AddMany(IEnumerable<T> list)
	{
		_dataList.AddRange(list);
	}

	public virtual void Clear()
	{
		_dataList.Clear();
	}

	public override int GetCount()
	{
		return _dataList.Count;
	}

	public virtual T GetItem(int index)
	{
		return _dataList[index];
	}

	public virtual void Insert(int index, T element)
	{
		_dataList.Insert(index, element);
	}

	public virtual void RemoveItem(T element)
	{
		if (_dataList.Count > 0)
		{
			_dataList.Remove(element);
		}
	}

	public virtual void RemoveItemAt(int index)
	{
		if (_dataList.Count > 0)
		{
			_dataList.RemoveAt(index);
		}
	}
}
