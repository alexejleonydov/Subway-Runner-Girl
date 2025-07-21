using UnityEngine;

public class UIFinishPanel : MonoBehaviour
{
	[SerializeField]
	private UILabel okLbl;

	[SerializeField]
	private UIChestReward[] uiChestRewards;

	[SerializeField]
	private Transform[] points;

	[SerializeField]
	private Animation anim;

	public void InitUIChestRewards(PrizeEntry[] entries)
	{
		Hide();
		int num = entries.Length;
		int num2 = 0;
		if (num >= 3)
		{
			num2 = 0;
		}
		if (num == 2)
		{
			num2 = 1;
		}
		if (num == 1)
		{
			num2 = 2;
		}
		int i = 0;
		for (int num3 = uiChestRewards.Length; i < num3; i++)
		{
			if (i < num)
			{
				uiChestRewards[i].gameObject.SetActive(true);
				if (i < 3)
				{
					uiChestRewards[i].transform.localPosition = points[num2 + i * 2].localPosition;
				}
				else
				{
					uiChestRewards[i].transform.localPosition = points[i + 2].localPosition;
				}
				uiChestRewards[i].Show(entries[i].itemType, entries[i].min);
				uiChestRewards[i].InitProgress(false);
			}
			else
			{
				uiChestRewards[i].gameObject.SetActive(false);
			}
		}
	}

	public void Show()
	{
		okLbl.text = Strings.Get(LanguageKey.BOX_OPEN_SHOW_ALL_REWARD_BTN);
		AudioPlayer.Instance.PlaySound("show_reward", true);
		anim.Play();
	}

	public void Hide()
	{
		base.transform.localPosition = Vector3.up * 3000f;
	}
}
