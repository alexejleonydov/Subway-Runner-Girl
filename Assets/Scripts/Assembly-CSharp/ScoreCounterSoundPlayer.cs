using System.Collections;
using UnityEngine;

public class ScoreCounterSoundPlayer : MonoBehaviour
{
	public AudioClip scoreSound;

	public AudioClip vipSound;

	public float volume = 0.3f;

	public float stepDelay = 0.0625f;

	private float count;

	private bool playScore;

	private AudioSource scoreSource;

	private void Awake()
	{
		scoreSource = base.gameObject.AddComponent<AudioSource>();
		scoreSource.volume = volume;
		scoreSource.clip = scoreSound;
		scoreSource.playOnAwake = false;
		scoreSource.spatialBlend = 0f;
	}

	public void PlayCoinSound(float countFactor)
	{
		scoreSource.clip = scoreSound;
		count = countFactor;
		if (!playScore)
		{
			playScore = true;
			StartCoroutine(ScoreSoundTimer());
		}
	}

	public void PlayVipSound()
	{
		scoreSource.clip = vipSound;
		scoreSource.Play();
	}

	public void StopVipSound()
	{
		scoreSource.Stop();
	}

	private IEnumerator ScoreSoundTimer()
	{
		while (playScore)
		{
			scoreSource.pitch = Mathf.Pow(2f, count);
			scoreSource.Play();
			yield return new WaitForSeconds(stepDelay);
		}
		scoreSource.pitch = 2f;
		scoreSource.Play();
	}

	public void StopScoreSound()
	{
		playScore = false;
	}
}
