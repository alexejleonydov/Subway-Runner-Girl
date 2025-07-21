using UnityEngine;

public class AddChar : MonoBehaviour
{
	public SkinnedMeshRenderer[] childSkins;

	public Transform parent;

	[ContextMenu("Add Chars")]
	public void ResetBonesForAllchild()
	{
		SkinnedMeshRenderer[] array = childSkins;
		foreach (SkinnedMeshRenderer childSkin in array)
		{
			ResetSkinnedMeshBones(childSkin);
		}
	}

	public void ResetSkinnedMeshBones(SkinnedMeshRenderer childSkin)
	{
		Transform[] array = new Transform[childSkin.bones.Length];
		for (int i = 0; i < childSkin.bones.Length; i++)
		{
			array[i] = FindNameInChild(childSkin.bones[i].name, parent);
		}
		childSkin.rootBone = parent.Find("Bip001 Pelvis");
		childSkin.bones = array;
	}

	private Transform FindNameInChild(string name, Transform parentRoot)
	{
		if (string.Equals(name, parentRoot.name))
		{
			return parentRoot;
		}
		foreach (Transform item in parentRoot)
		{
			Transform transform = FindNameInChild(name, item);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}
}
