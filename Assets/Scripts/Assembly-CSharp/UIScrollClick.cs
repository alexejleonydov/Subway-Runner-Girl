using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIScrollClick : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	private IScrollClick scollClick;

	private InputActions inputActions;

	private void Awake()
	{
		if (inputActions == null)
			inputActions = new InputActions();

		inputActions.Enable();

		inputActions.UI.Swipe.performed += ScrollSwipe;

		if (!(target == null))
		{
			scollClick = target.GetComponent<IScrollClick>();
		}
	}

    private void ScrollSwipe(InputAction.CallbackContext obj)
    {
		Debug.Log("Swipe");

		float direction = inputActions.UI.Swipe.ReadValue<float>();

		Vector2 pos = 200*direction*Vector2.right;

		Vector2 oldPos = new (755.94f, 393.29f);

		Vector2 newPose = oldPos + pos;

		scollClick.ScrollClicked(newPose);

	}

    private void OnClick()
	{
		if (scollClick != null)
		{
			Vector2 pos = UICamera.currentTouch.pos;

			Debug.Log("MousePosition" + pos);

			scollClick.ScrollClicked(pos);
		}
	}
}
