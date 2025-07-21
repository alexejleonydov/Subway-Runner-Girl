using System;
using UnityEngine;

[Serializable]
public class KeyFrameAudio : KeyFrameBase
{
	public delegate void ExtraKeyframeCall(KeyFrameAudio info);

	public AudioKeyFrameType audioKeyFrameType;

	public string audio;

	public Transform point;

	public ExtraKeyframeCall Callback;
}
