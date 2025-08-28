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
		//if (inputActions == null)
		inputActions = new InputActions();

		// inputActions.Enable();

		// inputActions.UI.Swipe.performed += ScrollSwipe;

		if (!(target == null))
		{
			scollClick = target.GetComponent<IScrollClick>();
		}
	}

	private void OnEnable()
	{
		inputActions.Enable();

		inputActions.UI.Swipe.performed += ScrollSwipe;
	}
	private void OnDisable()
	{
		inputActions.Disable();

		inputActions.UI.Swipe.performed -= ScrollSwipe;
	}

	private void Update()
	{
		Vector3 mouseScreenPosition = Input.mousePosition;
		//		Debug.Log("Mouse Screen Position: " + mouseScreenPosition);
	}

	private void ScrollSwipe(InputAction.CallbackContext obj)
	{


		float direction = inputActions.UI.Swipe.ReadValue<float>();

		Vector2 pos = 250 * direction * Vector2.right;

		//Vector2 oldPos = new(968.94f, 178.29f);
		Vector2 oldPos = new(960.94f, 410.29f);

		Vector2 newPose = oldPos + pos;

		scollClick.ScrollClicked(newPose);

		Debug.Log("Swipe on " + newPose);

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
