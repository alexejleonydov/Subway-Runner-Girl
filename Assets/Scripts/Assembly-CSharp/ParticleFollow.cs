using UnityEngine;

public class ParticleFollow : MonoBehaviour
{
	public Transform Target;

	public float TweenTime;

	private float tweenVelocity;

	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	private void LateUpdate()
	{
		Vector3 position = Target.position;
		position.x = base.transform.position.x;
		float num = Mathf.SmoothDamp(position.x, Target.position.x, ref tweenVelocity, TweenTime);
		if (!float.IsNaN(num))
		{
			position.x = num;
		}
		else
		{
			tweenVelocity = 0f;
		}
		base.transform.position = position;
	}

	private void OnDisable()
	{
		tweenVelocity = 0f;
	}

	private void OnEnable()
	{
		tweenVelocity = 0f;
	}
}
