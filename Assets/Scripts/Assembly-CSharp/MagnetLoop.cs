using UnityEngine;

public class MagnetLoop : MonoBehaviour, SoundLoop
{
	public AudioClip magnetLoop;

	public float magnetVolume = 0.4f;

	public float magnetMinPitch = 0.8f;

	public float magnetMaxPitch = 1.1f;

	private AudioSource magnetSource;

	private void Awake()
	{
		magnetSource = base.gameObject.AddComponent<AudioSource>();
		magnetSource.clip = magnetLoop;
		magnetSource.volume = magnetVolume;
		magnetSource.loop = true;
		magnetSource.playOnAwake = false;
		magnetSource.spatialBlend = 0.5f;
	}

	public void Play()
	{
		magnetSource.pitch = Random.Range(magnetMinPitch, magnetMaxPitch);
		magnetSource.Play();
	}

	public void Stop()
	{
		if (magnetSource.isPlaying)
		{
			magnetSource.Stop();
		}
	}
}
