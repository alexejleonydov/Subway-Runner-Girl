internal class HelmetManager
{
	public delegate void OnHelmetChangeDelegate(Helmets.HelmType helmet);

	private Helmets.HelmType currentlyDisplayedHelmet;

	private static HelmetManager instance;

	public static HelmetManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new HelmetManager();
			}
			return instance;
		}
	}

	public event OnHelmetChangeDelegate OnDisplayedHelmetChange;

	public bool isHelmActive(Helmets.HelmType helmType)
	{
		if (Helmets.helmData[helmType].unlockType == Helmets.UnlockType.hiddenUntillUnlocked)
		{
			return false;
		}
		return true;
	}

	public bool isHelmetUnlocked(Helmets.HelmType helmType)
	{
		if (Helmets.helmData[helmType].unlockType == Helmets.UnlockType.alwaysUnlocked)
		{
			return true;
		}
		if (PlayerInfo.Instance._helmetUnlockStatus.ContainsKey(helmType))
		{
			return PlayerInfo.Instance._helmetUnlockStatus[helmType];
		}
		return false;
	}

	public void ChangedDisplayedHelmet(Helmets.HelmType newHelm)
	{
		currentlyDisplayedHelmet = newHelm;
		if (this.OnDisplayedHelmetChange != null)
		{
			this.OnDisplayedHelmetChange(newHelm);
		}
	}

	public Helmets.HelmType CurrentlyDisplayedHelmet()
	{
		return currentlyDisplayedHelmet;
	}

	public Helmets.HelmType CurrentlyEquippedHelmet()
	{
		return PlayerInfo.Instance.currentHelmet;
	}
}
