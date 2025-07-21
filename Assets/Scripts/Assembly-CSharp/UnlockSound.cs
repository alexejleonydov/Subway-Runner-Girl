using UnityEngine;

public class UnlockSound : MonoBehaviour, SoundLoop
{
	public AudioClip unlockSound;

	public float unlockSoundVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource unlockSource;

	private void Awake()
	{
		unlockSource = base.gameObject.AddComponent<AudioSource>();
		unlockSource.clip = unlockSound;
		unlockSource.volume = unlockSoundVolume;
		unlockSource.playOnAwake = false;
		unlockSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		unlockSource.Play();
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(fadeUpTime, pauseTime));
	}

	public void Stop()
	{
		if (unlockSource.isPlaying)
		{
			unlockSource.Stop();
		}
	}
}
