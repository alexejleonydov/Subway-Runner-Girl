using System.Collections;
using UnityEngine;

public class StageMenuSequence : MonoBehaviour
{
	protected float animationTime;

	public Animation anim;

	protected IEnumerator animationRoutine;

	protected bool isPlay;

	public virtual void StartPlayIdleRummagesAnimation()
	{
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	protected virtual IEnumerator Play()
	{
		yield return null;
	}

	public virtual void StopPlayIdleRummagesAnimation()
	{
		isPlay = false;
		animationTime = 0f;
	}

	public void OnUpdate()
	{
		if (animationRoutine != null)
		{
			animationRoutine.MoveNext();
		}
	}

	public void OnNewGameStart()
	{
		isPlay = false;
		animationTime = 0f;
	}
}
