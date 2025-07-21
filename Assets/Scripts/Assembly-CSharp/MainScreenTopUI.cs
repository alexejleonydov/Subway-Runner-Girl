using System;
using Network;
using UnityEngine;

public class MainScreenTopUI : MonoBehaviour
{
	[SerializeField]
	private UILabel keyAmountLabel;

	[SerializeField]
	private UILabel coinAmountLabel;

	[SerializeField]
	private UILabel levelAmountLabel;

	[SerializeField]
	private UILabel expAmountLabel;

	[SerializeField]
	private UITexture headTexture;

	[SerializeField]
	private UISlider expSlider;

	private void OnEnable()
	{
		PlayerInfo.Instance.onCoinsChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onCoinsChanged, new Action(OnCoinsChanged));
		PlayerInfo.Instance.onKeysChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onKeysChanged, new Action(OnKeysChanged));
		PlayerInfo.Instance.onLevelChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onLevelChanged, new Action(OnLevelChanged));
		PlayerInfo.Instance.onExpChanged = (Action)Delegate.Combine(PlayerInfo.Instance.onExpChanged, new Action(OnExpChanged));
		OnCoinsChanged();
		OnKeysChanged();
		OnLevelChanged();
		OnExpChanged();
		RefreshHeadUI();
		ServerManager.Instance.RegisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void Awake()
	{
		RefreshHeadUI();
	}

	private void RefreshHeadUI()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			headTexture.mainTexture = pictureUrl.Image;
		}
	}

	private void OnExpChanged()
	{
		float num = Game.Instance.GetExpCoefficient() * (float)(PlayerInfo.Instance.amountOfLevel - 1) + 8f;
		float num2 = 0f;
		num2 = PlayerInfo.Instance.amountOfExp;
		expAmountLabel.text = num2.ToString() + "/" + num;
		expSlider.value = num2 / num;
	}

	private void OnLevelChanged()
	{
		levelAmountLabel.text = "LV. " + PlayerInfo.Instance.amountOfLevel;
		OnExpChanged();
	}

	private void OnDisable()
	{
		PlayerInfo.Instance.onCoinsChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onCoinsChanged, new Action(OnCoinsChanged));
		PlayerInfo.Instance.onKeysChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onKeysChanged, new Action(OnKeysChanged));
		PlayerInfo.Instance.onLevelChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onLevelChanged, new Action(OnLevelChanged));
		PlayerInfo.Instance.onExpChanged = (Action)Delegate.Remove(PlayerInfo.Instance.onExpChanged, new Action(OnExpChanged));
		ServerManager.Instance.UnregisterOnPictrueUrlChange(RefreshHeadUI);
	}

	private void OnCoinsChanged()
	{
		coinAmountLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
	}

	private void OnKeysChanged()
	{
		keyAmountLabel.text = PlayerInfo.Instance.amountOfKeys.ToString();
	}
}
