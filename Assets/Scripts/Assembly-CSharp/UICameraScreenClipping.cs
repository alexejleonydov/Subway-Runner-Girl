using UnityEngine;

public class UICameraScreenClipping : MonoBehaviour
{
	private Camera _cam;

	private const int POPUP_WIDTH = 265;

	private const int SCREEN_WIDTH = 300;

	public void CalculateClipping(bool popupSizedClip)
	{
		if (UIScreenController.Instance.root == null)
		{
			Debug.LogError("UIRoot not set in UIScreenController");
		}
		if (_cam == null)
		{
			_cam = base.gameObject.GetComponent<Camera>();
		}
		Rect rect = _cam.rect;
		if (!UIBaseScreen.IsOutOfProportion())
		{
			float num = Screen.width * 480 / Screen.height;
			float num2 = (popupSizedClip ? 265f : 300f) / num;
			float x = 0.5f - num2 / 2f;
			rect.x = x;
			rect.width = num2;
		}
		else if (popupSizedClip)
		{
			rect.x = 0.085f;
			rect.width = 0.83f;
		}
		else
		{
			rect.x = 0.05f;
			rect.width = 0.9f;
		}
		_cam.rect = rect;
	}

	private void Start()
	{
		_cam = base.gameObject.GetComponent<Camera>();
		if (_cam == null)
		{
			Debug.LogError("The UICameraScreenClipping script is not attached to a Camera");
		}
		else
		{
			CalculateClipping(false);
		}
	}
}
