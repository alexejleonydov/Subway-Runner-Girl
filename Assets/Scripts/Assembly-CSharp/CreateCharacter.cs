using System.Collections.Generic;
using UnityEngine;

public class CreateCharacter : Point
{
	public string[] characterName;

	public int[] characterThemeId;

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		InitializeModel(manager);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.GoToNextPoint();
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
		InitializeModel(manager);
	}

	private void InitializeModel(PointsManager manager)
	{
		Characters.CharacterType currentCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		string modelName = Characters.characterData[currentCharacter].modelName;
		List<string> list = new List<string>();
		int i = 0;
		for (int num = characterName.Length; i < num; i++)
		{
			string text = characterName[i];
			if (!modelName.Equals(text) && !PointsManager.hasShowedModels.Contains(text))
			{
				list.Add(text);
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("No one valid characterName");
			return;
		}
		int num2 = Random.Range(0, list.Count);
		manager.InitializeCharacterModel(base.transform, characterName[num2], characterThemeId[num2]);
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}
}
