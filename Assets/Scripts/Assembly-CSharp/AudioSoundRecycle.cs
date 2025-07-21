using UnityEngine;

public class AudioSoundRecycle : AbsRecycle<AudioRecycleItem>
{
	public override AudioRecycleItem NewRecycleItem(GameObject newGo)
	{
		AudioRecycleItem audioRecycleItem = new AudioRecycleItem();
		audioRecycleItem.audio = newGo.GetComponent<AudioSource>();
		return audioRecycleItem;
	}

	public virtual void Pause()
	{
		for (int i = 0; i < recycleItems.Count; i++)
		{
			recycleItems[i].audio.Pause();
		}
	}

	public virtual void Stop()
	{
		for (int i = 0; i < recycleItems.Count; i++)
		{
			recycleItems[i].audio.Stop();
		}
	}

	public override void WarmUp()
	{
		while (recycleItems.Count < max)
		{
			if (origGo != null)
			{
				GameObject gameObject = Object.Instantiate(origGo);
				gameObject.transform.parent = base.transform;
				AbsRecycleItem absRecycleItem = NewRecycleItem(gameObject);
				recycleItems.Add((AudioRecycleItem)absRecycleItem);
			}
		}
	}
}
