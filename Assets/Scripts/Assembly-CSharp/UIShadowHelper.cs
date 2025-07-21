using UnityEngine;

[ExecuteInEditMode]
public class UIShadowHelper<T> : MonoBehaviour where T : UIWidget
{
	public T shadow;

	public bool updateDynamically;

	public Vector3 shadowOffset;

	protected T _front;

	protected Transform _frontTransform;

	protected Transform _shadowTransform;

	private void Awake()
	{
		_front = base.gameObject.GetComponent<T>();
		_frontTransform = base.transform;
		if (shadow != null)
		{
			_shadowTransform = shadow.cachedTransform;
			shadow.depth = _front.depth - 1;
			shadow.gameObject.name = base.name + "Shadow";
		}
	}

	private void OnDisable()
	{
		if (shadow != null)
		{
			shadow.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (shadow != null)
		{
			shadow.gameObject.SetActive(true);
		}
	}

	private void Start()
	{
		UpdateT();
	}

	private void Update()
	{
		if (_front == shadow)
		{
			Debug.LogError("front and shadow label are the same!", base.gameObject);
			shadow = (T)null;
		}
		if (updateDynamically)
		{
			if (_shadowTransform == null)
			{
				_shadowTransform = shadow.cachedTransform;
			}
			UpdateT();
		}
	}

	public void UpdateNow()
	{
		UpdateT();
	}

	private void UpdateT()
	{
		if (shadow != null)
		{
			withUpdate();
			if (shadow.depth != _front.depth - 1)
			{
				shadow.depth = _front.depth - 1;
			}
			_shadowTransform.localPosition = _frontTransform.localPosition + shadowOffset;
		}
	}

	protected virtual void withUpdate()
	{
	}
}
