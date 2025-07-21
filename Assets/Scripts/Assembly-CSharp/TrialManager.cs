using System;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
	[SerializeField]
	private TrialInfo[] infos;

	[SerializeField]
	private int[] orders;

	[SerializeField]
	private int year;

	[SerializeField]
	private int month;

	[SerializeField]
	private int day;

	[SerializeField]
	private bool isContinue;

	public DateTime begainDateTime;

	[HideInInspector]
	public TrialInfo currentTrialInfo;

	private int preCharacter;

	private int preCharacterSkin;

	private int preHelmet;

	public const int NUMBER_OF_TRIALS = 3;

	private static TrialManager _instance;

	public bool IsTestChar { get; set; }

	public bool IsTestHelm { get; set; }

	public bool preUseTryRole { get; set; }

	public bool nothingElse { get; set; }

	public static TrialManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(TrialManager)) as TrialManager;
			}
			return _instance;
		}
	}

	private void Awake()
	{
		begainDateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
		if (!PlayerPrefs.HasKey("NewTrialOrder1"))
		{
			PlayerInfo.Instance.currentTrialIndex = -1;
			PlayerInfo.Instance.totalTrialDays = 0;
			PlayerPrefs.SetInt("NewTrialOrder1", 1);
		}
		Check();
	}

	private void Check()
	{
		int num = PlayerInfo.Instance.currentTrialIndex;
		if (num >= 3)
		{
			num = -1;
		}
		if (num == -1)
		{
			Next();
			return;
		}
		currentTrialInfo = infos[orders[num]];
		if (begainDateTime.AddDays(PlayerInfo.Instance.totalTrialDays) < DateTime.UtcNow || !CheckTrialValidly())
		{
			Next();
		}
	}

	private void Next()
	{
		bool flag = false;
		int num = 0;
		if (isContinue)
		{
			do
			{
				currentTrialInfo = infos[orders[PlayerInfo.Instance.NextTrial()]];
				flag = CheckTrialValidly();
				num++;
			}
			while (!flag && num <= 3);
			if (flag)
			{
				PlayerInfo.Instance.totalTrialDays = (DateTime.UtcNow.Date - begainDateTime.Date).Days + currentTrialInfo.days;
			}
		}
		else
		{
			bool flag2 = true;
			do
			{
				currentTrialInfo = infos[orders[PlayerInfo.Instance.NextTrial()]];
				PlayerInfo.Instance.totalTrialDays += currentTrialInfo.days;
				flag2 = begainDateTime.AddDays(PlayerInfo.Instance.totalTrialDays) < DateTime.UtcNow;
				flag = CheckTrialValidly();
				if (!flag)
				{
					num++;
				}
			}
			while ((flag2 || !flag) && num <= 3);
		}
		if (num > 3)
		{
			nothingElse = true;
			currentTrialInfo = null;
			PlayerInfo.Instance.currentTrialIndex = -1;
		}
	}

	public TrialInfo SelectValidlyTrialInfo()
	{
		if (CheckTrialValidly())
		{
			return currentTrialInfo;
		}
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			if (CheckTrialValidly(infos[orders[i]]))
			{
				return infos[orders[i]];
			}
		}
		return null;
	}

	private bool CheckTrialValidly(TrialInfo info)
	{
		if (info == null)
		{
			return false;
		}
		if (info.type == TrialType.Character)
		{
			if (info.characterThemeId == 0)
			{
				return !PlayerInfo.Instance.IsCollectionComplete(info.characterType);
			}
			return !PlayerInfo.Instance.IsThemeUnlockedForCharacter(info.characterType, info.characterThemeId);
		}
		if (info.type == TrialType.Helmet)
		{
			return !HelmetManager.Instance.isHelmetUnlocked(info.helmetType);
		}
		return false;
	}

	public bool CheckTrialValidly()
	{
		return CheckTrialValidly(currentTrialInfo);
	}

	public bool IsCurrentCharacterTrial(Characters.CharacterType characterType, int themeId)
	{
		if (currentTrialInfo == null)
		{
			return false;
		}
		return currentTrialInfo.type == TrialType.Character && currentTrialInfo.characterType == characterType && currentTrialInfo.characterThemeId == themeId;
	}

	public bool HasTrialCharacter(Characters.CharacterType characterType)
	{
		TrialInfo trialInfo = null;
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			trialInfo = infos[orders[i]];
			if (trialInfo.type == TrialType.Character && trialInfo.characterType == characterType)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCurrentHelmetTrial(Helmets.HelmType helmetType)
	{
		if (currentTrialInfo == null)
		{
			return false;
		}
		return currentTrialInfo.type == TrialType.Helmet && currentTrialInfo.helmetType == helmetType;
	}

	public bool HasHelmetTrial(Helmets.HelmType helmetType)
	{
		TrialInfo trialInfo = null;
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			trialInfo = infos[orders[i]];
			if (trialInfo.type == TrialType.Helmet && trialInfo.helmetType == helmetType)
			{
				return true;
			}
		}
		return false;
	}

	private void OnEnable()
	{
		TrialInfo trialInfo = null;
		int i = 0;
		for (int num = orders.Length; i < num; i++)
		{
			trialInfo = infos[orders[i]];
			if (trialInfo.type == TrialType.Character && PlayerInfo.Instance.currentCharacter == (int)trialInfo.characterType && PlayerInfo.Instance.currentThemeIndex == trialInfo.characterThemeId)
			{
				if (trialInfo.characterThemeId == 0 && !PlayerInfo.Instance.IsCollectionComplete(trialInfo.characterType))
				{
					PlayerInfo.Instance.currentCharacter = 0;
					break;
				}
				if (trialInfo.characterThemeId > 0 && !PlayerInfo.Instance.IsThemeUnlockedForCharacter(trialInfo.characterType, trialInfo.characterThemeId))
				{
					PlayerInfo.Instance.currentCharacter = 0;
					PlayerInfo.Instance.currentThemeIndex = 0;
					break;
				}
			}
			if (trialInfo.type == TrialType.Helmet && PlayerInfo.Instance.currentHelmet == trialInfo.helmetType)
			{
				if (!HelmetManager.Instance.isHelmetUnlocked(trialInfo.helmetType))
				{
					PlayerInfo.Instance.currentHelmet = Helmets.HelmType.normal;
				}
				break;
			}
		}
	}

	public void OnDisable()
	{
		if (IsTestChar)
		{
			IsTestChar = false;
			preUseTryRole = true;
			PlayerInfo.Instance.currentCharacter = preCharacter;
			PlayerInfo.Instance.currentThemeIndex = preCharacterSkin;
			PlayerInfo.Instance.SaveIfDirty();
		}
		if (IsTestHelm)
		{
			IsTestHelm = false;
			preUseTryRole = true;
			PlayerInfo.Instance.currentHelmet = (Helmets.HelmType)preHelmet;
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void End()
	{
		if (IsTestChar)
		{
			IsTestChar = false;
			preUseTryRole = true;
			CharacterScreenManager.Instance.SelectCharacter((Characters.CharacterType)preCharacter, preCharacterSkin);
			PlayerInfo.Instance.SaveIfDirty();
		}
		if (IsTestHelm)
		{
			IsTestHelm = false;
			preUseTryRole = true;
			PlayerInfo.Instance.currentHelmet = (Helmets.HelmType)preHelmet;
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void Begin()
	{
		if (currentTrialInfo != null)
		{
			if (currentTrialInfo.type == TrialType.Character)
			{
				IsTestChar = true;
				preCharacter = PlayerInfo.Instance.currentCharacter;
				preCharacterSkin = PlayerInfo.Instance.currentThemeIndex;
				CharacterScreenManager.Instance.SelectCharacter(currentTrialInfo.characterType, currentTrialInfo.characterThemeId);
			}
			else if (currentTrialInfo.type == TrialType.Helmet)
			{
				IsTestHelm = true;
				preHelmet = (int)PlayerInfo.Instance.currentHelmet;
				PlayerInfo.Instance.currentHelmet = currentTrialInfo.helmetType;
			}
		}
	}

	public bool IsInTest()
	{
		return IsTestChar || IsTestHelm;
	}

	public bool CheckOnMainScreen()
	{
		return !preUseTryRole && currentTrialInfo != null;
	}

	public int CurrentTrialIndex()
	{
		return orders[PlayerInfo.Instance.currentTrialIndex];
	}
}
