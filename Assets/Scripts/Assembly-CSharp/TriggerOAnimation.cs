using UnityEngine;

public class TriggerOAnimation : TriggerO
{
	[SerializeField]
	private string triggerClip;

	[SerializeField]
	private AudioClipInfo triggerAudio;

	public override void TriggerOnEnter(Collider collider)
	{
		if (collider.gameObject.layer == Layers.Instance.Character)
		{
			if (string.IsNullOrEmpty(triggerClip) || anim[triggerClip] == null)
			{
				Debug.Log("triggerClip is null Or " + base.name + "'s animation has not triggerClip.");
			}
			anim.CrossFade(triggerClip, 0.2f);
			if (triggerAudio.Clip != null)
			{
				AudioPlayer.Instance.PlaySound(triggerAudio.Clip.name, true);
			}
		}
	}
}
