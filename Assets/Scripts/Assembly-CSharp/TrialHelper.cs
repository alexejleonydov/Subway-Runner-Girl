using System;
using UnityEngine;

public class TrialHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite tryIcon;

	private bool isActive;

	private void OnEnable()
	{
		if (TrialManager.Instance.nothingElse || TrialManager.Instance.currentTrialInfo == null)
		{
			isActive = false;
			base.gameObject.SetActive(false);
		}
		else if ((TrialManager.Instance.begainDateTime.AddDays(PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow).Ticks < 0)
		{
			isActive = false;
			base.gameObject.SetActive(false);
			TrialManager.Instance.currentTrialInfo = null;
		}
		else
		{
			isActive = true;
			base.gameObject.SetActive(true);
			tryIcon.spriteName = TrialManager.Instance.currentTrialInfo.icon;
			tryIcon.MakePixelPerfect();
		}
	}

	private void Update()
	{
		if (isActive)
		{
			if (TrialManager.Instance.nothingElse || TrialManager.Instance.currentTrialInfo == null)
			{
				isActive = false;
				base.gameObject.SetActive(false);
				TrialManager.Instance.currentTrialInfo = null;
			}
			else if ((TrialManager.Instance.begainDateTime.AddDays(PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow).Ticks < 0)
			{
				isActive = false;
				base.gameObject.SetActive(false);
				TrialManager.Instance.currentTrialInfo = null;
			}
			else
			{
				isActive = true;
				base.gameObject.SetActive(true);
			}
		}
	}
}
