using System;
using System.Collections.Generic;

public class CharacterScreenManager
{
	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private Characters.CharacterType _currentlyShownCharacter;

	private bool _hasCenteredOnCharacter;

	private static CharacterScreenManager _instance;

	private Action _onCharacterSelected;

	private Action<Characters.CharacterType, int> _onCharacterUnlocked;

	private PlayerInfo _playerInfo = PlayerInfo.Instance;

	private bool _purchaseInProgress;

	public Characters.CharacterType currenCharacterShown
	{
		get
		{
			if (!_hasCenteredOnCharacter)
			{
				_hasCenteredOnCharacter = true;
				_currentlyShownCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
			}
			return _currentlyShownCharacter;
		}
		set
		{
			_currentlyShownCharacter = value;
		}
	}

	public int currentCThemeShownIndex { get; set; }

	public static CharacterScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CharacterScreenManager();
			}
			return _instance;
		}
	}

	private CharacterScreenManager()
	{
	}

	public void AddOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			_onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Combine(_onCharacterUnlocked, handler);
		}
	}

	public void AddOnShownCharacterSelectedListener(Action handler)
	{
		if (handler != null)
		{
			_onCharacterSelected = (Action)Delegate.Combine(_onCharacterSelected, handler);
		}
	}

	public void CharacterPurchaseFailure()
	{
		if (_purchaseInProgress)
		{
			_purchaseInProgress = false;
		}
	}

	public void CharacterPurchaseSuccessful(Characters.CharacterType purchasedCharacter, int themeIndex)
	{
		if (_purchaseInProgress)
		{
			switch (Characters.characterOrder.IndexOf(purchasedCharacter))
			{
			case 0:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles1st", 0);
				break;
			case 1:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles2nd", 0);
				break;
			case 2:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles3rd", 0);
				break;
			case 3:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles4th", 0);
				break;
			case 4:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles5th", 0);
				break;
			case 5:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles6th", 0);
				break;
			case 6:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles7th", 0);
				break;
			case 7:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles8th", 0);
				break;
			case 8:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles9th", 0);
				break;
			case 9:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles10th", 0);
				break;
			case 10:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles11th", 0);
				break;
			case 11:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles12th", 0);
				break;
			case 12:
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_roles13th", 0);
				break;
			}
			_purchaseInProgress = false;
			OnCharacterUnlocked(purchasedCharacter, themeIndex);
			UIScreenController.Instance.ShowUnlockAnimationForCharacter(purchasedCharacter, themeIndex);
			if (TrialManager.Instance.IsCurrentCharacterTrial(purchasedCharacter, themeIndex))
			{
				TrialManager.Instance.currentTrialInfo = null;
			}
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public List<KeyValuePair<Characters.CharacterType, Characters.Model>> GetCharacterList()
	{
		return _characterList;
	}

	public int GetLastSelectedThemeForCharacterType(Characters.CharacterType charType)
	{
		int result = 0;
		if (CharacterThemes.GetThemeForCharacter(charType, _playerInfo.GetIndexForLastSelectedTheme(charType)) != null)
		{
			result = _playerInfo.GetIndexForLastSelectedTheme(charType);
		}
		return result;
	}

	public void InitCharacters()
	{
		_characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		int i = 0;
		for (int count = Characters.characterOrder.Count; i < count; i++)
		{
			Characters.Model value = Characters.characterData[Characters.characterOrder[i]];
			if (_playerInfo.IsCollectionComplete(Characters.characterOrder[i]) || _playerInfo.isCharacterActive(Characters.characterOrder[i]))
			{
				_characterList.Add(new KeyValuePair<Characters.CharacterType, Characters.Model>(Characters.characterOrder[i], value));
			}
		}
	}

	private void OnCharacterUnlocked(Characters.CharacterType character, int version)
	{
		if (_onCharacterUnlocked != null)
		{
			SelectCharacter(character, version);
			_onCharacterUnlocked(character, version);
		}
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, int themeIndex)
	{
		if (!_purchaseInProgress)
		{
			_purchaseInProgress = true;
			PurchaseHandler.Instance.PurchaseCharacter(characterType, themeIndex, false);
		}
	}

	public void RemoveOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			_onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Remove(_onCharacterUnlocked, handler);
		}
	}

	public void RemoveOnShownCharacterSelectedListener(Action handler)
	{
		if (handler != null)
		{
			_onCharacterSelected = (Action)Delegate.Remove(_onCharacterSelected, handler);
		}
	}

	public void SelectCharacter(Characters.CharacterType charType, int themeIndex)
	{
		UIModelController.Instance.SelectCharacterForPlay(charType, themeIndex);
		CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
		if (_onCharacterSelected != null)
		{
			_onCharacterSelected();
		}
	}
}
