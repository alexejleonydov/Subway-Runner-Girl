using System;
using UnityEngine;

public class CharacterThemeButton : MonoBehaviour
{
	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private UISprite _locked;

	[SerializeField]
	private UISprite _select;

	[SerializeField]
	private Animation _anim;

	private bool _isSelected;

	private int _index;

	private Action<int> _onPress;

	public void AddOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onPress = (Action<int>)Delegate.Combine(_onPress, handler);
		}
	}

	private void ButtonPressed()
	{
		Action<int> onPress = _onPress;
		if (onPress != null)
		{
			onPress(_index);
		}
	}

	public void RemoveOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onPress = (Action<int>)Delegate.Remove(_onPress, handler);
		}
	}

	public void SetColors(string bgColor, string iconColor, int index, bool unlock, bool selected)
	{
		_background.spriteName = bgColor;
		_icon.spriteName = iconColor;
		_index = index;
		_locked.enabled = !unlock;
		_select.enabled = selected;
		_isSelected = selected;
	}

	public void SetIndex(int index)
	{
		_index = index;
	}

	public void Refresh(bool unlock, bool selected)
	{
		_locked.enabled = !unlock;
		if (selected != _isSelected)
		{
			OnSelected();
		}
	}

	public void OnSelected()
	{
		if (!_isSelected)
		{
			_isSelected = true;
			_select.transform.localScale = Vector3.one;
			_select.enabled = true;
			_anim.Play();
		}
	}

	public void UnSelect()
	{
		if (_isSelected)
		{
			_isSelected = false;
			_select.transform.localScale = Vector3.one;
			_select.enabled = false;
			_anim.Stop();
		}
	}
}
