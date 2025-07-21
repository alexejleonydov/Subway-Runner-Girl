using UnityEngine;

public class AnimationEventDriver : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem ps;

	public void PlayParticleSystem()
	{
		ps.Play();
	}
}
