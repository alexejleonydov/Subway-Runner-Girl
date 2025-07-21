using System;
using System.Collections;
using UnityEngine;

public class UIHelmetUpdateLabel : MonoBehaviour
{
	private bool _countUpCoroutineRunning;

	private int _lastAmountSet = -1;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite blinkSprite;

	private static readonly Vector3 SCROLL_TEXT_ENDOFFSET = new Vector3(0f, 20f, 0f);

	private static readonly Vector3 SCROLL_TEXT_STARTOFFSET = new Vector3(0f, -20f, 0f);

	[SerializeField]
	private GameObject scrollingLabelPrefab;

	private IEnumerator CountUpAndFlashCoroutine()
	{
		if (_countUpCoroutineRunning)
		{
			yield break;
		}
		_countUpCoroutineRunning = true;
		int targetAmount;
		while (true)
		{
			targetAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
			if (_lastAmountSet >= targetAmount)
			{
				break;
			}
			_lastAmountSet++;
			amountLabel.text = _lastAmountSet.ToString();
			SpawnScrollingLabel();
			blinkSprite.enabled = true;
			float aniFactor = 0f;
			while (aniFactor < 1f)
			{
				aniFactor = Mathf.Clamp01(aniFactor + 4f * Time.deltaTime);
				blinkSprite.color = new Color(1f, 1f, 1f, 1f - aniFactor);
				yield return null;
			}
			blinkSprite.enabled = false;
			yield return new WaitForSeconds(0.1f);
		}
		if (_lastAmountSet != targetAmount)
		{
			amountLabel.text = targetAmount.ToString();
			_lastAmountSet = targetAmount;
		}
		_countUpCoroutineRunning = false;
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		_countUpCoroutineRunning = false;
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(UpdateLabels));
		_lastAmountSet = -1;
		UpdateLabels();
		blinkSprite.enabled = false;
	}

	private void SpawnScrollingLabel()
	{
		ScrollingTextLabel component = NGUITools.AddChild(base.gameObject, scrollingLabelPrefab).GetComponent<ScrollingTextLabel>();
		if (component != null)
		{
			component.StartScrolling("+1", SCROLL_TEXT_STARTOFFSET, SCROLL_TEXT_ENDOFFSET, 1f, 0.5f, true);
		}
		else
		{
			Debug.LogError("No ScrollingTextLabel component on scrollingLabelPrefab");
		}
	}

	private void UpdateLabels()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		if (_lastAmountSet == -1)
		{
			amountLabel.text = upgradeAmount.ToString();
			_lastAmountSet = upgradeAmount;
		}
		else if (_lastAmountSet < upgradeAmount)
		{
			if (!_countUpCoroutineRunning)
			{
				StartCoroutine(CountUpAndFlashCoroutine());
			}
		}
		else
		{
			amountLabel.text = upgradeAmount.ToString();
			_lastAmountSet = upgradeAmount;
		}
	}
}
