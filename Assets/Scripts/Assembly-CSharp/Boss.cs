using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[Serializable]
	public class CatchAnimationSet
	{
		public AnimationClip death;

		public AnimationClip avatar;

		public AnimationClip guard;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	[Serializable]
	public class DefaultAnimations
	{
		public AnimationClip[] introGuard;

		public AnimationClip[] runGuard;

		public AnimationClip[] jumpGuard;

		public AnimationClip[] dodgeLeftGuard;

		public AnimationClip[] dodgeRigthGuard;

		public AnimationClip[] rollGuard;

		public AnimationClip[] catchupGuard;
	}

	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);

	public DefaultAnimations defaultAnimations;

	public float distanceToCharacterMin = 10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Animation guardAnimation;

	public CatchAnimationSet[] caughtSets;

	public int debugCatchAnimationToPlay = -1;

	public Transform[] enemies;

	public OnCatchPlayerDelegate OnCatchPlayer;

	public float guardProximityLoopVolume = 0.9f;

	private AudioSource audioSource;

	private bool caught;

	private Character character;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private Vector3[] enemiesStartPos;

	private Renderer[] enemyRenderers;

	private Game game;

	private static Boss instance;

	private bool isPaused = true;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private float verticalSpeed;

	private SmoothDampFloat x;

	private float y;

	public static Boss Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Boss)) as Boss;
			}
			return instance;
		}
	}

	public void CatchPlayer(Animation characterAnimation)
	{
		if (game.IsInFlypackMode || game.IsInSpringJumpMode || game.IsInBoundJumpMode)
		{
			return;
		}
		int num = debugCatchAnimationToPlay;
		audioSource.Stop();
		StopAllCoroutines();
		caught = true;
		List<int> list = new List<int>(caughtSets.Length);
		if (debugCatchAnimationToPlay < 0 || debugCatchAnimationToPlay >= caughtSets.Length)
		{
			int i = 0;
			for (int num2 = caughtSets.Length; i < num2; i++)
			{
				if (characterAnimation[caughtSets[i].death.name].enabled)
				{
					list.Add(i);
				}
			}
		}
		if (list.Count <= 0)
		{
			ShowEnemies(false);
			return;
		}
		num = UnityEngine.Random.Range(0, list.Count);
		guardAnimation.CrossFade(caughtSets[num].guard.name, 0.2f);
		float num3 = caughtSets[num].catchAvatarAnimationPlayOffset / 25f;
		if (OnCatchPlayer != null)
		{
			OnCatchPlayer(caughtSets[num].avatar.name, num3, caughtSets[num].waitTimeBeforeScreen);
		}
		StartCoroutine(myTween.To(num3, delegate(float t)
		{
			for (int j = 0; j < enemies.Length; j++)
			{
				Vector3 position = character.transform.position;
				enemies[j].position = Vector3.Lerp(enemies[j].position, position, t);
			}
		}));
	}

	public void CatchUp()
	{
		CatchUp(catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			ShowEnemies(true);
			StopAllCoroutines();
			guardAnimation.Play(ReturnRandomAnimations(defaultAnimations.catchupGuard));
			guardAnimation.PlayQueued(ReturnRandomAnimations(defaultAnimations.runGuard));
			audioSource.timeSamples = UnityEngine.Random.Range(0, audioSource.timeSamples);
			audioSource.Play();
			audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(myTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(myTween.To(duration, delegate(float t)
			{
				audioSource.volume = Mathf.SmoothStep(0f, guardProximityLoopVolume, t);
			}));
			closeToCharacter = true;
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
			}
			isPaused = true;
		}
		else
		{
			if (isPaused)
			{
				audioSource.Play();
			}
			isPaused = false;
		}
	}

	public void HitByTrainSequence()
	{
		audioSource.Stop();
		StartCoroutine(HitByTrainSequenceCoroutine());
	}

	public void FallIntoWater()
	{
		audioSource.Stop();
		StartCoroutine(FallIntoWaterCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		float catchUpTime = 0.2f;
		yield return StartCoroutine(myTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
		yield return new WaitForSeconds(0.4f);
		guardAnimation.Play("Guard_death_moving");
	}

	public IEnumerator FallIntoWaterCoroutine()
	{
		GameStats.Instance.guardFallWater++;
		float catchUpTime = 0.5f;
		Vector3 endPos = characterTransform.position - Vector3.forward * 5f;
		yield return StartCoroutine(myTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, endPos, t);
			}
		}));
		guardAnimation.Play("FallIntoWater");
	}

	public void Initialize()
	{
		game = Game.Instance;
		characterController = Game.Charactercontroller;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		enemiesStartPos = new Vector3[enemies.Length];
		audioSource = GetComponent<AudioSource>();
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].localPosition;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		audioSource.volume = guardProximityLoopVolume;
		game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(game.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		int j = 0;
		for (int num = caughtSets.Length; j < num; j++)
		{
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, caughtSets[j].avatar);
			SetupGuardAnimationsStates(guardAnimation, caughtSets[j].guard);
		}
		List<AnimationClip> list = new List<AnimationClip>();
		foreach (AnimationState item in guardAnimation)
		{
			list.Add(item.clip);
		}
		InitializeClips(guardAnimation, list, defaultAnimations.jumpGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.runGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.introGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeRigthGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeLeftGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.rollGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.catchupGuard);
	}

	private void InitializeClips(Animation animationComponent, List<AnimationClip> addedClips, AnimationClip[] clips)
	{
		foreach (AnimationClip animationClip in clips)
		{
			if (!addedClips.Contains(animationClip))
			{
				addedClips.Add(animationClip);
				animationComponent.AddClip(animationClip, animationClip.name);
			}
		}
	}

	public void Jump(float delay)
	{
		if (distanceToCharacter <= distanceToCharacterMin)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.GuardJump);
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private void JumpAnimation()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.jumpGuard);
		guardAnimation.Play(animation);
		string animation2 = ReturnRandomAnimations(defaultAnimations.runGuard);
		guardAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		JumpAnimation();
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 0.7f;
	}

	public void LateUpdate()
	{
		x.Target = characterTransform.position.x;
		x.Update();
		if (character.isStairing)
		{
			y = base.transform.position.y;
			lastGroundedSmooth = character.running.GetStairYAtZ(character.z - distanceToCharacter);
			if (y > lastGroundedSmooth)
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			y += verticalSpeed * Time.deltaTime;
			y = Mathf.Max(y, lastGroundedSmooth);
		}
		else
		{
			lastGroundedSmooth = Mathf.SmoothDamp(lastGroundedSmooth, character.lastGroundedY, ref lastGroundedVelocity, lastGroundedSmoothTime);
			if (y > lastGroundedSmooth)
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			if (float.IsNaN(lastGroundedSmooth))
			{
				lastGroundedSmooth = 0f;
			}
			y += verticalSpeed * Time.deltaTime;
			y = Mathf.Max(y, lastGroundedSmooth);
		}
		Vector3 position = characterTransform.position - Vector3.forward * distanceToCharacter;
		position.y = y;
		position.x = x.Value;
		base.transform.position = position;
	}

	public void MuteProximityLoop()
	{
		audioSource.Stop();
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (!caught)
		{
			if (characterController.isGrounded)
			{
				string animation = ((direction == Character.OnChangeTrackDirection.Left) ? ReturnRandomAnimations(defaultAnimations.dodgeLeftGuard) : ReturnRandomAnimations(defaultAnimations.dodgeRigthGuard));
				guardAnimation[animation].speed = game.NormalizedGameSpeed;
				guardAnimation.CrossFade(animation, 0.1f);
			}
			if (!character.IsJumping)
			{
				string animation2 = ReturnRandomAnimations(defaultAnimations.runGuard);
				guardAnimation.CrossFadeQueued(animation2, 0.1f);
			}
		}
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRollGuard -= OnRoll;
		character.OnRoll -= OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
	}

	public void OnEnable()
	{
		lastGroundedSmooth = character.lastGroundedY;
		lastGroundedVelocity = 0f;
		y = character.lastGroundedY;
		x.Value = character.transform.position.x;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
		character.OnJump += OnJump;
		character.OnRollGuard += OnRoll;
		character.OnRoll += OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
	}

	private void OnJump()
	{
		Jump(distanceToCharacter / game.currentSpeed);
	}

	private void OnRoll()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.rollGuard);
		guardAnimation[animation].time = 0f;
		guardAnimation[animation].speed = 1f;
		guardAnimation.CrossFade(animation, 0.1f);
		string animation2 = ReturnRandomAnimations(defaultAnimations.runGuard);
		guardAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private void OnRollNoAnimation()
	{
		StartCoroutine(RollCoroutine(distanceToCharacter / game.currentSpeed));
	}

	public void PlayIntro()
	{
		game.topMenu.AddEnemyOnStopEvent(Run);
	}

	public void Run()
	{
		closeToCharacter = false;
		CatchUp();
		game.topMenu.RemoveEnemyOnStopEvent(Run);
	}

	public void ResetCatchUp()
	{
		ResetCatchUp(resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		StartCoroutine(ResetCatchUpCoroutine(duration));
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			closeToCharacter = false;
			StartCoroutine(myTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMax, t);
			}));
			yield return StartCoroutine(myTween.To(duration * 2f, delegate(float t)
			{
				audioSource.volume = Mathf.SmoothStep(guardProximityLoopVolume, 0f, t);
			}));
			audioSource.Stop();
			if (!game.isDead)
			{
				ShowEnemies(false);
			}
		}
	}

	public void ResetModelRootPosition()
	{
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].localPosition = enemiesStartPos[i];
			enemies[i].localRotation = Quaternion.identity;
		}
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = (closeToCharacter ? distanceToCharacterMin : distanceToCharacterMax);
	}

	private string ReturnRandomAnimations(AnimationClip[] guardAnimations)
	{
		string empty = string.Empty;
		if (guardAnimations != null)
		{
			empty = guardAnimations[UnityEngine.Random.Range(0, guardAnimations.Length)].name;
		}
		return empty;
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		verticalSpeed = 0f - character.CalculateJumpVerticalSpeed();
	}

	private void SetupAvatarAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		if (animation.GetClip(animationClip.name) == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
		animation[animationClip.name].enabled = false;
		animation[animationClip.name].layer = 4;
	}

	private void SetupGuardAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		if (animation.GetClip(animationClip.name) == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
	}

	public void ShowEnemies(bool vis)
	{
		isShowing = vis;
		caught = false;
		int i = 0;
		for (int num = enemyRenderers.Length; i < num; i++)
		{
			enemyRenderers[i].gameObject.SetActive(vis);
		}
	}
}
