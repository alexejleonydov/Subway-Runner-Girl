using UnityEngine;

public class UIScrollClick : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	private IScrollClick scollClick;

	private void Awake()
	{
		if (!(target == null))
		{
			scollClick = target.GetComponent<IScrollClick>();
		}
	}

	private void OnClick()
	{
		if (scollClick != null)
		{
			Vector2 pos = UICamera.currentTouch.pos;
			scollClick.ScrollClicked(pos);
		}
	}
}
