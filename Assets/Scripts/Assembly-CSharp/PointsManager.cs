using System;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
	[SerializeField]
	private PointList list;

	[SerializeField]
	private GameObject model;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private PointAnimationCtrl targetAnim;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Transform thing;

	[SerializeField]
	private CharacterController characterController;

	[SerializeField]
	private Transform trans;

	private bool wait;

	private Transform refrence;

	private float pointToPointThreshold = 2f;

	private Point currentPoint;

	private Vector3 velocity;

	public Action onStop;

	private Vector3 temp;

	public static List<string> hasShowedModels;

	private static bool _isApplicationPaused;

	public PointAnimationCtrl TargetAnim
	{
		get
		{
			return targetAnim;
		}
	}

	public Transform Refrence
	{
		get
		{
			return refrence;
		}
	}

	public bool Wait
	{
		get
		{
			return wait;
		}
		set
		{
			if (value)
			{
				wait = value;
			}
			else if (list.CurrentId == -1)
			{
				wait = value;
				OnStop();
			}
			else if (wait != value)
			{
				wait = value;
				GoToNextPoint();
			}
			else
			{
				list.WaitNext(this);
				SetPointStart();
			}
		}
	}

	private void Awake()
	{
		if (target == null)
		{
			target = new GameObject("Points Target").transform;
		}
		hasShowedModels = new List<string>();
	}

	private void Start()
	{
		if (targetAnim == null)
		{
			targetAnim = GetComponent<PointAnimationCtrl>();
		}
		if (targetAnim != null && targetAnim.anim == null && thing != null)
		{
			targetAnim.anim = Character.Instance.characterModel.characterAnimation;
		}
		_isApplicationPaused = false;
	}

	private void SetPointStart()
	{
		currentPoint = list.CurrentPoint;
		if (currentPoint != null)
		{
			temp = currentPoint.transform.position;
			temp.y = 0f;
			target.position = temp;
			temp -= trans.position;
			temp.y = 0f;
			if (temp == Vector3.zero)
			{
				target.forward = currentPoint.transform.forward;
			}
			else
			{
				target.forward = temp.normalized;
			}
			if (!currentPoint.move)
			{
				trans.position = currentPoint.transform.position;
				trans.rotation = currentPoint.transform.rotation;
			}
			else
			{
				trans.rotation = Quaternion.identity;
			}
			currentPoint.OnStart(this);
		}
	}

	public void Play()
	{
		_isApplicationPaused = false;
		hasShowedModels.Clear();
		if (model != null && thing != null)
		{
			UnityEngine.Object.Destroy(thing.gameObject, 0.1f);
			thing = null;
		}
		if (characterController != null)
		{
			characterController.enabled = true;
		}
		base.enabled = true;
		if (targetAnim != null && targetAnim.anim != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				list[i].OnInit(this);
			}
		}
		list.Play();
		SetPointStart();
	}

	public void BreakLoop()
	{
		if (currentPoint != null)
		{
			currentPoint.OnEnd(this);
		}
		int num = list.OnBreak();
		if (num == -1)
		{
			OnStop();
		}
		else
		{
			SetPointStart();
		}
	}

	public void OnStop()
	{
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			list[i].OnWholeEnd(this);
		}
		if (onStop != null)
		{
			onStop();
		}
		if (base.transform.childCount > 0)
		{
			int j = 0;
			for (int childCount = base.transform.childCount; j < childCount; j++)
			{
				Transform child = base.transform.GetChild(j);
				if (child != null)
				{
					UnityEngine.Object.Destroy(child.gameObject, 0.1f);
				}
			}
			thing = null;
			if (targetAnim != null)
			{
				targetAnim.anim = null;
			}
			refrence = null;
		}
		if (thing == null && characterController != null)
		{
			characterController.enabled = false;
		}
		currentPoint = null;
		base.enabled = false;
	}

	public void GoToNextPoint()
	{
		currentPoint.OnEnd(this);
		if (list.Next() == -1)
		{
			OnStop();
		}
		else
		{
			SetPointStart();
		}
	}

	private void Update()
	{
		if (_isApplicationPaused || !(currentPoint != null))
		{
			return;
		}
		if (currentPoint.OnUpdate(this))
		{
			if (characterController != null && characterController.enabled)
			{
				velocity.y = -10f * Time.deltaTime;
				characterController.Move(velocity);
			}
			else
			{
				velocity.y = 0f;
				trans.position += velocity;
			}
		}
		velocity = Vector3.zero;
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			_isApplicationPaused = true;
			return;
		}
		_isApplicationPaused = false;
		GC.Collect();
	}

	public void Check()
	{
		temp = target.position - trans.position;
		temp.y = 0f;
		if (temp.magnitude < pointToPointThreshold || Vector3.Dot(temp, target.forward) < 0f)
		{
			GoToNextPoint();
		}
	}

	public void InitializeCharacterModel(Transform point, string name, int themeId)
	{
		if (!(thing != null))
		{
			CharacterModelSample characterModelSample = CharacterModelSampleFactory.Instance.BuildCharacterModelSample(name, themeId);
			thing = characterModelSample.transform;
			thing.parent = base.transform;
			targetAnim.anim = thing.GetComponentInChildren<Animation>();
			thing.localPosition = Vector3.zero;
			thing.localRotation = Quaternion.identity;
			refrence = characterModelSample.BoneHead;
			hasShowedModels.Add(name);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				list[i].OnInit(this);
			}
		}
	}

	public void InitializedModel(Vector3 localPos, Vector3 localRotation)
	{
		thing = UnityEngine.Object.Instantiate(model).transform;
		thing.parent = base.transform;
		thing.localPosition = localPos;
		thing.localRotation = Quaternion.Euler(localRotation);
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			list[i].OnInit(this);
		}
	}

	public void PlaySound(AudioClipInfo clip, bool loop)
	{
		if (!(audioSource == null) && clip != null && !(clip.Clip == null))
		{
			audioSource.clip = clip.Clip;
			audioSource.loop = loop;
			audioSource.volume = UnityEngine.Random.Range(clip.minVolume, clip.maxVolume);
			audioSource.pitch = UnityEngine.Random.Range(clip.minPitch, clip.maxPitch);
			audioSource.rolloffMode = clip.Rollof;
			audioSource.Play();
		}
	}

	public void StopSound()
	{
		if (!(audioSource == null))
		{
			audioSource.Stop();
		}
	}

	public float GetDistanceToTarget()
	{
		return Vector3.Distance(target.position, trans.position);
	}

	public void SetTransformToTarget()
	{
		SetTranformPQ(trans, target);
	}

	public void SetTransformTo(Transform trans)
	{
		SetTranformPQ(this.trans, trans);
	}

	private void SetTranformPQ(Transform trans, Transform target)
	{
		trans.position = target.position;
		trans.rotation = target.rotation;
	}

	public void SetPositionAccordingRefrence(Vector3 pos)
	{
		trans.position = refrence.localPosition + pos;
		refrence.rotation = trans.rotation;
	}

	public void Move(float delta)
	{
		velocity = target.forward * delta;
	}

	public void RotatoToTarget()
	{
		trans.rotation = Quaternion.Lerp(trans.rotation, target.rotation, Time.deltaTime * 10f);
	}

	public void RotatoToPoint()
	{
		trans.rotation = Quaternion.Lerp(trans.rotation, currentPoint.transform.rotation, Time.deltaTime * 10f);
	}
}
