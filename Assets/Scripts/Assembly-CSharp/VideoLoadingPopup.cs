using UnityEngine;

public class VideoLoadingPopup : UIBaseScreen
{
	public static int adType;

	public static int rewardId;

	[SerializeField]
	private int min_delay = 10;

	[SerializeField]
	private int max_delay = 60;

	private int delay;

	[SerializeField]
	private int max = 1800;

	private int frame;

	public override void Show()
	{
		base.Show();
		Game.Instance.closePopupOnAdEvent = true;
		frame = 0;
		delay = Random.Range(min_delay, max_delay);
	}

	private void Update()
	{
		if (frame < max)
		{
			if (Game.Instance.Paused)
			{
				return;
			}
			frame++;
			if (frame != delay)
			{
				return;
			}
			if (adType == 2)
			{
				RiseSdk.Instance.ShowRewardAd(rewardId);
			}
			else if (adType == 1)
			{
				if (rewardId == 1)
				{
					RiseSdk.Instance.ShowAd("custom");
				}
				else if (rewardId == 2)
				{
					RiseSdk.Instance.ShowAd("passlevel1");
				}
				else if (rewardId == 3)
				{
					RiseSdk.Instance.ShowAd("custom");
				}
			}
		}
		else
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}
}
