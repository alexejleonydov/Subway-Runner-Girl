using System;
using UnityEngine;

public class SpeedStripes : MonoBehaviour
{
	[SerializeField]
	private float _fadeInTime = 0.5f;

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Material _OriginalMaterial;

	private Material _material;

	private Color _originalColor;

	private void Activate()
	{
		Activate(_fadeInTime);
	}

	private void Activate(float time)
	{
		_renderer.enabled = true;
		_animator.gameObject.SetActive(true);
		StartCoroutine(myTween.To(time, delegate(float t)
		{
			_material.SetColor(Shaders.Instance.MainColor, Color.Lerp(Color.black, _originalColor, t));
		}));
	}

	private void Start()
	{
		Flypack instance = Flypack.Instance;
		instance.OnDeactivateTurboHeadstart += OnStopTuboHeadstart;
		instance.OnStart = (Flypack.OnStartDelegate)Delegate.Combine(instance.OnStart, new Flypack.OnStartDelegate(OnStartFlypack));
		instance.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(instance.OnStop, new Flypack.OnStopDelegate(OnStopFlypack));
		Game instance2 = Game.Instance;
		instance2.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(instance2.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(Reset));
		_material = new Material(_OriginalMaterial);
		_originalColor = _material.GetColor(Shaders.Instance.TintColor);
		_material.SetColor(Shaders.Instance.MainColor, Color.black);
		_renderer.material = _material;
		_renderer.enabled = false;
		_animator.gameObject.SetActive(false);
	}

	private void Deactivate()
	{
		Deactivate(_fadeInTime);
	}

	private void Deactivate(float time)
	{
		_material.SetColor(Shaders.Instance.MainColor, Color.black);
		_renderer.enabled = false;
		_animator.gameObject.SetActive(false);
	}

	private void OnStartFlypack(bool isHeadstart)
	{
		if (!isHeadstart)
		{
			Deactivate(_fadeInTime);
		}
		else
		{
			Activate(_fadeInTime);
		}
	}

	private void OnStopFlypack()
	{
		Deactivate(_fadeInTime);
	}

	private void OnStopTuboHeadstart()
	{
		if (!Helmet.Instance.IsActive)
		{
			Deactivate(_fadeInTime);
		}
	}

	private void Reset()
	{
		StopAllCoroutines();
		_material.SetColor(Shaders.Instance.MainColor, Color.black);
		_renderer.enabled = false;
		_animator.gameObject.SetActive(false);
	}
}
