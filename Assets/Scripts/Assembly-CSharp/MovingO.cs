using System.Collections.Generic;
using UnityEngine;

public class MovingO : BaseO
{
	private static List<MovingO> activeMovings = new List<MovingO>();

	protected static CharacterController characterController;

	public static float autoPilotActivationDistance = 200f;

	public float speed = 1f;

	protected Game game;

	protected bool autoPilot;

	protected BoxCollider Collider;

	protected Transform child;

	protected Transform curTrans;

	private Vector3 position;

	protected virtual float Distance { get; private set; }

	protected virtual float Speed { get; private set; }

	protected override void Awake()
	{
		game = Game.Instance;
		if (game != null)
		{
			if (game.awakeDone)
			{
				Init();
			}
			if (base.transform.childCount == 0)
			{
				Debug.Log("No train child");
			}
			characterController = game.character.characterController;
			child = base.transform.GetChild(0);
			Collider = GetComponent<BoxCollider>();
			base.enabled = false;
		}
		curTrans = base.transform;
		base.Awake();
	}

	protected virtual void Init()
	{
	}

	public override void OnActivate()
	{
		base.enabled = true;
		if (!activeMovings.Contains(this))
		{
			activeMovings.Add(this);
		}
		autoPilot = false;
		child.localPosition = new Vector3(0f, 0f, Distance * speed);
	}

	public override void OnDeactivate()
	{
		if (activeMovings.Contains(this))
		{
			activeMovings.Remove(this);
		}
		base.enabled = false;
		child.transform.localPosition = -200f * Vector3.up;
	}

	protected virtual void Update()
	{
		if (game != null && !autoPilot)
		{
			position = new Vector3(0f, 0f, Distance * Speed);
			child.localPosition = position;
		}
	}

	public static void ActivateAutoPilot()
	{
		int i = 0;
		for (int count = activeMovings.Count; i < count; i++)
		{
			if (activeMovings[i].Collider.bounds.min.z - characterController.transform.position.z < autoPilotActivationDistance)
			{
				activeMovings[i].autoPilot = true;
			}
		}
	}

	public void OnDrawGizmos()
	{
		if (child != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(child.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(child.position, 5f);
		}
	}
}
