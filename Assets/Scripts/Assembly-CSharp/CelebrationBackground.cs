using UnityEngine;

public class CelebrationBackground : MonoBehaviour
{
	public Transform backgroundRoot;

	public Transform celebrationCloner;

	[SerializeField]
	private AnimationCurve _celebrationAnimationCurve;

	private bool shouldRotate;

	public bool visible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			base.gameObject.SetActive(value);
		}
	}

	public float GetCelebrationAnimationCurveValue(float pos)
	{
		return _celebrationAnimationCurve.Evaluate(pos);
	}

	public void Hide()
	{
		visible = false;
	}

	public void InvertParticlesDirection(bool invert)
	{
		if (invert)
		{
			celebrationCloner.localPosition = new Vector3(0f, 100f, 0f);
			celebrationCloner.localRotation = new Quaternion(0f, 0f, 180f, 0f);
		}
		else
		{
			celebrationCloner.localPosition = new Vector3(0f, -20f, 0f);
			celebrationCloner.localRotation = Quaternion.identity;
		}
	}

	public void SetParticleAnimationSpeed(float speed)
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>(false);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.playbackSpeed = speed;
		}
	}

	public void Show(Quaternion rotation, bool shouldAnimateStripes, bool shouldRotateBackground)
	{
		if (!visible)
		{
			visible = true;
		}
		if (celebrationCloner.gameObject.activeSelf != shouldAnimateStripes)
		{
			celebrationCloner.gameObject.SetActive(shouldAnimateStripes);
		}
		shouldRotate = shouldRotateBackground;
		backgroundRoot.rotation = rotation;
		InvertParticlesDirection(false);
	}

	private void Update()
	{
		if (shouldRotate)
		{
			backgroundRoot.Rotate(0f, 0f, 45f * Time.deltaTime);
		}
	}
}
