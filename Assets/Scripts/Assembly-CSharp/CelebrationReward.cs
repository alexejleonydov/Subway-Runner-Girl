using System;
using System.Text;

public class CelebrationReward
{
	public CelebrationRewardOrigin CelebrationRewardOrigin;

	public CelebrationRewardType rewardType;

	public int amount;

	public Characters.CharacterType characterType;

	public int characterThemeIndex;

	public Helmets.HelmType helmType;

	public PropType powerupType;

	public long Uid;

	public int rank = 1;

	public int score;

	public void PopulateFromString(string rewardAsString)
	{
		ResetToDefaultValues();
		char[] separator = new char[1] { ',' };
		string[] array = rewardAsString.Split(separator);
		try
		{
			if (array.Length >= 11)
			{
				CelebrationRewardOrigin = (CelebrationRewardOrigin)(int)Enum.Parse(typeof(CelebrationRewardOrigin), array[0]);
				rewardType = (CelebrationRewardType)(int)Enum.Parse(typeof(CelebrationRewardType), array[1]);
				amount = int.Parse(array[2]);
				characterType = (Characters.CharacterType)(int)Enum.Parse(typeof(Characters.CharacterType), array[3]);
				characterThemeIndex = int.Parse(array[4]);
				helmType = (Helmets.HelmType)(int)Enum.Parse(typeof(Helmets.HelmType), array[5]);
				powerupType = (PropType)(int)Enum.Parse(typeof(PropType), array[7]);
				Uid = long.Parse(array[8]);
				rank = int.Parse(array[9]);
				score = int.Parse(array[10]);
			}
		}
		catch
		{
			PlayerInfo.Instance.pendingRewards.Clear();
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	private void ResetToDefaultValues()
	{
		CelebrationRewardOrigin = CelebrationRewardOrigin.Notset;
		rewardType = CelebrationRewardType._notset;
		amount = 0;
		characterType = Characters.CharacterType.slick;
		characterThemeIndex = 0;
		helmType = Helmets.HelmType.normal;
		powerupType = PropType._notset;
		Uid = 0L;
		rank = 1;
		score = 0;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(CelebrationRewardOrigin).Append(",");
		stringBuilder.Append(rewardType).Append(",");
		stringBuilder.Append(amount).Append(",");
		stringBuilder.Append(characterType).Append(",");
		stringBuilder.Append(characterThemeIndex).Append(",");
		stringBuilder.Append(helmType).Append(",");
		stringBuilder.Append("helmPower").Append(",");
		stringBuilder.Append(powerupType).Append(",");
		stringBuilder.Append(Uid).Append(",");
		stringBuilder.Append(rank).Append(",");
		stringBuilder.Append(score).Append(",");
		return stringBuilder.ToString();
	}

	public bool Find(CelebrationReward cr)
	{
		return Uid == cr.Uid;
	}
}
