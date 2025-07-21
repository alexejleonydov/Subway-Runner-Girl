using UnityEngine;

public class PointAnimationCtrl : MonoBehaviour
{
	public Animation anim;

	public Animation follow;

	public AnimationState this[string clip]
	{
		get
		{
			return anim[clip];
		}
	}

	public void AddClip(AnimationClip clip)
	{
		if (anim[clip.name] == null)
		{
			anim.AddClip(clip, clip.name);
		}
		if (follow != null && follow[clip.name] == null)
		{
			follow.AddClip(clip, clip.name);
		}
	}

	public void RemoveClip(AnimationClip clip)
	{
		if (anim[clip.name] != null)
		{
			anim.RemoveClip(clip);
		}
		if (follow != null && follow[clip.name] == null)
		{
			follow.RemoveClip(clip);
		}
	}

	public void CrossFade(string clip, float time)
	{
		anim.CrossFade(clip, time);
		if (follow != null)
		{
			follow.CrossFade(clip, time);
		}
	}

	public void CrossFadeQueued(string clip, float time)
	{
		anim.CrossFadeQueued(clip, time);
		if (follow != null)
		{
			follow.CrossFadeQueued(clip, time);
		}
	}

	public void Play(string clip)
	{
		anim.Play(clip);
		if (follow != null)
		{
			follow.Play(clip);
		}
	}

	public bool IsEnable(string clip)
	{
		return anim[clip].enabled;
	}

	public void SetSpeed(string clip, float speed)
	{
		anim[clip].speed = speed;
		if (follow != null)
		{
			follow[clip].speed = speed;
		}
	}

	public void Stop()
	{
		anim.Stop();
	}

	public void Sample(AnimationClip clip, int frame)
	{
		string text = clip.name;
		AnimationState animationState = anim[text];
		animationState.enabled = true;
		animationState.speed = 0f;
		animationState.normalizedTime = (float)frame / (clip.frameRate * clip.length);
		anim.Sample();
	}
}
