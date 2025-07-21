using UnityEngine;

public class Layers
{
	public readonly int Default = FindLayer("Default");

	public readonly int HitBounceOnly = FindLayer("HitBounceOnly");

	public readonly int KeepOnHelmet = FindLayer("KeepOnHelmet");

	public readonly int _3DGUI = FindLayer("3DGUI");

	public readonly int CELEBRATION = FindLayer("CELEBRATION");

	public readonly int _2DGUI = FindLayer("2DGUI");

	public readonly int Character = FindLayer("Character");

	public readonly int LoadScreenLayer = FindLayer("LoadScreenLayer");

	private static Layers _instance;

	public static Layers Instance
	{
		get
		{
			if (_instance == null)
			{
				Init();
			}
			return _instance;
		}
	}

	private Layers()
	{
	}

	private static int FindLayer(string name)
	{
		int num = LayerMask.NameToLayer(name);
		if (num == -1)
		{
			Debug.LogError("Could not find layer '" + name + "'.");
		}
		return num;
	}

	public static void Init()
	{
		_instance = new Layers();
	}
}
