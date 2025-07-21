using UnityEngine;

public class MovingAnimal : MovingO
{
	[SerializeField]
	private Animation anim;

	[SerializeField]
	private string runClip;

	private float originLPosZForChild;

	private bool moveFirstUpdate;

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
		originLPosZForChild = child.localPosition.z;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		child.localPosition = new Vector3(0f, 0f, originLPosZForChild);
		moveFirstUpdate = true;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		anim.Stop();
		child.transform.localPosition = new Vector3(0f, 0f, originLPosZForChild);
	}

	protected override void Update()
	{
		if (!(Distance * Speed > originLPosZForChild))
		{
			if (moveFirstUpdate)
			{
				anim.Play(runClip);
				moveFirstUpdate = false;
			}
			base.Update();
		}
	}
}
