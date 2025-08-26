using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("GUI/Interaction/Change Screen Button")]
public class UIButtonChangeScreen : UIBasicButton
{
	private InputActions inputActions;


	void Awake()
	{
		inputActions = new InputActions();
	}

	private void OnEnable()
	{
		CloseSubscribePopupIfActive();

		//Debug.Log("UIButtonChangeScreen connected to..." + gameObject.name);



		inputActions.Enable();
		inputActions.Play.CoinsShop.performed += OnCoinsShopPerformed;
		inputActions.Play.CharacterShop.performed += OnCharacterShopPerformed;
		inputActions.UI.Board.performed += OnBoardPerformed;
		inputActions.Play.Exit.performed += OnFrontUIPerformed;
		inputActions.Play.PlayGame.performed += OnStartPerformed;

		CloseSubscribePopupIfActive();
		SubscribePopupClose();
	}

	private void OnDisable()
	{
		inputActions.Disable();
		inputActions.Play.CoinsShop.performed -= OnCoinsShopPerformed;
		inputActions.Play.CharacterShop.performed -= OnCharacterShopPerformed;
		inputActions.UI.Board.performed -= OnBoardPerformed;
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

	private void OnBoardPerformed(InputAction.CallbackContext context)
	{

		GameObject FrontUIPopup = GameObject.Find("FrontUI(Clone)");
		if (FrontUIPopup != null && FrontUIPopup.activeInHierarchy)
		{

			ScreenNameToOpen = "HelmetPopup";
			screenChangeType = ScreenChangeType.QueuePopup;

			Send();
		}
	}

	private void SubscribePopupClose()
	{
		GameObject SubscribePopup = GameObject.Find("SubscribePopup(Clone)");
		if (SubscribePopup != null && SubscribePopup.activeInHierarchy)
		{
			screenChangeType = ScreenChangeType.ClosePopup;
			ScreenNameToOpen = "IngameUI";
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

	private void OnFrontUIPerformed(InputAction.CallbackContext context)
	{

		GameObject gameOverUIPopup = GameObject.Find("4Footer");
		Debug.Log("gameOverUIPopup in Button is..." + gameOverUIPopup);

		if (gameOverUIPopup != null && gameOverUIPopup.activeInHierarchy)
		{
			//screenChangeType = ScreenChangeType.ClosePopup;
			//ScreenNameToOpen = "GameoverUI";
			Send();
			Debug.Log("gameOverUIPopup button is clicked");
		}

		GameObject CoinboxQuickPopup = GameObject.Find("CoinboxQuick(Clone)");
		if (CoinboxQuickPopup != null && CoinboxQuickPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "FrontUI";
			screenChangeType = ScreenChangeType.PushScreen;
			Send();
		}

		GameObject HelmetPopup = GameObject.Find("HelmetPopup(Clone)");
		Debug.Log("Helmet object is " + gameObject.name);
		if (HelmetPopup != null && HelmetPopup.activeInHierarchy)
		{
			Debug.Log("Helmet is closed");
			ScreenNameToOpen = "HelmetPopup";
			screenChangeType = ScreenChangeType.ClosePopup;
			Send();
			Send();
		}


		GameObject TryHoverboardPopup = GameObject.Find("TryHoverboardPopup(Clone)");
		if (TryHoverboardPopup != null && TryHoverboardPopup.activeInHierarchy)
		{
			//ScreenNameToOpen = "FrontUI";
			screenChangeType = ScreenChangeType.ClosePopup;
			Send();
		}
		GameObject LevelUpPopup = GameObject.Find("LevelUpPopup(Clone)");
		if (LevelUpPopup != null && LevelUpPopup.activeInHierarchy)
		{

			screenChangeType = ScreenChangeType.ClosePopup;
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

		GameObject box_OpenPopup = GameObject.Find("Box_Open(Clone)");
		if (box_OpenPopup != null && box_OpenPopup.activeInHierarchy)
		{
			ScreenNameToOpen = "IngameUI";
			screenChangeType = ScreenChangeType.ClosePopup;
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


		GameObject LevelUpPopup = GameObject.Find("LevelUpPopup(Clone)");
		if (LevelUpPopup != null && LevelUpPopup.activeInHierarchy)
		{

			screenChangeType = ScreenChangeType.ClosePopup;
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
			Debug.Log("screenChangeType is: " + screenChangeType);

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
				Debug.Log("screenChangeType is: " + screenChangeType + ScreenNameToOpen);
				instance.ClosePopup(ScreenNameToOpen);
			}
		}
	}
}
