using System.Collections;
using UnityEngine;

public class UIPanelStaticDelayer : MonoBehaviour
{
	private UIPanel _panel;

	[SerializeField]
	private int framesToWait = 2;

	private bool inited;

	[SerializeField]
	private bool refreshOnEnable;

	private void Awake()
	{
		_panel = GetComponent<UIPanel>();
		if (_panel == null)
		{
			Debug.LogWarning("UIPanelStaticDelayer is not set on a UIPanel");
			return;
		}
		if (framesToWait < 0)
		{
			Debug.LogWarning("UIPanelStaticDelayer.framesToWait can not be less than 0");
			return;
		}
		OrderStaticDelay();
		inited = true;
	}

	private void OnEnable()
	{
		if (refreshOnEnable && inited)
		{
			OrderStaticDelay();
		}
	}

	private void OrderStaticDelay()
	{
		if (_panel.widgetsAreStatic)
		{
			_panel.widgetsAreStatic = false;
		}
		StartCoroutine(SetStaticDelayed(framesToWait, _panel));
	}

	public IEnumerator SetStaticDelayed(int delayFrames, UIPanel panel)
	{
		int num = 0;
		while (num < delayFrames)
		{
			panel.Refresh();
			num++;
			yield return null;
		}
		panel.widgetsAreStatic = true;
	}
}
