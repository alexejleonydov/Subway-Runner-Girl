using UnityEngine;

public class ChestOpenSound : MonoBehaviour, SoundLoop
{
	public AudioClip chestOpenSound;

	public float chestVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource chestSource;

	private void Awake()
	{
		chestSource = base.gameObject.AddComponent<AudioSource>();
		chestSource.clip = chestOpenSound;
		chestSource.volume = chestVolume;
		chestSource.playOnAwake = false;
		chestSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		chestSource.Play();
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(fadeUpTime, pauseTime));
	}

	public void Stop()
	{
		if (chestSource.isPlaying)
		{
			chestSource.Stop();
		}
	}
}
