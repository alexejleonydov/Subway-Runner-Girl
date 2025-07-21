using UnityEngine;

public class SetParticleZVelo : MonoBehaviour
{
	private Game _game;

	private ParticleSystem _particleSystem;

	private ParticleSystem.MainModule mainModule;

	[SerializeField]
	private float _speedMultiplier = 1f;

	private void Awake()
	{
		_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		if (_particleSystem == null)
		{
			base.enabled = false;
			Object.Destroy(this);
		}
		else
		{
			mainModule = _particleSystem.main;
			_game = Game.Instance;
		}
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
		mainModule.startSpeed = _game.currentSpeed * _speedMultiplier;
	}
}
