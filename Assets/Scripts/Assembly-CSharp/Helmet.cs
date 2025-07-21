using System.Collections;
using UnityEngine;

public class Helmet : ICharacterAttachment
{
	public delegate void OnStartHelmetDelegate();

	public delegate void OnEndHelmetDelegate();

	public delegate void OnHardResetDelegate();

	public delegate void OnHelmetJumpDelegate();

	public delegate void OnJumpFromWaterDelegate();

	public delegate void OnRunDelegate();

	public delegate void OnSpeedStartDelegate();

	public delegate void OnSwitchToHelmetDelegate(GameObject helmet);

	public float cooldownDistance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	public float WaitForParticlesDelay = 0.5f;

	public float RemoveObstaclesDistance = 250f;

	public float pullSpeed = 200f;

	private bool isInCooldown;

	public bool isAllowed = true;

	private ActiveProp Powerup;

	private Game game;

	private Character character;

	private CharacterModel characterModel;

	private Animation characterAnimation;

	private CharacterController characterController;

	private OnTriggerObject coinMagnetCollider;

	private Transform coinEFX;

	private GameObject helmetRoot;

	private static Helmet instance;

	private TrackController trackController;

	private GameObject helm;

	private bool useMagnet;

	private bool useMutiplier;

	public static Helmet Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Helmet();
			}
			return instance;
		}
	}

	public bool ShouldPauseInFlypack
	{
		get
		{
			return true;
		}
	}

	public bool UseMagent
	{
		get
		{
			return useMagnet;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	public event OnStartHelmetDelegate OnStartHelmet;

	public event OnEndHelmetDelegate OnEndHelmet;

	public event OnHardResetDelegate OnHardReset;

	public event OnHelmetJumpDelegate OnJump;

	public event OnJumpFromWaterDelegate OnJumpFromWater;

	public event OnRunDelegate OnRun;

	public event OnSwitchToHelmetDelegate OnSwitchToHelmet;

	public Helmet()
	{
		character = Character.Instance;
		characterModel = character.characterModel;
		characterController = character.characterController;
		characterAnimation = CharacterRendering.Instance.characterAnimation;
		coinMagnetCollider = character.coinMagnetCollider;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		helmetRoot = characterModel.BoneHelmet.gameObject;
		trackController = TrackController.Instance;
		game = Game.Instance;
		cooldownDistance = HelmetModelPreviewFactory.Instance.cooldownDistance;
		slowMotionDistance = HelmetModelPreviewFactory.Instance.slowMotionDistance;
		slowDownToScale = HelmetModelPreviewFactory.Instance.slowDownToScale;
		WaitForParticlesDelay = HelmetModelPreviewFactory.Instance.WaitForParticlesDelay;
		RemoveObstaclesDistance = HelmetModelPreviewFactory.Instance.RemoveObstaclesDistance;
	}

	private void _OnStartHelmet()
	{
		if (this.OnStartHelmet != null)
		{
			this.OnStartHelmet();
		}
	}

	private void _OnEndHelmet()
	{
		if (this.OnEndHelmet != null)
		{
			this.OnEndHelmet();
		}
	}

	private void _OnHardReset()
	{
		if (this.OnHardReset != null)
		{
			this.OnHardReset();
		}
	}

	private void Prepare()
	{
		PlayerInfo.Instance.UseUpgrade(PropType.helmet);
		TasksManager.Instance.PlayerDidThis(TaskTarget.Helmet);
		Powerup = GameStats.Instance.RegisterPowerup(PropType.helmet);
		useMagnet = (useMutiplier = false);
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		IsActive = true;
		helmetRoot.SetActive(true);
		helm = characterModel.AddHelmetModel();
		if (this.OnSwitchToHelmet != null)
		{
			this.OnSwitchToHelmet(helm);
		}
		_OnStartHelmet();
		character.CharacterPickupParticleSystem.PickedupPowerUp();
		character.immuneToCriticalHit = true;
		SetupAbility();
		Stop = StopFlag.DONT_STOP;
	}

	private void End()
	{
		IsActive = false;
		character.immuneToCriticalHit = false;
		isInCooldown = true;
		if (useMagnet)
		{
			useMagnet = false;
			if (!CoinMagnet.Instance.IsActive)
			{
				coinMagnetCollider.GetComponent<Collider>().enabled = false;
				characterModel.meshCoinMagnet.enabled = false;
				coinEFX.localPosition = PickupParticles.coinEfxOffset;
				characterAnimation["hold_magnet"].enabled = false;
			}
		}
		if (useMutiplier)
		{
			useMutiplier = false;
			GameStats.Instance.scoreBooster10Activated = false;
		}
		_OnEndHelmet();
	}

	private void DontStopUntil()
	{
		if (Stop != 0)
		{
			return;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.HelmetExpire);
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
		if (character.IsFalling || character.IsJumping)
		{
			if (Game.Instance.HitType == Character.CriticalHitType.FallIntoWater)
			{
				if (this.OnJumpFromWater != null)
				{
					this.OnJumpFromWater();
				}
			}
			else if (this.OnJump != null)
			{
				this.OnJump();
			}
		}
		else if (this.OnRun != null)
		{
			this.OnRun();
		}
		Object.Destroy(helm);
	}

	private void IfExplose()
	{
		character.helmetCrashParticleSystem.gameObject.SetActive(true);
		character.helmetCrashParticleSystem.Play();
		AudioPlayer.Instance.PlaySound("leyou_Hr_H_crash", true);
		if (Game.Instance.HitType == Character.CriticalHitType.FallIntoWater)
		{
			if (this.OnJumpFromWater != null)
			{
				this.OnJumpFromWater();
			}
		}
		else if (this.OnJump != null)
		{
			this.OnJump();
		}
		Object.Destroy(helm);
	}

	public IEnumerator Begain()
	{
		if (!isAllowed || isInCooldown)
		{
			yield break;
		}
		Prepare();
		while (Powerup.timeLeft > 0f && Stop == StopFlag.DONT_STOP)
		{
			if (useMagnet)
			{
				coinEFX.position = characterModel.meshCoinMagnet.transform.position;
			}
			yield return null;
		}
		End();
		DontStopUntil();
		if (Stop != StopFlag.STOP)
		{
			Paused = false;
			IsActive = false;
			isAllowed = true;
			_OnHardReset();
			yield break;
		}
		IfExplose();
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		trackController.LayEmptyPieces(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		if (!character.IsJumping)
		{
			character.IsJumping = true;
			character.inAirJump = false;
			character.IsFalling = false;
			character.verticalSpeed = character.CalculateJumpVerticalSpeed(10f);
		}
		float newSlowMotionDistance = slowMotionDistance * Game.Instance.NormalizedGameSpeed;
		float newCoolDownDist = cooldownDistance * Game.Instance.NormalizedGameSpeed;
		float distanceLeft = newSlowMotionDistance;
		bool didStopCooldown = false;
		while (distanceLeft > 0f)
		{
			distanceLeft -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			newCoolDownDist -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			if (newCoolDownDist < 0f && !didStopCooldown)
			{
				didStopCooldown = true;
			}
			yield return null;
		}
		character.helmetCrashParticleSystem.gameObject.SetActive(false);
	}

	public void HardReset()
	{
		if (useMagnet)
		{
			useMagnet = false;
			coinMagnetCollider.GetComponent<Collider>().enabled = false;
			characterModel.meshCoinMagnet.enabled = false;
			coinEFX.localPosition = PickupParticles.coinEfxOffset;
			characterAnimation["hold_magnet"].enabled = false;
		}
		if (useMutiplier)
		{
			useMutiplier = false;
			GameStats.Instance.scoreBooster10Activated = false;
		}
		_OnHardReset();
		Paused = false;
		IsActive = false;
		isInCooldown = false;
		isAllowed = true;
	}

	public void Pause()
	{
		Paused = true;
		helmetRoot.SetActive(false);
	}

	public void Reset()
	{
		character.immuneToCriticalHit = true;
		character.characterController.enabled = true;
		character.characterCollider.enabled = true;
		helmetRoot.SetActive(true);
		Time.timeScale = 1f;
		character.helmetCrashParticleSystem.gameObject.SetActive(false);
	}

	public void Resume()
	{
		Paused = false;
		helmetRoot.SetActive(true);
	}

	public void SetupAbility()
	{
		Helmets.Helm helm = Helmets.helmData[PlayerInfo.Instance.currentHelmet];
		if (helm.useMagent)
		{
			useMagnet = true;
			if (!CoinMagnet.Instance.IsActive)
			{
				AudioPlayer.Instance.PlaySound("bmx_magnet_on", true);
				characterModel.meshCoinMagnet.enabled = true;
				characterAnimation["hold_magnet"].enabled = true;
				characterAnimation.Play("hold_magnet");
				coinMagnetCollider.GetComponent<Collider>().enabled = true;
				coinMagnetCollider.OnEnter = CoinTriggerHit;
			}
		}
		if (helm.useMutiplier)
		{
			useMutiplier = true;
			GameStats.Instance.scoreBooster10Activated = true;
		}
	}

	public void CoinTriggerHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		Glow componentInChildren = collider.GetComponentInChildren<Glow>();
		if (component != null)
		{
			float num = 70f;
			if ((character.transform.position.y >= num && component.transform.position.y >= num) || (character.transform.position.y < num && character.transform.position.y >= 0f && component.transform.position.y < num && component.transform.position.y >= 0f) || (character.transform.position.y < 0f && component.transform.position.y < 0f))
			{
				component.GetComponent<Collider>().enabled = false;
				CoroutineC.Instance.StartCoroutineC(Pull(component, componentInChildren));
			}
		}
	}

	private IEnumerator Pull(Coin coin, Glow glow)
	{
		Vector3 coinPosition = coin.transform.position;
		Vector3 vector = coinPosition - characterController.transform.position;
		if (glow == null)
		{
			yield return CoroutineC.Instance.StartCoroutineC(myTween.To(vector.magnitude / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
			{
				coin.transform.position = Vector3.Lerp(coinPosition, characterModel.meshCoinMagnet.transform.position, t * t);
			}));
		}
		else
		{
			Vector3 glowPosition = glow.transform.position;
			yield return CoroutineC.Instance.StartCoroutineC(myTween.To(vector.magnitude / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
			{
				coin.transform.position = Vector3.Lerp(coinPosition, characterModel.meshCoinMagnet.transform.position, t * t);
				glow.transform.position = Vector3.Lerp(glowPosition, characterModel.meshCoinMagnet.transform.position, t * t);
			}));
		}
		IPickup pickup = coin.GetComponent<IPickup>();
		character.NotifyPickup(pickup);
		GameStats.Instance.coinsCoinMagnet++;
	}
}
