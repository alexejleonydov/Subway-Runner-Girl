using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
	private float AnimationTime = 1f;

	[SerializeField]
	private float ShakeIntensity = 1f;

	[SerializeField]
	private float ShakeLength = 0.5f;

	private Vector3 diff;

	public void Shake()
	{
		AnimationTime = 0f;
		diff = Vector3.zero;
	}

	public Vector3 UpdateShakeController()
	{
		diff += Random.insideUnitSphere * ShakeIntensity;
		Vector3 result = (1f - AnimationTime) * diff * Time.deltaTime;
		AnimationTime = Mathf.Min(AnimationTime + Time.deltaTime / ShakeLength, 1f);
		return result;
	}
}
