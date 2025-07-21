using UnityEngine;

public class TopMenuLoop : MonoBehaviour, SoundLoop
{
	public AudioClip topmenuLoop;

	public void Play()
	{
		SoundManager.Instance.ingame.ChangeAudioClip(topmenuLoop);
	}

	public void Stop()
	{
		SoundManager.Instance.ingame.ResetAudioClip();
	}
}
