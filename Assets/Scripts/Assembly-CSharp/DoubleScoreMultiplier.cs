using System.Collections;

public class DoubleScoreMultiplier : ICharacterAttachment
{
	private static DoubleScoreMultiplier instance;

	private ActiveProp Powerup;

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

	public static DoubleScoreMultiplier Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DoubleScoreMultiplier();
			}
			return instance;
		}
	}

	public IEnumerator Begain()
	{
		Prepare();
		while (Powerup.timeLeft > 0f && Stop == StopFlag.DONT_STOP)
		{
			yield return null;
		}
		End();
	}

	private void Prepare()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		Stop = StopFlag.DONT_STOP;
		Powerup = GameStats.Instance.RegisterPowerup(PropType.doubleMultiplier);
		PlayerInfo.Instance.doubleScore = true;
	}

	private void End()
	{
		PlayerInfo.Instance.doubleScore = false;
		if (Powerup.timeLeft <= 0f)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
		}
	}

	public void Reset()
	{
		Paused = false;
		PlayerInfo.Instance.doubleScore = false;
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
