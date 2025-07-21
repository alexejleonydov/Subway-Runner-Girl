using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	private Character character;

	private Game game;

	private Helmet helmet;

	private Dictionary<Character.StumbleType, string> stumbleClips;

	public void Start()
	{
		helmet = Helmet.Instance;
		character = GetComponent<Character>();
		character.OnChangeTrack += HandleOnChangeTrack;
		character.OnJump += HandleOnJump;
		character.OnRoll += HandleOnRoll;
		character.OnLanding += HandleOnLanding;
		character.OnStumble += HandleOnStumble;
		stumbleClips = new Dictionary<Character.StumbleType, string>();
		stumbleClips.Add(Character.StumbleType.Normal, "leyou_Hr_stumble");
		stumbleClips.Add(Character.StumbleType.Bush, "leyou_Hr_stumble_bush");
		stumbleClips.Add(Character.StumbleType.Side, "leyou_Hr_stumble_side");
		game = Game.Instance;
		game.OnTurboHeadstartInput += HandleOnTurboHeadstartInput;
		SlideinPowerupHelper.OnScoreBoostActivated += HandleOnScoreBoostActivated;
	}

	private void HandleOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_run_dodge", base.transform.position);
	}

	private void HandleOnJump()
	{
		if (game.HasSuperShoes)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_superSneakers_jump", true);
		}
		else
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_run_jump", true);
		}
	}

	private void HandleOnRoll()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_run_roll", true);
	}

	private void HandleOnLanding(Transform characterTransform)
	{
		if (!character.IsRolling)
		{
			if (helmet.IsActive && character.IsAboveGround)
			{
				AudioPlayer.Instance.PlaySound("leyou_Hr_H_land", true);
			}
			else
			{
				AudioPlayer.Instance.PlaySound("leyou_Hr_landing", true);
			}
		}
	}

	private void HandleOnScoreBoostActivated()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
	}

	private void HandleOnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		AudioPlayer.Instance.PlaySound(stumbleClips[stumbleType], true);
	}

	private void HandleOnTurboHeadstartInput()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_turboheadstart", true);
	}

	private void FlypackOnStart(bool isHeadStart)
	{
		if (!isHeadStart)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
		}
		AudioPlayer.Instance.PlaySound("leyou_Hr_flyPack_mainLOOP", 0.5f, 0.5f, 1f);
	}

	private void FlypackOnStop()
	{
		AudioPlayer.Instance.StopSound("leyou_Hr_flyPack_mainLOOP");
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
	}
}
