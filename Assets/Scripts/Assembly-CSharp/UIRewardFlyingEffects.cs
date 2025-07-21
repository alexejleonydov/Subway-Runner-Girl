using System;
using UnityEngine;

public class UIRewardFlyingEffects : MonoBehaviour
{
	[SerializeField]
	private FlyEffect scorebooster;

	[SerializeField]
	private FlyEffect turtleShell;

	[SerializeField]
	private FlyEffect coin;

	[SerializeField]
	private FlyEffect key;

	[SerializeField]
	private FlyEffect headSprint;

	[SerializeField]
	private FlyEffect twoPole;

	private static UIRewardFlyingEffects _instance;

	public static UIRewardFlyingEffects Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Utils.FindObject<UIRewardFlyingEffects>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public void ScoreBooster(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		scorebooster.Flying(start, end, duration, action);
	}

	public void TurtleShell(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		turtleShell.Flying(start, end, duration, action);
	}

	public void Coin(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		coin.Flying(start, end, duration, action);
	}

	public void Key(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		key.Flying(start, end, duration, action);
	}

	public void HeadSprint(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		headSprint.Flying(start, end, duration, action);
	}

	public void TwoPole(Vector2 start, Vector2 end, float duration, Action action = null)
	{
		twoPole.Flying(start, end, duration, action);
	}
}
