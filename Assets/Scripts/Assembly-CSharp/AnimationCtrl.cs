using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCtrl : MonoBehaviour
{
	public enum AniState
	{
		none = 0,
		enter = 1,
		idle = 2,
		selected = 3,
		quit = 4
	}

	public Animation _animation;

	public string idleName;

	public string enterName;

	public string selectName;

	public string quitName;

	private bool isInAnimation;

	private Dictionary<AniState, string> state2clip = new Dictionary<AniState, string>();

	public bool IsInAnimation
	{
		get
		{
			return isInAnimation;
		}
	}

	private void Start()
	{
		_animation = GetComponent<Animation>();
		state2clip.Add(AniState.enter, enterName);
		state2clip.Add(AniState.quit, quitName);
		state2clip.Add(AniState.selected, selectName);
		state2clip.Add(AniState.idle, idleName);
	}

	private void SetState(AniState newstate)
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || IsInAnimation)
		{
			return;
		}
		string value = string.Empty;
		if (state2clip.TryGetValue(newstate, out value) && _animation.GetClip(value) != null)
		{
			_animation.CrossFade(value);
			if (_animation[value].wrapMode != WrapMode.Loop)
			{
				StartCoroutine(waitingForAni(_animation[value].length));
			}
		}
	}

	public void PlayEnterAni()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && !IsInAnimation)
		{
			SetState(AniState.enter);
		}
	}

	public void PlayQuitAni()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && !IsInAnimation)
		{
			SetState(AniState.quit);
		}
	}

	public void PlaySelectAni()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && !IsInAnimation)
		{
			SetState(AniState.selected);
		}
	}

	public void PlayIdleAni()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && !IsInAnimation)
		{
			SetState(AniState.idle);
		}
	}

	private IEnumerator waitingForAni(float time)
	{
		isInAnimation = true;
		yield return new WaitForSeconds(time);
		isInAnimation = false;
	}
}
