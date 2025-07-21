using UnityEngine;

public class AudioRecycleItem : AbsRecycleItem
{
	public AudioSource audio;

	public bool isUsing;

	public override bool IsUsing()
	{
		return isUsing;
	}

	public override void Release()
	{
		audio.Stop();
		isUsing = false;
	}

	public override void Retain()
	{
		audio.Play();
		isUsing = true;
	}
}
