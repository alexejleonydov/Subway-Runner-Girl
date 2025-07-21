using System;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
	[Serializable]
	public class CustomControl
	{
		public string name;

		public CustomizeControl customSets;
	}

	private Texture2D gradient;

	public CustomControl[] Customs;

	public void Customize(string name, int themeIndex, ref SkinnedMeshRenderer model)
	{
		for (int i = 0; i < Customs.Length; i++)
		{
			if (Customs[i].name.Equals(name))
			{
				Customs[i].customSets.gameObject.SetActive(true);
				Customs[i].customSets.ChangeSkin(themeIndex);
				model = Customs[i].customSets.body;
			}
			else
			{
				Customs[i].customSets.gameObject.SetActive(false);
			}
		}
	}
}
