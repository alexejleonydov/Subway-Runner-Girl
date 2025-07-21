using System;
using UnityEngine;

public class CenterOnChild : MonoBehaviour
{
	[NonSerialized]
	public bool characterWasClicked;

	private GameObject mCenteredObject;

	private UIScrollView mDrag;

	private GameObject mSelectedObject;

	public GameObject centeredObject
	{
		get
		{
			return mSelectedObject;
		}
	}

	public bool CenterOnClosestChildAtPosition(Vector2 clickPositionOnScreen)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIScrollView), " on a parent object in order to work"), this);
				base.enabled = false;
				return false;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (mDrag.panel != null)
		{
			Vector4 finalClipRegion = mDrag.panel.finalClipRegion;
			Transform cachedTransform = mDrag.panel.cachedTransform;
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x += finalClipRegion.x;
			localPosition.y += finalClipRegion.y;
			localPosition = cachedTransform.parent.TransformPoint(localPosition);
			Vector3 vector = clickPositionOnScreen;
			vector.x -= (float)Screen.width * 0.5f;
			vector *= (float)UIScreenController.Instance.root.manualWidth / (float)Screen.width;
			vector.x += finalClipRegion.x;
			float num = float.MaxValue;
			Transform transform = null;
			Transform transform2 = base.transform;
			int i = 0;
			for (int childCount = transform2.childCount; i < childCount; i++)
			{
				Transform child = transform2.GetChild(i);
				float num2 = Mathf.Abs(child.localPosition.x - vector.x);
				if (num2 < num)
				{
					num = num2;
					transform = child;
				}
			}
			if (transform != null)
			{
				if (mSelectedObject == transform.gameObject && mCenteredObject == transform.gameObject)
				{
					return true;
				}
				mCenteredObject = null;
				characterWasClicked = true;
				Vector3 vector2 = cachedTransform.InverseTransformPoint(transform.position) - cachedTransform.InverseTransformPoint(localPosition);
				if (!mDrag.canMoveHorizontally)
				{
					vector2.x = 0f;
				}
				if (!mDrag.canMoveVertically)
				{
					vector2.y = 0f;
				}
				vector2.z = 0f;
				SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector2, 8f).onFinished = OnStringFinished;
				if (mSelectedObject == transform.gameObject && mCenteredObject != transform.gameObject)
				{
					return true;
				}
				mSelectedObject = transform.gameObject;
			}
		}
		return false;
	}

	public void CenterOnTransform(Transform target, bool instant = false)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIScrollView), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (!(mDrag.panel != null))
		{
			return;
		}
		Transform transform = base.transform;
		bool flag = false;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			if (transform.GetChild(i) == target)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		Vector4 finalClipRegion = mDrag.panel.finalClipRegion;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += finalClipRegion.x;
		localPosition.y += finalClipRegion.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		mDrag.currentMomentum = Vector3.zero;
		if (target != null)
		{
			mSelectedObject = target.gameObject;
			Vector3 vector = cachedTransform.InverseTransformPoint(target.position) - cachedTransform.InverseTransformPoint(localPosition);
			if (!mDrag.canMoveHorizontally)
			{
				vector.x = 0f;
			}
			if (!mDrag.canMoveVertically)
			{
				vector.y = 0f;
			}
			vector.z = 0f;
			if (instant)
			{
				Vector3 localPosition2 = cachedTransform.localPosition - vector;
				mDrag.panel.clipOffset += new Vector2(vector.x, vector.y);
				cachedTransform.localPosition = localPosition2;
				if (mDrag != null)
				{
					mDrag.UpdateScrollbars(false);
				}
			}
			else
			{
				SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector, 8f).onFinished = OnStringFinished;
			}
		}
		else
		{
			mSelectedObject = null;
		}
	}

	public void CharacterFocusedFromClick()
	{
		characterWasClicked = false;
	}

	public void ClearCenterObject()
	{
		mSelectedObject = null;
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Recenter();
		}
	}

	private void OnEnable()
	{
		Recenter();
	}

	private void OnStringFinished()
	{
		mCenteredObject = mSelectedObject;
	}

	public void Recenter()
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIScrollView), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (!(mDrag.panel != null))
		{
			return;
		}
		Vector4 finalClipRegion = mDrag.panel.finalClipRegion;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += finalClipRegion.x;
		localPosition.y += finalClipRegion.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 vector = localPosition - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int i = 0;
		for (int childCount = transform2.childCount; i < childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - vector);
			if (num2 < num)
			{
				num = num2;
				transform = child;
			}
		}
		if (transform != null)
		{
			mSelectedObject = transform.gameObject;
			Vector3 vector2 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector3 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector4 = vector2 - vector3;
			if (!mDrag.canMoveHorizontally)
			{
				vector4.x = 0f;
			}
			if (!mDrag.canMoveVertically)
			{
				vector4.y = 0f;
			}
			vector4.z = 0f;
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector4, 8f).onFinished = OnStringFinished;
		}
		else
		{
			mSelectedObject = null;
		}
	}
}
