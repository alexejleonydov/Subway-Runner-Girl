using System;
using UnityEngine;

public class TopMenuAnimations : MonoBehaviour
{
	[SerializeField]
	private PointsManager[] managers;

	[SerializeField]
	private PointsManager player;

	[SerializeField]
	private PointsManager enemy;

	public void AddPlayerOnStopEvent(Action action)
	{
		PointsManager pointsManager = player;
		pointsManager.onStop = (Action)Delegate.Combine(pointsManager.onStop, action);
	}

	public void AddEnemyOnStopEvent(Action action)
	{
		PointsManager pointsManager = enemy;
		pointsManager.onStop = (Action)Delegate.Combine(pointsManager.onStop, action);
	}

	public void RemovePlayerOnStopEvent(Action action)
	{
		PointsManager pointsManager = player;
		pointsManager.onStop = (Action)Delegate.Remove(pointsManager.onStop, action);
	}

	public void RemoveEnemyOnStopEvent(Action action)
	{
		PointsManager pointsManager = enemy;
		pointsManager.onStop = (Action)Delegate.Remove(pointsManager.onStop, action);
	}

	public void StartPlayIdleRummagesAnimation()
	{
		base.enabled = true;
		int i = 0;
		for (int num = managers.Length; i < num; i++)
		{
			managers[i].Play();
		}
		player.Play();
		enemy.Play();
	}

	public void StopPlayIdleRummagesAnimation()
	{
		base.enabled = false;
		int i = 0;
		for (int num = managers.Length; i < num; i++)
		{
			managers[i].OnStop();
		}
		player.OnStop();
		enemy.OnStop();
	}

	public void OnNewGameStart()
	{
		int i = 0;
		for (int num = managers.Length; i < num; i++)
		{
			managers[i].BreakLoop();
		}
		player.BreakLoop();
		enemy.BreakLoop();
	}

	public void Continue()
	{
		int i = 0;
		for (int num = managers.Length; i < num; i++)
		{
			managers[i].Wait = false;
		}
	}
}
