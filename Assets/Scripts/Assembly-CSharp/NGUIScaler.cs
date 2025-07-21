using UnityEngine;

[RequireComponent(typeof(UIRoot))]
public class NGUIScaler : MonoBehaviour
{
	private UIRoot UIRoot;

	private void Awake()
	{
		UIRoot = base.gameObject.GetComponent<UIRoot>();
		if (UIBaseScreen.IsOutOfProportion())
		{
			if ((float)Screen.height > 1280f)
			{
				float num = (float)Screen.width / 720f;
				num = (float)Screen.height / num;
				UIRoot.manualHeight = (int)num;
			}
		}
		else
		{
			UIRoot.manualHeight = 1280;
		}
	}
}
