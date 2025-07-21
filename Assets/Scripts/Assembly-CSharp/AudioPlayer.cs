using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	[Serializable]
	public class AudioClipInfo
	{
		public AudioClip Clip;

		public float minPitch = 0.8f;

		public float maxPitch = 1.1f;

		public float minVolume = 0.5f;

		public float maxVolume = 0.7f;

		public bool playOnAwake;

		public bool loop;

		public AudioRolloffMode Rollof = AudioRolloffMode.Linear;
	}

	public List<AudioClipInfo> musics;

	private List<AudioSource> musicSrcs;

	public List<AudioClipInfo> sounds;

	private List<AudioSource> soundSrcs;

	private List<AudioSoundRecycle> audioSoundRecycles;

	private AudioSource currentMusicAudioSource;

	private bool isFading;

	private float currentMusicMaxVolume;

	private float musicVolumnRateWithSound;

	private bool audioPlayPaused;

	private float currentMusicAudioSourceTime;

	private static AudioPlayer _instance;

	public static AudioPlayer Instance
	{
		get
		{
			return _instance;
		}
	}

	[ContextMenu("Resetdd")]
	private void Resetss()
	{
		Transform transform = base.transform.Find("Sound");
		if (transform == null)
		{
			transform = new GameObject("Sound").transform;
			transform.parent = base.transform;
			transform.transform.position = Vector3.zero;
			transform.transform.rotation = Quaternion.identity;
			soundSrcs = new List<AudioSource>(sounds.Count);
			int i = 0;
			for (int count = sounds.Count; i < count; i++)
			{
				soundSrcs.Add(Warmup(sounds[i], transform));
			}
		}
		Transform transform2 = base.transform.Find("Music");
		if (transform2 == null)
		{
			transform2 = new GameObject("Music").transform;
			transform2.parent = base.transform;
			transform2.transform.position = Vector3.zero;
			transform2.transform.rotation = Quaternion.identity;
			musicSrcs = new List<AudioSource>(musics.Count);
			int j = 0;
			for (int count2 = musics.Count; j < count2; j++)
			{
				musicSrcs.Add(Warmup(musics[j], transform2));
			}
		}
		Transform transform3 = base.transform.Find("Recycle");
		if (transform3 == null)
		{
			transform3 = new GameObject("Recycle").transform;
			transform3.parent = base.transform;
			transform3.transform.position = Vector3.zero;
			transform3.transform.rotation = Quaternion.identity;
		}
		audioSoundRecycles = new List<AudioSoundRecycle>();
		currentMusicAudioSource = null;
	}

	private void Awake()
	{
		Resetss();
		if (_instance == null)
		{
			_instance = this;
		}
		isFading = false;
		WarmUp("leyou_Hr_coin");
		WarmUp("leyou_Hr_run_dodge");
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			if (currentMusicAudioSource != null && currentMusicAudioSource.isPlaying)
			{
				audioPlayPaused = true;
				currentMusicAudioSourceTime = currentMusicAudioSource.time;
				currentMusicAudioSource.Stop();
			}
		}
		else if (audioPlayPaused)
		{
			audioPlayPaused = false;
			currentMusicAudioSource.time = currentMusicAudioSourceTime;
			currentMusicAudioSource.Play();
		}
	}

	private AudioSource Warmup(AudioClipInfo info, Transform parent)
	{
		if (info.Clip == null)
		{
			return null;
		}
		GameObject gameObject = new GameObject(info.Clip.name);
		gameObject.transform.parent = parent;
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = info.Clip;
		audioSource.volume = UnityEngine.Random.Range(info.minVolume, info.maxVolume);
		audioSource.pitch = UnityEngine.Random.Range(info.minPitch, info.maxPitch);
		audioSource.loop = info.loop;
		audioSource.playOnAwake = info.playOnAwake;
		audioSource.rolloffMode = info.Rollof;
		return audioSource;
	}

	public AudioSource GetAudioSource(string name)
	{
		return GetAudioSource(name, soundSrcs);
	}

	private AudioSource GetAudioSource(string soundName, List<AudioSource> sources)
	{
		AudioSource result = null;
		int i = 0;
		for (int count = sources.Count; i < count; i++)
		{
			if (sources[i].clip.name == soundName)
			{
				result = sources[i];
				break;
			}
		}
		return result;
	}

	private AudioSoundRecycle GetAudioSourceRecycle(string name)
	{
		AudioSoundRecycle result = null;
		int i = 0;
		for (int count = audioSoundRecycles.Count; i < count; i++)
		{
			if (audioSoundRecycles[i].origGo.name == name)
			{
				result = audioSoundRecycles[i];
				break;
			}
		}
		return result;
	}

	public AudioSoundRecycle AddAudioSourceRecycle(AudioSource audioSource)
	{
		GameObject gameObject = new GameObject(audioSource.name + "_recycle");
		gameObject.transform.parent = base.transform.Find("Recycle");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		AudioSoundRecycle audioSoundRecycle = gameObject.AddComponent<AudioSoundRecycle>();
		audioSoundRecycle.origGo = audioSource.gameObject;
		audioSoundRecycles.Add(audioSoundRecycle);
		return audioSoundRecycle;
	}

	public AudioSoundRecycle AddAudioSourceRecycle(string name)
	{
		GameObject gameObject = new GameObject(name + "_recycle");
		gameObject.transform.parent = base.transform.Find("Recycle");
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		AudioSoundRecycle audioSoundRecycle = gameObject.AddComponent<AudioSoundRecycle>();
		audioSoundRecycle.origGo = GetAudioSource(name, soundSrcs).gameObject;
		audioSoundRecycles.Add(audioSoundRecycle);
		return audioSoundRecycle;
	}

	private AudioSoundRecycle GetOrNewAudioSourceRecycle(string name)
	{
		AudioSource audioSource = GetAudioSource(name, soundSrcs);
		AudioSoundRecycle audioSoundRecycle = GetAudioSourceRecycle(audioSource.name);
		if (audioSoundRecycle == null)
		{
			audioSoundRecycle = AddAudioSourceRecycle(audioSource);
		}
		return audioSoundRecycle;
	}

	private AudioSoundRecycle GetOrNewAudioSourceRecycle(AudioSource audioSource)
	{
		AudioSoundRecycle audioSoundRecycle = GetAudioSourceRecycle(audioSource.name);
		if (audioSoundRecycle == null)
		{
			audioSoundRecycle = AddAudioSourceRecycle(audioSource);
		}
		return audioSoundRecycle;
	}

	public bool IsPlaying(string name)
	{
		return IsPlaying(GetAudioSource(name, soundSrcs));
	}

	private bool IsPlaying(AudioSource audioSource)
	{
		if (audioSource == null)
		{
			return false;
		}
		return audioSource.isPlaying;
	}

	public virtual void Loop(string name)
	{
		AudioSource audioSource = GetAudioSource(name, soundSrcs);
		if (audioSource != null)
		{
			audioSource.loop = true;
		}
	}

	public void PlaySound(string name, bool restart = false)
	{
		PlaySound(GetAudioSource(name, soundSrcs), restart);
	}

	private void PlaySound(AudioSource audioSource, bool restart = false)
	{
		if (restart || (audioSource != null && !audioSource.isPlaying))
		{
			audioSource.Play();
			if (!audioSource.loop)
			{
				StopSoundDelay(audioSource, audioSource.clip.length);
			}
		}
	}

	public void PlaySound(string name, float delay, bool restart = false)
	{
		PlaySoundDelay(GetAudioSource(name, soundSrcs), delay, restart);
	}

	private void PlaySoundDelay(AudioSource audioSource, float delay, bool restart = false)
	{
		StartCoroutine(PlaySoundDelay_C(audioSource, delay, restart));
	}

	private IEnumerator PlaySoundDelay_C(AudioSource audioSource, float delay, bool restart = false)
	{
		yield return new WaitForSeconds(delay);
		PlaySound(audioSource, restart);
	}

	public void PlaySound(string name, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		PlaySoundFade(GetAudioSource(name, soundSrcs), fadeDownTime, pauseTime, fadeupTime);
	}

	private void PlaySoundFade(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (isFading)
		{
			PlaySound(audioSource, true);
		}
		else
		{
			StartCoroutine(PlaySoundFade_C(audioSource, fadeDownTime, pauseTime, fadeupTime));
		}
	}

	private IEnumerator PlaySoundFade_C(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (!(audioSource == null))
		{
			audioSource.Play();
			float factor2 = 0f;
			float start = currentMusicAudioSource.volume;
			isFading = true;
			while (factor2 < 1f)
			{
				currentMusicAudioSource.volume = Mathf.SmoothStep(start, 0f, factor2);
				factor2 += Time.deltaTime / fadeDownTime;
				yield return null;
			}
			currentMusicAudioSource.volume = 0f;
			yield return new WaitForSeconds(pauseTime);
			factor2 = 0f;
			while (factor2 < 1f)
			{
				factor2 += Time.deltaTime / fadeupTime;
				currentMusicAudioSource.volume = Mathf.SmoothStep(0f, start, factor2);
				yield return null;
			}
			isFading = false;
		}
	}

	public void StopSound(string name)
	{
		StopSound(GetAudioSource(name, soundSrcs));
	}

	private void StopSound(AudioSource audioSource)
	{
		if (audioSource != null)
		{
			audioSource.Stop();
		}
	}

	private void StopSoundDelay(AudioSource audioSource, float delay)
	{
		StartCoroutine(StopSoundDelay_C(audioSource, delay));
	}

	private IEnumerator StopSoundDelay_C(AudioSource audioSource, float delay)
	{
		float time = 0f;
		while (time < delay)
		{
			time += Time.deltaTime;
			yield return null;
		}
		audioSource.Stop();
	}

	public void PauseSound(string name)
	{
		PauseSound(GetAudioSource(name, soundSrcs));
	}

	private void PauseSound(AudioSource audioSource)
	{
		if (audioSource != null)
		{
			audioSource.Pause();
		}
	}

	public void PlaySound(string name, Vector3 position)
	{
		PlaySoundASR(GetAudioSource(name, soundSrcs), position);
	}

	private void PlaySoundASR(AudioSource audioSource, Vector3 position)
	{
		StartCoroutine(PlaySoundASR_C(audioSource, audioSource.pitch, position));
	}

	public void PlaySound(string name, float pitch, Vector3 position)
	{
		PlaySoundASR(GetAudioSource(name, soundSrcs), pitch, position);
	}

	private void PlaySoundASR(AudioSource audioSource, float pitch, Vector3 position)
	{
		StartCoroutine(PlaySoundASR_C(audioSource, pitch, position));
	}

	public void PlaySound(string name, float delay, float pitch, Vector3 position)
	{
		PlaySoundASRDelay(GetAudioSource(name, soundSrcs), delay, pitch, position);
	}

	private void PlaySoundASRDelay(AudioSource audioSource, float delay, float pitch, Vector3 position)
	{
		StartCoroutine(PlaySoundASRDelay_C(audioSource, delay, pitch, position));
	}

	private IEnumerator PlaySoundASR_C(AudioSource audioSource, float pitch, Vector3 position)
	{
		if (!(audioSource == null))
		{
			AudioSoundRecycle asr = GetOrNewAudioSourceRecycle(audioSource);
			AudioRecycleItem item = asr.Retain();
			if (item != null)
			{
				item.audio.transform.position = position;
				item.audio.pitch = pitch;
				yield return new WaitForSeconds(item.audio.clip.length);
				item.Release();
			}
		}
	}

	private IEnumerator PlaySoundASRDelay_C(AudioSource audioSource, float delay, float pitch, Vector3 position)
	{
		yield return new WaitForSeconds(delay);
		PlaySoundASR(audioSource, pitch, position);
	}

	public void PlayMusic(string name, bool restart = false)
	{
		PlayMusic(GetAudioSource(name, musicSrcs));
	}

	private void PlayMusic(AudioSource music, bool restart = false)
	{
		if (currentMusicAudioSource == music || music == null)
		{
			return;
		}
		if (music != null && !music.isPlaying)
		{
			if (currentMusicAudioSource != null)
			{
				currentMusicAudioSource.Stop();
				float num = currentMusicAudioSource.volume / currentMusicMaxVolume;
				currentMusicAudioSource.volume = currentMusicMaxVolume;
				currentMusicMaxVolume = music.volume;
				music.volume *= num;
			}
			currentMusicAudioSource = music;
			currentMusicAudioSource.Play();
		}
		else if (restart)
		{
			currentMusicAudioSource.Play();
		}
	}

	public void PlayMusic(string name, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		PlayMusicFade(GetAudioSource(name, musicSrcs), fadeDownTime, pauseTime, fadeupTime);
	}

	public void PlayMusic(string name, float fadeupTime)
	{
		PlayMusicFade(GetAudioSource(name, musicSrcs), fadeupTime);
	}

	private void PlayMusicFade(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		if (currentMusicAudioSource != null)
		{
			if (isFading)
			{
				PlayMusic(audioSource);
			}
			else
			{
				StartCoroutine(MusicFader(audioSource, fadeDownTime, pauseTime, fadeupTime));
			}
		}
		else
		{
			PlayMusicFade(audioSource, fadeupTime);
		}
	}

	private void PlayMusicFade(AudioSource audioSource, float fadeupTime)
	{
		if (isFading)
		{
			PlayMusic(audioSource);
		}
		else
		{
			StartCoroutine(MusicFader(audioSource, fadeupTime));
		}
	}

	private IEnumerator MusicFader(AudioSource audioSource, float fadeDownTime, float pauseTime, float fadeupTime)
	{
		float factor2 = 0f;
		float start2 = currentMusicAudioSource.volume;
		isFading = true;
		while (factor2 < 1f)
		{
			currentMusicAudioSource.volume = Mathf.SmoothStep(start2, 0f, factor2);
			factor2 += Time.deltaTime / fadeDownTime;
			yield return null;
		}
		yield return new WaitForSeconds(pauseTime);
		currentMusicAudioSource.volume = 0f;
		currentMusicAudioSource.Stop();
		currentMusicAudioSource.volume = currentMusicMaxVolume;
		currentMusicMaxVolume = audioSource.volume;
		audioSource.volume = 0f;
		currentMusicAudioSource = audioSource;
		start2 = currentMusicMaxVolume;
		currentMusicAudioSource.Play();
		yield return new WaitForSeconds(pauseTime);
		factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / fadeupTime;
			currentMusicAudioSource.volume = Mathf.SmoothStep(0f, start2, factor2);
			yield return null;
		}
		currentMusicAudioSource.volume = start2;
		isFading = false;
	}

	private IEnumerator MusicFader(AudioSource audioSource, float fadeupTime)
	{
		float factor = 0f;
		if (currentMusicAudioSource != null)
		{
			currentMusicAudioSource.Stop();
			currentMusicAudioSource.volume = currentMusicMaxVolume;
		}
		currentMusicAudioSource = audioSource;
		currentMusicMaxVolume = audioSource.volume;
		audioSource.volume = 0f;
		currentMusicAudioSource.Play();
		isFading = true;
		while (factor < 1f)
		{
			factor += Time.deltaTime / fadeupTime;
			currentMusicAudioSource.volume = Mathf.Lerp(0f, currentMusicMaxVolume, factor);
			yield return null;
		}
		currentMusicAudioSource.volume = currentMusicMaxVolume;
		isFading = false;
	}

	public void WarmUp(string name)
	{
		GetOrNewAudioSourceRecycle(name).WarmUp();
	}
}
