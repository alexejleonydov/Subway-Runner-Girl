using System.Collections;
using UnityEngine;

public class Glow : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	[SerializeField]
	private Animation anim;

	private void Reset()
	{
		if (meshRenderer == null)
		{
			meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
		if (anim == null)
		{
			anim = GetComponentInChildren<Animation>();
		}
	}

	public void Awake()
	{
		Transform parent = base.transform.parent;
		Transform transform = ((!(parent != null)) ? null : parent.parent);
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && (parent.gameObject.name.Contains("coin") || (transform != null && transform.gameObject.name.Contains("coin"))))
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					Object.Destroy(transform2.gameObject);
				}
			}
			finally
			{
			}
			if (meshRenderer == null)
			{
				meshRenderer = GetComponentInChildren<MeshRenderer>();
			}
			if (anim == null)
			{
				anim = GetComponentInChildren<Animation>();
			}
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
				base.enabled = false;
				Object.Destroy(meshRenderer);
				meshRenderer = null;
			}
		}
		else
		{
			if (meshRenderer == null)
			{
				meshRenderer = GetComponentInChildren<MeshRenderer>();
			}
			if (anim == null)
			{
				anim = GetComponentInChildren<Animation>();
			}
		}
	}

	public void SetVisible(bool visible)
	{
		if (anim != null)
		{
			if (visible)
			{
				anim.Play();
			}
			else
			{
				anim.Stop();
			}
		}
		if (meshRenderer != null)
		{
			meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}
}
