using System.Collections;
using UnityEngine;

public class RankLoadingPopup : UIBaseScreen
{
	[SerializeField]
	private GameObject bgBtn;

	[SerializeField]
	private UILabel tipLbl;

	private TweenAlpha ta;

	public override void Init()
	{
		base.Init();
		UIEventListener uIEventListener = UIEventListener.Get(bgBtn);
		uIEventListener.onPress = OnPress;
		ta = tipLbl.GetComponent<TweenAlpha>();
		ta.enabled = false;
	}

	public void OnPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
		{
			OnPress(base.gameObject, true);
		}
	}

	public override void Show()
	{
		base.Show();
		RefreshLable();
		StartCoroutine(PlayTween());
	}

	private void RefreshLable()
	{
		tipLbl.text = Strings.Get(LanguageKey.UI_POPUP_RANK_LOADING_CONTENT);
	}

	private IEnumerator PlayTween()
	{
		tipLbl.enabled = true;
		tipLbl.alpha = 0f;
		ta.ResetToBeginning2();
		ta.PlayForward();
		yield return new WaitForSeconds(ta.delay + 4f);
		ta.Stop();
		tipLbl.enabled = false;
	}

	public override void Hide()
	{
		ta.enabled = false;
		tipLbl.enabled = false;
		base.Hide();
	}
}
