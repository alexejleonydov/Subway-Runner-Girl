using UnityEngine;

public class CalcCircularPoint
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	private int _copyCount;

	private float _radius = 1f;

	private Axis _axis = Axis.Z;

	public CalcCircularPoint(int count, Axis axis, float radius)
	{
		_copyCount = count;
		_radius = radius;
		_axis = axis;
	}

	public Vector3 CalcCenterOffset(int index)
	{
		Vector3 zero = Vector3.zero;
		float f = (float)index * 3.141593f * 2f / (float)_copyCount;
		switch (_axis)
		{
		case Axis.X:
			return new Vector3(0f, Mathf.Cos(f) * _radius, Mathf.Sin(f) * _radius);
		case Axis.Y:
			return new Vector3(Mathf.Cos(f) * _radius, 0f, Mathf.Sin(f) * _radius);
		case Axis.Z:
			return new Vector3(Mathf.Cos(f) * _radius, Mathf.Sin(f) * _radius, 0f);
		default:
			return zero;
		}
	}
}
