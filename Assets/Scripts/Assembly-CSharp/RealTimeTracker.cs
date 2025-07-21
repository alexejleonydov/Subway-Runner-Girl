using UnityEngine;

public class RealTimeTracker : MonoBehaviour
{
	private static RealTimeTracker mInst;

	private float mRealDelta;

	private float mRealTime;

	public static float deltaTime
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealDelta;
		}
	}

	public static float time
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealTime;
		}
	}

	private static void Spawn()
	{
		GameObject gameObject = new GameObject("_RealTimeTracker");
		Object.DontDestroyOnLoad(gameObject);
		mInst = gameObject.AddComponent<RealTimeTracker>();
		mInst.mRealTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mRealDelta = Mathf.Clamp01(realtimeSinceStartup - mRealTime);
		mRealTime = realtimeSinceStartup;
	}
}
