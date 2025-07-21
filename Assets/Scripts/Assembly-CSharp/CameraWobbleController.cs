using UnityEngine;

public class CameraWobbleController : MonoBehaviour
{
	private float AnimationTime = 1f;

	[SerializeField]
	private float WobbleIntensity = 1f;

	[SerializeField]
	private float WobbleLength = 0.5f;

	[SerializeField]
	private AnimationCurve WobbleX_AC;

	[SerializeField]
	private AnimationCurve WobbleY_AC;

	public void Wobble()
	{
		AnimationTime = 0f;
	}

	public Vector3 UpdateWobbleController()
	{
		Vector3 result = new Vector3(0f - WobbleX_AC.Evaluate(AnimationTime), WobbleY_AC.Evaluate(AnimationTime), 0f) * WobbleIntensity;
		AnimationTime = Mathf.Min(AnimationTime + Time.deltaTime / WobbleLength, 1f);
		return result;
	}
}
