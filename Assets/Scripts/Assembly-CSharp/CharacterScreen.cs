using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreen : UIBaseScreen, IScrollClick
{
	[SerializeField]
	private GameObject characterAnchor;

	[SerializeField]
	private GameObject saveMeSkillGo;

	[SerializeField]
	private UILabel skillLbl;

	[SerializeField]
	private UILabel skillContentLbl;

	[SerializeField]
	private UIGrid characterGrid;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private UICharacterThemeShifter characterThemeShifter;

	[SerializeField]
	private CharacterScreenSelectButton _characterSelectButton;

	[SerializeField]
	private GameObject centerAnchor;

	private float _cellWidth;

	private CenterOnChild _centerer;

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private bool _charactersEnabled;

	private bool _hasInited;

	private bool _hasShownModel;

	private bool _inappOverlayActive;

	private bool _popupActive;

	private CharacterScreenManager _screenManagerInstance;

	private List<OverlayIndex> characterIndices = new List<OverlayIndex>();

	private List<GameObject> characterModels = new List<GameObject>();

	public static bool forceCenterOnCurrentlySelectedCharacter;

	public static bool forceRefreshOnCharacterScreen;

	[SerializeField]
	private int minScale = 12;

	[SerializeField]
	private int maxScale = 16;

	[SerializeField]
	private UIDrag drag;

	private void OnEnable()
	{
		drag.onHandleDir = (UIDrag.OnSwipeDelegate)Delegate.Combine(drag.onHandleDir, new UIDrag.OnSwipeDelegate(OnHandleDir));
	}

	private void OnDisable()
	{
		drag.onHandleDir = (UIDrag.OnSwipeDelegate)Delegate.Remove(drag.onHandleDir, new UIDrag.OnSwipeDelegate(OnHandleDir));
	}

	private void OnHandleDir(SwipeDir dir)
	{
		Characters.CharacterType currenCharacterShown = _screenManagerInstance.currenCharacterShown;
		int num = Characters.characterOrder.IndexOf(currenCharacterShown);
		if (num != -1)
		{
			bool flag = false;
			if (dir == SwipeDir.Right && num > 0)
			{
				flag = true;
				num--;
			}
			if (dir == SwipeDir.Left && num < _characterList.Count - 1)
			{
				flag = true;
				num++;
			}
			if (flag)
			{
				_centerer.CenterOnTransform(characterIndices[num].transform, true);
				_centerer.Recenter();
			}
		}
	}

	private void CenterScrollOnCharacterType(Characters.CharacterType charType)
	{
		int num = 0;
		int i = 0;
		for (int count = _characterList.Count; i < count && charType != _characterList[i].Key; i++)
		{
			num++;
		}
		if (num >= characterIndices.Count)
		{
			Debug.LogWarning("CharacterScreen: Index of character: " + num + " - is bigger or equal than character indices count: " + characterIndices.Count);
			num = characterIndices.Count - 1;
		}
		bool flag = false;
		if (_centerer.centeredObject != null)
		{
			flag = true;
		}
		if (flag)
		{
			ChangeCurrentCharacterShow(_screenManagerInstance.currenCharacterShown, _screenManagerInstance.currentCThemeShownIndex);
			return;
		}
		_centerer.CenterOnTransform(characterIndices[num].transform, true);
		_centerer.Recenter();
	}

	private void ChangeCurrentCharacterShow(Characters.CharacterType charType, int index)
	{
		_screenManagerInstance.currenCharacterShown = charType;
		_screenManagerInstance.currentCThemeShownIndex = index;
		DisplayCharacter3dModel(charType, index);
		UpdateUIElements();
		List<CharacterTheme> themes = CharacterThemes.TryGetCustomThemesForChar(charType);
		bool flag = CharacterThemes.characterCustomThemes.ContainsKey(charType);
		characterThemeShifter.gameObject.SetActive(flag);
		if (flag)
		{
			characterThemeShifter.MakeCustomThemesAvailable(charType, themes);
			characterThemeShifter.UpdateUIForCharacter(charType, index);
		}
	}

	private void ClearArraysAndDestroyCachedGameObjects()
	{
		int i = 0;
		for (int count = characterModels.Count; i < count; i++)
		{
			UnityEngine.Object.Destroy(characterModels[i]);
		}
		characterModels.Clear();
		int j = 0;
		for (int count2 = characterIndices.Count; j < count2; j++)
		{
			UnityEngine.Object.Destroy(characterIndices[j].gameObject);
		}
		characterIndices.Clear();
		foreach (Transform item in characterGrid.transform)
		{
			item.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(item);
		}
		if (!characterAnchor.activeSelf)
		{
			characterAnchor.SetActive(true);
		}
		characterAnchor.transform.localPosition = new Vector3(0f, characterAnchor.transform.localPosition.y, characterAnchor.transform.localPosition.z);
		scrollPanel.cachedTransform.localPosition = Vector3.zero;
		scrollPanel.clipOffset = new Vector2(0f, 0f);
		_centerer.ClearCenterObject();
		SpringPanel component = scrollPanel.GetComponent<SpringPanel>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private void DisplayCharacter3dModel(Characters.CharacterType charType, int themeIndex)
	{
		UIModelController.Instance.ShowMenuCharacterModel(charType, themeIndex);
	}

	public override void Hide()
	{
		characterThemeShifter.RemoveOnChangeThemeListener(ThemeButtonClicked);
		UIModelController.Instance.ClearModels();
		CharacterScreenManager.Instance.RemoveOnCharacterUnlockedListener(OnCharacterUnlocked);
		CharacterScreenManager.Instance.RemoveOnShownCharacterSelectedListener(UpdateSelectedCharacter);
		base.Hide();
	}

	public override void Init()
	{
		base.Init();
		_screenManagerInstance = CharacterScreenManager.Instance;
		_cellWidth = characterGrid.cellWidth;
		_centerer = characterGrid.GetComponent<CenterOnChild>();
		InitScrollWithCharacterModels();
		InitializeSelectButton();
		InitializeCoinbox(true, true, true, true);
		forceCenterOnCurrentlySelectedCharacter = true;
		_hasInited = true;
	}

	protected override void AfterShow()
	{
		if (PlayerInfo.Instance.tutorialStep == 1)
		{
			UIScreenController.Instance.ShowTutorial(characterThemeShifter.Instantiate(), UIPosScalesAndNGUIAtlas.Instance.characterScreenShifterFingerOffset / UIScreenController.Instance.root.activeHeight * 2f, UIPosScalesAndNGUIAtlas.Instance.characterScreenShifterFingerRotZ);
		}
	}

	private void InitializeSelectButton()
	{
		_characterSelectButton.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.characterScreenSelectedButtonPos;
		characterThemeShifter.InitValues(_screenManagerInstance.currenCharacterShown, _screenManagerInstance.currentCThemeShownIndex);
		_characterSelectButton.InitButton();
	}

	private void InitScrollWithCharacterModels()
	{
		_screenManagerInstance.InitCharacters();
		_characterList = _screenManagerInstance.GetCharacterList();
		int num = 0;
		int i = 0;
		for (int count = _characterList.Count; i < count; i++)
		{
			Characters.CharacterType key = _characterList[i].Key;
			int lastSelectedThemeForCharacterType = _screenManagerInstance.GetLastSelectedThemeForCharacterType(key);
			GameObject characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(key.ToString(), lastSelectedThemeForCharacterType);
			if (characterModelSample == null)
			{
				Debug.LogError(string.Concat("Character Screen: GetCharacterModelPreview for character ", key, " has failed. Creating default model."));
				characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(Characters.CharacterType.frank.ToString(), 0);
			}
			characterModelSample.name = string.Format("{0:000}{1}", num, key.ToString());
			characterModels.Add(characterModelSample);
			Transform transform = characterModelSample.transform;
			transform.parent = characterAnchor.transform;
			transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
			transform.localScale = Vector3.one * minScale;
			transform.localEulerAngles = new Vector3(53f, 183f, 360f);
			transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			GameObject gameObject = NGUITools.AddChild(characterGrid.gameObject, dummyObject);
			characterIndices.Add(gameObject.AddComponent<OverlayIndex>());
			characterIndices[num].index = num;
			gameObject.name = string.Format("{0:000}{1}", num, key.ToString());
			num++;
		}
		Utility.SetLayerRecursively(characterAnchor.transform, 20);
		characterGrid.Reposition();
		ScaleScrollModels();
	}

	private void OnCharacterUnlocked(Characters.CharacterType charType, int version)
	{
		DisplayCharacter3dModel(charType, version);
		UpdateUIElements();
	}

	private void ScaleScrollModels()
	{
		float num = Mathf.Abs(characterAnchor.transform.localPosition.x);
		for (int i = 0; i < characterModels.Count; i++)
		{
			float num2 = Mathf.Abs(num - (float)i * _cellWidth);
			float num3 = 1.5f * _cellWidth;
			float num4 = Mathf.SmoothStep(maxScale, minScale, num2 / num3);
			characterModels[i].transform.localScale = Vector3.one * num4;
		}
	}

	public void ScrollClicked(Vector2 pos)
	{
		Characters.CharacterType currenCharacterShown = _screenManagerInstance.currenCharacterShown;
		bool flag = _centerer.CenterOnClosestChildAtPosition(pos);
		bool flag2 = PlayerInfo.Instance.IsCollectionComplete(currenCharacterShown);
		int indexForLastSelectedTheme = PlayerInfo.Instance.GetIndexForLastSelectedTheme(currenCharacterShown);
		bool flag3 = indexForLastSelectedTheme == _screenManagerInstance.currentCThemeShownIndex;
		if (flag && flag2 && flag3)
		{
			UIModelController.Instance.SelectThemeForCurrentCharacterModel(indexForLastSelectedTheme);
			UpdateSelectedCharacter();
			NGUITools.PlaySound(selectSound);
		}
	}

	public override void Show()
	{
		base.Show();
		if (PlayerInfo.Instance.tutorialStep == 1)
		{
			UIScreenController.Instance.ReadyTutorial();
		}
		characterThemeShifter.AddOnChangeThemeListener(ThemeButtonClicked);
		CharacterScreenManager.Instance.AddOnCharacterUnlockedListener(OnCharacterUnlocked);
		CharacterScreenManager.Instance.AddOnShownCharacterSelectedListener(UpdateSelectedCharacter);
		if (forceCenterOnCurrentlySelectedCharacter)
		{
			forceCenterOnCurrentlySelectedCharacter = false;
			CenterScrollOnCharacterType((Characters.CharacterType)PlayerInfo.Instance.currentCharacter);
		}
		else if (_hasShownModel)
		{
			ChangeCurrentCharacterShow(_screenManagerInstance.currenCharacterShown, _screenManagerInstance.currentCThemeShownIndex);
		}
		else
		{
			Update();
		}
		characterThemeShifter.gameObject.SetActive(CharacterThemes.characterCustomThemes.ContainsKey(_screenManagerInstance.currenCharacterShown));
	}

	private void ThemeButtonClicked(int index)
	{
		_screenManagerInstance.currentCThemeShownIndex = index;
		DisplayCharacter3dModel(_screenManagerInstance.currenCharacterShown, index);
		UpdateUIElements();
		characterThemeShifter.UpdateUIForCharacter(_screenManagerInstance.currenCharacterShown, index);
		if (PlayerInfo.Instance.tutorialStep == 1)
		{
			UIScreenController.Instance.ShowTutorial(Instantiate(), UIPosScalesAndNGUIAtlas.Instance.characterScreenSelectFingerOffset / UIScreenController.Instance.root.activeHeight * 2f, UIPosScalesAndNGUIAtlas.Instance.characterScreenSelectFingerRotZ);
		}
	}

	public GameObject Instantiate()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_characterSelectButton.gameObject);
		gameObject.transform.parent = _characterSelectButton.transform.parent;
		gameObject.transform.localPosition = Vector3.up * _characterSelectButton.transform.localPosition.y;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<CharacterScreenSelectButton>().InitButton();
		return gameObject;
	}

	private void Update()
	{
		if (!_hasInited)
		{
			return;
		}
		if (_centerer.centeredObject != null)
		{
			int index = _centerer.centeredObject.GetComponent<OverlayIndex>().index;
			Characters.CharacterType key = _characterList[index].Key;
			if (!_hasShownModel)
			{
				_hasShownModel = true;
				ChangeCurrentCharacterShow(key, PlayerInfo.Instance.GetIndexForLastSelectedTheme(key));
			}
			Characters.CharacterType currenCharacterShown = _screenManagerInstance.currenCharacterShown;
			if (currenCharacterShown != key)
			{
				int index2 = PlayerInfo.Instance.GetIndexForLastSelectedTheme(key);
				if (key == (Characters.CharacterType)PlayerInfo.Instance.currentCharacter)
				{
					index2 = PlayerInfo.Instance.currentThemeIndex;
				}
				ChangeCurrentCharacterShow(key, index2);
			}
		}
		ScaleScrollModels();
		if (UIScreenController.Instance.isShowingPopup)
		{
			if (!_popupActive || !_inappOverlayActive)
			{
				_popupActive = true;
			}
		}
		else if (_popupActive && !_inappOverlayActive)
		{
			_popupActive = false;
		}
		if (_inappOverlayActive)
		{
			_inappOverlayActive = false;
		}
		if (_inappOverlayActive || _popupActive)
		{
			if (_charactersEnabled)
			{
				characterAnchor.SetActive(false);
				_charactersEnabled = false;
			}
		}
		else if (!_charactersEnabled)
		{
			characterAnchor.SetActive(true);
			_charactersEnabled = true;
		}
	}

	private void UpdateSelectedCharacter()
	{
		Characters.CharacterType currentCharacterModelShown = UIModelController.Instance.currentCharacterModelShown;
		int currentCThemeShownIndex = UIModelController.Instance.currentCThemeShownIndex;
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentCharacterModelShown);
		bool flag2 = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentCharacterModelShown, currentCThemeShownIndex);
		if (flag && flag2)
		{
			PlayerInfo.Instance.SetLastSelectedTheme(currentCharacterModelShown, currentCThemeShownIndex);
			characterThemeShifter.UpdateUIForCharacter(currentCharacterModelShown, currentCThemeShownIndex);
			characterThemeShifter.UpdateUIForSelectCharacter(currentCThemeShownIndex);
			_characterSelectButton.ReloadButton();
			RefreshCharacterModelInScrollList(currentCharacterModelShown, currentCThemeShownIndex);
		}
		else
		{
			Debug.LogWarning("Trying to select a character or theme we don't currently own.");
		}
	}

	private void RefreshCharacterModelInScrollList(Characters.CharacterType ctype, int themeIndex)
	{
		int num = 0;
		GameObject gameObject = characterModels.Find((GameObject g) => g.name.Substring(3).Equals(ctype.ToString()));
		num = characterModels.IndexOf(gameObject);
		characterModels.Remove(gameObject);
		GameObject characterModelSample = CharacterModelSampleFactory.Instance.GetCharacterModelSample(ctype.ToString(), themeIndex);
		characterModels.Insert(num, characterModelSample);
		characterModelSample.name = gameObject.name;
		Transform transform = characterModelSample.transform;
		Transform transform2 = gameObject.transform;
		if (!characterAnchor.activeSelf)
		{
			characterAnchor.SetActive(true);
		}
		transform.parent = characterAnchor.transform;
		transform.localPosition = transform2.localPosition;
		transform.localScale = transform2.localScale;
		transform.localEulerAngles = transform2.localEulerAngles;
		Utility.SetLayerRecursively(transform, 20);
	}

	private void UpdateUIElements()
	{
		Characters.Model model = Characters.characterData[_screenManagerInstance.currenCharacterShown];
		if (model.freeReviveCount > 0)
		{
			skillLbl.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL_TITLE_R);
			skillContentLbl.text = string.Format(Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_R), model.freeReviveCount);
			saveMeSkillGo.SetActive(true);
		}
		else
		{
			saveMeSkillGo.SetActive(false);
		}
		_characterSelectButton.ReloadButton();
	}
}
