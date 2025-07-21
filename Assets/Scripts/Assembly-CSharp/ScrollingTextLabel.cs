using System.Collections;
using UnityEngine;

public class ScrollingTextLabel : MonoBehaviour
{
	private bool _destroyOnDisable;

	private bool _isScrolling;

	private Color _labelBaseColor;

	private Transform _labelTransform;

	[SerializeField]
	private UILabel label;

	private void OnDisable()
	{
		_isScrolling = false;
	}

	private void OnEnable()
	{
		label.enabled = false;
		_labelTransform = label.transform;
		_labelBaseColor = label.color;
		if (_destroyOnDisable)
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	public void StartScrolling(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (duration <= 0f)
		{
			Debug.LogError("Duration must be > 0", this);
		}
		else if (!_isScrolling)
		{
			StartCoroutine(StartScrollingCoroutine(text, startLocalPos, endLocalPos, duration, fadeOutDuration, destroyWhenDone));
		}
	}

	private IEnumerator StartScrollingCoroutine(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (_isScrolling)
		{
			yield break;
		}
		_isScrolling = true;
		_destroyOnDisable = destroyWhenDone;
		float fadeOutAniFactorStart__0 = Mathf.Clamp01((duration - fadeOutDuration) / duration);
		label.enabled = true;
		label.text = text;
		label.color = _labelBaseColor;
		startLocalPos.z = -1f;
		endLocalPos.z = -1f;
		float aniFactor__1 = 0f;
		while (aniFactor__1 < 1f)
		{
			aniFactor__1 = Mathf.Clamp01(aniFactor__1 + Time.deltaTime / duration);
			_labelTransform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, aniFactor__1);
			if (aniFactor__1 >= fadeOutAniFactorStart__0)
			{
				float num = (aniFactor__1 - fadeOutAniFactorStart__0) / (1f - fadeOutAniFactorStart__0);
				Color labelBaseColor = _labelBaseColor;
				labelBaseColor.a *= 1f - num;
				label.color = labelBaseColor;
			}
			yield return null;
		}
		label.enabled = false;
		_isScrolling = false;
		if (destroyWhenDone)
		{
			NGUITools.Destroy(base.gameObject);
		}
	}
}
