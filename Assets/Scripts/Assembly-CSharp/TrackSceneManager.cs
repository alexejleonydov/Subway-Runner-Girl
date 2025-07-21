using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSceneManager
{
	public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode, Action update, Action finish)
	{
		AsyncOperation ao2 = SceneManager.LoadSceneAsync(sceneName, mode);
		while (!ao2.isDone)
		{
			if (update != null)
			{
				update();
			}
			yield return null;
		}
		if (finish != null)
		{
			finish();
		}
		ao2 = null;
	}

	public IEnumerator UnloadSceneAsync(string sceneName, Action update, Action finish)
	{
		AsyncOperation ao2 = SceneManager.UnloadSceneAsync(sceneName);
		while (!ao2.isDone)
		{
			if (update != null)
			{
				update();
			}
			yield return null;
		}
		if (finish != null)
		{
			finish();
		}
		ao2 = null;
	}
}
