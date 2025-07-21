using System.Collections;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public delegate void OnReviveDelegate();

	public delegate void OnSwitchToRunningDelegate();

	public float WaitForParticlesDelay = 0.144f;

	public float RemoveObstaclesDistance = 250f;

	[OptionalField]
	[SerializeField]
	private ParticleSystem reviveParticle;

	private Character character;

	private Game game;

	private static Revive instance;

	private TrackController trackController;

	public static Revive Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Utils.FindObject<Revive>();
			}
			return instance;
		}
	}

	public event OnReviveDelegate OnRevive;

	public event OnSwitchToRunningDelegate OnSwitchToRunning;

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		trackController = TrackController.Instance;
	}

	private IEnumerator ReviveNow()
	{
		reviveParticle.gameObject.SetActive(true);
		reviveParticle.Play();
		AudioPlayer.Instance.PlaySound("leyou_deat combo", 0.5f, 0.5f, 1.5f);
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		trackController.LayEmptyPieces(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		if (this.OnRevive != null)
		{
			this.OnRevive();
		}
		game.Revive();
		if (this.OnSwitchToRunning != null)
		{
			this.OnSwitchToRunning();
		}
		yield return null;
		character.IsJumping = true;
		character.IsFalling = false;
		character.verticalSpeed = character.CalculateJumpVerticalSpeed((Game.Instance.HitType != Character.CriticalHitType.FallIntoWater) ? 15f : 0f);
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("IngameUI") as IngameScreen).CountDown();
		}
		reviveParticle.gameObject.SetActive(false);
	}

	public void SendRevive()
	{
		StartCoroutine(ReviveNow());
	}

	public void SendSkipRevive()
	{
		StartCoroutine(game.SkipRevive());
	}
}
