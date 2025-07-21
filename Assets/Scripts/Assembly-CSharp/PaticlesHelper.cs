using UnityEngine;

public class PaticlesHelper : MonoBehaviour
{
	private ParticleSystem[] particles;

	public bool IsPlaying { get; private set; }

	private void Awake()
	{
		particles = GetComponentsInChildren<ParticleSystem>();
	}

	public void Play()
	{
		IsPlaying = true;
		int i = 0;
		for (int num = particles.Length; i < num; i++)
		{
			particles[i].Play();
		}
	}

	public void Stop()
	{
		int i = 0;
		for (int num = particles.Length; i < num; i++)
		{
			particles[i].Stop();
		}
		IsPlaying = false;
	}
}
