using System.Collections;
using UnityEngine;

public class CoinMagnet : ICharacterAttachment
{
	public float pullSpeed = 200f;

	private ActiveProp Powerup;

	private Character character;

	private Animation characterAnimation;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private Transform coinEFX;

	private OnTriggerObject coinMagnetCollider;

	private Game game;

	private static CoinMagnet instance;

	public static CoinMagnet Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CoinMagnet();
			}
			return instance;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public bool ShouldPauseInFlypack
	{
		get
		{
			return false;
		}
	}

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	public CoinMagnet()
	{
		character = Character.Instance;
		characterController = character.characterController;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		characterAnimation = characterRendering.characterAnimation;
		coinMagnetCollider = character.coinMagnetCollider;
		characterAnimation["hold_magnet"].AddMixingTransform(characterRendering.CharacterModel.shoulderTransform);
		characterAnimation["hold_magnet"].layer = 3;
		characterAnimation["hold_magnet"].weight = 0.9f;
		characterAnimation["hold_magnet"].enabled = false;
		game = Game.Instance;
		pullSpeed = HelmetModelPreviewFactory.Instance.pullSpeed;
	}

	public IEnumerator Begain()
	{
		Before();
		while (Powerup.timeLeft > 0f && Stop == StopFlag.DONT_STOP)
		{
			coinEFX.position = characterModel.meshCoinMagnet.transform.position;
			yield return null;
		}
		After();
	}

	private void Before()
	{
		GameStats.Instance.pickedUpPowerups++;
		Powerup = GameStats.Instance.RegisterPowerup(PropType.coinmagnet);
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		AudioPlayer.Instance.PlaySound("bmx_magnet_on", true);
		if (!Helmet.Instance.IsActive || !Helmet.Instance.UseMagent)
		{
			characterModel.meshCoinMagnet.enabled = true;
			characterAnimation["hold_magnet"].enabled = true;
			characterAnimation.Play("hold_magnet");
			coinMagnetCollider.OnEnter = CoinTriggerHit;
			coinMagnetCollider.GetComponent<Collider>().enabled = true;
		}
		IsActive = true;
		Stop = StopFlag.DONT_STOP;
	}

	private void After()
	{
		if (!Helmet.Instance.IsActive || !Helmet.Instance.UseMagent)
		{
			coinMagnetCollider.GetComponent<Collider>().enabled = false;
			characterModel.meshCoinMagnet.enabled = false;
			coinEFX.localPosition = PickupParticles.coinEfxOffset;
			characterAnimation["hold_magnet"].enabled = false;
			if (Powerup.timeLeft <= 0f)
			{
				AudioPlayer.Instance.PlaySound("bmx_magnet_off", true);
			}
		}
		IsActive = false;
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

	public void Reset()
	{
		Paused = false;
	}

	public void Pause()
	{
		Paused = true;
	}

	public void Resume()
	{
		Paused = false;
	}
}
