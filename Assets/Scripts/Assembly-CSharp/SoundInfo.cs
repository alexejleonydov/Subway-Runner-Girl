using System.Collections;
using UnityEngine;

public class SoundInfo
{
	private SoundManager _manager;

	private string _name;

	public AudioSource audioSource;

	public bool available = true;

	public bool destroyAfterPlay;

	private GameObject gameObject;

	public string name
	{
		get
		{
			return _name;
		}
	}

	public SoundInfo(SoundManager manager)
	{
		_manager = manager;
		gameObject = new GameObject();
		_name = "empty";
		gameObject.name = _name;
		gameObject.transform.parent = manager.transform;
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	public void DestroySelf()
	{
		_manager.removeSound(this);
		Object.Destroy(gameObject);
	}

	public IEnumerator Play(AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		available = false;
		gameObject.transform.position = position;
		audioSource.rolloffMode = rolloff;
		audioSource.volume = Random.Range(minVolume, maxVolume);
		audioSource.pitch = Random.Range(minPitch, maxPitch);
		audioSource.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(audioSource.clip.length + 0.1f);
		audioSource.Stop();
		if (destroyAfterPlay)
		{
			DestroySelf();
		}
		available = true;
	}

	public IEnumerator Play(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		_name = audioClip.name;
		gameObject.name = _name;
		audioSource.clip = audioClip;
		return Play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position);
	}
}
