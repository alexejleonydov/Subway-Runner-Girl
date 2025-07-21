using System.Collections;
using UnityEngine;

public class Fly : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem start;

	[SerializeField]
	private ParticleSystem fly;

	[SerializeField]
	private ParticleSystem end;

	[SerializeField]
	private ParticleSystem.Particle[] particles;

	[SerializeField]
	private int count;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private int maxVelocityFrame;

	[SerializeField]
	private float slowVelocityFrame;

	[SerializeField]
	private float minVelocityFrame;

	[SerializeField]
	private float velocityRate;

	[SerializeField]
	private float duration;

	[SerializeField]
	private float whenPlayEnd;

	private void Start()
	{
		ParticleSystem.MainModule main = fly.main;
		main.customSimulationSpace = target;
		particles = new ParticleSystem.Particle[count];
	}

	public IEnumerator Begain()
	{
		if ((bool)start)
		{
			start.Play();
		}
		fly.Emit(count);
		int frame3 = 0;
		while (frame3 < maxVelocityFrame)
		{
			frame3++;
			yield return null;
		}
		int length3 = 0;
		int k = 0;
		frame3 = 0;
		while ((float)frame3 < slowVelocityFrame)
		{
			frame3++;
			length3 = fly.GetParticles(particles);
			float rate = (float)frame3 / slowVelocityFrame * velocityRate;
			for (k = 0; k < length3; k++)
			{
				particles[k].velocity = Vector3.Lerp(particles[k].velocity, Vector3.zero, rate * rate);
			}
			fly.SetParticles(particles, length3);
			yield return null;
		}
		frame3 = 0;
		while ((float)frame3 < minVelocityFrame)
		{
			frame3++;
			yield return null;
		}
		bool hasPlayEnd = false;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			length3 = fly.GetParticles(particles);
			for (k = 0; k < length3; k++)
			{
				particles[k].position = Vector3.Lerp(particles[k].position, Vector3.zero, factor * factor);
			}
			fly.SetParticles(particles, length3);
			if (!hasPlayEnd && factor > whenPlayEnd)
			{
				hasPlayEnd = true;
				if ((bool)end)
				{
					end.Play();
				}
			}
			yield return null;
		}
		yield return null;
		fly.Clear();
	}
}
