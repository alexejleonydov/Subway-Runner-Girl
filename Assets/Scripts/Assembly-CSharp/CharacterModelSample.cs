using UnityEngine;

public class CharacterModelSample : MonoBehaviour, ICharacterModel
{
	[SerializeField]
	private Transform _boneHead;

	[SerializeField]
	private Transform _boneRightHand;

	[SerializeField]
	private Transform _boneHelmet;

	public CharacterCustomization.CustomControl[] Customs;

	public Transform BoneHead
	{
		get
		{
			return _boneHead;
		}
	}

	public Transform BoneHelmet
	{
		get
		{
			return _boneHelmet;
		}
	}

	public Transform BoneRightHand
	{
		get
		{
			return _boneRightHand;
		}
	}
}
