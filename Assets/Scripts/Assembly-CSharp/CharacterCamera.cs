using System;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	private Animation _anim;

	private Camera _camera;

	public CameraShakeController CameraShakeController;

	public CameraWobbleController CameraWobbleController;

	private CameraConfig CurrentCameraConfig;

	private CameraState CurrentCameraState;

	private CameraFollowMode CurrentFollowMode;

	private CameraConfig StartCameraConfig;

	private CameraConfig EndCameraConfig;

	[SerializeField]
	private CameraFollowPlayerConfig currentCameraFollowPlayerConfig;

	private float currentSpringProgress;

	private float CurrentTransitionProgress;

	private float currentTunnelProgress;

	private float InternalSubwayEnterCameraProgress;

	private static CharacterCamera instance;

	private float InternalSubwayExitCameraProgress;

	private float MaxCameraHeight = -1f;

	private float previousCharacterPosX;

	private float previousCharacterPosY;

	private bool SmoothCameraHorizontalMovement;

	private float StartPostionY;

	[SerializeField]
	private AnimationCurve SubwayEnterStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayExitStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayPostStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayCameraSpringBlending;

	[SerializeField]
	private AnimationCurve SubwaySpringDownCameraBlending;

	[SerializeField]
	private AnimationCurve SubwaySpringUpCameraBlending;

	[SerializeField]
	private SubwayCameraSettings SubwayStairCameraSettings;

	private CameraState topMenuCameraState;

	private CameraState topMenuCameraParentState;

	private float TransitionTime;

	[SerializeField]
	private AnimationCurve TunnelCameraBlending;

	[SerializeField]
	private float XFactor = 0.05f;

	[SerializeField]
	private float YFactor = 0.1f;

	[SerializeField]
	private float FOVBlendSpeed;

	private float valueSpeedX;

	private float valueSpeedY;

	public Animation Animation
	{
		get
		{
			return _anim;
		}
	}

	public Camera Camera
	{
		get
		{
			return _camera;
		}
	}

	public CameraFollowPlayerConfig CurrentCameraFollowPlayerConfig
	{
		get
		{
			return currentCameraFollowPlayerConfig;
		}
	}

	public float CurrentSpringProgress
	{
		set
		{
			currentSpringProgress = value;
		}
	}

	public float CurrentTunnelProgress
	{
		set
		{
			currentTunnelProgress = value;
		}
	}

	public static CharacterCamera Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera;
			}
			return instance;
		}
	}

	public float SubwayEnterCameraProgress
	{
		set
		{
			InternalSubwayEnterCameraProgress = value;
		}
	}

	public float SubwayExitCameraProgress
	{
		set
		{
			InternalSubwayExitCameraProgress = value;
		}
	}

	public void Awake()
	{
		_camera = GetComponent<Camera>();
		_anim = base.transform.parent.GetComponent<Animation>();
		CurrentCameraConfig = new CameraConfig();
		StartCameraConfig = new CameraConfig();
		EndCameraConfig = new CameraConfig();
		topMenuCameraState = new CameraState(_camera);
		topMenuCameraParentState = new CameraState(base.transform.parent);
	}

	private void NormalUpdatePosition(Vector3 position, Quaternion rotation, float fov, float deltaTime, bool allowHeightBlending)
	{
		float num = 0f;
		Vector3 normalized = Vector3.Cross(rotation * Vector3.forward, Vector3.right).normalized;
		if (allowHeightBlending)
		{
			num = Mathf.SmoothDamp(previousCharacterPosY, position.y, ref valueSpeedY, YFactor);
			if (!float.IsNaN(num))
			{
				position.y = num;
			}
		}
		Vector3 vector = new Vector3(y: (!(MaxCameraHeight < 0f)) ? Mathf.Min(MaxCameraHeight, position.y) : position.y, x: (!(position.x < 0f)) ? Mathf.Min(CurrentCameraConfig.CameraClamp.y, position.x) : Mathf.Max(CurrentCameraConfig.CameraClamp.x, position.x), z: position.z);
		if (SmoothCameraHorizontalMovement)
		{
			num = Mathf.SmoothDamp(previousCharacterPosX, vector.x, ref valueSpeedX, XFactor);
			if (!float.IsNaN(num))
			{
				vector.x = num;
			}
		}
		previousCharacterPosX = vector.x;
		previousCharacterPosY = vector.y;
		CurrentCameraState.Position = vector + rotation * CurrentCameraConfig.PositionOffset + CameraShakeController.UpdateShakeController();
		CurrentCameraState.Rotation = Quaternion.LookRotation(Vector3.Normalize(vector + rotation * Quaternion.Euler(CameraWobbleController.UpdateWobbleController()) * CurrentCameraConfig.LookAtOffset - CurrentCameraState.Position), normalized);
		CurrentCameraState.FieldOfView = Mathf.Lerp(CurrentCameraState.FieldOfView, fov / _camera.aspect, FOVBlendSpeed * deltaTime);
	}

	public void Reset(Vector3 position, Quaternion rotation, bool resetPos = false)
	{
		previousCharacterPosX = position.x;
		previousCharacterPosY = position.y;
		TransitionTime = 0f;
		CurrentTransitionProgress = 0f;
		MaxCameraHeight = -1f;
		InternalSubwayExitCameraProgress = 0f;
		InternalSubwayEnterCameraProgress = 0f;
		currentSpringProgress = 0f;
		currentTunnelProgress = 0f;
		SmoothCameraHorizontalMovement = true;
		SetCameraToNormal();
		if (resetPos)
		{
			_camera.transform.position = position + CurrentCameraConfig.PositionOffset;
			_camera.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(position + CurrentCameraConfig.LookAtOffset - _camera.transform.position), position);
			_camera.fieldOfView = CurrentCameraConfig.cameraFOV / _camera.aspect;
		}
		CurrentCameraState = new CameraState(_camera);
		CurrentFollowMode = CameraFollowMode.Normal;
		UpdatePosition(position, rotation, 1f, true);
	}

	public void Revive()
	{
		SetMaxCameraHeight(-1f);
	}

	private void SetCameraToNormal()
	{
		CurrentCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
	}

	public void SetCameraToTopMenu()
	{
		_anim.Stop();
		SetMaxCameraHeight(-1f);
		CurrentFollowMode = CameraFollowMode.TopMenu;
		topMenuCameraParentState.ApplyToObject(base.transform.parent);
		topMenuCameraState.ApplyToCamera(_camera);
		CurrentCameraState = new CameraState(_camera);
	}

	public void SetCameraToTunnleTransition()
	{
		SetMaxCameraHeight(-1f);
		CurrentFollowMode = CameraFollowMode.TunnelTransition;
		StartCameraConfig.ApplyNewConfig(CurrentCameraConfig);
		EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.TunnelCenterConfig);
	}

	public void SetCameraTransition(CameraFollowMode mode, float transitionTime = 1f)
	{
		SetMaxCameraHeight(-1f);
		TransitionTime = transitionTime;
		CurrentTransitionProgress = 0f;
		CurrentFollowMode = mode;
		StartCameraConfig.ApplyNewConfig(CurrentCameraConfig);
		switch (mode)
		{
			case CameraFollowMode.FlyUp:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.FlyingConfig);
				break;
			case CameraFollowMode.FlyDown:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				break;
			case CameraFollowMode.StairDown:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				break;
			case CameraFollowMode.StairUp:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				break;
			case CameraFollowMode.SpringUp:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.SpringConfig);
				break;
			case CameraFollowMode.SpringDown:
				StartCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				EndCameraConfig.ApplyNewConfig(CurrentCameraConfig);
				break;
			case CameraFollowMode.WallUp:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.WallConfig);
				break;
			case CameraFollowMode.WallDown:
				StartCameraConfig.ApplyNewConfig(EndCameraConfig);
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				break;
			case CameraFollowMode.SpeedUp:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.OverdriveConfig);
				break;
			case CameraFollowMode.SpeedDown:
				EndCameraConfig.ApplyNewConfig(currentCameraFollowPlayerConfig.NormalConfig);
				break;
			case CameraFollowMode.Flying:
			case CameraFollowMode.TransitionDown:
			case CameraFollowMode.TransitionUp:
				break;
		}
	}

	public void SetMaxCameraHeight(float value)
	{
		MaxCameraHeight = value;
	}

	public void SetStartPositionY(float startPositionY)
	{
		StartPostionY = startPositionY;
	}

	public void StartRun(Vector3 position, Quaternion rotation)
	{
		CurrentFollowMode = CameraFollowMode.Normal;
		Reset(position, rotation);
	}

	private void TransitionUpdatePosition(float deltaTime)
	{
		CurrentTransitionProgress += deltaTime / TransitionTime;
		CurrentCameraConfig.Lerp(StartCameraConfig, EndCameraConfig, CurrentTransitionProgress);
		if (!(CurrentTransitionProgress >= 1f))
		{
			return;
		}
		CurrentTransitionProgress = 0f;
		if (CurrentFollowMode != CameraFollowMode.FlyUp && CurrentFollowMode != CameraFollowMode.SpeedUp && CurrentFollowMode != CameraFollowMode.WallUp)
		{
			if (CurrentFollowMode != CameraFollowMode.FlyDown && CurrentFollowMode != CameraFollowMode.SpeedDown && CurrentFollowMode != CameraFollowMode.WallDown)
			{
				throw new Exception("Implement camera position update state change");
			}
			CurrentFollowMode = CameraFollowMode.Normal;
		}
		else
		{
			CurrentFollowMode = CameraFollowMode.Flying;
		}
	}

	private void UpdateJumpTransitionStepDownAfter(Vector3 position)
	{
		if (base.transform.position.y - position.y >= CurrentCameraConfig.PositionOffset.y)
		{
			CurrentCameraState.Position = new Vector3(CurrentCameraState.Position.x, position.y + CurrentCameraConfig.PositionOffset.y, CurrentCameraState.Position.z);
		}
		if (currentSpringProgress >= 1f)
		{
			currentSpringProgress = 0f;
			CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateJumpTransitionStepDownBefore()
	{
		float time = Mathf.Min(currentSpringProgress, 1f);
		float t = SubwaySpringDownCameraBlending.Evaluate(time);
		CurrentCameraConfig.Lerp(EndCameraConfig, StartCameraConfig, t);
	}

	private void UpdateJumpTransitionStepUpAfter()
	{
		float t = SubwayCameraSpringBlending.Evaluate(currentSpringProgress);
		float y = Mathf.Lerp(StartPostionY, CurrentCameraState.Position.y, t);
		CurrentCameraState.Position = new Vector3(CurrentCameraState.Position.x, y, CurrentCameraState.Position.z);
		if (currentSpringProgress >= 1f)
		{
			currentSpringProgress = 0f;
			StartPostionY = CurrentCameraState.Position.y;
			if (CurrentFollowMode != CameraFollowMode.SpringUp)
			{
				throw new Exception("Implement camera position update state change");
			}
			CurrentFollowMode = CameraFollowMode.SpringDown;
		}
	}

	private void UpdateJumpTransitionStepUpBefore()
	{
		float t = SubwaySpringUpCameraBlending.Evaluate(currentSpringProgress);
		CurrentCameraConfig.Lerp(StartCameraConfig, EndCameraConfig, t);
	}

	public void UpdatePosition(Vector3 position, Quaternion rotation, float deltaTime, bool allowHeightBlending)
	{
		//		Debug.Log("Current Camera Follow Mode: " + CurrentFollowMode);

		switch (CurrentFollowMode)
		{
			case CameraFollowMode.TunnelTransition:
				UpdateTunnelTransitionStep();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.Normal:
			case CameraFollowMode.Flying:
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.FlyUp:
			case CameraFollowMode.WallUp:
				TransitionUpdatePosition(deltaTime);
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.FlyDown:
			case CameraFollowMode.WallDown:
				TransitionUpdatePosition(deltaTime);
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.TransitionDown:
				UpdateStairTransitionStepDown();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.TransitionUp:
				UpdateStairTransitionStepUp();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.SpringUp:
				UpdateJumpTransitionStepUpBefore();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				UpdateJumpTransitionStepUpAfter();
				break;
			case CameraFollowMode.SpringDown:
				UpdateJumpTransitionStepDownBefore();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				UpdateJumpTransitionStepDownAfter(position);
				break;
			case CameraFollowMode.StairDown:
				UpdateStairTransitionStepDown();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.StairUp:
				UpdateStairTransitionStepUp();
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.SpeedUp:
				TransitionUpdatePosition(deltaTime);
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
			case CameraFollowMode.SpeedDown:
				TransitionUpdatePosition(deltaTime);
				NormalUpdatePosition(position, rotation, CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
				break;
		}
		CurrentCameraState.ApplyToCamera(_camera);
	}

	private void UpdateStairTransitionStepDown()
	{
		float num = 0f;
		if (InternalSubwayEnterCameraProgress <= 0.6f)
		{
			float time = Mathf.Min(InternalSubwayEnterCameraProgress / 0.6f, 1f);
			num = SubwayEnterStairCameraBlending.Evaluate(time);
			CurrentCameraConfig.Lerp(StartCameraConfig, SubwayStairCameraSettings.SubwayDownCenterCameraConfig, num);
			CurrentCameraConfig.LookAtOffset.y = Mathf.LerpUnclamped(StartCameraConfig.LookAtOffset.y, SubwayStairCameraSettings.SubwayDownCenterCameraConfig.LookAtOffset.y, SubwayStairCameraSettings.DescendLookAtYoffsetAC.Evaluate(time));
			CurrentCameraConfig.PositionOffset.y = Mathf.LerpUnclamped(StartCameraConfig.PositionOffset.y, SubwayStairCameraSettings.SubwayDownCenterCameraConfig.PositionOffset.y, SubwayStairCameraSettings.DescendPositionYoffsetAC.Evaluate(time));
		}
		else
		{
			num = SubwayPostStairCameraBlending.Evaluate(InternalSubwayEnterCameraProgress);
			CurrentCameraConfig.Lerp(SubwayStairCameraSettings.SubwayDownCenterCameraConfig, EndCameraConfig, num);
		}
		if (InternalSubwayEnterCameraProgress >= 1f)
		{
			InternalSubwayEnterCameraProgress = 0f;
			CurrentCameraConfig.ApplyNewConfig(EndCameraConfig);
			CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateStairTransitionStepUp()
	{
		float num = 0f;
		if (InternalSubwayExitCameraProgress < 0.6f)
		{
			float time = Mathf.Min(InternalSubwayExitCameraProgress / 0.6f, 1f);
			num = SubwayExitStairCameraBlending.Evaluate(time);
			CurrentCameraConfig.Lerp(StartCameraConfig, SubwayStairCameraSettings.SubwayUpCenterCameraConfig, num);
			CurrentCameraConfig.LookAtOffset.y = Mathf.LerpUnclamped(StartCameraConfig.LookAtOffset.y, SubwayStairCameraSettings.SubwayUpCenterCameraConfig.LookAtOffset.y, SubwayStairCameraSettings.AscendLookAtYoffsetAC.Evaluate(time));
			CurrentCameraConfig.PositionOffset.y = Mathf.LerpUnclamped(StartCameraConfig.PositionOffset.y, SubwayStairCameraSettings.SubwayUpCenterCameraConfig.PositionOffset.y, SubwayStairCameraSettings.AscendPositionYoffsetAC.Evaluate(time));
		}
		else
		{
			num = SubwayPostStairCameraBlending.Evaluate(Mathf.Min(InternalSubwayExitCameraProgress, 1f));
			CurrentCameraConfig.Lerp(SubwayStairCameraSettings.SubwayUpCenterCameraConfig, EndCameraConfig, num);
		}
		if (InternalSubwayExitCameraProgress >= 1f)
		{
			InternalSubwayExitCameraProgress = 0f;
			CurrentCameraConfig.ApplyNewConfig(EndCameraConfig);
			CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateTunnelTransitionStep()
	{
		float t = TunnelCameraBlending.Evaluate(currentTunnelProgress);
		CurrentCameraConfig.Lerp(StartCameraConfig, EndCameraConfig, t);
		if (currentTunnelProgress >= 1f)
		{
			currentTunnelProgress = 0f;
			CurrentCameraConfig.ApplyNewConfig(StartCameraConfig);
			CurrentFollowMode = CameraFollowMode.Normal;
		}
	}
}
