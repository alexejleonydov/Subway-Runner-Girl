using UnityEngine;

public class SmoothDampFloat
{
	private float smoothTime;

	private float target;

	private float value;

	public float valueSpeed;

	public float SmoothTime
	{
		get
		{
			return smoothTime;
		}
		set
		{
			smoothTime = value;
		}
	}

	public float Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public SmoothDampFloat(float value, float smoothTime)
	{
		this.smoothTime = smoothTime;
		this.value = value;
		target = value;
	}

	public void Update()
	{
		float f = Mathf.SmoothDamp(value, target, ref valueSpeed, smoothTime);
		if (!float.IsNaN(f))
		{
			value = f;
		}
	}
}
