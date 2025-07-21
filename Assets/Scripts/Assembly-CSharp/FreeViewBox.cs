using System;
using UnityEngine;

public class FreeViewBox : MonoBehaviour
{
	[SerializeField]
	private float wait;

	[SerializeField]
	private PaticlesHelper particlesHelp;

	private Animation anim;

	private Game game;

	private Vector3 originPos;

	private bool isShow;

	private float time;

	private void Start()
	{
		anim = GetComponent<Animation>();
		game = Game.Instance;
		originPos = base.transform.position;
		base.transform.position = Vector3.up * 1000f;
		game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(game.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
		time = wait;
		StopParticleSystem();
	}

	private void Update()
	{
		if (!isShow && PlayerInfo.Instance.tutorialStep != 0 && LoadScene.finished && game.IsInTopMenu.Value && "FrontUI".Equals(UIScreenController.Instance.GetTopScreenName()) && UIScreenController.Instance.IsPopupQueueEmpty())
		{
			if (time > 0f)
			{
				time -= Time.deltaTime;
				return;
			}
			isShow = true;
			base.transform.position = originPos;
			anim.Play("Box_fall");
			anim.CrossFadeQueued("Prop_box_Alert");
		}
	}

	public void PlayParticleSystem()
	{
		particlesHelp.Play();
		AudioPlayer.Instance.PlaySound("prop_box_fall_sfx", true);
	}

	public void StopParticleSystem()
	{
		particlesHelp.Stop();
	}

	private void OnIntroRun()
	{
		if (base.enabled)
		{
			isShow = false;
			anim.Stop();
			base.transform.position = Vector3.up * 1000f;
			time = wait;
			StopParticleSystem();
		}
	}

	public void OnMouseUpAsButton()
	{
		if ("FrontUI".Equals(UIScreenController.Instance.GetTopScreenName()) && UIScreenController.Instance.IsPopupQueueEmpty())
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "into_game_box", 0);
			UIScreenController.Instance.PushPopup("WatchVideoPopup");
		}
	}
}
