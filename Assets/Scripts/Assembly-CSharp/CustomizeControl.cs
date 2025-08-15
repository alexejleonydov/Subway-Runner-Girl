using UnityEngine;

public class CustomizeControl : MonoBehaviour
{
	public SkinnedMeshRenderer body;

	public SkinnedMeshRenderer[] Skins;

	public AnimationClipData clipdata;

	public bool adjustAnimation;

	public AnimationClip animClip;

	public int frame;

	public int totalFrame;

	private Animation anim;

	private AvatarEyeAnimation eyeAnima;

	private void Awake()
	{
		anim = GetComponentInParent<Animation>();
		eyeAnima = base.transform.GetChild(0).GetComponent<AvatarEyeAnimation>();
	}

	private void OnEnable()
	{
		if (!adjustAnimation)
		{
			ResetTransform();
			return;
		}
		anim.AddClip(animClip, animClip.name);
		anim.clip = animClip;
		if (totalFrame <= 0)
		{
			anim[animClip.name].time = 0f;
		}
		else
		{
			anim[animClip.name].time = (float)frame / (float)totalFrame;
		}
		anim[animClip.name].speed = 0f;
		anim.Play();
	}

	// private void OnEnable()
	// {
	// 	if (!adjustAnimation)
	// 	{
	// 		ResetTransform();
	// 		return;
	// 	}

	// 	anim.AddClip(animClip, animClip.name);
	// 	anim.clip = animClip;
	// 	if (totalFrame <= 0)
	// 	{
	// 		anim[animClip.name].time = 0f;
	// 	}
	// 	else
	// 	{
	// 		anim[animClip.name].time = (float)frame / (float)totalFrame;
	// 	}
	// 	anim[animClip.name].speed = 0f;
	// 	anim.Play();


	// 	Animator armatureAnimator = GetComponentInChildren<Animator>(true);
	// 	Debug.Log("Animator is... " + armatureAnimator);

	// 	if (armatureAnimator != null && armatureAnimator.runtimeAnimatorController != null)
	// 	{

	// 		if (armatureAnimator.runtimeAnimatorController.name == "Armature 2")
	// 		{
	// 			GameObject characterScreen = GameObject.Find("CharacterScreen(Clone)");
	// 			if (characterScreen != null && characterScreen.activeInHierarchy)
	// 			{
	// 				armatureAnimator.Play("Idle", 0, 0f);
	// 			}
	// 			else
	// 			{
	// 				armatureAnimator.Play("run", 0, 0f);
	// 			}
	// 		}
	// 	}
	// }


	private void ResetTransform()
	{
		if (clipdata == null)
		{
			return;
		}
		Transform transform = anim.transform;
		for (int i = 0; i < clipdata.datas.Length; i++)
		{
			Transform transform2 = transform.Find(clipdata.datas[i].path);
			ClipData clipData = clipdata.datas[i];
			switch (clipData.propertyName)
			{
				case "m_LocalRotation.x":
					{
						Quaternion localRotation = transform2.localRotation;
						localRotation.x = clipData.startValue;
						transform2.localRotation = localRotation;
						break;
					}
				case "m_LocalRotation.y":
					{
						Quaternion localRotation = transform2.localRotation;
						localRotation.y = clipData.startValue;
						transform2.localRotation = localRotation;
						break;
					}
				case "m_LocalRotation.z":
					{
						Quaternion localRotation = transform2.localRotation;
						localRotation.z = clipData.startValue;
						transform2.localRotation = localRotation;
						break;
					}
				case "m_LocalRotation.w":
					{
						Quaternion localRotation = transform2.localRotation;
						localRotation.w = clipData.startValue;
						transform2.localRotation = localRotation;
						break;
					}
				case "m_LocalPosition.x":
					{
						Vector3 localPosition = transform2.localPosition;
						localPosition.x = clipData.startValue;
						transform2.localPosition = localPosition;
						break;
					}
				case "m_LocalPosition.y":
					{
						Vector3 localPosition = transform2.localPosition;
						localPosition.y = clipData.startValue;
						transform2.localPosition = localPosition;
						break;
					}
				case "m_LocalPosition.z":
					{
						Vector3 localPosition = transform2.localPosition;
						localPosition.z = clipData.startValue;
						transform2.localPosition = localPosition;
						break;
					}
				case "m_LocalScale.x":
					{
						Vector3 localScale = transform2.localScale;
						localScale.x = clipData.startValue;
						transform2.localScale = localScale;
						break;
					}
				case "m_LocalScale.y":
					{
						Vector3 localScale = transform2.localScale;
						localScale.y = clipData.startValue;
						transform2.localScale = localScale;
						break;
					}
				case "m_LocalScale.z":
					{
						Vector3 localScale = transform2.localScale;
						localScale.z = clipData.startValue;
						transform2.localScale = localScale;
						break;
					}
			}
		}
	}

	private void OnDisable()
	{
		if (adjustAnimation)
		{
			anim.RemoveClip(animClip);
		}
	}

	public void ChangeSkin(int CustomIndx)
	{
		if (Skins == null)
		{
			return;
		}
		for (int i = 0; i < Skins.Length; i++)
		{
			Skins[i].gameObject.SetActive(false);
		}
		if (CustomIndx < Skins.Length)
		{
			Skins[CustomIndx].gameObject.SetActive(true);
			body.sharedMaterial = Skins[CustomIndx].sharedMaterial;
			if ((bool)eyeAnima)
			{
				eyeAnima.ChangeMat(Skins[CustomIndx].sharedMaterial);
			}
		}
	}
}
