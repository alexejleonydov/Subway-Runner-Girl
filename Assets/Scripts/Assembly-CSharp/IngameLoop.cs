using System.Collections;
using UnityEngine;

public class IngameLoop : MonoBehaviour, SoundLoop
{
	private AudioSource backgroundMusic;

	public AudioClip bgclip;

	public float menuMusicVolume = 0.3f;

	public float ingameMusicVolume = 0.3f;

	public float fadeDownTime = 0.5f;

	private bool hasPlayedIntro;

	private bool infading;

	private AudioClip nextClip;

	private float musicVolume;

	private void Awake()
	{
		backgroundMusic = base.gameObject.AddComponent<AudioSource>();
		base.gameObject.AddComponent<AudioLowPassFilter>();
		backgroundMusic.clip = bgclip;
		musicVolume = ingameMusicVolume;
		UpdateBackgroundMusic();
		backgroundMusic.loop = true;
		backgroundMusic.playOnAwake = true;
		backgroundMusic.spatialBlend = 0f;
		SetGameSoundVolume();
		backgroundMusic.bypassEffects = true;
		backgroundMusic.Play();
		infading = false;
	}

	public void Play()
	{
		if (!hasPlayedIntro)
		{
			hasPlayedIntro = true;
		}
		else
		{
			backgroundMusic.timeSamples = 0;
		}
		backgroundMusic.bypassEffects = true;
		musicVolume = ingameMusicVolume;
		UpdateBackgroundMusic();
		ResetAudioClip();
	}

	public void Stop()
	{
		if (hasPlayedIntro)
		{
			backgroundMusic.bypassEffects = false;
			musicVolume = menuMusicVolume;
			UpdateBackgroundMusic();
		}
	}

	public IEnumerator MusicFader(float fadeupTime, float pauseTime)
	{
		float factor2 = 0f;
		float start = musicVolume;
		infading = true;
		while (factor2 < 1f)
		{
			musicVolume = Mathf.SmoothStep(start, 0f, factor2);
			UpdateBackgroundMusic();
			factor2 += Time.deltaTime / fadeDownTime;
			yield return null;
		}
		musicVolume = 0f;
		UpdateBackgroundMusic();
		infading = false;
		yield return new WaitForSeconds(pauseTime);
		setNextClip();
		factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / fadeupTime;
			musicVolume = Mathf.SmoothStep(0f, ingameMusicVolume, factor2);
			UpdateBackgroundMusic();
			yield return null;
		}
		musicVolume = ingameMusicVolume;
		UpdateBackgroundMusic();
	}

	public IEnumerator MusicFader(float fadeupTime)
	{
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			musicVolume = Mathf.Lerp(musicVolume, ingameMusicVolume, factor);
			yield return null;
		}
		musicVolume = ingameMusicVolume;
		UpdateBackgroundMusic();
	}

	private void SetGameSoundVolume()
	{
		if (Settings.optionSound)
		{
			AudioListener.volume = 1f;
		}
		else
		{
			AudioListener.volume = 0f;
		}
	}

	public void ChangeAudioClip(AudioClip clip)
	{
		if (backgroundMusic.clip != clip)
		{
			if (infading)
			{
				nextClip = clip;
				return;
			}
			nextClip = null;
			backgroundMusic.clip = clip;
			backgroundMusic.Play();
		}
	}

	public void ResetAudioClip()
	{
		ChangeAudioClip(bgclip);
	}

	public void setNextClip()
	{
		if (nextClip != null && backgroundMusic.clip != nextClip)
		{
			backgroundMusic.clip = nextClip;
			backgroundMusic.Play();
			nextClip = null;
		}
	}

	private void UpdateBackgroundMusic()
	{
		backgroundMusic.volume = musicVolume;
	}
}
