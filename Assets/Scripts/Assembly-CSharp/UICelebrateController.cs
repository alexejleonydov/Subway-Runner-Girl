using UnityEngine;

public class UICelebrateController : MonoBehaviour
{
	public static UICelebrateController Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
