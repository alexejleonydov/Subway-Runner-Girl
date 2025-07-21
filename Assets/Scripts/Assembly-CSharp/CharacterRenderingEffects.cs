using System;
using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	[SerializeField]
	private GameObject flypackParticles;

	[SerializeField]
	private ParticleFollow flypackParticleStart;

	[SerializeField]
	private ParticleFollow flypackParticleIng;

	[SerializeField]
	private ParticleFollow flypackParticleEnd;

	[SerializeField]
	private GameObject speedup;

	[SerializeField]
	private ParticleFollow speedupFollow;

	[SerializeField]
	private ParticleSystem jumpRoolPs;

	[SerializeField]
	private ParticleFollow jumpRollFollow;

	[SerializeField]
	private bool _showSnow;

	[SerializeField]
	private ParticleSystem _snow;

	[SerializeField]
	private ParticleSystem _snowLow;

	[SerializeField]
	private bool _showStars;

	[SerializeField]
	private Transform _stars;

	[SerializeField]
	private float _zOffset;

	private Character _character;

	private ParticleSystem _currentSnow;

	private ParticleSystem.EmissionModule emissionModule;

	private Game _game;

	private float _originalEmissionRate;

	private bool _snowActive;

	private Vector3 _snowOriginalPosition;

	private Transform _snowTransform;

	private Vector3 _starsOriginalPosition;

	public GameObject FlypackParticles
	{
		get
		{
			return flypackParticles;
		}
	}

	public void ActivateSnow()
	{
		if (!_snowActive)
		{
			_currentSnow.Play();
			_snowActive = true;
		}
	}

	private void Awake()
	{
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && _showSnow)
		{
			_snow.gameObject.SetActive(false);
			_currentSnow = _snowLow;
		}
		else if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.High && _showSnow)
		{
			_snowLow.gameObject.SetActive(false);
			_currentSnow = _snow;
		}
		else
		{
			_snow.gameObject.SetActive(false);
			_snowLow.gameObject.SetActive(false);
			_currentSnow = _snowLow;
		}
		emissionModule = _currentSnow.emission;
		_originalEmissionRate = emissionModule.rateOverTimeMultiplier;
		if (_showStars)
		{
			_stars.gameObject.SetActive(true);
			_starsOriginalPosition = _stars.position;
		}
		else
		{
			_stars.gameObject.SetActive(false);
		}
	}

	public void DeactivateSnow()
	{
		if (_snowActive)
		{
			_snowActive = false;
			_currentSnow.Stop();
		}
	}

	public void Initialize(CharacterModel characterModel)
	{
		_game = Game.Instance;
		_game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(_game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(DeactivateSnow));
		_game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(_game.OnIntroRun, new Game.OnIntroRunDelegate(ActivateSnow));
		_character = Character.Instance;
		flypackParticleStart.Target = characterModel.flypackCloudPosition;
		flypackParticleIng.Target = characterModel.flypackCloudPosition;
		flypackParticleEnd.Target = characterModel.flypackCloudPosition;
		speedupFollow.Target = characterModel.transform;
		jumpRollFollow.Target = characterModel.BoneFoot;
		SetupSnow(characterModel.transform);
	}

	public void SetStartParticlesActive()
	{
		flypackParticleStart.gameObject.SetActive(true);
		flypackParticleStart.enabled = true;
		flypackParticleIng.gameObject.SetActive(false);
		flypackParticleEnd.gameObject.SetActive(false);
	}

	public void SetIngParticlesActive()
	{
		flypackParticleIng.gameObject.SetActive(true);
		flypackParticleIng.enabled = true;
		flypackParticleStart.gameObject.SetActive(false);
		flypackParticleEnd.gameObject.SetActive(false);
	}

	public void SetEndParticlesActive()
	{
		flypackParticleEnd.gameObject.SetActive(true);
		flypackParticleEnd.enabled = true;
		flypackParticleStart.gameObject.SetActive(false);
		flypackParticleIng.gameObject.SetActive(false);
	}

	public void SetSpeedupActive()
	{
		speedup.SetActive(true);
		speedupFollow.gameObject.SetActive(true);
		speedupFollow.enabled = true;
	}

	public void SetSpeedupDeactive()
	{
		speedupFollow.gameObject.SetActive(false);
		speedup.SetActive(false);
		speedupFollow.enabled = false;
	}

	public void SetJumpRool()
	{
		jumpRoolPs.gameObject.SetActive(true);
		jumpRoolPs.Play();
		jumpRollFollow.enabled = true;
	}

	private void SetupSnow(Transform root)
	{
		_snowTransform = _currentSnow.gameObject.transform;
		_snowOriginalPosition = _snowTransform.position;
	}

	private void Update()
	{
		if (_snowActive)
		{
			_snowTransform.position = new Vector3(_snowOriginalPosition.x, _snowOriginalPosition.y, _character.z + _zOffset);
			if (_character.IsInsideSubway && emissionModule.rateOverTimeMultiplier != 0f)
			{
				emissionModule.rateOverTimeMultiplier = 0f;
			}
			else if (!_character.IsInsideSubway && emissionModule.rateOverTimeMultiplier != _originalEmissionRate)
			{
				emissionModule.rateOverTimeMultiplier = _originalEmissionRate;
			}
		}
		if (_showStars)
		{
			_stars.position = new Vector3(_starsOriginalPosition.x, _starsOriginalPosition.y, _character.z + _starsOriginalPosition.z);
		}
	}
}
