using UnityEngine;

public class OnlineRewardManager : MonoBehaviour
{
	[SerializeField]
	private OnlineZone[] zones;

	public const int NUMBER_OF_ONLINEZONE = 4;

	private static OnlineRewardManager _instance;

	public static OnlineRewardManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Utils.FindObject<OnlineRewardManager>();
			}
			return _instance;
		}
	}

	public int PayedOut { get; set; }

	public OnlineZone[] Zones
	{
		get
		{
			return zones;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}
}
