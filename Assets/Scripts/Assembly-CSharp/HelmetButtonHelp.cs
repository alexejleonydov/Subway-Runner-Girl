using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
		OnClick();
		AnimatOut();
		Debug.Log("Helmet is Used!");
		AttachHoverBoard();

	}

	public void AttachHoverBoard()
	{
		GameObject avatars = GameObject.Find("avatars");
		GameObject hoverBoard = GameObject.Find("hoverBoard");

		if (avatars == null || hoverBoard == null)
		{
			Debug.LogWarning("avatars або hoverBoar не призначені!");
			return;
		}

		// Перебираємо усіх персонажів (slick, frank, ...)
		foreach (Transform character in avatars.transform)
		{
			// Знаходимо активний анімаційний контейнер всередині персонажа
			Transform animContainer = FindActiveAnimationContainer(character);
			if (animContainer != null)
			{
				hoverBoard.transform.SetParent(animContainer, true);
				hoverBoard.transform.localPosition = Vector3.zero;
				hoverBoard.transform.localRotation = Quaternion.identity;

				Debug.Log("hoverBoard прикріплено до: " + animContainer.name);
				return; // припиняємо після першого прикріплення
			}
		}

		Debug.LogWarning("Не знайдено активного анімаційного контейнера для прикріплення hoverBoar!");
	}

	private Transform FindActiveAnimationContainer(Transform parent)
	{
		foreach (Transform child in parent)
		{
			if (child.gameObject.activeInHierarchy && child.childCount > 0)
			{
				// Беремо першого активного нащадка як "рівень глибше"
				foreach (Transform grandChild in child)
				{
					if (grandChild.gameObject.activeInHierarchy)
						return grandChild;
				}
			}

			// Рекурсивно шукаємо глибше у інших дітей
			Transform found = FindActiveAnimationContainer(child);
			if (found != null) return found;
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
