using System.Collections;
using UnityEngine;

public class KickedInFly : BaseO, ITouchByCharacter
{
	public string kickedClip;

	private Vector3 originLPos;

	private Quaternion originLQuat;

	private Collider col;

	protected override void Awake()
	{
		originLPos = base.transform.localPosition;
		originLQuat = base.transform.localRotation;
		col = GetComponent<Collider>();
		base.Awake();
	}

	public override void OnActivate()
	{
		base.enabled = true;
		base.transform.localPosition = originLPos;
		base.transform.localRotation = originLQuat;
		col.enabled = true;
	}

	public bool BeTouched()
	{
		col.enabled = false;
		AudioPlayer.Instance.PlaySound(kickedClip, true);
		StartCoroutine(BeKickedOff_C());
		return true;
	}

	private IEnumerator BeKickedOff_C()
	{
		if (GetComponent<MovingO>() != null)
		{
			GetComponent<MovingO>().enabled = false;
		}
		float startZ = base.transform.position.z;
		float startX = base.transform.position.x;
		Quaternion quat = base.transform.rotation;
		Quaternion to = base.transform.rotation * Quaternion.Euler(-30f, 0f, 90f);
		float rate = 0f;
		float z2 = startZ;
		while (rate < 1f)
		{
			z2 = Mathf.Lerp(startZ, 2000f, rate);
			base.transform.position = new Vector3(Mathf.Lerp(startX / z2, 0.05f, rate) * z2, Mathf.Lerp(0f, 0.2f, rate) * z2, z2);
			base.transform.rotation = Quaternion.Lerp(quat, to, rate);
			rate += Time.deltaTime / 1.6f;
			yield return null;
		}
		OnDeactivate();
	}
}
