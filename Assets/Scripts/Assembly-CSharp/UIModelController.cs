using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelController : MonoBehaviour
{
	public enum ModelScreen
	{
		Character = 0,
		GameOver = 1,
		TrialRolePopup = 2,
		TrialHelmetPopup = 3,
		Popup = 4,
		Helms = 5,
		CelebrationCharacterUnlock = 6,
		CelebrationHelmUnlock = 7,
		CelebrationHighScore = 8,
		DuelResultPopup = 9,
		CelebrationTopRun = 10
	}

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject CelebrationPopupAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject PauseScreenAnchor;

	public GameObject ModelPrefab;

	private CharacterModel _cachedActiveModel;

	private ModelScreen _currentActivatedScreenModel;

	private Helmets.HelmType _currentHelmetShown;

	private Characters.CharacterType _currentCharacterShownModel;

	private int _currentCThemeShownIndex;

	private static UIModelController _instance;

	private bool _isCelebrationCharacterScreen;

	private Helmets.HelmType _currentCelebrateHelmType;

	private Helmets.HelmType _currentTryHelmType;

	private bool isShownInCelebrate;

	public List<AnimationClip> highScoreClips;

	private bool isAnimationPlaying = true;

	public Helmets.HelmType currentHelmetShown
	{
		get
		{
			return _currentHelmetShown;
		}
	}

	public Characters.CharacterType currentCharacterModelShown
	{
		get
		{
			return _currentCharacterShownModel;
		}
	}

	public int currentCThemeShownIndex
	{
		get
		{
			return _currentCThemeShownIndex;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(UIModelController)) as UIModelController;
			}
			return _instance;
		}
	}

	private GameObject _ActivateModel(Characters.CharacterType characterName, int modelIndex, ModelScreen screen)
	{
		if (_cachedActiveModel != null && !isShownInCelebrate)
		{
			ClearModels();
		}
		else
		{
			isShownInCelebrate = false;
		}
		Characters.Model model = Characters.characterData[characterName];
		string modelName = model.modelName;
		_currentActivatedScreenModel = screen;
		switch (screen)
		{
			case ModelScreen.Character:
				{
					GameObject gameObject7 = Object.Instantiate(ModelPrefab);
					gameObject7.transform.parent = CharacterAnchor.transform;
					gameObject7.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalPos;
					Utility.SetLayerRecursively(gameObject7.transform, CharacterAnchor.layer);
					gameObject7.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalScl;
					gameObject7.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalRot);
					CharacterModel component7 = gameObject7.GetComponent<CharacterModel>();
					component7.ChangeCharacterModel(modelName, modelIndex);
					component7.HideAllPowerups();
					component7.StartIdleAnimations();
					_cachedActiveModel = component7;
					return gameObject7;
				}
			case ModelScreen.TrialRolePopup:
				{
					GameObject gameObject6 = Object.Instantiate(ModelPrefab);
					gameObject6.transform.parent = TutorialPopupAnchor.transform;
					gameObject6.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalPos;
					Utility.SetLayerRecursively(gameObject6.transform, TutorialPopupAnchor.layer);
					gameObject6.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalScl;
					gameObject6.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalRot);
					CharacterModel component6 = gameObject6.GetComponent<CharacterModel>();
					component6.ChangeCharacterModel(modelName, modelIndex);
					component6.HideAllPowerups();
					component6.StartIdleAnimations();
					_cachedActiveModel = component6;
					return gameObject6;
				}
			case ModelScreen.TrialHelmetPopup:
				{
					GameObject gameObject5 = Object.Instantiate(ModelPrefab);
					gameObject5.transform.parent = TutorialPopupAnchor.transform;
					gameObject5.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalPos;
					gameObject5.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalScl;
					gameObject5.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalRot);
					CharacterModel component5 = gameObject5.GetComponent<CharacterModel>();
					component5.ChangeCharacterModel(modelName, modelIndex);
					component5.HideAllPowerups();
					component5.StartTryAnimation();
					GameObject helmetRoot2 = component5.GetHelmetRoot();
					HelmetModelPreviewFactory.Instance.ChangeHelmet(_currentTryHelmType, helmetRoot2, component5.GetAnimation(), true);
					Utility.SetLayerRecursively(gameObject5.transform, TutorialPopupAnchor.layer);
					_cachedActiveModel = component5;
					return gameObject5;
				}
			case ModelScreen.GameOver:
				return null;
			case ModelScreen.Helms:
				{
					GameObject gameObject4 = Object.Instantiate(ModelPrefab);
					gameObject4.transform.parent = CharacterAnchor.transform;
					gameObject4.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalPos;
					Utility.SetLayerRecursively(gameObject4.transform, CharacterAnchor.layer);
					gameObject4.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalScl;
					gameObject4.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalRot);
					CharacterModel component4 = gameObject4.GetComponent<CharacterModel>();
					component4.ChangeCharacterModel(modelName, modelIndex);
					component4.HideAllPowerups();
					_cachedActiveModel = component4;
					gameObject4.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
					return gameObject4;
				}
			case ModelScreen.CelebrationCharacterUnlock:
				{
					GameObject gameObject3 = Object.Instantiate(ModelPrefab);
					gameObject3.transform.parent = TutorialPopupAnchor.transform;
					gameObject3.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalPos;
					Utility.SetLayerRecursively(gameObject3.transform, TutorialPopupAnchor.layer);
					gameObject3.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalScl;
					gameObject3.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalRot);
					CharacterModel component3 = gameObject3.GetComponent<CharacterModel>();
					component3.ChangeCharacterModel(modelName, modelIndex);
					component3.HideAllPowerups();
					component3.StartIdleAnimations();
					_isCelebrationCharacterScreen = true;
					return gameObject3;
				}
			case ModelScreen.CelebrationHelmUnlock:
				{
					GameObject gameObject2 = Object.Instantiate(ModelPrefab);
					gameObject2.transform.parent = TutorialPopupAnchor.transform;
					gameObject2.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalPos;
					gameObject2.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalScl;
					gameObject2.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalRot);
					CharacterModel component2 = gameObject2.GetComponent<CharacterModel>();
					component2.ChangeCharacterModel(modelName, modelIndex);
					component2.HideAllPowerups();
					GameObject helmetRoot = component2.GetHelmetRoot();
					HelmetModelPreviewFactory.Instance.ChangeHelmet(_currentCelebrateHelmType, helmetRoot, component2.GetAnimation(), true);
					Utility.SetLayerRecursively(gameObject2.transform, TutorialPopupAnchor.layer);
					_isCelebrationCharacterScreen = false;
					StartCoroutine("AnimateUnlockBackground", gameObject2);
					return gameObject2;
				}
			case ModelScreen.CelebrationHighScore:
				{
					GameObject gameObject = Object.Instantiate(ModelPrefab);
					gameObject.transform.parent = TutorialPopupAnchor.transform;
					gameObject.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalPos;
					gameObject.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalScl;
					gameObject.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalRot);
					CharacterModel component = gameObject.GetComponent<CharacterModel>();
					component.ChangeCharacterModel(modelName, modelIndex);
					component.HideAllPowerups();
					component.StartHighScoreAnimations();
					Utility.SetLayerRecursively(gameObject.transform, TutorialPopupAnchor.layer);
					_isCelebrationCharacterScreen = false;
					return gameObject;
				}
			default:
				return null;
		}
	}

	public void ActivateCelebrationHelmWithStripes(Helmets.HelmType helmType, ModelScreen screen)
	{
		isShownInCelebrate = true;
		_currentCelebrateHelmType = helmType;
		if (screen == ModelScreen.CelebrationHighScore)
		{
			_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, screen);
		}
		else
		{
			_ActivateModel(currentCharacterModelShown, PlayerInfo.Instance.currentThemeIndex, screen);
		}
	}

	public void ActivateGameOverModel()
	{
		_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, ModelScreen.GameOver);
	}

	public void ActivateTrailRoleModel(Characters.CharacterType characterType, int themeIndex)
	{
		_currentCharacterShownModel = characterType;
		_currentCThemeShownIndex = themeIndex;
		_ActivateModel(characterType, themeIndex, ModelScreen.TrialRolePopup);
	}

	public void ActivateTrailHelmetModel(Helmets.HelmType helmType)
	{
		_currentCharacterShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_currentCThemeShownIndex = PlayerInfo.Instance.currentThemeIndex;
		_currentTryHelmType = helmType;
		_ActivateModel(_currentCharacterShownModel, _currentCThemeShownIndex, ModelScreen.TrialHelmetPopup);
	}

	public void ActivateHelmetModel()
	{
		_currentCharacterShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_currentCThemeShownIndex = PlayerInfo.Instance.currentThemeIndex;
		_ActivateModel(_currentCharacterShownModel, _currentCThemeShownIndex, ModelScreen.Helms);
	}

	private IEnumerator AnimateRunningCharacterForBragCelebration(GameObject model)
	{
		Animation _charAnim = model.GetComponentInChildren<Animation>();
		for (int i = 0; i < highScoreClips.Count; i++)
		{
			if (_charAnim[highScoreClips[i].name] == null)
			{
				_charAnim.AddClip(highScoreClips[i], highScoreClips[i].name);
			}
		}
		AnimationClip select = null;
		isAnimationPlaying = true;
		while (isAnimationPlaying)
		{
			List<AnimationClip> clips = highScoreClips.FindAll((AnimationClip a) => a != select);
			select = clips[Random.Range(0, clips.Count)];
			float time = 0f;
			float waitTime = select.length;
			_charAnim.Play(select.name);
			while (time < waitTime)
			{
				time += Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator AnimateUnlockBackground(GameObject go)
	{
		Vector3 charWithHelmPosition = new Vector3(0f, -100f, 0f);
		Vector3 charPosition = new Vector3(-5f, -110f, 0f);
		int index = 0;
		Vector3 currentRotation2 = Vector3.zero;
		Vector3 currentOffset2 = Vector3.zero;
		Vector3 currentBackgroundRotationOffset = Vector3.zero;
		Vector3 currentCharPosition2 = Vector3.zero;
		if (_currentActivatedScreenModel != ModelScreen.CelebrationHighScore)
		{
			while (true)
			{
				switch (index % 3)
				{
					case 0:
						if (_isCelebrationCharacterScreen)
						{
							currentRotation2 = new Vector3(0f, 130f, 0f);
							currentOffset2 = new Vector3(0f, 0f, 180f);
							currentBackgroundRotationOffset = new Vector3(35f, 20f, 0f);
							currentCharPosition2 = charPosition;
						}
						else
						{
							currentRotation2 = new Vector3(27f, 205f, 16f);
							currentOffset2 = new Vector3(45f, 100f, 130f);
							currentCharPosition2 = charWithHelmPosition;
						}
						break;
					case 1:
						if (_isCelebrationCharacterScreen)
						{
							currentRotation2 = new Vector3(0f, 180f, 0f);
							currentOffset2 = new Vector3(0f, 0f, 200f);
							currentBackgroundRotationOffset = new Vector3(35f, 0f, 0f);
							currentCharPosition2 = charPosition;
						}
						else
						{
							currentRotation2 = new Vector3(350f, 90f, 350f);
							currentOffset2 = new Vector3(0f, 90f, 170f);
							currentCharPosition2 = charWithHelmPosition;
						}
						break;
					default:
						if (_isCelebrationCharacterScreen)
						{
							currentRotation2 = new Vector3(0f, 240f, 0f);
							currentOffset2 = new Vector3(5f, 0f, 200f);
							currentBackgroundRotationOffset = new Vector3(35f, -30f, 0f);
							currentCharPosition2 = charPosition;
						}
						else
						{
							currentRotation2 = new Vector3(10f, 210f, 0f);
							currentOffset2 = new Vector3(0f, 100f, 120f);
							currentCharPosition2 = charWithHelmPosition;
						}
						break;
				}
				UpdateCelebrationRotation(go, currentRotation2, currentCharPosition2, currentOffset2, currentBackgroundRotationOffset);
				index++;
				yield return new WaitForSeconds(2.5f);
			}
		}
		UpdateCelebrationRotation(backgroundRotationOffset: new Vector3(0f, 0f, 350f), go: go, charRotation: new Vector3(5f, 210f, 0f), charPosition: charWithHelmPosition, charPositionOffset: new Vector3(0f, 90f, 100f));
	}

	public void ClearModels()
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel = null;
		}
		Transform transform = null;
		IEnumerator enumerator = CharacterAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
		enumerator = GameOverAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
		enumerator = TutorialPopupAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
		enumerator = CelebrationPopupAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
	}

	public void ClearTutorialPopup()
	{
		IEnumerator enumerator = TutorialPopupAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Transform transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
	}

	public void DeactivateCelebrationPopupModels()
	{
		StopCoroutine("AnimateUnlockBackground");
		isAnimationPlaying = false;
		StopCoroutine("AnimateRunningCharacterForBragCelebration");
		isShownInCelebrate = false;
		IEnumerator enumerator = TutorialPopupAnchor.transform.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Transform transform = (Transform)enumerator.Current;
			Object.Destroy(transform.gameObject);
		}
	}

	public void ActivateTutorialPopup(bool active)
	{
		NGUITools.SetActive(TutorialPopupAnchor, active);
		_PauseAnimations(!active, TutorialPopupAnchor.transform);
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform trans2 = (Transform)enumerator.Current;
				_PauseAnimations(pause, trans2);
			}
		}
		finally
		{
		}
		CharacterModel component = trans.GetComponent<CharacterModel>();
		if (!(component != null))
		{
			return;
		}
		if (pause)
		{
			if (_currentActivatedScreenModel == ModelScreen.TrialHelmetPopup)
			{
				component.StopTryAnimations();
			}
			else
			{
				component.StopIdleAnimations();
			}
		}
		else if (_currentActivatedScreenModel == ModelScreen.TrialHelmetPopup)
		{
			component.StartTryAnimation();
		}
		else
		{
			component.StartIdleAnimations();
		}
	}

	public void SelectCharacterForPlay(Characters.CharacterType characterType, int themeIndex)
	{
		PlayerInfo.Instance.currentCharacter = (int)characterType;
		PlayerInfo.Instance.currentThemeIndex = themeIndex;
		if (Game.Instance != null)
		{
			Game.Instance.Character.characterModel.ChangeCharacterOfPlayByPlayerInfo();
			Game.Instance.Character.characterModel.StartIdleAnimations();
		}
	}

	public void SelectCurrentHelmShown(Helmets.HelmType currentlyShownHelm)
	{
		PlayerInfo.Instance.currentHelmet = currentlyShownHelm;
	}

	public void SelectThemeForCurrentCharacterModel(int themeIndex)
	{
		_currentCThemeShownIndex = themeIndex;
		SelectCharacterForPlay(currentCharacterModelShown, currentCThemeShownIndex);
	}

	public GameObject ShowCharacterInCelebration(Characters.CharacterType charType, int themeIndex, ModelScreen screen)
	{
		isShownInCelebrate = true;
		return _ActivateModel(charType, themeIndex, screen);
	}

	public void ShowHelmetMenuModel(Helmets.HelmType currentHelmShown, bool updateAnimation)
	{
		_currentHelmetShown = currentHelmShown;
		GameObject helmetRoot = _cachedActiveModel.GetHelmetRoot();
		HelmetModelPreviewFactory.Instance.ChangeHelmet(currentHelmShown, helmetRoot, _cachedActiveModel.GetAnimation(), updateAnimation);
	}

	public void ShowMenuCharacterModel(Characters.CharacterType charType, int themeIndex)
	{
		if (_currentCharacterShownModel != charType || _currentCThemeShownIndex != themeIndex || _cachedActiveModel == null)
		{
			_currentCharacterShownModel = charType;
			_currentCThemeShownIndex = themeIndex;
			_ChangeCharacterModel(charType, themeIndex);
		}
	}

	private void _ChangeCharacterModel(Characters.CharacterType characterName, int themeIndex)
	{
		if (_cachedActiveModel != null)
		{
			Characters.Model model = Characters.characterData[characterName];
			string modelName = model.modelName;
			_cachedActiveModel.ChangeCharacterModel(modelName, themeIndex);
			_cachedActiveModel.HideAllPowerups();
			_cachedActiveModel.StartIdleAnimations();
		}
		else
		{
			_ActivateModel(characterName, themeIndex, ModelScreen.Character);
		}
	}

	private void UpdateCelebrationRotation(GameObject go, Vector3 charRotation, Vector3 charPosition, Vector3 charPositionOffset, Vector3 backgroundRotationOffset)
	{
		if (go != null)
		{
			go.transform.localRotation = Quaternion.Euler(charRotation);
			go.transform.localPosition = charPosition + charPositionOffset;
		}
	}
}
