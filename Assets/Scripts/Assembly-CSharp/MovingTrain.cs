using System;
using UnityEngine;

public class MovingTrain : MovingO
{
	public enum Type
	{
		Across = 0,
		Along = 1
	}

	public Type type = Type.Along;

	public int halfLengthOfEachSection = 30;

	public bool willStop;

	public int distanceWhenStop;

	public float trainCount = 3f;

	public AudioClip trianPassClip;

	public Transform childFollow;

	public int distanceWhenAnimation = 200;

	private Animation[] anims;

	private bool startAnimation;

	private bool hasStopped;

	private bool isInitialized;

	private bool isPaused;

	private bool startSound;

	private float curSpeed;

	private AudioSource trainPassSource;

	protected override float Distance
	{
		get
		{
			return curTrans.position.z - MovingO.characterController.transform.position.z - (float)distanceWhenStop;
		}
	}

	protected override float Speed
	{
		get
		{
			if (willStop)
			{
				curSpeed = Mathf.Lerp(0f, speed, Distance / 500f);
			}
			return curSpeed = Mathf.Clamp(curSpeed, 0f, speed);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Vector3 size = Collider.size;
		Vector3 center = Collider.center;
		if (type == Type.Along)
		{
			Collider.size = new Vector3(size.x, size.y, size.z / (1f + speed));
			Collider.center = new Vector3(0f, center.y, ((float)halfLengthOfEachSection * trainCount + 1f) / (1f + speed));
		}
		anims = GetComponentsInChildren<Animation>();
		int i = 0;
		for (int num = anims.Length; i < num; i++)
		{
			anims[i].enabled = false;
		}
		if (trianPassClip != null)
		{
			trainPassSource = base.gameObject.AddComponent<AudioSource>();
			trainPassSource.minDistance = 0f;
			trainPassSource.maxDistance = 200f;
			trainPassSource.playOnAwake = false;
			trainPassSource.loop = true;
			trainPassSource.spatialBlend = 1f;
			trainPassSource.clip = trianPassClip;
			trainPassSource.rolloffMode = AudioRolloffMode.Linear;
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (trainPassSource != null && trainPassSource.isPlaying)
			{
				trainPassSource.Pause();
			}
			if (startAnimation)
			{
				int i = 0;
				for (int num = anims.Length; i < num; i++)
				{
					if (anims[i].isPlaying)
					{
						anims[i].Stop();
					}
				}
			}
			isPaused = true;
			return;
		}
		if (trainPassSource != null && isPaused)
		{
			trainPassSource.Play();
		}
		if (startAnimation)
		{
			int j = 0;
			for (int num2 = anims.Length; j < num2; j++)
			{
				if (anims[j].gameObject.activeInHierarchy)
				{
					anims[j].Play();
				}
			}
		}
		isPaused = false;
	}

	protected override void Init()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			hasStopped = false;
			curSpeed = speed;
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		Game.Instance.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(Game.Instance.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		startAnimation = false;
		startSound = true;
		hasStopped = false;
		curSpeed = speed;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		Game instance = Game.Instance;
		instance.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Remove(instance.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		if (trainPassSource != null)
		{
			trainPassSource.Stop();
		}
		int i = 0;
		for (int num = anims.Length; i < num; i++)
		{
			if (anims[i].isPlaying)
			{
				anims[i].Stop();
			}
			if (anims[i].enabled)
			{
				anims[i].enabled = false;
			}
		}
		if (childFollow != null)
		{
			childFollow.transform.localPosition = child.localPosition;
		}
		hasStopped = false;
		curSpeed = speed;
	}

	protected override void Update()
	{
		if (startSound)
		{
			if (trainPassSource != null)
			{
				trainPassSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
				trainPassSource.volume = UnityEngine.Random.Range(0.1f, 0.6f);
				trainPassSource.timeSamples = UnityEngine.Random.Range(0, trainPassSource.timeSamples);
				trainPassSource.Play();
			}
			startSound = false;
		}
		if (hasStopped)
		{
			return;
		}
		base.Update();
		if (autoPilot)
		{
			return;
		}
		if (willStop && base.transform.position.z - MovingO.characterController.transform.position.z <= (float)distanceWhenStop)
		{
			hasStopped = true;
		}
		if (startAnimation || !(base.transform.position.z - MovingO.characterController.transform.position.z <= (float)distanceWhenAnimation))
		{
			return;
		}
		startAnimation = true;
		int i = 0;
		for (int num = anims.Length; i < num; i++)
		{
			if (anims[i].gameObject.activeInHierarchy)
			{
				anims[i].enabled = true;
				anims[i].Play();
			}
		}
	}

	private void LateUpdate()
	{
		if (!hasStopped && !(childFollow == null))
		{
			childFollow.transform.localPosition = child.localPosition;
		}
	}
}
