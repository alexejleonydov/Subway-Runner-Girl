using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	private Character character;

	private GameStats stats;

	private void OnChangeTrackDir(Character.OnChangeTrackDirection direction)
	{
		stats.trackChanges++;
	}

	private void OnCriticalHit(Character.CriticalHitType type)
	{
		switch (type)
		{
		case Character.CriticalHitType.Train:
			stats.trainHit++;
			break;
		case Character.CriticalHitType.Barrier:
			stats.barrierHit++;
			break;
		case Character.CriticalHitType.MovingTrain:
			stats.movingTrainHit++;
			break;
		case Character.CriticalHitType.FallIntoWater:
			stats.fallIntoWater++;
			break;
		case Character.CriticalHitType.Bus:
			stats.busHit++;
			break;
		case Character.CriticalHitType.MovingBus:
			stats.movingBusHit++;
			break;
		case Character.CriticalHitType.Car:
			stats.carHit++;
			break;
		case Character.CriticalHitType.MovingCar:
			stats.movingCarHit++;
			break;
		case Character.CriticalHitType.FlowerBed:
			stats.flowerBedHit++;
			break;
		}
	}

	private void OnJump()
	{
		stats.jumps++;
	}

	private void OnJumpOverTrain()
	{
		stats.jumpsOverTrains++;
	}

	private void OnJumpOverBus()
	{
		stats.jumpsOverBuses++;
	}

	private void OnJumpOverCar()
	{
		stats.jumpsOverCars++;
	}

	private void OnPassedObstacle(Character.ObstacleType type)
	{
		switch (type)
		{
		case Character.ObstacleType.JumpHighBarrier:
			stats.jumpBarrier++;
			stats.jumpHighBarrier++;
			break;
		case Character.ObstacleType.RollBarrier:
			stats.rollUnderBarrier++;
			break;
		case Character.ObstacleType.JumpBarrier:
			stats.jumpBarrier++;
			break;
		}
	}

	private void OnRoll()
	{
		stats.rolls++;
		if (character.TrackIndex == 0)
		{
			stats.rollsLeftTrack++;
		}
		if (character.TrackIndex == 1)
		{
			stats.rollsCenterTrack++;
		}
		if (character.TrackIndex == 2)
		{
			stats.rollsRightTrack++;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		switch (colliderName)
		{
		case "lightSignal":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpLightSignal);
			break;
		case "bush":
		case "powerbox":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpBush);
			break;
		case "side":
		case "collider":
			break;
		case "collider stumble":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpTrain);
			break;
		case "blocker_jump":
		case "blocker_roll":
		case "blocker_standard":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpBarrier);
			break;
		default:
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpTrain);
			break;
		}
	}

	public void Start()
	{
		stats = GameStats.Instance;
		character = GetComponent<Character>();
		character.OnChangeTrack += OnChangeTrackDir;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
		character.OnPassedObstacle += OnPassedObstacle;
		character.OnJumpOverTrain += OnJumpOverTrain;
		character.OnJumpOverBus += OnJumpOverBus;
		character.OnJumpOverCar += OnJumpOverCar;
		character.OnCriticalHit += OnCriticalHit;
		character.OnStumble += OnStumble;
	}
}
