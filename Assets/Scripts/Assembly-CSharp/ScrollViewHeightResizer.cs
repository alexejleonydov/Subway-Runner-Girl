using System.Collections;
using UnityEngine;

public class ScrollViewHeightResizer : MonoBehaviour
{
	[SerializeField]
	private GameObject _scrollBar;

	[SerializeField]
	private GameObject _scrollCollider;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private GameObject _grid;

	[SerializeField]
	private float _staticObjectsHeight;

	[SerializeField]
	private float _centerOffset;

	[SerializeField]
	private bool _addTopCollider;

	[SerializeField]
	private bool _addBottomcollider;

	[SerializeField]
	private float _zDepth;

	[SerializeField]
	private bool rearrange;

	private bool isCalculated;

	public Vector4 clipping
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.finalClipRegion;
		}
	}

	public float Height
	{
		get
		{
			return _staticObjectsHeight;
		}
		set
		{
			_staticObjectsHeight = value;
		}
	}

	public Vector3 scrollPanelPosition
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.transform.localPosition;
		}
	}

	private void AddColliders()
	{
		Vector3 zero = Vector3.zero;
		zero.z = _zDepth;
		if (!isCalculated)
		{
			if (_addTopCollider)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject.AddComponent<BoxCollider>().center = zero;
				gameObject.name = "TopScrollBlocker";
				float num = (float)Screen.height - (_scrollCollider.transform.localPosition.y + _scrollCollider.transform.localScale.y / 2f);
				float y = (float)Screen.height - num / 2f;
				float x = Screen.width;
				float x2 = 0f;
				float z = base.gameObject.transform.localPosition.z;
				gameObject.transform.localPosition = new Vector3(x2, y, z);
				gameObject.transform.localScale = new Vector3(x, num, 1f);
			}
			if (_addBottomcollider)
			{
				GameObject gameObject2 = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject2.AddComponent<BoxCollider>().center = zero;
				gameObject2.name = "BottomScrollBlocker";
				float num2 = _scrollCollider.transform.localPosition.y - _scrollCollider.transform.localScale.y / 2f;
				float y2 = num2 / 2f;
				float x3 = Screen.width;
				float x4 = 0f;
				float z2 = base.gameObject.transform.localPosition.z;
				gameObject2.transform.localPosition = new Vector3(x4, y2, z2);
				gameObject2.transform.localScale = new Vector3(x3, num2, 1f);
			}
		}
	}

	public void RearrangeWidgets()
	{
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		if (uIRoot == null)
		{
			Debug.LogWarning("Root is not set in the UIScreenController");
		}
		UIScrollBar component = _scrollBar.GetComponent<UIScrollBar>();
		float num = (float)uIRoot.manualHeight - _staticObjectsHeight;
		Vector4 baseClipRegion = _scrollPanel.baseClipRegion;
		baseClipRegion.w = num;
		_scrollPanel.baseClipRegion = baseClipRegion;
		Vector3 localPosition = _scrollPanel.transform.localPosition;
		localPosition.y = _scrollPanel.clipOffset.y * -1f;
		_scrollPanel.transform.localPosition = localPosition;
		Vector3 localPosition2 = _grid.transform.localPosition;
		localPosition2.y = 0f;
		_grid.transform.localPosition = localPosition2;
		float y = (float)(uIRoot.manualHeight / 2) + _centerOffset;
		float y2 = (float)uIRoot.manualHeight - _staticObjectsHeight;
		Vector3 localPosition3 = _scrollCollider.transform.localPosition;
		localPosition3.y = y;
		_scrollCollider.transform.localPosition = localPosition3;
		Vector3 localScale = _scrollCollider.transform.localScale;
		localScale.y = y2;
		_scrollCollider.transform.localScale = localScale;
		UIWidget backgroundWidget = component.backgroundWidget;
		UIWidget foregroundWidget = component.foregroundWidget;
		int num4 = (foregroundWidget.height = (backgroundWidget.height = Mathf.RoundToInt(num)));
		Vector3 localPosition4 = _scrollBar.transform.localPosition;
		localPosition4.y = num4 / 2;
		_scrollBar.transform.localPosition = localPosition4;
		StartCoroutine(RepositionscrollView(1));
		AddColliders();
		isCalculated = true;
	}

	private IEnumerator RepositionscrollView(int frames)
	{
		int index__0 = 0;
		while (index__0 < frames)
		{
			index__0++;
			yield return null;
		}
		UIScrollView dragPn__1 = _scrollPanel.GetComponent<UIScrollView>();
		if (!(dragPn__1 != null))
		{
		}
	}

	private void Start()
	{
		rearrange = false;
		if (_scrollBar == null)
		{
			Debug.LogError("ScrollBar not set in ScrollViewHeightResizer");
		}
		if (_scrollCollider == null)
		{
			Debug.LogError("ScrollCollider not set in ScrollViewHeightResizer");
		}
		if (_scrollPanel == null)
		{
			Debug.LogError("ScrollPanel not set in ScrollViewHeightResizer");
		}
		if (_grid == null)
		{
			Debug.LogError("Grid not set in ScrollViewHeightResizer");
		}
		if (_grid != null)
		{
		}
		RearrangeWidgets();
	}

	private void Update()
	{
		if (rearrange)
		{
			Debug.Log("ScrollViewResizer update (rearrange)" + base.gameObject.name, this);
			RearrangeWidgets();
			rearrange = false;
		}
	}
}
