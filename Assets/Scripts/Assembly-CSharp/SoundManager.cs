using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;

	private List<SoundInfo> _soundList;

	public int initialCapacity = 5;

	public int maxCapacity = 50;

	private FlybackLoop flyback;

	private MagnetLoop magnet;

	private ReviveLoop revive;

	private ChestOpenSound chestOpen;

	private UnlockSound unlock;

	private StartSound start;

	private ChestShowSound chestShow;

	private TopMenuLoop topmenu;

	public IngameLoop ingame;

	private SoundLoop currentSound;

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		_soundList = new List<SoundInfo>(initialCapacity);
		for (int i = 0; i < initialCapacity; i++)
		{
			_soundList.Add(new SoundInfo(this));
		}
		initLoop();
	}

	private void initLoop()
	{
		flyback = GetComponentInChildren<FlybackLoop>();
		magnet = GetComponentInChildren<MagnetLoop>();
		revive = GetComponentInChildren<ReviveLoop>();
		unlock = GetComponentInChildren<UnlockSound>();
		chestOpen = GetComponentInChildren<ChestOpenSound>();
		chestShow = GetComponentInChildren<ChestShowSound>();
		start = GetComponentInChildren<StartSound>();
		topmenu = GetComponentInChildren<TopMenuLoop>();
		ingame = GetComponentInChildren<IngameLoop>();
		currentSound = null;
	}

	public SoundInfo PlayAudioClip(AudioClipInfo audioClip)
	{
		if (audioClip.Clip == null)
		{
			return null;
		}
		return PlayAudioClip(audioClip.Clip, audioClip.Rollof, audioClip.minVolume, audioClip.maxVolume, audioClip.minPitch, audioClip.maxPitch, base.transform.position);
	}

	public SoundInfo PlayAudioClip(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		SoundInfo soundInfo = null;
		bool flag = false;
		bool flag2 = false;
		string text = audioClip.name;
		int i = 0;
		for (int count = _soundList.Count; i < count; i++)
		{
			if (!_soundList[i].available)
			{
				continue;
			}
			if (_soundList[i].name == text)
			{
				soundInfo = _soundList[i];
				flag = true;
				break;
			}
			if (!flag2)
			{
				if (_soundList[i].name == "empty")
				{
					flag2 = true;
				}
				soundInfo = _soundList[i];
			}
		}
		if (soundInfo == null)
		{
			soundInfo = _soundList[0];
			_soundList.Add(soundInfo);
		}
		if (flag)
		{
			StartCoroutine(soundInfo.Play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position));
			return soundInfo;
		}
		StartCoroutine(soundInfo.Play(audioClip, rolloff, minVolume, maxVolume, minPitch, maxPitch, position));
		return soundInfo;
	}

	public void removeSound(SoundInfo s)
	{
		_soundList.Remove(s);
	}

	public void SetLoopState(LoopType loop)
	{
		switch (loop)
		{
		case LoopType.flypack:
			currentSound = flyback;
			break;
		case LoopType.magnet:
			currentSound = magnet;
			break;
		case LoopType.revive:
			currentSound = revive;
			break;
		case LoopType.unlock:
			currentSound = unlock;
			break;
		case LoopType.start:
			currentSound = start;
			break;
		case LoopType.chestopen:
			currentSound = chestOpen;
			break;
		case LoopType.chestshow:
			currentSound = chestShow;
			break;
		case LoopType.ingame:
			currentSound = ingame;
			break;
		case LoopType.topmenu:
			currentSound = topmenu;
			break;
		}
	}

	public void BackgroundSoundChange(bool stop = false)
	{
		if (stop)
		{
			currentSound.Stop();
		}
		else
		{
			currentSound.Play();
		}
	}
}
