using UnityEngine;

public class LaunchAdpter : MonoBehaviour
{
	[SerializeField]
	private UIWidget texture;

	[SerializeField]
	private UIRoot root;

	private void Start()
	{
		float num = (float)texture.height / (float)texture.width;
		if (UIBaseScreen.IsOutOfProportion())
		{
			texture.height = root.activeHeight;
			texture.width = (int)((float)texture.height / num);
		}
		else
		{
			texture.width = (int)((float)root.activeHeight / (float)Screen.height * (float)Screen.width);
			texture.height = (int)((float)texture.width * num);
		}
	}
}
