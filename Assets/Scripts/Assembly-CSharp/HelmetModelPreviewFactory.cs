using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmetModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class HelmetSelection
	{
		public Helmets.HelmType helmType;

		public GameObject helmetPrefab;
	}

	[Serializable]
	public class HelmetModelMenuData
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPos;

		public AnimationClip hangtimeClip;

		public AnimationClip runClip;

		public GameObject helmetPrefab;

		public GameObject helmetPrefabInScroll;
	}

	[SerializeField]
	private HelmetSelection[] HelmetSelector;

	[SerializeField]
	private HelmetModelMenuData[] helmets;

	public float pullSpeed = 200f;

	public float cooldownDistance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private CharacterModel characterModel;

	private HelmetManager helmetManager;

	private static HelmetModelPreviewFactory instance;

	private Vector3 intialScale = new Vector3(-1f, -1f, -1f);

	private Dictionary<string, HelmetModelMenuData> name2setup = new Dictionary<string, HelmetModelMenuData>();

	private Dictionary<Helmets.HelmType, GameObject> helmetThatMatchHelmType = new Dictionary<Helmets.HelmType, GameObject>();

	public static HelmetModelPreviewFactory Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(HelmetModelPreviewFactory)) as HelmetModelPreviewFactory;
			}
			return instance;
		}
	}

	private void AddCustomModel(GameObject prefab, Transform root)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.parent = root;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = Layers.Instance._3DGUI;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].gameObject.layer = Layers.Instance._3DGUI;
		}
	}

	public void Awake()
	{
		int i = 0;
		for (int num = helmets.Length; i < num; i++)
		{
			name2setup.Add(helmets[i].name, helmets[i]);
		}
		int j = 0;
		for (int num2 = HelmetSelector.Length; j < num2; j++)
		{
			if (helmetThatMatchHelmType.ContainsKey(HelmetSelector[j].helmType))
			{
				throw new Exception("There are more helmets assigned to the helmet selection");
			}
			helmetThatMatchHelmType.Add(HelmetSelector[j].helmType, HelmetSelector[j].helmetPrefab);
		}
		helmetManager = HelmetManager.Instance;
	}

	public HelmetSelection GetHelmetSelection(Helmets.HelmType helm)
	{
		int i = 0;
		for (int num = HelmetSelector.Length; i < num; i++)
		{
			if (HelmetSelector[i].helmType == helm)
			{
				return HelmetSelector[i];
			}
		}
		return null;
	}

	public HelmetSelection GetHelmetSelection(int index)
	{
		if (index < 0 || index >= HelmetSelector.Length)
		{
			return null;
		}
		return HelmetSelector[index];
	}

	public GameObject GetHelmet()
	{
		GameObject value;
		if (helmetThatMatchHelmType.TryGetValue(GetCurrentEquippedHelmet(), out value))
		{
			return value;
		}
		return null;
	}

	public Helmets.HelmType GetCurrentEquippedHelmet()
	{
		return helmetManager.CurrentlyEquippedHelmet();
	}

	public void ChangeHelmet(Helmets.HelmType helmType, GameObject helmetGO, Animation characterAnimation, bool updateAnimation)
	{
		string helmModelName = Helmets.helmData[helmType].helmModelName;
		characterModel = characterAnimation.transform.parent.GetComponent<CharacterModel>();
		if (intialScale == new Vector3(-1f, -1f, -1f))
		{
			intialScale = characterModel.transform.localScale;
		}
		HelmetModelMenuData value;
		if (name2setup.TryGetValue(helmModelName, out value))
		{
			string helmName = ((!(helmetGO != null)) ? "helmet" : helmetGO.name);
			helmetGO = characterModel.SetNewHelmet(helmetGO, value.helmetPrefab, helmName);
			if (!updateAnimation)
			{
				return;
			}
			if (characterAnimation[value.hangtimeClip.name] == null)
			{
				characterAnimation.AddClip(value.hangtimeClip, value.hangtimeClip.name);
				characterAnimation[value.hangtimeClip.name].wrapMode = WrapMode.Once;
				if (characterAnimation[value.runClip.name] == null)
				{
					characterAnimation.AddClip(value.runClip, value.runClip.name);
				}
			}
			characterAnimation.Play(value.hangtimeClip.name);
			characterAnimation.CrossFadeQueued(value.runClip.name, 0.2f);
		}
		else
		{
			Debug.LogError("could not find sample character model for '" + helmModelName + "'.");
		}
	}

	public Quaternion GetHelmetDefaultRotation(string name)
	{
		HelmetModelMenuData value;
		if (name2setup.TryGetValue(name, out value))
		{
			return Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
		}
		return Quaternion.Euler(194f, 110.5f, 180f);
	}

	public GameObject GetHelmetModelForScroll(string name, Helmets.HelmType helmType)
	{
		HelmetModelMenuData value;
		if (name2setup.TryGetValue(name, out value))
		{
			GameObject gameObject = new GameObject("Helmet: " + name);
			GameObject gameObject2 = ((!(value.helmetPrefabInScroll != null)) ? UnityEngine.Object.Instantiate(value.helmetPrefab) : UnityEngine.Object.Instantiate(value.helmetPrefabInScroll));
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localRotation = Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.localPosition = localPosition + new Vector3(0f, value.menuYPos, 0f);
			return gameObject;
		}
		return null;
	}
}
