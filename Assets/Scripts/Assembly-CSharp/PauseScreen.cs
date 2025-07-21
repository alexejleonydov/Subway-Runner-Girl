using System;
using UnityEngine;

public class PauseScreen : UIBaseScreen
{
	[SerializeField]
	private TaskList taskList;

	private void OnEnable()
	{
		SettingsPopup.IsInPause = true;
	}

	private void ShowNativeAd()
	{
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			taskList.transform.localPosition = Vector3.up * 400f;
		}
		else
		{
			taskList.transform.localPosition = Vector3.up * 600f;
		}
	}

	private void OnDisable()
	{
		SettingsPopup.IsInPause = false;
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance != null)
		{
			Game.Instance.TriggerPause(true);
			int magnitude = (int)(Time.time - Character.Instance.sameLaneTimeStamp);
			Character.Instance.sameLaneTimeStamp = Time.time;
			TasksManager.Instance.PlayerDidThis(TaskTarget.StayInOneLane, magnitude);
			GC.Collect();
		}
		RiseSdk.Instance.enableBackHomeAd(false, "custom");
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			taskList.gameObject.SetActive(false);
		}
		else
		{
			taskList.Show();
		}
		ShowNativeAd();
	}

	public override void Hide()
	{
		if (!PlayerInfo.Instance.hasRemoveAd)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom");
		}
		base.Hide();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		ShowNativeAd();
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void OnMenuClick()
	{
		UIScreenController.Instance.GoToMainMenuFromGame(base.gameObject);
	}
}
