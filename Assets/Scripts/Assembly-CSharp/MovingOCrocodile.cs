using UnityEngine;

public class MovingOCrocodile : MovingO
{
	[SerializeField]
	private float trigger;

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private string runClip;

	[SerializeField]
	private string attackClip;

	[SerializeField]
	private Transform neck;

	[SerializeField]
	private AudioClip audioClip;

	private bool firstPlayAttackClip;

	private float temp;

	protected override float Distance
	{
		get
		{
			return curTrans.position.z - MovingO.characterController.transform.position.z;
		}
	}

	protected override float Speed
	{
		get
		{
			return speed;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		anim[attackClip].AddMixingTransform(neck);
		anim[attackClip].layer = 2;
		firstPlayAttackClip = false;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		anim.Play(runClip);
		anim[attackClip].enabled = false;
		firstPlayAttackClip = true;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		anim.Stop();
	}

	protected override void Update()
	{
		base.Update();
		temp = Distance;
		if (temp < trigger && firstPlayAttackClip)
		{
			anim[attackClip].enabled = true;
			anim.Play(attackClip);
			if (audioClip != null)
			{
				AudioPlayer.Instance.PlaySound(audioClip.name, true);
			}
			firstPlayAttackClip = false;
		}
	}
}
