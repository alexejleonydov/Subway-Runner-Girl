using UnityEngine;

public class FlybackLoop : MonoBehaviour, SoundLoop
{
	public AudioClip flypackLoop;

	public float flypackVolume = 0.4f;

	public float pauseTime = 2f;

	public float fadeUpTime = 3f;

	private AudioSource flypackSource;

	public void Awake()
	{
		flypackSource = base.gameObject.AddComponent<AudioSource>();
		flypackSource.clip = flypackLoop;
		flypackSource.volume = flypackVolume;
		flypackSource.loop = true;
		flypackSource.playOnAwake = false;
		flypackSource.spatialBlend = 0f;
	}

	public void Play()
	{
		flypackSource.Play();
		SoundManager.Instance.ingame.ingameMusicVolume *= 0.5f;
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(fadeUpTime, pauseTime));
	}

	public void Stop()
	{
		if (flypackSource.isPlaying)
		{
			flypackSource.Stop();
		}
		SoundManager.Instance.ingame.ingameMusicVolume *= 2f;
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(fadeUpTime));
	}
}
