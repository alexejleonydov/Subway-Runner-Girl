using System;

[Serializable]
public class CameraFollowPlayerConfig
{
	public CameraConfig BoundConfig = new CameraConfig();

	public CameraConfig FlyingConfig = new CameraConfig();

	public CameraConfig NormalConfig = new CameraConfig();

	public CameraConfig OverdriveConfig = new CameraConfig();

	public CameraConfig SpringConfig = new CameraConfig();

	public CameraConfig TunnelCenterConfig = new CameraConfig();

	public CameraConfig WallConfig = new CameraConfig();

	public CameraFollowPlayerConfig(bool customConstructor)
	{
	}

	public void ApplyNewConfig(CameraFollowPlayerConfig cameraFollowPlayerConfig)
	{
		NormalConfig.ApplyNewConfig(cameraFollowPlayerConfig.NormalConfig);
		FlyingConfig.ApplyNewConfig(cameraFollowPlayerConfig.FlyingConfig);
		SpringConfig.ApplyNewConfig(cameraFollowPlayerConfig.SpringConfig);
		TunnelCenterConfig.ApplyNewConfig(cameraFollowPlayerConfig.TunnelCenterConfig);
		OverdriveConfig.ApplyNewConfig(cameraFollowPlayerConfig.OverdriveConfig);
	}

	public static CameraFollowPlayerConfig GetNewCameraFollowPlayerConfig()
	{
		return new CameraFollowPlayerConfig(true);
	}
}
