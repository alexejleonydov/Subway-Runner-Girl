using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmetRendering : MonoBehaviour
{
	[Serializable]
	public class AnimationList
	{
		public AnimationClip defaultHelmetAnimation;

		public AnimationPair[] runAnimations;

		public AnimationPair[] superRunAnimations;

		public AnimationPair[] landAnimations;

		public AnimationPair[] jumpAnimations;

		public AnimationPair[] hangtimeAnimations;

		public AnimationPair[] rollAnimations;

		public AnimationPair[] dodgeLeftAnimations;

		public AnimationPair[] dodgeRightAnimations;

		public AnimationPair[] grindAnimations;

		public AnimationPair[] grindLandAnimations;

		public AnimationPair[] getOnHelmAnimations;
	}

	[OptionalField]
	public AnimationClip defaultHelmetAnimation;

	public AnimationPair[] runAnimations;

	public AnimationPair[] superRunAnimations;

	public AnimationPair[] landAnimations;

	public AnimationPair[] jumpAnimations;

	public AnimationPair[] hangtimeAnimations;

	public AnimationPair[] rollAnimations;

	public AnimationPair[] dodgeLeftAnimations;

	public AnimationPair[] dodgeRightAnimations;

	public AnimationPair[] grindAnimations;

	public AnimationPair[] grindLandAnimations;

	public AnimationPair[] getOnHelmAnimations;


	void Awake()
	{
		Debug.Log("Prefab instantiated by: " + StackTraceUtility.ExtractStackTrace());
	}

	private void AddClipsToAnimationComp(Animation animation, AnimationClip clip, List<AnimationClip> addedClipsList)
	{
		if (!(clip != null))
		{
			return;
		}
		if (addedClipsList != null)
		{
			if (!addedClipsList.Contains(clip))
			{
				addedClipsList.Add(clip);
				animation.AddClip(clip, clip.name);
			}
		}
		else
		{
			animation.AddClip(clip, clip.name);
		}
	}

	private string[] GetNamesAddAnimationClips(AnimationPair[] pairs, Animation avaterAnimation, Animation helmetAnimation, List<AnimationClip> addedClipsList)
	{
		string[] array = null;
		array = new string[pairs.Length];
		for (int i = 0; i < array.Length; i++)
		{
			AnimationPair animationPair = pairs[i];
			AnimationClip characterAnimationClip = animationPair.characterAnimationClip;
			if (avaterAnimation != null)
			{
				AddClipsToAnimationComp(avaterAnimation, characterAnimationClip, addedClipsList);
			}
			if (helmetAnimation != null && animationPair.helmetAnimationClip != null)
			{
				AddClipsToAnimationComp(helmetAnimation, animationPair.helmetAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character cannot be null.");
			}
			array[i] = characterAnimationClip.name;
		}
		return array;
	}

	public void Initialize(Animation avatarAnimation, Animation helmetAnimation, AnimationList defaultAbilityAnimationLists, List<AnimationClip> addedClipsList)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (jumpAnimations.Length != hangtimeAnimations.Length)
		{
			Debug.LogError("Lists of helmet jumps and helmet hangtime animations does not have the same length", this);
		}
		instance.animations.RUN = GetNamesAddAnimationClips((runAnimations.Length == 0) ? defaultAbilityAnimationLists.runAnimations : runAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.SUPERRUN = GetNamesAddAnimationClips((superRunAnimations.Length == 0) ? defaultAbilityAnimationLists.superRunAnimations : superRunAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.LAND = GetNamesAddAnimationClips((landAnimations.Length == 0) ? defaultAbilityAnimationLists.landAnimations : landAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.JUMP = GetNamesAddAnimationClips((jumpAnimations.Length == 0) ? defaultAbilityAnimationLists.jumpAnimations : jumpAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.HANGTIME = GetNamesAddAnimationClips((hangtimeAnimations.Length == 0) ? defaultAbilityAnimationLists.hangtimeAnimations : hangtimeAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.ROLL = GetNamesAddAnimationClips((rollAnimations.Length == 0) ? defaultAbilityAnimationLists.rollAnimations : rollAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = GetNamesAddAnimationClips((dodgeLeftAnimations.Length == 0) ? defaultAbilityAnimationLists.dodgeLeftAnimations : dodgeLeftAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = GetNamesAddAnimationClips((dodgeRightAnimations.Length == 0) ? defaultAbilityAnimationLists.dodgeRightAnimations : dodgeRightAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.GRIND = GetNamesAddAnimationClips((grindAnimations.Length == 0) ? defaultAbilityAnimationLists.grindAnimations : grindAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		GetNamesAddAnimationClips((grindLandAnimations.Length == 0) ? defaultAbilityAnimationLists.grindLandAnimations : grindLandAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.GET_ON = GetNamesAddAnimationClips((getOnHelmAnimations.Length == 0) ? defaultAbilityAnimationLists.getOnHelmAnimations : getOnHelmAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		if (helmetAnimation != null)
		{
			helmetAnimation.AddClip(defaultHelmetAnimation, defaultHelmetAnimation.name);
		}
		if (defaultHelmetAnimation != null)
		{
			instance.animations.DEFAULT_ANIMATION = defaultHelmetAnimation.name;
		}
	}
}
