using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour, ICharacterModel
{
	[OptionalField]
	public SkinnedMeshRenderer currentRender;

	[OptionalField]
	public SkinnedMeshRenderer meshSuperShoes;

	[OptionalField]
	public MeshRenderer meshHelmet;

	[OptionalField]
	public MeshRenderer meshCoinMagnet;

	[OptionalField]
	public Animation animFlypack;

	[OptionalField]
	public MeshRenderer[] meshFlypack;

	[OptionalField]
	public MeshRenderer meshBlobShadow;

	[OptionalField]
	public Animation characterAnimation;

	[OptionalField]
	public MeshRenderer shadow;

	[OptionalField]
	public Transform spineTransform;

	[OptionalField]
	public Transform shoulderTransform;

	[OptionalField]
	public Transform flypackCloudPosition;

	[OptionalField]
	public Transform leftFoot;

	[OptionalField]
	public Transform rightFoot;

	[OptionalField]
	public Transform headJoint;

	[OptionalField]
	public Transform footJoint;

	[OptionalField]
	public Transform rightHand;

	[HideInInspector]
	public GameObject currentRaft;

	[HideInInspector]
	public GameObject currentHelmet;

	public Animation PlayBall;

	public Animation Flower;

	public Animation Heart;

	[HideInInspector]
	public List<GameObject> currentModuleHelmModels = new List<GameObject>();

	[HideInInspector]
	public List<Renderer> currentModuleHelmModelsRenderes = new List<Renderer>();

	private Animation _animationComponent;

	private CharacterCustomization characterCustomization;

	private Transform helmetRoot;

	private Transform raftRoot;

	public Transform BoneFoot
	{
		get
		{
			return footJoint;
		}
	}

	public Transform BoneHead
	{
		get
		{
			return headJoint;
		}
	}

	public Transform BoneHelmet
	{
		get
		{
			return (!(meshHelmet != null)) ? null : meshHelmet.transform;
		}
	}

	public Transform BoneRightHand
	{
		get
		{
			return rightHand;
		}
	}

	public void Awake()
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		characterCustomization = base.gameObject.GetComponent<CharacterCustomization>();
		ChangeCharacterOfPlayByPlayerInfo();
		helmetRoot = meshHelmet.transform;
	}

	public void SetRaft(GameObject raft)
	{
		currentRaft = raft;
		raft.transform.parent = raftRoot.transform;
		raft.transform.localPosition = Vector3.zero;
		raft.transform.localRotation = Quaternion.identity;
		raft.transform.localScale = Vector3.one;
	}

	public void RemoveRaft()
	{
		currentRaft.transform.parent = null;
		currentRaft = null;
	}

	public GameObject SetNewHelmet(GameObject root, GameObject newHelm, string helmName)
	{
		if (currentHelmet != null)
		{
			Object.Destroy(currentHelmet);
		}
		DeleteHelmModels();
		GameObject gameObject = Object.Instantiate(newHelm);
		gameObject.transform.parent = root.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = helmName;
		Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].gameObject.layer = Layers.Instance._3DGUI;
		}
		currentHelmet = gameObject;
		return gameObject;
	}

	public void AddModuleHelmetModel(GameObject modelPrefab, bool isMenu, Transform parent)
	{
		if (!(modelPrefab != null))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(modelPrefab);
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		currentModuleHelmModels.Add(gameObject);
		currentModuleHelmModelsRenderes.Add(gameObject.GetComponentInChildren<Renderer>());
		if (isMenu)
		{
			gameObject.layer = Layers.Instance._3DGUI;
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].gameObject.layer = Layers.Instance._3DGUI;
			}
		}
	}

	public void AddModuleHelmetMenuFX(GameObject modelPrefab, Transform parent)
	{
		if (modelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(modelPrefab);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			currentModuleHelmModels.Add(gameObject);
		}
	}

	public GameObject AddHelmetModel()
	{
		currentHelmet = Object.Instantiate(HelmetModelPreviewFactory.Instance.GetHelmet());
		currentHelmet.transform.parent = helmetRoot;
		currentHelmet.transform.localPosition = Vector3.zero;
		currentHelmet.transform.localRotation = Quaternion.identity;
		currentHelmet.transform.localScale = Vector3.one;
		return currentHelmet;
	}

	public void DeleteHelmModels()
	{
		if (currentModuleHelmModels.Count > 0)
		{
			int i = 0;
			for (int count = currentModuleHelmModels.Count; i < count; i++)
			{
				Object.Destroy(currentModuleHelmModels[i]);
			}
			currentModuleHelmModels.Clear();
			currentModuleHelmModelsRenderes.Clear();
		}
	}

	public void ChangeCharacterOfPlayByPlayerInfo()
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		ChangeCharacterModel(model.modelName, PlayerInfo.Instance.currentThemeIndex);
	}

	public void ChangeCharacterModel(string name, int themeIndex)
	{
		SkinnedMeshRenderer model = null;
		characterCustomization.Customize(name, themeIndex, ref model);
		if (model != null)
		{
			currentRender = model;
		}
		if (Character.Instance.superShoes != null && Character.Instance.superShoes.IsActive)
		{
			meshSuperShoes.enabled = true;
		}
	}

	public Animation GetAnimation()
	{
		if (_animationComponent == null)
		{
			_animationComponent = GetComponentInChildren<Animation>();
		}
		return _animationComponent;
	}

	public GameObject GetHelmetRoot()
	{
		if (helmetRoot != null)
		{
			return helmetRoot.gameObject;
		}
		return null;
	}

	public void HideAllPowerups()
	{
		meshHelmet.enabled = false;
		int i = 0;
		for (int num = meshFlypack.Length; i < num; i++)
		{
			meshFlypack[i].enabled = false;
		}
		animFlypack.Stop();
		meshSuperShoes.enabled = false;
		meshCoinMagnet.enabled = false;
	}

	public void HideBlobShadow()
	{
		meshBlobShadow.enabled = false;
	}

	public void StartIdleAnimations()
	{
		if (currentRender != null)
		{
			AvatarAnimations component = currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdleAnimations();
			}
		}
	}

	public void StartTryAnimation()
	{
		if (!(currentRender != null))
		{
			return;
		}
		Animation animation = GetAnimation();
		TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
		if (currentTrialInfo != null && animation != null)
		{
			string text = currentTrialInfo.idel.name;
			if (animation.GetClip(text) == null)
			{
				animation.AddClip(currentTrialInfo.idel, text);
			}
			animation.Play(text);
			string text2 = currentTrialInfo.alert.name;
			if (animation.GetClip(text2) == null)
			{
				animation.AddClip(currentTrialInfo.alert, text2);
			}
			animation.CrossFadeQueued(text2, 0.1f);
		}
	}

	public void StopTryAnimations()
	{
		if (currentRender != null)
		{
			Animation animation = GetAnimation();
			if (animation != null)
			{
				animation.Stop();
			}
		}
	}

	public void StartHighScoreAnimations()
	{
		if (currentRender != null)
		{
			AvatarAnimations component = currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartHighscoreAnimations();
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (currentRender != null)
		{
			AvatarAnimations component = currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StopIdleAnimations();
			}
		}
	}
}
