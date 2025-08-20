using System.Collections;
using UnityEngine;

public class SuperShoes : ICharacterAttachment
{
	public delegate void OnSwitchToSuperShoesDelegate();

	public delegate void SuperShoesOnStopDelegate();

	private Character character;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private static SuperShoes instance;

	private LongMagnet longMagnet;

	private ActiveProp Powerup;

	private float timeActiveInARow;

	public static SuperShoes Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SuperShoes();
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

	public event OnSwitchToSuperShoesDelegate OnSwitchToSuperShoes;

	public event SuperShoesOnStopDelegate SuperShoesOnStop;

	public SuperShoes()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		longMagnet = character.GetComponentInChildren<LongMagnet>();
	}

	public IEnumerator Begain()
	{
		Prepare();
		while (Powerup.timeLeft > 0f && Stop == StopFlag.DONT_STOP)
		{
			if (!Paused)
			{
				timeActiveInARow += Time.deltaTime;
			}
			yield return null;
		}
		End();
	}


	private void Prepare()
	{
		GameStats.Instance.pickedUpPowerups++;
		Powerup = GameStats.Instance.RegisterPowerup(PropType.supershoes);
		character.IsJumpingHigher = true;
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		//characterModel.meshSuperShoes.enabled = true;

		characterModel.meshLeftShoe.enabled = true;
		characterModel.meshRightShoe.enabled = true;
		Debug.Log("SuperShoe is ON   for SuperShoes.cs");

		IsActive = true;
		longMagnet.Activate();
		if (this.OnSwitchToSuperShoes != null)
		{
			this.OnSwitchToSuperShoes();
		}
		Stop = StopFlag.DONT_STOP;
	}

	private void End()
	{
		timeActiveInARow = 0f;
		//characterModel.meshSuperShoes.enabled = false;

		characterModel.meshLeftShoe.enabled = false;
		characterModel.meshRightShoe.enabled = false;
		Debug.Log("SuperShoe is OFF for End in SuperShoes.cs");

		IsActive = false;
		longMagnet.Deactivate();
		if (Powerup.timeLeft <= 0f)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
		}
		character.IsJumpingHigher = false;
		if (this.SuperShoesOnStop != null)
		{
			this.SuperShoesOnStop();
		}
	}

	public void Reset()
	{
		Paused = false;
		character.IsJumpingHigher = false;
	}

	public void StopUse()
	{
		IsActive = false;
		//characterModel.meshSuperShoes.enabled = false;

		characterModel.meshLeftShoe.enabled = false;
		characterModel.meshRightShoe.enabled = false;

		Debug.Log("SuperShoe is OFF for StopUse in SuperShoes.cs");
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
