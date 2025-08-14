using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("GUI/Interaction/Change Screen Button")]
public class UIButtonChangeScreen : UIBasicButton
{
	private InputActions inputActions;


	private void OnEnable()
	{
		CloseSubscribePopupIfActive();

		//Debug.Log("UIButtonChangeScreen connected to..." + gameObject.name);

		inputActions = new InputActions();

		inputActions.Enable();
		inputActions.Play.CoinsShop.performed += OnCoinsShopPerformed;
		inputActions.Play.CharacterShop.performed += OnCharacterShopPerformed;
		inputActions.Play.Exit.performed += OnFrontUIPerformed;
		inputActions.Play.PlayGame.performed += OnStartPerformed;

		CloseSubscribePopupIfActive();
	}

	private void OnDisable()
	{
		inputActions.Disable();
		inputActions.Play.CoinsShop.performed -= OnCoinsShopPerformed;
		inputActions.Play.CharacterShop.performed -= OnCharacterShopPerformed;
		inputActions.Play.Exit.performed -= OnFrontUIPerformed;
		inputActions.Play.PlayGame.performed -= OnStartPerformed;
	}

	private void OnCoinsShopPerformed(InputAction.CallbackContext context)
	{
		ScreenNameToOpen = "CoinsUI_shop";


		Send();
	}

	private void OnCharacterShopPerformed(InputAction.CallbackContext context)
	{
		//ScreenNameToOpen = "CharacterScreen";

		if (ScreenNameToOpen == "CharacterScreen")
			Send();
	}

	private void OnFrontUIPerformed(InputAction.CallbackContext context)
	{
		GameObject gameOverUIPopup = GameObject.Find("4Footer");
		if (gameOverUIPopup != null && gameOverUIPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "FrontUI";
			Send();
		}

		GameObject CoinboxQuickPopup = GameObject.Find("CoinboxQuick(Clone)");
		if (CoinboxQuickPopup != null && CoinboxQuickPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "FrontUI";
			Send();
		}
	}

	private void OnStartPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
	{
		GameObject gameOverUIPopup = GameObject.Find("PauseUI(Clone)");
		if (gameOverUIPopup != null && gameOverUIPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "IngameUI";
			Send();
		}

		GameObject levelUpUIPopup = GameObject.Find("LevelUpPopup(Clone)");
		if (levelUpUIPopup != null && levelUpUIPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "IngameUI";
			Send();
		}

		GameObject boxOpenUIPopup = GameObject.Find("OK");
		if (boxOpenUIPopup != null && boxOpenUIPopup.activeInHierarchy)
		{
			//ScreenNameToOpen = "IngameUI";
			Send();
		}

		GameObject playerLevelPopup = GameObject.Find("PlayerLevelPopup(Clone)");
		if (playerLevelPopup != null && playerLevelPopup.activeInHierarchy)
		{
			//ScreenNameToOpen = "IngameUI";
			Send();
		}

	}


	private void CloseSubscribePopupIfActive()
	{
		GameObject subscribePopup = GameObject.Find("SubscribePopup(Clone)");
		//Debug.Log("Subscribe Popup in Button is..." + subscribePopup);
		if (subscribePopup != null && subscribePopup.activeInHierarchy)
		{
			UIScreenController.Instance.ClosePopupHandle("SubscribePopup(Clone)");
			//UIScreenController.Instance.ClosePopup(null);
			UIScreenController.Instance.ClosePopupHandle("SubscribePopup");
			Debug.Log("Subscribe Popup closed for" + subscribePopup);
			ScreenNameToOpen = "IngameUI";
			Send();
		}
	}

	public enum ScreenChangeType
	{
		PushScreen = 0,
		SwitchScreen = 1,
		None = 2,
		QueuePopup = 3,
		ClosePopup = 4,
		PushPopup = 5
	}

	public ScreenChangeType screenChangeType;

	public string ScreenNameToOpen;

	private bool useSend = true;

	public override void Send()
	{
		if (!useSend)
		{
			return;
		}
		if (ScreenNameToOpen == "PauseUI")
		{
			Game.Instance.wasButtonClicked = true;
		}
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (string.IsNullOrEmpty(ScreenNameToOpen) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
		{
			Debug.LogError(base.name + " tried to send an empty Change Screen message");
		}
		UIScreenController instance = UIScreenController.Instance;
		if (!(instance == null))
		{
			if (screenChangeType == ScreenChangeType.PushScreen)
			{
				instance.PushScreen(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.SwitchScreen)
			{
				instance.SwitchScreen(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.QueuePopup)
			{
				instance.QueuePopup(ScreenNameToOpen);
			}
			else if (screenChangeType == ScreenChangeType.PushPopup)
			{
				instance.PushPopup(ScreenNameToOpen);
			}
			if (screenChangeType == ScreenChangeType.ClosePopup)
			{
				instance.ClosePopup(ScreenNameToOpen);
			}
		}
	}
}
