using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStats
{
	public delegate void CoinsChangedIngame();

	public float duration;

	public Action OnCoinsChanged;

	public Action OnGameOverPlayLotteryCountIncreased;

	public Action OnScoreChanged;

	public Action OnJumpsChanged;

	public Action OnRollsChanged;

	public Action OnChestsChanged;

	public Action OnCoinsWithHelmetChanged;

	public Action<float> OnHelmetInCooling;

	public float meters;

	public float metersRunLeftTrack;

	public float metersRunCenterTrack;

	public float metersRunRightTrack;

	public float metersFly;

	public float metersRunGround;

	public float metersRunTrain;

	public float metersRunStation;

	public int trackChanges;

	private int _reviveCount;

	private int _allCoinsInFlypack;

	private int _amountLeftToConsume;

	private int _barrierHit;

	private int _coinCollectedOnLeftTrack;

	private int _coinMagnetsPickups;

	private int _coinsCoinMagnet;

	private int _coinsCollectedOnCenterTrack;

	private int _coinsCollectedOnRightTrack;

	private int _coinsInAir;

	private int _coinsNotTouchingGround;

	private int _coinsWithSpringJump;

	private int _coinsWithHelmet;

	private int _rollUnderBarrier;

	private int _doubleMultiplierPickups;

	private int _grindedTrains;

	private int _guardHitScreen;

	private int _guardFallWater;

	private int _flypackPickups;

	private int _jumpBarrier;

	private int _jumpHighBarrier;

	private int _jumps;

	private List<ActiveProp> _listOfActivePowerups = new List<ActiveProp>();

	private float _meterScore;

	private float _metersLastUsedForScore;

	private Dictionary<PropType, bool> _taskEachPowerupPickupStatus = new Dictionary<PropType, bool>();

	private int _trainHit;

	private int _busHit;

	private int _carHit;

	private int _flowerBedHit;

	private int _movingTrainHit;

	private int _movingBusHit;

	private int _movingCarHit;

	private int _jumpsOverTrains;

	private int _jumpsOverBuses;

	private int _jumpsOverCars;

	private int _fallIntoWater;

	private int _chestPickups;

	private int _pickedUpPowerups;

	private int _powerJumperPickups;

	private int _rolls;

	private int _rollsCenterTrack;

	private int _rollsLeftTrack;

	private int _rollsRightTrack;

	private int _saveMeSymbolPickup;

	private int _score;

	private bool _scoreBooster5Activated;

	private bool _scoreBooster10Activated;

	private int _superChestPickups;

	private int _superShoesPickups;

	private int _xoredNumberOfCoins;

	private static GameStats instance;

	private bool pausePowerups;

	private int _gameOverPlayLotteryCount;

	public int allCoinsInFlypack
	{
		get
		{
			return _allCoinsInFlypack;
		}
		set
		{
			_allCoinsInFlypack = value;
		}
	}

	public int barrierHit
	{
		get
		{
			return _barrierHit;
		}
		set
		{
			_barrierHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashBarriers);
			}
		}
	}

	public int coinMagnetsPickups
	{
		get
		{
			return _coinMagnetsPickups;
		}
		set
		{
			_coinMagnetsPickups = value;
			if (value == 0)
			{
				if (_taskEachPowerupPickupStatus.ContainsKey(PropType.coinmagnet))
				{
					_taskEachPowerupPickupStatus[PropType.coinmagnet] = false;
				}
				else
				{
					_taskEachPowerupPickupStatus.Add(PropType.coinmagnet, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PropType.coinmagnet);
				TasksManager.Instance.PlayerDidThis(TaskTarget.Magnets);
			}
		}
	}

	public int coins
	{
		get
		{
			return Utils.EncryptDecryptXORValue(_xoredNumberOfCoins);
		}
		set
		{
			_xoredNumberOfCoins = Utils.EncryptDecryptXORValue(value);
			Action onCoinsChanged = OnCoinsChanged;
			if (onCoinsChanged != null)
			{
				onCoinsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, (!PlayerInfo.Instance.hasSubscribed) ? 1 : 2);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoCoinsWithoutScore);
			}
		}
	}

	public int coinsCoinMagnet
	{
		get
		{
			return _coinsCoinMagnet;
		}
		set
		{
			_coinsCoinMagnet = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CoinsWithMagnet);
			}
		}
	}

	public int coinsCollectedOnCenterTrack
	{
		get
		{
			return _coinsCollectedOnCenterTrack;
		}
		set
		{
			_coinsCollectedOnCenterTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsCenterLane);
			}
		}
	}

	public int coinsCollectedOnLeftTrack
	{
		get
		{
			return _coinCollectedOnLeftTrack;
		}
		set
		{
			_coinCollectedOnLeftTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsLeftLane);
			}
		}
	}

	public int coinsCollectedOnRightTrack
	{
		get
		{
			return _coinsCollectedOnRightTrack;
		}
		set
		{
			_coinsCollectedOnRightTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsRightLane);
			}
		}
	}

	public int coinsInAir
	{
		get
		{
			return _coinsInAir;
		}
		set
		{
			_coinsInAir = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoinWithoutTouchingGround);
			}
		}
	}

	public int coinsNotTouchingGround
	{
		get
		{
			return _coinsNotTouchingGround;
		}
		set
		{
			_coinsNotTouchingGround = value;
		}
	}

	public int coinsWithFlypack
	{
		get
		{
			return _allCoinsInFlypack;
		}
		set
		{
			_allCoinsInFlypack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CoinsWithJetpack);
			}
		}
	}

	public int coinsWithSpringJump
	{
		get
		{
			return _coinsWithSpringJump;
		}
		set
		{
			_coinsWithSpringJump = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsWithPowerJumper);
			}
		}
	}

	public int coinsWithHelmet
	{
		get
		{
			return _coinsWithHelmet;
		}
		set
		{
			if (value != _coinsWithHelmet)
			{
				_coinsWithHelmet = value;
				if (OnCoinsWithHelmetChanged != null)
				{
					OnCoinsWithHelmetChanged();
				}
			}
		}
	}

	public int reviveCount
	{
		get
		{
			return _reviveCount;
		}
		set
		{
			_reviveCount = value;
		}
	}

	public int rollUnderBarrier
	{
		get
		{
			return _rollUnderBarrier;
		}
		set
		{
			_rollUnderBarrier = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollUnderBarriers);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DodgeBarriers);
			}
		}
	}

	public int doubleMultiplierPickups
	{
		get
		{
			return _doubleMultiplierPickups;
		}
		set
		{
			_doubleMultiplierPickups = value;
			if (value == 0)
			{
				if (_taskEachPowerupPickupStatus.ContainsKey(PropType.doubleMultiplier))
				{
					_taskEachPowerupPickupStatus[PropType.doubleMultiplier] = false;
				}
				else
				{
					_taskEachPowerupPickupStatus.Add(PropType.doubleMultiplier, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PropType.doubleMultiplier);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DoubleMultiplier);
			}
		}
	}

	public int grindedTrains
	{
		get
		{
			return _grindedTrains;
		}
		set
		{
			_grindedTrains = value;
		}
	}

	public int guardHitScreen
	{
		get
		{
			return _guardHitScreen;
		}
		set
		{
			_guardHitScreen = value;
		}
	}

	public int guardFallWater
	{
		get
		{
			return _guardFallWater;
		}
		set
		{
			_guardFallWater = value;
		}
	}

	public static GameStats Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameStats();
			}
			return instance;
		}
	}

	public int flypackPickups
	{
		get
		{
			return _flypackPickups;
		}
		set
		{
			_flypackPickups = value;
			if (value == 0)
			{
				if (_taskEachPowerupPickupStatus.ContainsKey(PropType.flypack))
				{
					_taskEachPowerupPickupStatus[PropType.flypack] = false;
				}
				else
				{
					_taskEachPowerupPickupStatus.Add(PropType.flypack, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PropType.flypack);
				TasksManager.Instance.PlayerDidThis(TaskTarget.Jetpack);
			}
		}
	}

	public int jumpBarrier
	{
		get
		{
			return _jumpBarrier;
		}
		set
		{
			_jumpBarrier = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpBarriers);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DodgeBarriers);
			}
		}
	}

	public int jumpHighBarrier
	{
		get
		{
			return _jumpHighBarrier;
		}
		set
		{
			_jumpHighBarrier = value;
		}
	}

	public int jumps
	{
		get
		{
			return _jumps;
		}
		set
		{
			_jumps = value;
			Action onJumpsChanged = OnJumpsChanged;
			if (onJumpsChanged != null)
			{
				onJumpsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.Jump);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoJumpsWithoutScore);
			}
		}
	}

	public Dictionary<PropType, bool> TaskEachPowerupPickupStatus
	{
		get
		{
			return _taskEachPowerupPickupStatus;
		}
	}

	public int fallIntoWater
	{
		get
		{
			return _fallIntoWater;
		}
		set
		{
			_fallIntoWater = value;
		}
	}

	public int gameOverPlayLotteryCount
	{
		get
		{
			return _gameOverPlayLotteryCount;
		}
		set
		{
			_gameOverPlayLotteryCount = value;
			if (value >= 2 && OnGameOverPlayLotteryCountIncreased != null)
			{
				OnGameOverPlayLotteryCountIncreased();
			}
		}
	}

	public int chestPickups
	{
		get
		{
			return _chestPickups;
		}
		set
		{
			if (value != _chestPickups)
			{
				_chestPickups = value;
				if (OnChestsChanged != null)
				{
					OnChestsChanged();
				}
				if (value != 0)
				{
					TasksManager.Instance.PlayerDidThis(TaskTarget.Chestes);
				}
			}
		}
	}

	public bool PAUSEPOWERUPS
	{
		get
		{
			return pausePowerups;
		}
		set
		{
			pausePowerups = value;
		}
	}

	public int pickedUpPowerups
	{
		get
		{
			return _pickedUpPowerups;
		}
		set
		{
			_pickedUpPowerups = value;
			if (value != 0)
			{
				PlayerInfo.Instance.stats[Stat.PickupPowerup]++;
				TasksManager.Instance.PlayerDidThis(TaskTarget.Powerups);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoPowerUpsWithoutScore);
			}
		}
	}

	public int powerJumperPickups
	{
		get
		{
			return _powerJumperPickups;
		}
		set
		{
			_powerJumperPickups = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.PickUpPowerJumpers);
			}
		}
	}

	public int rolls
	{
		get
		{
			return _rolls;
		}
		set
		{
			_rolls = value;
			Action onRollsChanged = OnRollsChanged;
			if (onRollsChanged != null)
			{
				onRollsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.Roll);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoRollsWithoutScore);
			}
		}
	}

	public int rollsCenterTrack
	{
		get
		{
			return _rollsCenterTrack;
		}
		set
		{
			_rollsCenterTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollCenter);
			}
		}
	}

	public int rollsLeftTrack
	{
		get
		{
			return _rollsLeftTrack;
		}
		set
		{
			_rollsLeftTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollLeft);
			}
		}
	}

	public int rollsRightTrack
	{
		get
		{
			return _rollsRightTrack;
		}
		set
		{
			_rollsRightTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollRight);
			}
		}
	}

	public int saveMeSymbolPickup
	{
		get
		{
			return _saveMeSymbolPickup;
		}
		set
		{
			_saveMeSymbolPickup = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.PickupKeys);
			}
		}
	}

	public int score
	{
		get
		{
			int num = ~(_score ^ 0x1CC5);
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			_score = ~value ^ 0x1CC5;
		}
	}

	public bool scoreBooster5Activated
	{
		get
		{
			return _scoreBooster5Activated;
		}
		set
		{
			_scoreBooster5Activated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public bool scoreBooster10Activated
	{
		get
		{
			return _scoreBooster10Activated;
		}
		set
		{
			_scoreBooster10Activated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public int superChestPickups
	{
		get
		{
			return _superChestPickups;
		}
		set
		{
			_superChestPickups = value;
		}
	}

	public int superShoesPickups
	{
		get
		{
			return _superShoesPickups;
		}
		set
		{
			_superShoesPickups = value;
			if (value == 0)
			{
				if (_taskEachPowerupPickupStatus.ContainsKey(PropType.supershoes))
				{
					_taskEachPowerupPickupStatus[PropType.supershoes] = false;
				}
				else
				{
					_taskEachPowerupPickupStatus.Add(PropType.supershoes, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PropType.supershoes);
				TasksManager.Instance.PlayerDidThis(TaskTarget.SuperSneakers);
			}
		}
	}

	public int jumpsOverTrains
	{
		get
		{
			return _jumpsOverTrains;
		}
		set
		{
			_jumpsOverTrains = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpTrain);
			}
		}
	}

	public int jumpsOverBuses
	{
		get
		{
			return _jumpsOverBuses;
		}
		set
		{
			_jumpsOverBuses = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpBus);
			}
		}
	}

	public int jumpsOverCars
	{
		get
		{
			return _jumpsOverCars;
		}
		set
		{
			_jumpsOverCars = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpCar);
			}
		}
	}

	public int trainHit
	{
		get
		{
			return _trainHit;
		}
		set
		{
			_trainHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashTrains);
			}
		}
	}

	public int movingTrainHit
	{
		get
		{
			return _movingTrainHit;
		}
		set
		{
			_movingTrainHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashTrains);
			}
		}
	}

	public int busHit
	{
		get
		{
			return _busHit;
		}
		set
		{
			_busHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashBuses);
			}
		}
	}

	public int movingBusHit
	{
		get
		{
			return _movingBusHit;
		}
		set
		{
			_movingBusHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashBuses);
			}
		}
	}

	public int carHit
	{
		get
		{
			return _carHit;
		}
		set
		{
			_carHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashCars);
			}
		}
	}

	public int movingCarHit
	{
		get
		{
			return _movingCarHit;
		}
		set
		{
			_movingCarHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashCars);
			}
		}
	}

	public int flowerBedHit
	{
		get
		{
			return _flowerBedHit;
		}
		set
		{
			_flowerBedHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashFlowerBeds);
			}
		}
	}

	private void _AddScoreForPickup(bool wasPowerup)
	{
		int num = PlayerInfo.Instance.scoreMultiplier * 30;
		score += num;
		TasksManager.Instance.PlayerDidThis(TaskTarget.Score, num);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoCoinsWithoutScore, num);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoRollsWithoutScore, num);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoJumpsWithoutScore, num);
		if (!wasPowerup)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoPowerUpsWithoutScore, num);
		}
	}

	public void AddScoreForPickup(PropType type)
	{
		switch (type)
		{
		case PropType.chest:
		case PropType.letters:
			_AddScoreForPickup(false);
			break;
		case PropType.flypack:
		case PropType.supershoes:
		case PropType.coinmagnet:
		case PropType.doubleMultiplier:
			_AddScoreForPickup(true);
			break;
		case PropType.superchest:
			break;
		}
	}

	public void CalculateScore()
	{
		if (_metersLastUsedForScore < meters)
		{
			_meterScore = meters - _metersLastUsedForScore;
			_metersLastUsedForScore = meters;
			int num = (int)(_meterScore * (float)PlayerInfo.Instance.scoreMultiplier);
			score += num;
			TasksManager.Instance.PlayerDidThis(TaskTarget.Score, num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoCoinsWithoutScore, num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoRollsWithoutScore, num);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoJumpsWithoutScore, num);
			if (_listOfActivePowerups.Count == 0 || (_listOfActivePowerups.Count == 1 && _listOfActivePowerups[0].type == PropType.helmet))
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.NoPowerUpsWithoutScore, num);
			}
			if (scoreBooster5Activated)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.ScoreWithScorebooster, num);
			}
			Action onScoreChanged = OnScoreChanged;
			if (onScoreChanged != null)
			{
				onScoreChanged();
			}
		}
	}

	public void ClearPowerups()
	{
		_listOfActivePowerups.Clear();
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
	}

	public static int CoinToScoreConversion(int coins)
	{
		return coins * 2 * PlayerInfo.Instance.rawMultiplier;
	}

	public List<ActiveProp> GetActivePowerups()
	{
		return _listOfActivePowerups;
	}

	public void RemoveHoverHelmPowerup()
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == PropType.helmet)
			{
				_listOfActivePowerups[num].timeLeft = 0f;
			}
		}
	}

	private void ReportOneOfEachPowerupIfApplicable(PropType powerupType)
	{
		if (TasksManager.Instance.IsTaskTargetActive(TaskTarget.OneOfEachPowerup) && _taskEachPowerupPickupStatus.ContainsKey(powerupType) && !_taskEachPowerupPickupStatus[powerupType])
		{
			_taskEachPowerupPickupStatus[powerupType] = true;
			TasksManager.Instance.PlayerDidThis(TaskTarget.OneOfEachPowerup);
		}
	}

	public void Reset()
	{
		duration = 0f;
		ResetScore();
		coins = 0;
		coinsCoinMagnet = 0;
		coinsWithHelmet = 0;
		coinsWithFlypack = 0;
		allCoinsInFlypack = 0;
		scoreBooster5Activated = false;
		scoreBooster10Activated = false;
		meters = 0f;
		metersRunLeftTrack = 0f;
		metersRunCenterTrack = 0f;
		metersRunRightTrack = 0f;
		metersRunGround = 0f;
		metersRunTrain = 0f;
		metersRunStation = 0f;
		metersFly = 0f;
		grindedTrains = 0;
		jumps = 0;
		rolls = 0;
		rollsLeftTrack = 0;
		rollsCenterTrack = 0;
		rollsRightTrack = 0;
		trackChanges = 0;
		rollUnderBarrier = 0;
		jumpBarrier = 0;
		jumpHighBarrier = 0;
		fallIntoWater = 0;
		jumpsOverTrains = 0;
		jumpsOverBuses = 0;
		jumpsOverCars = 0;
		trainHit = 0;
		movingTrainHit = 0;
		busHit = 0;
		movingBusHit = 0;
		carHit = 0;
		movingCarHit = 0;
		flowerBedHit = 0;
		guardHitScreen = 0;
		guardFallWater = 0;
		barrierHit = 0;
		flypackPickups = 0;
		superShoesPickups = 0;
		coinMagnetsPickups = 0;
		_chestPickups = 0;
		pickedUpPowerups = 0;
		doubleMultiplierPickups = 0;
		gameOverPlayLotteryCount = 0;
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		reviveCount = model.freeReviveCount;
	}

	public void ResetScore()
	{
		score = 0;
		_metersLastUsedForScore = 0f;
		_meterScore = 0f;
	}

	public ActiveProp RegisterPowerup(PropType type)
	{
		ActiveProp activeProp = new ActiveProp();
		activeProp.type = type;
		activeProp.timeActivated = Time.time;
		activeProp.timeLeft = PlayerInfo.Instance.GetPowerupDuration(type);
		if (type == PropType.headstart2000 || type == PropType.headstart500 || type == PropType.headstartLong || type == PropType.headstart)
		{
			activeProp.timeLeft = 0f;
		}
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == activeProp.type)
			{
				_listOfActivePowerups.RemoveAt(num);
			}
		}
		AddScoreForPickup(type);
		_listOfActivePowerups.Add(activeProp);
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
		TasksManager.Instance.PlayerDidThis(TaskTarget.ActivePowerups, _listOfActivePowerups.Count);
		return activeProp;
	}

	public void UpdatePowerupTimes(float deltaTime)
	{
		if (pausePowerups)
		{
			return;
		}
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if ((Game.Instance.IsInFlypackMode || Game.Instance.IsInSpringJumpMode || Game.Instance.IsInBoundJumpMode) && (_listOfActivePowerups[num].type == PropType.helmet || _listOfActivePowerups[num].type == PropType.supershoes))
			{
				continue;
			}
			ActiveProp activeProp = _listOfActivePowerups[num];
			activeProp.timeLeft -= deltaTime;
			if (!(_listOfActivePowerups[num].timeLeft < 0f) || (Game.Instance.IsInFlypackMode && _listOfActivePowerups[num].type == PropType.flypack))
			{
				continue;
			}
			if (_listOfActivePowerups[num].type == PropType.helmet)
			{
				float num2 = Helmet.Instance.WaitForParticlesDelay + PlayerInfo.Instance.GetHelmCoolDown();
				if (_listOfActivePowerups[num].timeLeft > 0f - num2)
				{
					if (OnHelmetInCooling != null)
					{
						OnHelmetInCooling(_listOfActivePowerups[num].timeLeft / num2 + 1f);
					}
					continue;
				}
				Helmet.Instance.HardReset();
			}
			_listOfActivePowerups.RemoveAt(num);
			TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
			TasksManager.Instance.PlayerDidThis(TaskTarget.ActivePowerups, _listOfActivePowerups.Count);
		}
	}
}
