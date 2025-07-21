using System;
using UnityEngine;

[Serializable]
public class CameraConfig
{
	public Vector3 PositionOffset;

	public Vector3 LookAtOffset;

	public float cameraFOV;

	public Vector2 CameraClamp;

	public void ApplyNewConfig(CameraConfig CameraConfig)
	{
		LookAtOffset = CameraConfig.LookAtOffset;
		PositionOffset = CameraConfig.PositionOffset;
		cameraFOV = CameraConfig.cameraFOV;
		CameraClamp = CameraConfig.CameraClamp;
	}

	public void Lerp(CameraConfig a, CameraConfig b, float t)
	{
		cameraFOV = Mathf.LerpUnclamped(a.cameraFOV, b.cameraFOV, t);
		LookAtOffset = Vector3.LerpUnclamped(a.LookAtOffset, b.LookAtOffset, t);
		PositionOffset = Vector3.LerpUnclamped(a.PositionOffset, b.PositionOffset, t);
		CameraClamp = Vector2.LerpUnclamped(a.CameraClamp, b.CameraClamp, t);
	}
}
