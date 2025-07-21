using System.Collections;
using Network;
using UnityEngine;

public class NewHighScoreHandler : MonoBehaviour
{
	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private UILabel _highScoreLbl;

	[SerializeField]
	private UILabel _myRankLbl;

	[SerializeField]
	private UILabel _myScoreLbl;

	[SerializeField]
	private UILabel _myNameLbl;

	[SerializeField]
	private UITexture _myHeadTxt;

	[SerializeField]
	private UILabel _otherRankLbl;

	[SerializeField]
	private UILabel _otherScoreLbl;

	[SerializeField]
	private UILabel _otherNameLbl;

	[SerializeField]
	private UITexture _otherHeadTxt;

	[SerializeField]
	private Animation upAnim;

	private void Awake()
	{
		_Title.alpha = 0f;
		_highScoreLbl.alpha = 0f;
		StartCoroutine(SetUI());
	}

	private IEnumerator SetUI()
	{
		_Title.text = Strings.Get(LanguageKey.BRAG_CELEBRATION_NEW_HIGHSCORE);
		_highScoreLbl.text = PlayerInfo.Instance.highestScore.ToString();
		_highScoreLbl.alpha = 1f;
		_Title.alpha = 1f;
		upAnim.gameObject.SetActive(false);
		if (HighestScoreSystem.Instance.lastBeatenTopRun != null)
		{
			RefreshUI();
			yield return new WaitForSeconds(1f);
			upAnim.gameObject.SetActive(true);
			upAnim.Play();
		}
	}

	private void RefreshUI()
	{
		HighestScoreSystem instance = HighestScoreSystem.Instance;
		TopRun lastBeatenTopRun = instance.lastBeatenTopRun;
		_myRankLbl.text = lastBeatenTopRun.rank.ToString();
		PlayerName playerName = ServerManager.Instance.PlayerName;
		if (playerName == null)
		{
			_myNameLbl.text = "----";
		}
		else
		{
			_myNameLbl.text = playerName.Value;
		}
		_myScoreLbl.text = PlayerInfo.Instance.highestScore.ToString();
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl == null)
		{
			_myHeadTxt.mainTexture = null;
		}
		else
		{
			_myHeadTxt.mainTexture = pictureUrl.Image;
		}
		if (instance.isAI)
		{
			_otherNameLbl.text = lastBeatenTopRun.userId;
			_otherRankLbl.text = (lastBeatenTopRun.rank + 1).ToString();
			_otherScoreLbl.text = lastBeatenTopRun.highestScore.ToString();
			_otherHeadTxt.mainTexture = null;
			return;
		}
		_otherRankLbl.text = (lastBeatenTopRun.rank + 1).ToString();
		_otherScoreLbl.text = lastBeatenTopRun.highestScore.ToString();
		_otherHeadTxt.mainTexture = null;
		TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(lastBeatenTopRun.userId);
		if (topRunInfo != null)
		{
			_otherNameLbl.text = topRunInfo.playerName;
			_otherHeadTxt.mainTexture = ImageManager.Instance.GetTexture(topRunInfo.pictureUrl);
		}
	}
}
