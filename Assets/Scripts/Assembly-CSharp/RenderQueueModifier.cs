using UnityEngine;

public class RenderQueueModifier : MonoBehaviour
{
	public enum RenderType
	{
		FRONT = 0,
		MID = 1,
		BACK = 2
	}

	private int _lastQueue;

	private Renderer[] _renderers;

	public UIWidget m_target;

	public RenderType m_type;

	private void LateUpdate()
	{
		if (!(m_target != null) || !(m_target.drawCall != null))
		{
			return;
		}
		int num = 2;
		if (m_type == RenderType.BACK)
		{
			num = -2;
		}
		if (m_type == RenderType.MID)
		{
			num = 1;
		}
		num += m_target.drawCall.renderQueue;
		if (_lastQueue != num)
		{
			_lastQueue = num;
			int i = 0;
			for (int num2 = _renderers.Length; i < num2; i++)
			{
				_renderers[i].material.renderQueue = _lastQueue;
			}
		}
	}

	private void Start()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}
}
