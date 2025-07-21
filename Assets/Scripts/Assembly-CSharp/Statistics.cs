using System;
using UnityEngine;

public class Statistics
{
	public delegate void StatChangedDelegate(Stat stat, int newValue);

	public delegate void TempStatChangedDelegate(TempStat tempStat, int newValue);

	private StatChangedDelegate[] _statChangedHandlers;

	private TempStatChangedDelegate[] _tempStatChangedHandlers;

	private int[] _tempStatValues;

	private int[] _values = new int[Enum.GetNames(typeof(Stat)).Length];

	public bool dirty;

	public int this[string stat]
	{
		get
		{
			return _values[(int)Enum.Parse(typeof(Stat), stat, true)];
		}
		set
		{
			this[(Stat)(int)Enum.Parse(typeof(Stat), stat, true)] = value;
		}
	}

	public int this[TempStat tempStat]
	{
		get
		{
			return _tempStatValues[(int)tempStat];
		}
		set
		{
			if (_tempStatValues[(int)tempStat] == value)
			{
				return;
			}
			_tempStatValues[(int)tempStat] = value;
			dirty = true;
			if (_tempStatChangedHandlers != null)
			{
				TempStatChangedDelegate tempStatChangedDelegate = _tempStatChangedHandlers[(int)tempStat];
				if (tempStatChangedDelegate != null)
				{
					tempStatChangedDelegate(tempStat, value);
				}
			}
		}
	}

	public int this[Stat stat]
	{
		get
		{
			return _values[(int)stat];
		}
		set
		{
			if (_values[(int)stat] == value)
			{
				return;
			}
			_values[(int)stat] = value;
			dirty = true;
			if (_statChangedHandlers != null)
			{
				StatChangedDelegate statChangedDelegate = _statChangedHandlers[(int)stat];
				if (statChangedDelegate != null)
				{
					statChangedDelegate(stat, value);
				}
			}
		}
	}

	public void AddStatChangedHandler(Stat stat, StatChangedDelegate handler)
	{
		if (_statChangedHandlers == null)
		{
			_statChangedHandlers = new StatChangedDelegate[_values.Length];
		}
		_statChangedHandlers[(int)stat] = (StatChangedDelegate)Delegate.Combine(_statChangedHandlers[(int)stat], handler);
	}

	public bool AnyListenerForStat(Stat stat)
	{
		return _statChangedHandlers != null && _statChangedHandlers[(int)stat] != null;
	}

	public void Clear()
	{
		foreach (int value in Enum.GetValues(typeof(Stat)))
		{
			this[(Stat)value] = 0;
		}
	}

	public void Copy(Statistics src)
	{
		foreach (int value in Enum.GetValues(typeof(Stat)))
		{
			this[(Stat)value] = src[(Stat)value];
		}
	}

	public static Statistics Parse(string s)
	{
		Statistics statistics = new Statistics();
		if (s.Length > 0)
		{
			char[] separator = new char[1] { ';' };
			string[] array = s.Split(separator);
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int num2 = array[i].IndexOf('=');
				string text = array[i].Substring(0, num2);
				try
				{
					statistics._values[(int)Enum.Parse(typeof(Stat), text, true)] = int.Parse(array[i].Substring(num2 + 1));
				}
				catch (ArgumentException)
				{
					Debug.LogWarning(string.Format("Unknown int stat \"{0}\"encountered while reading statistics: ", text));
				}
			}
		}
		return statistics;
	}

	public void RemoveStatChangedHandler(Stat stat, StatChangedDelegate handler)
	{
		if (_statChangedHandlers != null)
		{
			_statChangedHandlers[(int)stat] = (StatChangedDelegate)Delegate.Remove(_statChangedHandlers[(int)stat], handler);
		}
	}

	public override string ToString()
	{
		int num = 0;
		int[] values = _values;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] != 0)
			{
				num++;
			}
		}
		Stat[] array = (Stat[])Enum.GetValues(typeof(Stat));
		string[] array2 = new string[num];
		int num2 = 0;
		int j = 0;
		for (int num3 = array.Length; j < num3; j++)
		{
			if (this[array[j]] != 0)
			{
				array2[num2] = string.Format("{0}={1}", array[j], this[array[j]]);
				num2++;
			}
		}
		return string.Join(";", array2);
	}
}
