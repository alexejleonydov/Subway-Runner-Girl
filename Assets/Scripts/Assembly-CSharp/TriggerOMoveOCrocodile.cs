using UnityEngine;

public class TriggerOMoveOCrocodile : TriggerO
{
	[SerializeField]
	private string transitionClip;

	[SerializeField]
	private string triggerClip;

	private Transform child;

	private float delayTime;

	private float time;

	private float originLPosZForChild;

	private float rate;

	private Character character;

	private bool moveFirstUpdate;

	protected override void Awake()
	{
		base.Awake();
		child = base.transform.GetChild(0);
		originLPosZForChild = child.localPosition.z;
		character = Character.Instance;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		child.localPosition = new Vector3(0f, 0f, originLPosZForChild);
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		base.enabled = false;
	}

	public override void TriggerOnEnter(Collider collider)
	{
		if (collider.gameObject.layer == Layers.Instance.Character)
		{
			if (transitionClip != null)
			{
				anim.CrossFade(transitionClip, 0.1f);
				anim.CrossFadeQueued(triggerClip, 0.1f);
				delayTime = anim[transitionClip].length;
			}
			else
			{
				anim.CrossFade(triggerClip, 0.1f);
				delayTime = 0f;
			}
			time = 0f;
			moveFirstUpdate = true;
			base.enabled = true;
		}
	}

	private void Update()
	{
		if (time < delayTime)
		{
			time += Time.deltaTime;
			return;
		}
		if (moveFirstUpdate)
		{
			if (character.z >= base.transform.position.z)
			{
				Debug.LogError("Character has already exceeded this Object.");
				base.enabled = false;
				return;
			}
			rate = originLPosZForChild / (base.transform.position.z - character.z);
			moveFirstUpdate = false;
		}
		child.localPosition = new Vector3(0f, 0f, (base.transform.position.z - character.z) * rate);
	}
}
