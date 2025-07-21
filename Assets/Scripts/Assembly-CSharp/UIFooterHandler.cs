using System;
using UnityEngine;

public class UIFooterHandler : MonoBehaviour
{
	public FootItem helm;

	public FootItem character;

	public FootItem upgrade;

	public FootItem store;

	[SerializeField]
	private UISprite[] tips = new UISprite[3];

	private void OnEnable()
	{
		UpdateTips();
		PurchaseHandler.Instance.AddOnUpgradePurchase(UpdateTips);
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Combine(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(UpdateTipsBY));
	}

	private void OnDisable()
	{
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(UpdateTips);
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Remove(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(UpdateTipsBY));
	}

	private void UpdateTipsBY(Helmets.HelmType obj)
	{
		UpdateTips();
	}

	public void InitButtonType()
	{
		helm.SetFill(false);
		character.SetFill(false);
		upgrade.SetFill(false);
		store.SetFill(false);
	}

	private void UpdateTips()
	{
		tips[0].enabled = PlayerInfo.Instance.CanUnlockCharacter();
		tips[1].enabled = PlayerInfo.Instance.CanUnlockHelm();
		tips[2].enabled = PlayerInfo.Instance.CanIncreasePowerup();
	}

	public void OnButtonClick(int selected)
	{
		InitButtonType();
		switch (selected)
		{
		case 1:
			character.SetFill(true);
			break;
		case 2:
			helm.SetFill(true);
			break;
		case 3:
			upgrade.SetFill(true);
			break;
		case 4:
			store.SetFill(true);
			break;
		default:
			Debug.Log("No button was selected in the footer?", this);
			break;
		}
	}
}
