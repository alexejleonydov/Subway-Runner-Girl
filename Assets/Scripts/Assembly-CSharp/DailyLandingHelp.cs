using UnityEngine;

public class DailyLandingHelp : MonoBehaviour
{
	[SerializeField]
	private int dayIndex;

	[SerializeField]
	private UILabel dayLbl;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UISprite checkMskSpr;

	[SerializeField]
	private UISprite backgroundInactive;

	[SerializeField]
	private UISprite backgroundActive;

	[SerializeField]
	private UISprite rewardIconSpr;

	private DailyLandingAward award;

	public void Init(int dayId)
	{
		dayIndex = dayId;
		award = DailyLandingAwards.GetDailyLandingAwardByID(dayId);
		if (award != null)
		{
			if (award.type != DailyLandingAward.DailyLandingRewardType.Chest)
			{
				rewardLbl.text = award.Amount.ToString();
			}
			else
			{
				rewardLbl.enabled = false;
			}
		}
	}

	public void Refresh()
	{
		bool Istoday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyLandingDaysInRow(out Istoday), 0, DailyLandingAwards.awards.Length);
		if (Istoday)
		{
			num--;
		}
		if (dayIndex - 1 == num)
		{
			dayLbl.text = Strings.Get(LanguageKey.DAILY_CHALLENGE_TODAY_2);
			dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblActive;
			rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblActive;
			checkMskSpr.enabled = Istoday;
			backgroundInactive.enabled = false;
			backgroundActive.enabled = true;
		}
		else if (dayIndex - 1 < num)
		{
			dayLbl.text = string.Format(Strings.Get(LanguageKey.DAILY_CHALLENGE_DAY), dayIndex);
			dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblActive;
			rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblActive;
			checkMskSpr.enabled = true;
			backgroundInactive.enabled = false;
			backgroundActive.enabled = true;
		}
		else
		{
			dayLbl.text = string.Format(Strings.Get(LanguageKey.DAILY_CHALLENGE_DAY), dayIndex);
			dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblInactive;
			rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblInactive;
			checkMskSpr.enabled = false;
			backgroundInactive.enabled = true;
			backgroundActive.enabled = false;
		}
	}
}
