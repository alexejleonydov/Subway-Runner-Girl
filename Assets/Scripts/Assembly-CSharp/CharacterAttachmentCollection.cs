using System.Collections.Generic;

public class CharacterAttachmentCollection
{
	private CoinMagnet coinMagnet;

	private List<ICharacterAttachment> deadModifiers = new List<ICharacterAttachment>();

	private DoubleScoreMultiplier doubleScireMutiplier;

	private Helmet helmet;

	private List<ICharacterAttachment> modifiers = new List<ICharacterAttachment>();

	private SuperShoes superShoes;

	public CoinMagnet CoinMagnet
	{
		get
		{
			return coinMagnet;
		}
	}

	public DoubleScoreMultiplier DoubleScoreMultiplier
	{
		get
		{
			return doubleScireMutiplier;
		}
	}

	public Helmet Helmet
	{
		get
		{
			return helmet;
		}
	}

	public SuperShoes SuperShoes
	{
		get
		{
			return superShoes;
		}
	}

	public CharacterAttachmentCollection()
	{
		helmet = Helmet.Instance;
		superShoes = SuperShoes.Instance;
		coinMagnet = CoinMagnet.Instance;
		doubleScireMutiplier = DoubleScoreMultiplier.Instance;
	}

	public void Add(ICharacterAttachment modifier)
	{
		if (!modifiers.Contains(modifier))
		{
			modifiers.Add(modifier);
			modifier.Current = modifier.Begain();
		}
		else
		{
			modifier.Reset();
			modifier.Current = modifier.Begain();
		}
	}

	public bool IsActive(ICharacterAttachment modifier)
	{
		return modifiers.Contains(modifier);
	}

	public void Pause()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			modifiers[i].Pause();
		}
	}

	public void PauseInFlypackMode()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			if (modifiers[i].ShouldPauseInFlypack)
			{
				modifiers[i].Pause();
			}
		}
	}

	public void Reset()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			modifiers[i].Reset();
		}
		modifiers.Clear();
	}

	public void Resume()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			modifiers[i].Resume();
		}
	}

	public void Stop()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			modifiers[i].Stop = StopFlag.STOP;
		}
	}

	public void StopWithNoEnding()
	{
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			modifiers[i].Stop = StopFlag.STOP_NO_ENDING;
		}
	}

	public void Update()
	{
		if (modifiers.Count <= 0)
		{
			return;
		}
		deadModifiers.Clear();
		int i = 0;
		for (int count = modifiers.Count; i < count; i++)
		{
			if (!modifiers[i].Paused && !modifiers[i].Current.MoveNext())
			{
				deadModifiers.Add(modifiers[i]);
			}
		}
		if (deadModifiers.Count > 0)
		{
			int j = 0;
			for (int count2 = deadModifiers.Count; j < count2; j++)
			{
				modifiers.Remove(deadModifiers[j]);
			}
		}
	}
}
