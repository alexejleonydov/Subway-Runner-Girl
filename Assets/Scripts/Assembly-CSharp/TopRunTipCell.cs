using UnityEngine;

public class TopRunTipCell : RankCell
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private Color goldColor;

	[SerializeField]
	private Color silveryColor;

	[SerializeField]
	private Color copperColor;

	[SerializeField]
	private UISprite[] cupIcons = new UISprite[4];

	private int _rankId;

	public override void RefreshUI(TopRun topRun)
	{
		if (_rankId == topRun.rank)
		{
			return;
		}
		_rankId = topRun.rank;
		LanguageKey key = LanguageKey.TOPRUN_RANK0_TITLE;
		switch (_rankId)
		{
		case -1:
		{
			key = LanguageKey.TOPRUN_RANK0_TITLE;
			title.color = goldColor;
			for (int j = 0; j < cupIcons.Length; j++)
			{
				cupIcons[j].spriteName = "AX_cup_Gold";
			}
			break;
		}
		case -2:
		{
			key = LanguageKey.TOPRUN_RANK1_TITLE;
			title.color = silveryColor;
			for (int k = 0; k < cupIcons.Length; k++)
			{
				cupIcons[k].spriteName = "AX_cup_Silver";
			}
			break;
		}
		case -3:
		{
			key = LanguageKey.TOPRUN_RANK2_TITLE;
			title.color = copperColor;
			for (int i = 0; i < cupIcons.Length; i++)
			{
				cupIcons[i].spriteName = "AX_cup_Copper";
			}
			break;
		}
		}
		title.text = Strings.Get(key);
	}
}
