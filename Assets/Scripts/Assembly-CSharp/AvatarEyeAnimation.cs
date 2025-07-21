using UnityEngine;

public class AvatarEyeAnimation : MonoBehaviour
{
	private bool animating;

	public Renderer closedEyes;

	public float blinkTime = 0.1f;

	public float blinkWaitTimeMax = 5.7f;

	public float blinkWaitTimeMin = 0.6f;

	private float waitForBlinkEndTime;

	private float blinkEndTime;

	public bool IsAnimating()
	{
		return animating;
	}

	public void ForceBlink()
	{
		waitForBlinkEndTime = Time.time;
		blinkEndTime = waitForBlinkEndTime + blinkTime;
	}

	public void ChangeMat(Material mat)
	{
		closedEyes.material = mat;
	}

	public void StartAnimatingEyes()
	{
		waitForBlinkEndTime = Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
		blinkEndTime = waitForBlinkEndTime + blinkTime;
		animating = true;
	}

	public void StopAnimatingEyes()
	{
		animating = false;
		closedEyes.enabled = false;
	}

	private void Update()
	{
		if (animating)
		{
			UpdateEyeBlinking();
		}
		else
		{
			closedEyes.enabled = false;
		}
	}

	private void UpdateEyeBlinking()
	{
		if (!(closedEyes != null))
		{
			return;
		}
		if (blinkWaitTimeMax >= blinkWaitTimeMin)
		{
			if (!(Time.time >= waitForBlinkEndTime))
			{
				return;
			}
			if (Time.time < blinkEndTime)
			{
				if (!closedEyes.enabled)
				{
					closedEyes.enabled = true;
				}
			}
			else
			{
				closedEyes.enabled = false;
				waitForBlinkEndTime = Time.time + Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
				blinkEndTime = waitForBlinkEndTime + blinkTime;
			}
		}
		else
		{
			Debug.Log("AvatarEyeAnimation: You need to make blinkWaitTimeMax larger then or equal blinkWaitTimeMin", this);
		}
	}
}
