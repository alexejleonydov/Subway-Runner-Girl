using UnityEngine;

public class Mirror : BaseO
{
	private Transform[] children;

	protected override void Awake()
	{
		children = new Transform[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			children[i] = base.transform.GetChild(i);
		}
		base.Awake();
	}

	public override void OnActivate()
	{
		int num = Random.Range(0, 2) * 2 - 1;
		for (int i = 0; i < children.Length; i++)
		{
			Vector3 localPosition = children[i].localPosition;
			localPosition.x *= num;
			children[i].localPosition = localPosition;
		}
	}
}
