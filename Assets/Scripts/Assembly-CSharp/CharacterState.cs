using System.Collections;
using UnityEngine;

public abstract class CharacterState : MonoBehaviour
{
	public virtual bool PauseActiveModifiers
	{
		get
		{
			return false;
		}
	}

	public virtual IEnumerator Begin()
	{
		yield return null;
	}

	public virtual void HandleCriticalHit(bool isShake = true)
	{
	}

	public virtual void HandleDoubleTap()
	{
	}

	public virtual void HandleSwipe(SwipeDir swipeDir)
	{
	}
}
