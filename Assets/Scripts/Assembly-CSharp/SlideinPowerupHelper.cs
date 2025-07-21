using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideinPowerupHelper : MonoBehaviour
{
	[SerializeField]
	private SlideinPowerupButton[] powerupButtons;

	[SerializeField]
	private List<PropType> types;

	private int _currentHeadstartGear;

	private bool _hasInited;

	[SerializeField]
	private Vector3 positionOff = new Vector3(-100f, 255f, 0f);

	[SerializeField]
	private Vector3 positionOn = new Vector3(100f, 255f, 0f);

	[SerializeField]
	private float yOff = 200f;

	private float showDuration;

	public static event Action OnScoreBoostActivated;

	private IEnumerator AnimateColor(UISprite sprite, Color32 startValue, Color32 endValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			sprite.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
	}

	public void HidePowerup(bool instant, int index)
	{
		if (!_hasInited)
		{
			InitHelper();
		}
		powerupButtons[index].Hide(instant, positionOff + Vector3.up * yOff * index);
	}

	public void HidePowerups()
	{
		HidePowerups(false);
	}

	public void HidePowerups(bool instant)
	{
		Game.Instance.OnGameEnded = (Action)Delegate.Remove(Game.Instance.OnGameEnded, new Action(OnGameEnded));
		for (int i = 0; i < powerupButtons.Length; i++)
		{
			HidePowerup(instant, i);
		}
	}

	private void InitHelper()
	{
		for (int i = 0; i < powerupButtons.Length; i++)
		{
			powerupButtons[i].InitSlideinButton(this, i, types[i], positionOff + yOff * Vector3.up);
			PlayerInfo instance = PlayerInfo.Instance;
			instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(powerupButtons[i].UpdateAmount));
		}
		_hasInited = true;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < powerupButtons.Length; i++)
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(powerupButtons[i].UpdateAmount));
		}
	}

	private void OnDisable()
	{
		UIScreenController instance = UIScreenController.Instance;
		if (!(instance == null))
		{
			instance.OnChangedScreen = (Action<string>)Delegate.Remove(instance.OnChangedScreen, new Action<string>(ScreenDidChange));
		}
	}

	private void OnEnable()
	{
		UIScreenController.Instance.OnChangedScreen = (Action<string>)Delegate.Combine(UIScreenController.Instance.OnChangedScreen, new Action<string>(ScreenDidChange));
	}

	private void OnGameEnded()
	{
		HidePowerups();
	}

	private void ScreenDidChange(string screenOrPopupName)
	{
		if (screenOrPopupName.Equals("SaveMePopup"))
		{
			HidePowerups();
		}
	}

	public void ShowPowerup(int index)
	{
		if (PlayerInfo.Instance.GetUpgradeAmount(types[index]) > 0)
		{
			powerupButtons[index].Show(positionOff + yOff * Vector3.up * index, positionOn + yOff * Vector3.up * index);
		}
	}

	public void ShowPowerups()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
		for (int i = 0; i < powerupButtons.Length; i++)
		{
			ShowPowerup(i);
		}
		Game.Instance.OnGameEnded = (Action)Delegate.Combine(Game.Instance.OnGameEnded, new Action(OnGameEnded));
		showDuration = 5f;
		base.enabled = true;
	}

	private void Update()
	{
		if (showDuration > 0f)
		{
			showDuration -= Time.deltaTime;
			return;
		}
		HidePowerups();
		base.enabled = false;
	}

	public void SlideinPowerupClicked(int index)
	{
		if (Game.Instance.isPaused || !powerupButtons[index].GetComponent<Collider>().enabled)
		{
			return;
		}
		if (types[index] == PropType.scorebooster)
		{
			GameStats.Instance.scoreBooster5Activated = true;
			TasksManager.Instance.PlayerDidThis(TaskTarget.ScoreBooster);
			if (SlideinPowerupHelper.OnScoreBoostActivated != null)
			{
				SlideinPowerupHelper.OnScoreBoostActivated();
			}
			HidePowerup(false, index);
		}
		else if (types[index] == PropType.headstart2000)
		{
			_currentHeadstartGear++;
			int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.headstart2000);
			if (upgradeAmount > 0)
			{
				HidePowerup(false, index);
				Game.Instance.Megaheadstart();
				TasksManager.Instance.PlayerDidThis(TaskTarget.Headstart);
			}
		}
		PlayerInfo.Instance.UseUpgrade(types[index]);
	}

	private void Start()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
	}
}
