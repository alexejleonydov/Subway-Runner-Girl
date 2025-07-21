using System.Collections;
using UnityEngine;

public class CoroutineC : MonoBehaviour
{
	private static CoroutineC _instance;

	public static CoroutineC Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("CoroutineC");
				_instance = gameObject.AddComponent<CoroutineC>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public Coroutine StartCoroutineC(IEnumerator coroutine)
	{
		return StartCoroutine(coroutine);
	}
}
