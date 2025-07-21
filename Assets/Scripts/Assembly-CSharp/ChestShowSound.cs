using UnityEngine;

public class ChestShowSound : MonoBehaviour, SoundLoop
{
	public AudioClip chestRewardShowSound;

	public float chestRewardVolume = 1f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private AudioSource chestRewardSource;

	private void Awake()
	{
		chestRewardSource = base.gameObject.AddComponent<AudioSource>();
		chestRewardSource.clip = chestRewardShowSound;
		chestRewardSource.volume = chestRewardVolume;
		chestRewardSource.playOnAwake = false;
		chestRewardSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		chestRewardSource.Play();
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(fadeUpTime, pauseTime));
	}

	public void Stop()
	{
		if (chestRewardSource.isPlaying)
		{
			chestRewardSource.Stop();
		}
	}
}
