using UnityEngine;

public class UnlockNewScreenPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel tipLbl;

	[SerializeField]
	private UILabel contentLbl;

	[SerializeField]
	private UITexture cityTexture;

	public override void Show()
	{
		base.Show();
		if (PlayerInfo.Instance.forceNextCityOrder > 0)
		{
			City city = TrackController.Instance.GetCity(PlayerInfo.Instance.forceNextCityOrder);
			if (city == null)
			{
				tipLbl.text = string.Empty;
				contentLbl.text = string.Empty;
			}
			else
			{
				PlayerInfo.Instance.forceNextCityOrder = -PlayerInfo.Instance.forceNextCityOrder;
				tipLbl.text = string.Format(Strings.Get(LanguageKey.UNLOCK_NEWSCENE_POPUP_SCENE_NAME), Strings.Get(city.cityName));
				contentLbl.text = string.Format(Strings.Get(LanguageKey.UNLOCK_NEWSCENE_POPUP_SCENE_DESCRIP), Strings.Get(city.cityName));
			}
		}
	}

	private void OnEnable()
	{
		titleLbl.text = Strings.Get(LanguageKey.UNLOCK_NEWSCENE_POPUP_TITLE);
	}
}
