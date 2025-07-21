using UnityEngine;

public class StartSound : MonoBehaviour, SoundLoop
{
	public AudioClip startSound;

	public float startVolume = 1f;

	public float startPuseTime = 1f;

	public float startFadeUpTime = 2f;

	private AudioSource startSource;

	private void Awake()
	{
		startSource = base.gameObject.AddComponent<AudioSource>();
		startSource.clip = startSound;
		startSource.volume = startVolume;
		startSource.playOnAwake = false;
		startSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		startSource.Play();
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(startFadeUpTime, startPuseTime));
	}

	public void Stop()
	{
		if (startSource.isPlaying)
		{
			startSource.Stop();
		}
	}
}
