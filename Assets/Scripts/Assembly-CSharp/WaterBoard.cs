using System;
using UnityEngine;

public class WaterBoard : BaseO, ITouchByCharacter
{
	[SerializeField]
	private float distance;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private float MoveZ;

	[SerializeField]
	private Transform end;

	private BoxCollider BCollider;

	private Vector3 originLPos;

	private Transform curTrans;

	private Character character;

	private float endZ;

	private float offset;

	private bool beTouchedFirstUpdate;

	private int x;

	public static Action<int> GetOnWaterBoard;

	public static Action<int> GetOffWaterBoard;

	protected override void Awake()
	{
		curTrans = base.transform;
		originLPos = target.localPosition;
		character = Character.Instance;
		BCollider = GetComponent<BoxCollider>();
		if (distance < 0f)
		{
			distance = 0f;
		}
		Vector3 center = BCollider.center;
		Vector3 size = BCollider.size;
		curTrans.localPosition = new Vector3(0f, 0f, distance - size.z * 0.5f);
		size.z -= distance;
		center.z = size.z * 0.5f;
		BCollider.size = size;
		BCollider.center = center;
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		beTouchedFirstUpdate = true;
		target.localPosition = originLPos;
		if (!end)
		{
			endZ = target.position.z + MoveZ;
		}
		else
		{
			endZ = end.position.z;
		}
	}

	public override void OnDeactivate()
	{
		base.enabled = false;
	}

	public bool BeTouched()
	{
		if (!beTouchedFirstUpdate)
		{
			return base.enabled;
		}
		beTouchedFirstUpdate = false;
		offset = target.position.z - character.z;
		x = ((!(target.position.x < -10f)) ? ((!(target.position.x > 10f)) ? 1 : 2) : 0);
		if (GetOnWaterBoard != null)
		{
			GetOnWaterBoard(x);
		}
		base.enabled = true;
		return true;
	}

	private void LateUpdate()
	{
		target.position = new Vector3(target.position.x, target.position.y, character.z + offset);
		if (target.position.z >= endZ)
		{
			OnEnd();
		}
	}

	private void OnEnd()
	{
		base.enabled = false;
		if (GetOffWaterBoard != null)
		{
			GetOffWaterBoard(x);
		}
	}
}
