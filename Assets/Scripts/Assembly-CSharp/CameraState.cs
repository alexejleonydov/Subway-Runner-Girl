using System;
using UnityEngine;

[Serializable]
public struct CameraState
{
	public Vector3 Position;

	public Quaternion Rotation;

	public float FieldOfView;

	public CameraState(Camera camera)
	{
		if (camera != null)
		{
			Position = camera.transform.position;
			Rotation = camera.transform.rotation;
			FieldOfView = camera.fieldOfView;
		}
		else
		{
			Position = Vector3.zero;
			Rotation = Quaternion.identity;
			FieldOfView = 60f;
		}
	}

	public CameraState(Transform trans)
	{
		if (trans != null)
		{
			Position = trans.position;
			Rotation = trans.rotation;
			FieldOfView = 60f;
		}
		else
		{
			Position = Vector3.zero;
			Rotation = Quaternion.identity;
			FieldOfView = 60f;
		}
	}

	public void ApplyToCamera(Camera camera)
	{
		camera.transform.position = Position;
		camera.transform.rotation = Rotation;
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPad)
		{
			camera.fieldOfView = Mathf.Min(FieldOfView + 26f, 70f);
		}
		else
		{
			camera.fieldOfView = FieldOfView;
		}
	}

	public void ApplyToObject(Transform trans)
	{
		trans.position = Position;
		trans.rotation = Rotation;
	}
}
