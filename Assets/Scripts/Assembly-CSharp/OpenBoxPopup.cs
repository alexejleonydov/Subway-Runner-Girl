using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenBoxPopup : UIBaseScreen
{
	[SerializeField]
	private UILabel skipLbl;

	[SerializeField]
	private GameObject boxParent;

	[SerializeField]
	private Animation numberAnim;

	[SerializeField]
	private UILabel numberLbl;

	[SerializeField]
	private UILabel tapToCollectLabel;

	[SerializeField]
	private Collider openButton;

	[SerializeField]
	private Collider skipButton;

	[SerializeField]
	private UIChestReward uiChestReward;

	[SerializeField]
	private UIFinishPanel finishPanel;

	[SerializeField]
	private ParticleSystem appearPs;

	[SerializeField]
	private ParticleSystem openPs;

	[SerializeField]
	private ParticleSystem endPs;

	[SerializeField]
	private Fly fly;

	private PrizeEntry[] _priceEntries;

	private ChestType _chestType;

	private Chest _currentChest;

	private int _openCount;

	private List<int> _gameChestIndexs;

	private PrizeEntry _currentEntry;

	private GameObject _currentChestBox;

	private Animation _chestBoxAnim;

	private CoinBoxSizer coinBoxSizer;

	private bool _isFingerPressed;

	private bool _openHasStarted;

	private InputActions inputActions;

	public override void Init()
	{
		base.Init();
		coinBoxSizer = InitializeCoinbox(false, true, true, false);
	}


	void OnEnable()
	{
		inputActions = new InputActions();

		inputActions.Enable();
		inputActions.Play.PlayGame.performed += OnOpenChestPressed;
		inputActions.Play.Exit.performed += OnSkipChestPressed;

		finishPanel.gameObject.SetActive(false);
	}

	void OnDisable()
	{
		inputActions.Disable();
		inputActions.Play.PlayGame.performed -= OnOpenChestPressed;
		inputActions.Play.Exit.performed -= OnSkipChestPressed;
	}

	private void OnOpenChestPressed(InputAction.CallbackContext context)
	{
		if (openButton.enabled)
		{
			OnPressed();
			OnReleased();
		}
		finishPanel.gameObject.SetActive(true);
		openButton.enabled = false;
	}

	private void OnSkipChestPressed(InputAction.CallbackContext context)
	{
		if (skipButton.enabled)
		{
			Skip();
		}
	}


	public override void Show()
	{
		base.Show();
		First();
		SetupOpenBox();
	}

	private void First()
	{
		skipLbl.text = Strings.Get(LanguageKey.BOX_OPEN_SKIP);
		_chestType = ShopManager.Instance.chestType;
		_currentChest = ChestsData.GetChest(_chestType);
		_priceEntries = _currentChest.Roll();
		if (_chestType == ChestType.Game)
		{
			_gameChestIndexs = new List<int>(_currentChest.entryCount);
			_openCount = 0;
			int num = GameStats.Instance.chestPickups;
			int num2 = 0;
			while (num > 0)
			{
				if (num % 2 == 1)
				{
					_gameChestIndexs.Add(num2);
					_openCount++;
				}
				num /= 2;
				num2++;
			}
		}
		else
		{
			_openCount = _currentChest.entryCount;
		}
		for (int i = 0; i < _openCount; i++)
		{
			if (_chestType == ChestType.Game)
			{
				int num3 = _gameChestIndexs[i];
				PayoutReward(_priceEntries[num3]);
			}
			else
			{
				PayoutReward(_priceEntries[i]);
			}
		}
		numberLbl.text = _openCount.ToString();
		numberAnim.gameObject.SetActive(false);
		tapToCollectLabel.text = Strings.Get(LanguageKey.CELEBRATION_POPUP_CONTINUE);
		tapToCollectLabel.alpha = 0f;
		openButton.enabled = false;
		finishPanel.gameObject.SetActive(false);

		skipLbl.alpha = 0f;
		skipButton.enabled = false;
		if (_chestType == ChestType.Game)
		{
			PrizeEntry[] array = new PrizeEntry[_openCount];
			for (int j = 0; j < _openCount; j++)
			{
				int num4 = _gameChestIndexs[j];
				array[j] = _priceEntries[num4];
			}
			finishPanel.InitUIChestRewards(array);
		}
		else
		{
			finishPanel.InitUIChestRewards(_priceEntries);
		}
		uiChestReward.transform.localScale = Vector3.zero;
		StartCoroutine(InitChestBox());
	}

	private void SetupOpenBox()
	{
		if (_chestType == ChestType.Game)
		{
			int num = _gameChestIndexs[_gameChestIndexs.Count - _openCount];
			_currentEntry = _priceEntries[num];
		}
		else
		{
			_currentEntry = _priceEntries[_priceEntries.Length - _openCount];
		}
	}

	private IEnumerator InitChestBox()
	{
		_currentChestBox = NGUITools.AddChild(boxParent, ChestModelFactory.Instance.GetChestForOpen(_chestType));
		_currentChestBox.transform.localPosition = Vector3.zero;
		_currentChestBox.transform.localScale = Vector3.one;
		_currentChestBox.transform.localRotation = Quaternion.identity;
		Utility.SetLayerRecursively(_currentChestBox.transform, boxParent.layer);
		InitAssets.Instance.SetChestBox(_currentChestBox.GetComponentsInChildren<MeshRenderer>(), new Vector4(0f, 0f, 1f, 1f), 1f);
		AudioPlayer.Instance.PlaySound("leyou_Hr_ChestOpen", 0.5f, 0.5f, 1f);
		_currentChestBox.SetActive(false);
		if (appearPs != null)
		{
			appearPs.Play();
		}
		float factor = 0f;
		while (factor < 0.1f)
		{
			factor += Time.deltaTime;
			yield return null;
		}
		numberAnim.gameObject.SetActive(true);
		numberAnim.Play();
		_chestBoxAnim = _currentChestBox.GetComponentInChildren<Animation>();
		_currentChestBox.SetActive(true);
		if (endPs != null)
		{
			endPs.Play();
		}
		if (!(_chestBoxAnim == null) && !(_chestBoxAnim["Appear"] == null))
		{
			_chestBoxAnim.Play("Appear");
			while (_chestBoxAnim["Appear"].enabled && _chestBoxAnim["Appear"].normalizedTime < 0.9f)
			{
				yield return null;
			}
		}
		if (appearPs != null)
		{
			appearPs.Stop();
			appearPs.Clear();
		}
		openButton.enabled = true;
		skipButton.enabled = true;
		StartCoroutine(AnimateAlpha(tapToCollectLabel, 0.5f, 1f));
		StartCoroutine(AnimateAlpha(skipLbl, 0.5f, 1f));
	}

	public void OnPressed()
	{
		if (!_isFingerPressed)
		{
			_isFingerPressed = true;
			if (_chestBoxAnim != null && _chestBoxAnim["Open"] != null)
			{
				_chestBoxAnim.Play("Open");
			}
		}
	}

	public void OnReleased()
	{
		if (!_openHasStarted)
		{
			openButton.enabled = false;
			_openCount--;
			numberLbl.text = _openCount.ToString();
			if (_openCount == 0)
			{
				numberAnim.gameObject.SetActive(false);
			}
			StartCoroutine(ShowReward());
			_openHasStarted = true;
		}
	}

	private IEnumerator ShowReward()
	{
		uiChestReward.transform.localScale = Vector3.zero;
		yield return null;
		AudioPlayer.Instance.PlaySound("kapai_open", true);
		uiChestReward.Show(_currentEntry.itemType, _currentEntry.min);
		int frame3 = 0;
		while (frame3 < 4)
		{
			frame3++;
			yield return null;
		}
		if (openPs != null)
		{
			openPs.Play();
		}
		float time = uiChestReward.Appear();
		float factor = 0f;
		while (factor < time)
		{
			factor += Time.deltaTime;
			yield return null;
		}
		bool end = _openCount == 0;
		if (_currentEntry.itemType == PrizeEntryType.LEB || _currentEntry.itemType == PrizeEntryType.MEB || _currentEntry.itemType == PrizeEntryType.SEB)
		{
			if (!end)
			{
				StartCoroutine(fly.Begain());
			}
			else
			{
				yield return StartCoroutine(fly.Begain());
			}
		}
		if (uiChestReward.InitProgress(true))
		{
			int count = _currentEntry.min;
			int number = 0;
			while (number < count)
			{
				number++;
				uiChestReward.RefreshProgress(number);
				frame3 = 0;
				while (frame3 < 5)
				{
					frame3++;
					yield return null;
				}
			}
		}
		else if (_currentEntry.itemType == PrizeEntryType.Coin || _currentEntry.itemType == PrizeEntryType.Key)
		{
			StartCoroutine(CountUp(_currentEntry.itemType, _currentEntry.min));
		}
		else if (_currentEntry.itemType == PrizeEntryType.LEB || _currentEntry.itemType == PrizeEntryType.MEB || _currentEntry.itemType == PrizeEntryType.SEB)
		{
			PrizeEntryTemplate prizeEntryTemplate = ChestsData.GetPrizeEntryTemplate(_currentEntry.itemType);
			if (prizeEntryTemplate != null)
			{
				StartCoroutine(CountUp(_currentEntry.itemType, _currentEntry.min * prizeEntryTemplate.ExpPoint));
			}
		}
		if (openPs != null)
		{
			openPs.Stop();
			openPs.Clear();
		}
		frame3 = 0;
		while (frame3 < 10)
		{
			frame3++;
			yield return null;
		}
		FinishOpen();
	}

	private IEnumerator CountUp(PrizeEntryType itemType, int amount)
	{
		float countFactor = 0f;
		float countTime = Mathf.Lerp(1.5f, 3f, (float)amount / 1000f);
		int from = 0;
		int count2 = 0;
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			count2 = Mathf.RoundToInt(Mathf.SmoothStep(from, amount, countFactor));
			switch (itemType)
			{
				case PrizeEntryType.Coin:
					coinBoxSizer.AddCoins(count2);
					break;
				case PrizeEntryType.Key:
					coinBoxSizer.AddKeys(count2);
					break;
				case PrizeEntryType.SEB:
				case PrizeEntryType.MEB:
				case PrizeEntryType.LEB:
					coinBoxSizer.AddExps(count2);
					break;
			}
			yield return null;
		}
		switch (itemType)
		{
			case PrizeEntryType.Coin:
				coinBoxSizer.AddOriginCoins(amount);
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, amount);
				break;
			case PrizeEntryType.Key:
				coinBoxSizer.AddOriginKeys(amount);
				break;
			case PrizeEntryType.SEB:
			case PrizeEntryType.MEB:
			case PrizeEntryType.LEB:
				coinBoxSizer.AddOriginExps(amount);
				break;
		}
	}

	public void Skip()
	{
		StopAllCoroutines();
		uiChestReward.Stop();
		_openCount = 0;
		numberAnim.gameObject.SetActive(false);
		FinishOpen();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && openButton.enabled)
		{
			OnPressed();
			OnReleased();
		}
	}

	private void FinishOpen()
	{
		_openHasStarted = false;
		_isFingerPressed = false;
		if (_openCount == 0)
		{
			if (endPs != null)
			{
				endPs.Stop();
				endPs.Clear();
			}
			openButton.enabled = false;
			skipLbl.alpha = 0f;
			skipButton.enabled = false;
			Object.DestroyObject(_currentChestBox);
			uiChestReward.transform.localScale = Vector3.zero;
			tapToCollectLabel.alpha = 0f;
			finishPanel.Show();
		}
		else
		{
			openButton.enabled = true;
			SetupOpenBox();
		}
	}

	private void PayoutReward(PrizeEntry prizeEntry)
	{
		if (prizeEntry.itemType == PrizeEntryType.Coin)
		{
			PlayerInfo.Instance.amountOfCoins += prizeEntry.min;
			TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, prizeEntry.min);
			if (_chestType == ChestType.Game)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_game_chest", 0, prizeEntry.min);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_shop_chest", 0, prizeEntry.min);
			}
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_coins_total", 0, prizeEntry.min);
		}
		else if (prizeEntry.itemType == PrizeEntryType.Key)
		{
			PlayerInfo.Instance.amountOfKeys += prizeEntry.min;
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_total", 0, prizeEntry.min);
			if (_chestType == ChestType.Game)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_game_chest", 0, prizeEntry.min);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "get_gems_shop_chest", 0, prizeEntry.min);
			}
		}
		else
		{
			PrizeEntryTemplate prizeEntryTemplate = ChestsData.GetPrizeEntryTemplate(prizeEntry.itemType);
			prizeEntryTemplate.PayoutReward(prizeEntry.min);
		}
		PlayerInfo.Instance.SaveIfDirty();
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor2 = 0f;
		while (factor2 < 1f)
		{
			factor2 += Time.deltaTime / duration;
			factor2 = Mathf.Clamp01(factor2);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor2);
			yield return null;
		}
	}
}
