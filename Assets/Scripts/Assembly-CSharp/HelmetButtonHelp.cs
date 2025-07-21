using System;
using UnityEngine;

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

	private void Awake()
	{
		helmet = Helmet.Instance;
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		helmet.OnStartHelmet -= OnStartHelmet;
		helmet.OnHardReset -= OnHardReset;
	}

	private void OnEnable()
	{
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
