using System;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterThemeShifter : MonoBehaviour
{
	[SerializeField]
	private CharacterThemeButton _defaultCharacterThemeButton;

	[SerializeField]
	private CharacterThemeButton _customCharacterThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	[SerializeField]
	private UISprite _bg;

	private int _activeTheme;

	private Characters.CharacterType _cachedCharacter;

	private List<CharacterThemeButton> _customButtons = new List<CharacterThemeButton>();

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private Action<int> _onThemeButtonPressed;

	private CharacterTheme[] _sortedThemes;

	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	public void AddOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onThemeButtonPressed = (Action<int>)Delegate.Combine(_onThemeButtonPressed, handler);
		}
	}

	private void Awake()
	{
		Vector3 localPosition = _defaultCharacterThemeButton.transform.localPosition;
		Vector3 localPosition2 = _customCharacterThemeButton.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		_customButtons.Add(_customCharacterThemeButton);
		_defaultCharacterThemeButton.AddOnPressListener(ThemeButtonPressed);
		_customButtons[0].AddOnPressListener(ThemeButtonPressed);
		_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
	}

	public void InitValues(Characters.CharacterType charType, int index)
	{
		_cachedCharacter = charType;
		_activeTheme = index;
	}

	public void MakeCustomThemesAvailable(Characters.CharacterType charType, List<CharacterTheme> themes)
	{
		if (themes.Count > 0)
		{
			_sortedThemes = SortThemes(themes);
		}
		int i = 0;
		for (int count = _customButtons.Count; i < count; i++)
		{
			_customButtons[i].gameObject.SetActive(false);
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count > 3) ? 3 : themes.Count);
		}
		_bg.height = (num + 1) * 125;
		Characters.Model model = Characters.characterData[charType];
		bool unlock = PlayerInfo.Instance.IsCollectionComplete(charType);
		bool selected = PlayerInfo.Instance.currentCharacter == (int)charType && PlayerInfo.Instance.currentThemeIndex == 0;
		_defaultCharacterThemeButton.SetColors(model.buttonBgSpriteName, model.buttonIconSpriteName, 0, unlock, selected);
		for (int j = 0; j < num; j++)
		{
			if (_customButtons.Count > j)
			{
				CharacterThemeButton characterThemeButton = _customButtons[j];
				if (_sortedThemes != null && _sortedThemes.Length > j)
				{
					CharacterTheme characterTheme = _sortedThemes[j];
					unlock = PlayerInfo.Instance.IsThemeUnlockedForCharacter(charType, j + 1);
					selected = PlayerInfo.Instance.currentCharacter == (int)charType && PlayerInfo.Instance.currentThemeIndex == j + 1;
					characterThemeButton.SetColors(characterTheme.buttonBgSpriteName, characterTheme.buttonIconSpriteName, j + 1, unlock, selected);
				}
				characterThemeButton.gameObject.SetActive(true);
				continue;
			}
			CharacterThemeButton component = UnityEngine.Object.Instantiate(_customCharacterThemeButton.gameObject).GetComponent<CharacterThemeButton>();
			if (component != null)
			{
				component.transform.parent = base.gameObject.transform;
				component.transform.localPosition = _customCharacterThemeButton.transform.localPosition + _distanceBetweenButtons * j;
				component.transform.localScale = Vector3.one;
				component.name = j + 1 + "CharThemeBtn";
				if (_sortedThemes != null && _sortedThemes.Length > j)
				{
					CharacterTheme characterTheme2 = _sortedThemes[j];
					unlock = PlayerInfo.Instance.IsThemeUnlockedForCharacter(charType, j + 1);
					selected = PlayerInfo.Instance.currentCharacter == (int)charType && PlayerInfo.Instance.currentThemeIndex == j + 1;
					component.SetColors(characterTheme2.buttonBgSpriteName, characterTheme2.buttonIconSpriteName, j + 1, unlock, selected);
				}
				_customButtons.Add(component);
				component.AddOnPressListener(ThemeButtonPressed);
			}
		}
	}

	public GameObject Instantiate()
	{
		CharacterThemeButton component = UnityEngine.Object.Instantiate(_customCharacterThemeButton.gameObject).GetComponent<CharacterThemeButton>();
		component.transform.parent = base.gameObject.transform;
		component.transform.localPosition = Vector3.up * _customCharacterThemeButton.transform.localPosition.y;
		component.transform.localScale = Vector3.one;
		component.name = "CharThemeBtn";
		component.SetIndex(1);
		component.AddOnPressListener(ThemeButtonPressed);
		return component.gameObject;
	}

	private void Refresh()
	{
		bool unlock = PlayerInfo.Instance.IsCollectionComplete(_cachedCharacter);
		bool selected = PlayerInfo.Instance.currentCharacter == (int)_cachedCharacter && PlayerInfo.Instance.currentThemeIndex == 0;
		_defaultCharacterThemeButton.Refresh(unlock, selected);
		int i = 0;
		for (int num = _sortedThemes.Length; i < num; i++)
		{
			CharacterThemeButton characterThemeButton = _customButtons[i];
			if (_sortedThemes != null && _sortedThemes.Length > i)
			{
				CharacterTheme characterTheme = _sortedThemes[i];
				unlock = PlayerInfo.Instance.IsThemeUnlockedForCharacter(_cachedCharacter, i + 1);
				selected = PlayerInfo.Instance.currentCharacter == (int)_cachedCharacter && PlayerInfo.Instance.currentThemeIndex == i + 1;
				characterThemeButton.Refresh(unlock, selected);
			}
		}
	}

	private void OnEnable()
	{
		PlayerInfo.Instance.OnSubscribed = (Action)Delegate.Combine(PlayerInfo.Instance.OnSubscribed, new Action(Refresh));
	}

	private void OnDisable()
	{
		int i = 0;
		for (int count = _customButtons.Count; i < count; i++)
		{
			_customButtons[i].gameObject.SetActive(false);
		}
		PlayerInfo.Instance.OnSubscribed = (Action)Delegate.Remove(PlayerInfo.Instance.OnSubscribed, new Action(Refresh));
	}

	public void RemoveOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onThemeButtonPressed = (Action<int>)Delegate.Remove(_onThemeButtonPressed, handler);
		}
	}

	private void SelectButton(int index)
	{
		bool flag = index == 0;
		if (_selectedMarker != null)
		{
			if (flag)
			{
				_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
			}
			else
			{
				_selectedMarker.transform.position = _customButtons[index - 1].transform.position;
			}
		}
	}

	private void SelectedCharacter(int index)
	{
		if (index == 0)
		{
			_defaultCharacterThemeButton.OnSelected();
			if (_sortedThemes != null)
			{
				int i = 0;
				for (int num = _sortedThemes.Length; i < num; i++)
				{
					_customButtons[i].UnSelect();
				}
			}
			return;
		}
		_defaultCharacterThemeButton.UnSelect();
		int j = 0;
		for (int num2 = _sortedThemes.Length; j < num2; j++)
		{
			if (index - 1 == j)
			{
				_customButtons[j].OnSelected();
			}
			else
			{
				_customButtons[j].UnSelect();
			}
		}
	}

	private CharacterTheme[] SortThemes(List<CharacterTheme> themes)
	{
		CharacterTheme[] array = new CharacterTheme[themes.Count];
		List<CharacterTheme> list = new List<CharacterTheme>();
		list.AddRange(themes);
		int num = int.MaxValue;
		CharacterTheme characterTheme = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			int j = 0;
			for (int count = list.Count; j < count; j++)
			{
				if (list[j].uiPriority < num)
				{
					num = list[j].uiPriority;
					characterTheme = list[j];
				}
			}
			array[i] = characterTheme;
			list.Remove(characterTheme);
			num = int.MaxValue;
		}
		return array;
	}

	private void ThemeButtonPressed(int index)
	{
		if (_onThemeButtonPressed != null)
		{
			_onThemeButtonPressed(index);
		}
	}

	public void UpdateUIForCharacter(Characters.CharacterType charType, int outfitIndex)
	{
		if (_activeTheme != outfitIndex || _cachedCharacter != charType)
		{
			_cachedCharacter = UIModelController.Instance.currentCharacterModelShown;
			_activeTheme = outfitIndex;
			PlayerInfo.Instance.ThemeSeen(_cachedCharacter, outfitIndex);
			SelectButton(outfitIndex);
		}
	}

	public void UpdateUIForSelectCharacter(int outfitIndex)
	{
		if (base.gameObject.activeInHierarchy)
		{
			SelectedCharacter(outfitIndex);
		}
	}
}
