using UnityEngine;

public class UIDrag : MonoBehaviour
{
	public delegate void OnSwipeDelegate(SwipeDir dir);

	[SerializeField]
	private float sensitivity;

	public OnSwipeDelegate onHandleDir;

	private Vector2 mMovemont;

	private bool mDragStart;

	private void OnDragStart()
	{
		mDragStart = true;
		mMovemont = Vector2.zero;
	}

	private void OnDrag(Vector2 delta)
	{
		if (!mDragStart)
		{
			return;
		}
		mMovemont += delta;
		SwipeDir swipeDir = AnalysisDirction(mMovemont);
		if (swipeDir != SwipeDir.None)
		{
			if (onHandleDir != null)
			{
				onHandleDir(swipeDir);
			}
			mDragStart = false;
		}
	}

	private void OnDragEnd()
	{
		mDragStart = false;
	}

	private SwipeDir AnalysisDirction(Vector2 movement)
	{
		if (movement.magnitude < sensitivity)
		{
			return SwipeDir.None;
		}
		Vector2 normalized = movement.normalized;
		SwipeDir result = SwipeDir.None;
		float num = 0f;
		float num2 = Vector2.Dot(normalized, Vector2.up);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Up;
		}
		num2 = Vector2.Dot(normalized, Vector2.right);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Right;
		}
		num2 = Vector2.Dot(normalized, Vector2.down);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Down;
		}
		num2 = Vector2.Dot(normalized, Vector2.left);
		if (num2 > num)
		{
			num = num2;
			result = SwipeDir.Left;
		}
		return result;
	}
}
