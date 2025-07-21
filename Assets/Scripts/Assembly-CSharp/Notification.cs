using UnityEngine;

public class Notification : MonoBehaviour
{
	[SerializeField]
	private UISprite[] dots;

	[SerializeField]
	private int[] ids;

	public int[] GetIds()
	{
		return ids;
	}

	public void SetNotification(int id, bool value)
	{
		if (id >= 0 && id < dots.Length)
		{
			dots[id].enabled = value;
		}
	}
}
