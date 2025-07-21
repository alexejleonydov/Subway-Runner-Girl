using System;
using System.Collections.Generic;
using UnityEngine;

public class RaftRendering : MonoBehaviour
{
	public AnimationClip defaultAnimation;

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

	public AnimationPair[] getOnRaftAnimations;

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

	private string[] GetNamesAddAnimationClips(AnimationPair[] pairs, Animation avaterAnimation, Animation raftAnimation, List<AnimationClip> addedClipsList)
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
			if (raftAnimation != null && animationPair.helmetAnimationClip != null)
			{
				AddClipsToAnimationComp(raftAnimation, animationPair.helmetAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character cannot be null.");
			}
			array[i] = characterAnimationClip.name;
		}
		return array;
	}

	public void Initialize(Animation avatarAnimation, Animation raftAnimation, List<AnimationClip> addedClipsList)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (jumpAnimations.Length != hangtimeAnimations.Length)
		{
			Debug.LogError("Lists of raft jump and hangtime animations does not have the same length", this);
		}
		instance.animations.RUN = GetNamesAddAnimationClips(runAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.LAND = GetNamesAddAnimationClips(landAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.JUMP = GetNamesAddAnimationClips(jumpAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.HANGTIME = GetNamesAddAnimationClips(hangtimeAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.ROLL = GetNamesAddAnimationClips(rollAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = GetNamesAddAnimationClips(dodgeLeftAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = GetNamesAddAnimationClips(dodgeRightAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.GRIND = GetNamesAddAnimationClips(grindAnimations, avatarAnimation, raftAnimation, addedClipsList);
		GetNamesAddAnimationClips(grindLandAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.GET_ON = GetNamesAddAnimationClips(getOnRaftAnimations, avatarAnimation, raftAnimation, addedClipsList);
		if (raftAnimation != null)
		{
			raftAnimation.AddClip(defaultAnimation, defaultAnimation.name);
		}
		if (defaultAnimation != null)
		{
			instance.animations.DEFAULT_ANIMATION = defaultAnimation.name;
		}
	}
}
