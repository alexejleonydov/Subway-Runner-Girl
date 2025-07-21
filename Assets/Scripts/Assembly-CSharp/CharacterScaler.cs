using UnityEngine;

public class CharacterScaler : MonoBehaviour
{
	public enum ScaleAnchorType
	{
		CharacterAnchor = 0,
		GameOverAnchor = 1,
		TutorialPupupAnchor = 2,
		CelebrationPopupAnchor = 3
	}

	[SerializeField]
	public bool lookAtCamera;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private ScaleAnchorType _anchorType;

	private float _posX = 90f;

	private float _posY = 225f;

	private UIRoot _root;

	private float _scaleDelta = 450f;

	private float _scaleMultiplierForRotation = 56f;

	private void PositionCharacter()
	{
		float posX = _posX;
		float posY = _posY;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = posX;
		localPosition.y = posY;
		transform.localPosition = localPosition;
	}

	private void RotateCharacter()
	{
		if (_camera != null)
		{
			Vector3 zero = Vector3.zero;
			float num = (float)_root.manualHeight - 720f;
			float num2 = ((_scaleMultiplierForRotation != 0f) ? (num / _scaleMultiplierForRotation + 5f) : 0f);
			zero.x = 0f - num2;
			zero.y = 0f;
			zero.z = 0f;
			base.gameObject.transform.localRotation = Quaternion.Euler(zero);
		}
		else
		{
			Debug.LogWarning("Camera not set in the CharacterModel prefab");
		}
	}

	private void ScaleCharacter()
	{
		float num = _root.manualHeight;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = ((_scaleDelta != 0f) ? (num - _scaleDelta) : 0f);
		transform.transform.localPosition = localPosition;
	}

	private void SetScreenRelatedSettings()
	{
		if (_anchorType == ScaleAnchorType.CharacterAnchor)
		{
			_posX = 0f;
			_posY = 30f;
			_scaleMultiplierForRotation = 56f;
			_scaleDelta = 900f;
		}
		else if (_anchorType == ScaleAnchorType.GameOverAnchor)
		{
			_posX = -70f;
			_posY = (float)(_root.manualHeight / 2) - 230f;
			_scaleMultiplierForRotation = 32f;
			_scaleDelta = 720f;
		}
		else if (_anchorType == ScaleAnchorType.TutorialPupupAnchor)
		{
			_posX = 0f;
			_posY = -50f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = 600f;
		}
		else if (_anchorType == ScaleAnchorType.CelebrationPopupAnchor)
		{
			_posX = 0f;
			_posY = 0f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = _root.manualHeight;
		}
	}

	private void Start()
	{
		if (_camera == null)
		{
			_camera = NGUITools.FindInParents<Camera>(base.gameObject);
		}
		_root = UIScreenController.Instance.root;
		SetScreenRelatedSettings();
		if (_root == null)
		{
			Debug.LogWarning("Root not set in the UIScreenController prefab");
		}
		ScaleCharacter();
		PositionCharacter();
		RotateCharacter();
	}

	private void Update()
	{
		if (lookAtCamera)
		{
			RotateCharacter();
			lookAtCamera = false;
		}
	}
}
