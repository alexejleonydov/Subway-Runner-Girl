using System.Collections.Generic;

public class VariableBool
{
	public delegate void OnChangeDelegate(bool value);

	private HashSet<object> objects = new HashSet<object>();

	public OnChangeDelegate OnChange;

	private bool value;

	public bool Value
	{
		get
		{
			return value;
		}
	}

	public void Add(object o)
	{
		if (!objects.Contains(o))
		{
			objects.Add(o);
		}
		UpdateValue();
	}

	public void Clear()
	{
		objects.Clear();
		UpdateValue();
	}

	public void FireOnChange()
	{
		NotifyOnChange();
	}

	private void NotifyOnChange()
	{
		if (OnChange != null)
		{
			OnChange(value);
		}
	}

	public void Remove(object o)
	{
		if (objects.Contains(o))
		{
			objects.Remove(o);
		}
		UpdateValue();
	}

	private void UpdateValue()
	{
		bool flag = objects.Count > 0;
		bool flag2 = flag != value;
		value = flag;
		if (flag2)
		{
			NotifyOnChange();
		}
	}
}
