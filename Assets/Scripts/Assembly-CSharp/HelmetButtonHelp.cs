using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HelmetButtonHelp : MonoBehaviour
{
	public enum AnimatingState
	{
		_notset = 0,
		OnScreen = 1,
		OffScreen = 2,
		AnimatingIn = 3,
		AnimatingOut = 4
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Vector3 onScreen;

	[SerializeField]
	private Vector3 offScreen;

	[SerializeField]
	private float _duration = 0.5f;

	[SerializeField]
	private UILabel amountLbl;

	private AnimatingState animatingState = AnimatingState.OffScreen;

	private float _current;

	private Helmet helmet;

	public bool IsActive { get; private set; }

	private InputActions inputActions;

	private void Awake()
	{
		inputActions = new InputActions();

		helmet = Helmet.Instance;
	}

	private void OnDisable()
	{
		inputActions.Disable();
		inputActions.UI.Board.performed -= OnBoardPerformed;

		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		helmet.OnStartHelmet -= OnStartHelmet;
		helmet.OnHardReset -= OnHardReset;
	}

	private void OnEnable()
	{
		inputActions.Enable();
		inputActions.UI.Board.performed += OnBoardPerformed;

		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		UpdateLabels();
		helmet.OnStartHelmet += OnStartHelmet;
		helmet.OnHardReset += OnHardReset;
	}

	public void Init()
	{
		animatingState = AnimatingState.OffScreen;
		target.localPosition = offScreen;
	}

	public void Show()
	{
		if (PlayerInfo.Instance.tutorialStep == 0 || PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) <= 0)
		{
			base.gameObject.SetActive(false);
			IsActive = false;
			return;
		}
		base.gameObject.SetActive(true);
		IsActive = true;
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		if (upgradeAmount > 0)
		{
			AnimatIn();
		}
	}

	private void UpdateLabels()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		amountLbl.text = upgradeAmount.ToString();
		if (upgradeAmount == 0)
		{
			AnimatOut();
		}
	}

	private void AnimatIn()
	{
		if (animatingState == AnimatingState.OffScreen)
		{
			_current = 0f;
		}
		else if (animatingState == AnimatingState.AnimatingOut)
		{
			_current = _duration - _current;
		}
		animatingState = AnimatingState.AnimatingIn;
	}

	private void AnimatOut()
	{
		if (animatingState == AnimatingState.OnScreen)
		{
			_current = 0f;
		}
		else if (animatingState == AnimatingState.AnimatingIn)
		{
			_current = _duration - _current;
		}
		animatingState = AnimatingState.AnimatingOut;
	}

	private void OnStartHelmet()
	{
		AnimatOut();
	}

	private void OnBoardPerformed(InputAction.CallbackContext context)
	{
		AttachHoverBoard();

		OnClick();
		AnimatOut();
		Debug.Log("Helmet is Used!");


	}

	public void AttachHoverBoard()
	{
		GameObject avatars = GameObject.Find("avatars");
		//GameObject hoverBoard = GameObject.Find("hoverBoard");
		GameObject hoverBoard = FindInactiveByTag("HoverBoard");

		if (avatars == null || hoverBoard == null)
		{
			Debug.LogWarning("avatars or hoverBoard is not assigned!");
			return;
		}

		hoverBoard.SetActive(false);

		foreach (Transform character in avatars.transform)
		{
			Transform animContainer = FindActiveAnimationContainer(character);
			if (animContainer != null)
			{
				hoverBoard.transform.SetParent(animContainer, true);
				hoverBoard.transform.localPosition = Vector3.zero;
				hoverBoard.transform.localRotation = Quaternion.identity;


				StartCoroutine(ActivateWithDelay(hoverBoard, 0.3f));

				Debug.Log("hoverBoard attached to : " + animContainer.name);
				return;
			}
		}

		Debug.LogWarning("Not find any container for hoverBoard!");
	}


	private IEnumerator ActivateWithDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		obj.SetActive(true);
	}

	private Transform FindActiveAnimationContainer(Transform parent)
	{
		foreach (Transform child in parent)
		{
			if (child.gameObject.activeInHierarchy && child.childCount > 0)
			{

				foreach (Transform grandChild in child)
				{
					if (grandChild.gameObject.activeInHierarchy)
						return grandChild;
				}
			}


			Transform found = FindActiveAnimationContainer(child);
			if (found != null) return found;
		}

		return null;
	}


	private GameObject FindInactiveByTag(string tag)
	{

		GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

		foreach (GameObject obj in allObjects)
		{
			if (obj.CompareTag(tag))
			{

				if (obj.scene.IsValid())
					return obj;
			}
		}

		return null;
	}


	private void OnHardReset()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		if (upgradeAmount > 0)
		{
			AnimatIn();
		}
	}

	private void OnHelmetInCooling(float rate)
	{
		if (rate <= float.MinValue)
		{
			AnimatIn();
		}
	}

	private void OnClick()
	{
		if (Game.Instance.IsInRunningMode && !Game.Instance.isDead && PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) > 0 && !Game.Instance.Attachment.IsActive(Game.Instance.Attachment.Helmet))
		{
			Game.Instance.Attachment.Add(Game.Instance.Attachment.Helmet);
		}
	}

	private void Update()
	{
		if (animatingState == AnimatingState.AnimatingIn)
		{
			_current += Time.deltaTime;
			target.localPosition = Vector3.Lerp(offScreen, onScreen, _current / _duration);
			if (_current >= _duration)
			{
				animatingState = AnimatingState.OnScreen;
				target.localPosition = onScreen;
			}
		}
		else if (animatingState == AnimatingState.AnimatingOut)
		{
			_current += Time.deltaTime;
			target.localPosition = Vector3.Lerp(onScreen, offScreen, _current / _duration);
			if (_current >= _duration)
			{
				animatingState = AnimatingState.OffScreen;
				target.localPosition = offScreen;
			}
		}
	}
}
