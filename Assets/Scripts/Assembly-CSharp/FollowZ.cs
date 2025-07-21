using UnityEngine;

public class FollowZ : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private float zOffset;

	[SerializeField]
	private float translationTime;

	private Transform mTrans;

	private Vector3 initPos;

	private void Awake()
	{
		mTrans = base.transform;
		mTrans.rotation = Quaternion.identity;
		initPos = mTrans.position;
		base.enabled = false;
	}

	private void LateUpdate()
	{
		mTrans.position = new Vector3(mTrans.position.x, mTrans.position.y, target.position.z + zOffset);
	}

	private void OnDisable()
	{
		mTrans.position = initPos;
	}
}
