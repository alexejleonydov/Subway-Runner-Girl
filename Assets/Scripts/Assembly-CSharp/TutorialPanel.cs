using UnityEngine;

public class TutorialPanel : UIBaseScreen
{
	[SerializeField]
	private Transform mParent;

	[SerializeField]
	private Transform mFinger;

	private GameObject _tutorialButton;

	public void Ready()
	{
		if (_tutorialButton != null)
		{
			Object.Destroy(_tutorialButton);
		}
		mFinger.position = Vector3.one * 10f;
	}

	public void Show(GameObject go, Vector3 offset, float angle)
	{
		if (_tutorialButton != null)
		{
			Object.Destroy(_tutorialButton);
		}
		_tutorialButton = go;
		_tutorialButton.transform.parent = mParent;
		Vector3 localPosition = _tutorialButton.transform.localPosition;
		localPosition.z = 0f;
		_tutorialButton.transform.localPosition = localPosition;
		_tutorialButton.transform.localScale = Vector3.one;
		mFinger.position = _tutorialButton.transform.position + offset;
		mFinger.localRotation = Quaternion.Euler(0f, 0f, angle);
	}

	public override void Hide()
	{
		if (_tutorialButton != null)
		{
			Object.Destroy(_tutorialButton);
		}
		base.Hide();
	}
}
