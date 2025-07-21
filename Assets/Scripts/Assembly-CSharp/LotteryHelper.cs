using UnityEngine;

public class LotteryHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite tip;

	[SerializeField]
	private int interval;

	private int frame;

	private void OnEnable()
	{
		frame = 0;
		tip.enabled = PlayerInfo.Instance.CheckIfLotteryCanFree();
	}

	private void Update()
	{
		frame++;
		if (frame > interval)
		{
			tip.enabled = PlayerInfo.Instance.CheckIfLotteryCanFree();
			frame = 0;
		}
	}
}
