using UnityEngine;

public class ReviveLoop : MonoBehaviour, SoundLoop
{
	public AudioClip reviveSound;

	public float reviveVolume = 1f;

	public float revivePauseTime = 1f;

	public float reviveFadeUpTime = 2f;

	private AudioSource reviveSource;

	private void Awake()
	{
		reviveSource = base.gameObject.AddComponent<AudioSource>();
		reviveSource.clip = reviveSound;
		reviveSource.volume = reviveVolume;
		reviveSource.playOnAwake = false;
		reviveSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		reviveSource.Play();
		StartCoroutine(SoundManager.Instance.ingame.MusicFader(reviveFadeUpTime, revivePauseTime));
	}

	public void Stop()
	{
		if (reviveSource.isPlaying)
		{
			reviveSource.Stop();
		}
	}
}
