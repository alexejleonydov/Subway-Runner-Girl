using Network;
using UnityEngine;

public class TimeLeft : MonoBehaviour
{
	[SerializeField]
	private UILabel frontLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private GameObject timeLeft;

	private float time;

	private void OnEnable()
	{
		frontLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_TIME);
		timeLbl.text = DateManager.TopRunRemainTimeToString(ServeTimeUpdate.Instance.ServerTime, ServeTimeUpdate.Instance.time);
	}

	private void OnDisable()
	{
		time = 0f;
	}

	private void Update()
	{
		if (time < 1f)
		{
			time += Time.deltaTime;
			return;
		}
		timeLbl.text = DateManager.TopRunRemainTimeToString(ServeTimeUpdate.Instance.ServerTime, ServeTimeUpdate.Instance.time);
		time = 0f;
	}
}
