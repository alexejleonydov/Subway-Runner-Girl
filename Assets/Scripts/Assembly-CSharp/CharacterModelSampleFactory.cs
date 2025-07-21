using System.Collections.Generic;
using UnityEngine;

public class CharacterModelSampleFactory : MonoBehaviour
{
	[SerializeField]
	private GameObject characterModelSamplePrefab;

	private Dictionary<string, GameObject> CMPreviews = new Dictionary<string, GameObject>();

	private static CharacterModelSampleFactory instance;

	public static CharacterModelSampleFactory Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(CharacterModelSampleFactory)) as CharacterModelSampleFactory;
			}
			return instance;
		}
	}

	private GameObject BuildCharacter(string name, int version)
	{
		GameObject value = null;
		if (!CMPreviews.TryGetValue(name, out value))
		{
			value = Object.Instantiate(characterModelSamplePrefab);
			CMPreviews.Add(name, value);
		}
		SelectCustom(value, name, version);
		return value;
	}

	public GameObject GetCharacterModelSample(string name, int version)
	{
		return BuildCharacter(name, version);
	}

	public CharacterModelSample BuildCharacterModelSample(string name, int version)
	{
		return BuildCharacterSample(name, version);
	}

	private CharacterModelSample BuildCharacterSample(string name, int version)
	{
		GameObject go = Object.Instantiate(characterModelSamplePrefab);
		return SelectCustom(go, name, version);
	}

	private CharacterModelSample SelectCustom(GameObject go, string name, int version)
	{
		CharacterModelSample component = go.GetComponent<CharacterModelSample>();
		for (int i = 0; i < component.Customs.Length; i++)
		{
			if (component.Customs[i].name.Equals(name))
			{
				component.Customs[i].customSets.gameObject.SetActive(true);
				component.Customs[i].customSets.ChangeSkin(version);
			}
			else
			{
				component.Customs[i].customSets.gameObject.SetActive(false);
			}
		}
		return component;
	}
}
