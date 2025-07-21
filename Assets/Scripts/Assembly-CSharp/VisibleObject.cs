using UnityEngine;

public class VisibleObject : MonoBehaviour
{
	public delegate void OnVisibleChangeDelegate(bool isVisible);

	public OnVisibleChangeDelegate OnVisibleChange;

	public void OnBecameInvisible()
	{
		if (OnVisibleChange != null)
		{
			OnVisibleChange(false);
		}
	}

	public void OnBecameVisible()
	{
		if (OnVisibleChange != null)
		{
			OnVisibleChange(true);
		}
	}
}
