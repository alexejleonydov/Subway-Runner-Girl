using System.Collections.Generic;
using UnityEngine;

public class UIPowerupHandler : MonoBehaviour
{
	private int _bottomOffset = 5;

	private Vector3 _offScreenPosition = new Vector3(0f, -1000f, 0f);

	private List<UIPowerupHelper> _powerupSlots = new List<UIPowerupHelper>();

	private int _screenOffset = 26;

	public GameObject PowerupPrefab;

	public int getNumberOfSpaces(int i)
	{
		if (i % 2 == 0)
		{
			return _screenOffset / 4 * Mathf.CeilToInt((float)i / 2f + 1f);
		}
		return _screenOffset / 4 * Mathf.CeilToInt((float)i / 2f);
	}

	public float getSliderHights(int i, int slotPosition)
	{
		return (float)Mathf.FloorToInt(i / 2) * _powerupSlots[slotPosition].getSliderHeight();
	}

	private void Update()
	{
		List<ActiveProp> activePowerups = GameStats.Instance.GetActivePowerups();
		int i = 0;
		for (int num = activePowerups.Count - 1; num >= 0; num--)
		{
			if (_powerupSlots.Count < activePowerups.Count)
			{
				int count = _powerupSlots.Count;
				GameObject gameObject = NGUITools.AddChild(base.gameObject, PowerupPrefab);
				gameObject.transform.localPosition = _offScreenPosition;
				_powerupSlots.Add(gameObject.GetComponent<UIPowerupHelper>());
				if (count % 2 == 0)
				{
					_powerupSlots[count].Anchor.side = UIAnchor.Side.BottomLeft;
				}
				else
				{
					_powerupSlots[count].Anchor.side = UIAnchor.Side.BottomRight;
				}
				if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
				{
					_powerupSlots[count].Anchor.pixelOffset.y = 102f;
				}
				else
				{
					_powerupSlots[count].Anchor.pixelOffset.y = 0f;
				}
			}
			_powerupSlots[i].SetPowerupSlot(activePowerups[num]);
			if (!_powerupSlots[i].gameObject.activeInHierarchy)
			{
				_powerupSlots[i].gameObject.SetActive(true);
			}
			float x = _screenOffset + _powerupSlots[i].getHalfIconBGWidth();
			if (_powerupSlots[i].Anchor.side == UIAnchor.Side.BottomRight)
			{
				x = _powerupSlots[i].getSliderWidth() * -1 - _screenOffset;
			}
			float num2 = 0f;
			num2 = (float)getNumberOfSpaces(num) + getSliderHights(num, i) + (float)_bottomOffset;
			_powerupSlots[i].setContainerPosition(x, num2, 0f);
			i++;
		}
		for (; i < _powerupSlots.Count; i++)
		{
			if (_powerupSlots[i].gameObject.activeInHierarchy)
			{
				_powerupSlots[i].HidePowerupSlot();
				_powerupSlots[i].transform.localPosition = _offScreenPosition;
				_powerupSlots[i].gameObject.SetActive(false);
			}
		}
	}
}
