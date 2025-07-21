using UnityEngine;

public class SubcriptionHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel tipLbl;

	[SerializeField]
	private UILabel role_big_Lbl;

	[SerializeField]
	private UILabel role_small_Lbl;

	[SerializeField]
	private UILabel doubleCoin_big_Lbl;

	[SerializeField]
	private UILabel doubleCoin_small_Lbl;

	[SerializeField]
	private UILabel gem_small_Lbl;

	[SerializeField]
	private UILabel vip_big_Lbl;

	[SerializeField]
	private UILabel vip_small_Lbl;

	[SerializeField]
	private UILabel trialLbl;

	private void OnEnable()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_TRIAL_LABEL);
		tipLbl.text = Strings.Get(LanguageKey.UI_LAB_VIP_TIME_LABEL);
		role_big_Lbl.text = Strings.Get(LanguageKey.UI_POPUP_SUBSCRIBE_CONTENT_MONK_CAPITAL);
		role_small_Lbl.text = Strings.Get(LanguageKey.UI_LAB_ROLE_CAPITAL);
		doubleCoin_big_Lbl.text = Strings.Get(LanguageKey.UI_LAB_DOUBLE_CAPITAL);
		doubleCoin_small_Lbl.text = Strings.Get(LanguageKey.UI_LAB_COINS_CAPITAL);
		gem_small_Lbl.text = Strings.Get(LanguageKey.UI_LAB_GEMS_CAPITAL);
		vip_big_Lbl.text = Strings.Get(LanguageKey.UI_LAB_VIP_BADGE_CAPITAL);
		vip_small_Lbl.text = Strings.Get(LanguageKey.UI_LAB_RANK_CAPITAL);
		trialLbl.text = Strings.Get(LanguageKey.UI_LAB_TRIAL_CAPITAL);
	}
}
