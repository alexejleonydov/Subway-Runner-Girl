using UnityEngine;

public class CelebrationClone : MonoBehaviour
{
	[SerializeField]
	private GameObject _targetObject;

	[SerializeField]
	private int _copyCount = 4;

	[SerializeField]
	private bool _destroyOriginal = true;

	[SerializeField]
	private float _radius = 1f;

	[SerializeField]
	private CalcCircularPoint.Axis _axis = CalcCircularPoint.Axis.Z;

	[SerializeField]
	private bool _alignToCloner = true;

	private CalcCircularPoint calc;

	private void Awake()
	{
		if (!(_targetObject != null))
		{
			return;
		}
		CalcCircularPoint calcCircularPoint = new CalcCircularPoint(_copyCount, _axis, _radius);
		Vector3 position = base.transform.position;
		Quaternion rotation = _targetObject.transform.rotation;
		for (int i = 0; i < _copyCount; i++)
		{
			Vector3 vector = position + calcCircularPoint.CalcCenterOffset(i);
			if (_alignToCloner)
			{
				rotation = Quaternion.LookRotation(base.transform.up, position - vector);
			}
			GameObject gameObject = Object.Instantiate(_targetObject, vector, rotation);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = _targetObject.transform.localScale;
		}
		if (_destroyOriginal)
		{
			Object.Destroy(_targetObject);
			_targetObject = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		CalcCircularPoint calcCircularPoint = new CalcCircularPoint(_copyCount, _axis, _radius);
		for (int i = 0; i < _copyCount; i++)
		{
			Vector3 vector = position + calcCircularPoint.CalcCenterOffset(i);
			if (_alignToCloner)
			{
				Gizmos.DrawRay(vector, base.transform.up * _radius * 0.5f);
			}
			Gizmos.DrawSphere(vector, _radius * 0.05f);
		}
	}
}
