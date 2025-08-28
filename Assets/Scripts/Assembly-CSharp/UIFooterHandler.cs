using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class UIFooterHandler : MonoBehaviour
{
	public FootItem helm;

	public FootItem character;

	public FootItem upgrade;

	public FootItem store;

	public float currentIndex;

	private float direction = 0;

	private bool canPress = true;

	[SerializeField]
	private UISprite[] tips = new UISprite[3];

	private InputActions inputActions;

	public FootItem[] footItems;

	public IndexController indexController;

	void Awake()
	{
		inputActions = new InputActions();
	}
	private void Start()
	{
		//currentIndex = 1;


	}

	private void OnEnable()
	{
		indexController = FindObjectOfType<IndexController>();
		currentIndex = indexController.currentInd;
		OnButtonClick((int)currentIndex);
		UpdateTips();
		PurchaseHandler.Instance.AddOnUpgradePurchase(UpdateTips);
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Combine(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(UpdateTipsBY));

		inputActions.Enable();

		inputActions.UI.FooterMove.performed += MoveFooterButtons;

	}
	private void MoveFooterButtonsDirection(InputAction.CallbackContext obj)
	{

	}

	public void SetIndex()
	{
		FootItem[] footItemsNew = { character, helm, upgrade, store };

		footItems = footItemsNew;

		for (int i = 0; i < 4; i++)
		{
			footItems[i].index = i + 1;
		}
	}


	// public void AddInput()
	// {
	// 	if (inputActions == null)
	// 		inputActions = new InputActions();

	// 	//if(currentIndex == 1)
	// 	//OnButtonClick(1);

	// 	inputActions.Enable();

	// 	//inputActions.UI.FooterMove.performed+= MoveFooterButtonsDirection;

	// 	inputActions.UI.FooterMove.performed += MoveFooterButtons;
	// }

	private void MoveFooterButtons(InputAction.CallbackContext obj)
	{
		//if (canPress)
		//{
		//canPress = false;

		direction = inputActions.UI.FooterMove.ReadValue<float>();

		Debug.Log("CurrentIndexValue:" + inputActions.UI.FooterMove.ReadValue<float>());

		currentIndex += direction;

		currentIndex = Mathf.Clamp(currentIndex, 1, 4);

		indexController.currentInd = currentIndex;

		OnButtonPress((int)currentIndex);

		OnButtonClick((int)currentIndex);

		//StartCoroutine(ButtonClickDelay());

		Debug.Log("CurrentIndex:" + currentIndex + direction);
		//}
	}

	private void OnDisable()
	{
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(UpdateTips);
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Remove(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(UpdateTipsBY));


		inputActions.Disable();

		inputActions.UI.FooterMove.performed -= MoveFooterButtons;
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

	public IEnumerator ButtonClickDelay()
	{
		yield return new WaitForSeconds(0.5f);

		canPress = true;
	}


	public void OnButtonClick(int selected)
	{
		InitButtonType();

		Debug.Log("CurrentIndex2:" + selected);
		if (canPress)
		{
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

	public void OnButtonPress(int selected)
	{

		Debug.Log("CurrentIndex3:" + selected);

		switch (selected)
		{
			case 1:
				Debug.Log("CurrentIndexC:" + selected);
				character.GetParentBuuton().Send();
				break;
			case 2:
				Debug.Log("CurrentIndexH:" + selected);
				helm.GetParentBuuton().Send();
				break;
			case 3:
				Debug.Log("CurrentIndexU:" + selected);
				upgrade.GetParentBuuton().Send();
				break;
			case 4:
				Debug.Log("CurrentIndexS:" + selected);
				store.GetParentBuuton().Send();
				break;
			default:
				Debug.Log("No button was selected in the footer?", this);
				break;
		}
	}
}
