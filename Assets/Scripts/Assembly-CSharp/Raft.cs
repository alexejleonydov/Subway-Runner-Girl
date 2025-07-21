using System.Collections;
using UnityEngine;

public class Raft : ICharacterAttachment
{
	public delegate void OnEndRaftDelegate();

	public delegate void OnSwitchToRoftDelegate(GameObject roft);

	private static Raft instance;

	public bool isAllowed = true;

	public GameObject raft;

	private Character character;

	private CharacterModel characterModel;

	private GameObject roftRoot;

	public static Raft Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Raft();
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

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	public event OnEndRaftDelegate _OnEndRaft;

	public event OnSwitchToRoftDelegate OnSwitchToRoft;

	public Raft()
	{
		character = Character.Instance;
		characterModel = character.characterModel;
		roftRoot = characterModel.BoneHelmet.gameObject;
	}

	private void OnEndRaft()
	{
		if (this._OnEndRaft != null)
		{
			this._OnEndRaft();
		}
	}

	public IEnumerator Begain()
	{
		if (isAllowed)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.Raft);
			Paused = false;
			if (character.IsStumbling)
			{
				character.StopStumble();
			}
			IsActive = true;
			roftRoot.SetActive(true);
			characterModel.SetRaft(raft);
			if (this.OnSwitchToRoft != null)
			{
				this.OnSwitchToRoft(raft);
			}
			character.immuneToCriticalHit = true;
			Stop = StopFlag.DONT_STOP;
			while (Stop == StopFlag.DONT_STOP)
			{
				yield return null;
			}
			IsActive = false;
			characterModel.RemoveRaft();
			character.immuneToCriticalHit = false;
			OnEndRaft();
			if (Stop == StopFlag.STOP)
			{
				IsActive = false;
			}
			character.IsJumping = true;
			character.IsFalling = false;
			character.verticalSpeed = character.CalculateJumpVerticalSpeed(10f);
		}
	}

	public void Pause()
	{
		Paused = true;
		roftRoot.SetActive(false);
	}

	public void Reset()
	{
		character.immuneToCriticalHit = true;
		character.characterController.enabled = true;
		character.characterCollider.enabled = true;
		roftRoot.SetActive(true);
	}

	public void Resume()
	{
		Paused = false;
		roftRoot.SetActive(true);
	}
}
