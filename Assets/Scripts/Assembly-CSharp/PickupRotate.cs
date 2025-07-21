using UnityEngine;

public class PickupRotate : BaseO
{
	public Transform target;

	public float speed = 180f;

	public float rotatePhase = 0.9f;

	private float z;

	public float Z
	{
		get
		{
			return z;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.enabled = false;
	}

	public override void OnActivate()
	{
		z = base.transform.position.z;
		CoinPool.Instance.AddActiveRotatePickups(this);
		base.enabled = true;
	}

	public override void OnDeactivate()
	{
		CoinPool.Instance.RemoveActiveRotatePickups(this);
		base.enabled = false;
	}

	public void PhasedRotate()
	{
		target.localRotation = Quaternion.AngleAxis(Time.time * speed + z * rotatePhase, Vector3.up);
	}
}
