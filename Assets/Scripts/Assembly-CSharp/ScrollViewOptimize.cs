using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewOptimize : UIScrollView
{
	public enum Direction
	{
		Down = 0,
		Up = 1
	}

	public enum Sorting
	{
		None = 0,
		Alphabetic = 1,
		Horizontal = 2,
		Vertical = 3,
		Custom = 4
	}

	[SerializeField]
	private int tipHeight = 110;

	[SerializeField]
	private int cellWidth = 100;

	[SerializeField]
	private int cellHeight = 100;

	[SerializeField]
	private int initCount = 10;

	[SerializeField]
	private List<Transform> children;

	[SerializeField]
	private Transform panelTrans;

	private int lastStart;

	private float originPanelY;

	protected override void Start()
	{
		base.Start();
		UIScrollView component = GetComponent<UIScrollView>();
		if (component != null)
		{
			component.onDragStarted = (OnDragNotification)Delegate.Combine(component.onDragStarted, new OnDragNotification(OnDragStart));
			component.onDragFinished = (OnDragNotification)Delegate.Combine(component.onDragFinished, new OnDragNotification(OnDragFinished));
		}
		originPanelY = panelTrans.localPosition.y;
	}

	private void OnDragFinished()
	{
		base.enabled = false;
	}

	private void OnDragStart()
	{
		base.enabled = true;
	}

	private void RefreshUI()
	{
		float num = panelTrans.localPosition.y - originPanelY;
		int num2 = -1;
		float num3;
		for (num3 = 0f; num3 < num; num3 = ((num2 != 0 && num2 != 4 && num2 != 12) ? (num3 + (float)cellHeight) : (num3 + (float)tipHeight)))
		{
			num2++;
		}
		int num4 = num2;
		float num5 = num3;
		while (num4 < num2 + initCount)
		{
			num4++;
			num5 = ((num4 != 0 && num4 != 4 && num4 != 12) ? (num5 + (float)cellHeight) : (num5 + (float)tipHeight));
		}
		if (lastStart != num2)
		{
			while (lastStart < num2)
			{
				Transform transform = children[0];
				children.RemoveAt(0);
				children.Add(transform);
				transform.localPosition = new Vector3(0f, num5, 0f);
			}
		}
		lastStart = num2;
	}
}
