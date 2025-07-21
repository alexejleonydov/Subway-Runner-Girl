using System.Collections;
using UnityEngine;

public class RatingPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel descripeLbl;

	[SerializeField]
	private UILabel rateLbl;

	[SerializeField]
	private Animation rateStarAni;

	[SerializeField]
	private GameObject star1;

	[SerializeField]
	private GameObject star2;

	[SerializeField]
	private GameObject star3;

	[SerializeField]
	private GameObject star4;

	[SerializeField]
	private GameObject star5;

	[SerializeField]
	private GameObject SubmitButton;

	private bool isLowStar;

	private bool isInAni;

	public override void Show()
	{
		base.Show();
		SubmitButton.gameObject.SetActive(false);
		rateStarAni.Play();
		rateStarAni[rateStarAni.clip.name].speed = 0f;
		RefreshLabel(true);
	}

	public override void Init()
	{
		base.Init();
		UIEventListener uIEventListener = UIEventListener.Get(star1);
		uIEventListener.onClick = OnStarClick;
		uIEventListener = UIEventListener.Get(star2);
		uIEventListener.onClick = OnStarClick;
		uIEventListener = UIEventListener.Get(star3);
		uIEventListener.onClick = OnStarClick;
		uIEventListener = UIEventListener.Get(star4);
		uIEventListener.onClick = OnStarClick;
		uIEventListener = UIEventListener.Get(star5);
		uIEventListener.onClick = OnStarClick;
	}

	private void OnStarClick(GameObject go)
	{
		if (!isInAni)
		{
			rateStarAni.Stop();
			rateStarAni[rateStarAni.clip.name].speed = 1f;
			float alltime = 0f;
			float length = rateStarAni.clip.length;
			switch (go.name)
			{
			case "star1":
				alltime = length / 5f * 1f;
				isLowStar = true;
				break;
			case "star2":
				alltime = length / 5f * 2f;
				isLowStar = true;
				break;
			case "star3":
				alltime = length / 5f * 3f;
				isLowStar = true;
				break;
			case "star4":
				alltime = length / 5f * 4f;
				isLowStar = false;
				break;
			case "star5":
				alltime = length / 5f * 5f;
				isLowStar = false;
				break;
			}
			StartCoroutine(StopAni(alltime));
			rateStarAni.Play();
		}
	}

	private IEnumerator StopAni(float alltime)
	{
		isInAni = true;
		float time = 0f;
		while (time < alltime)
		{
			time += Time.deltaTime;
			yield return null;
		}
		SubmitButton.gameObject.SetActive(true);
		isInAni = false;
		RefreshLabel(isLowStar);
		rateStarAni[rateStarAni.clip.name].speed = 0f;
	}

	private void RefreshLabel(bool isLowStar)
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_TITLE);
		if (!isLowStar)
		{
			descripeLbl.gameObject.SetActive(true);
		}
		else
		{
			descripeLbl.gameObject.SetActive(false);
		}
		descripeLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_CONTENT);
		rateLbl.text = Strings.Get(LanguageKey.UI_POPUP_RATE_BUTTON_GOOD);
	}

	public void OkClicked()
	{
		if (isLowStar)
		{
			RiseSdk.Instance.Suport("help@ivymobile.com", "Suggestion");
		}
		else
		{
			RiseSdk.Instance.Rate();
		}
		SubmitButton.gameObject.SetActive(false);
		UIScreenController.Instance.ClosePopup(null);
	}
}
