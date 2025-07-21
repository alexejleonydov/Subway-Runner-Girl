using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	[SerializeField]
	private UILabel loadingLbl;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UITexture background;

	private AsyncOperation ao;

	public static bool finished;

	private float progress;

	private List<AsyncOperation> aoList = new List<AsyncOperation>();

	private float Progress
	{
		get
		{
			float num = 0f;
			num = progress * 0.5f;
			if (progress > 0.6f)
			{
				num += 1.25f * progress - 0.75f;
			}
			return num;
		}
	}

	private void Awake()
	{
		slider.value = 0f;
	}

	private void OnEnable()
	{
		loadingLbl.text = Strings.Get(LanguageKey.START_APP_LOADING);
	}

	private void Start()
	{
		StartCoroutine(LoadSceneAsync());
	}

	private IEnumerator LoadSceneAsync()
	{
		float ratio2 = 1f;
		ratio2 = ((!GlobalInit.Instance.debug) ? 0.5f : 0.7f);
		SceneManager.LoadScene("Merge", LoadSceneMode.Additive);
		slider.value = ratio2 * 0.5f;
		yield return null;
		if (GlobalInit.Instance.debug)
		{
			ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.cityScenename, LoadSceneMode.Additive);
			ao.allowSceneActivation = false;
			aoList.Add(ao);
		}
		else
		{
			int j = 0;
			for (int num = GlobalInit.Instance.citiesScenename.Length; j < num; j++)
			{
				ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.citiesScenename[j], LoadSceneMode.Additive);
				ao.allowSceneActivation = false;
				aoList.Add(ao);
			}
		}
		float curValue = slider.value;
		float remainProgress = 1f - curValue;
		int len = aoList.Count;
		for (int i = 0; i < len; i++)
		{
			aoList[i].allowSceneActivation = true;
			while (!aoList[i].isDone)
			{
				slider.value = curValue + remainProgress * ((float)i + aoList[i].progress) / (float)len;
				yield return null;
			}
		}
		finished = true;
		yield return null;
		Object.Destroy(base.gameObject);
		Resources.UnloadAsset(background.mainTexture);
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator inProgressing()
	{
		float ratio2 = 1f;
		ratio2 = ((!GlobalInit.Instance.debug) ? 0.5f : 0.7f);
		while (!ao.isDone)
		{
			progress = 0f;
			if (ao != null)
			{
				progress = ao.progress;
			}
			slider.value = Progress * ratio2;
			yield return null;
		}
		if (GlobalInit.Instance.debug)
		{
			ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.cityScenename, LoadSceneMode.Additive);
			while (!ao.isDone)
			{
				progress = 0f;
				if (ao != null)
				{
					progress = ao.progress;
				}
				slider.value = progress * (1f - ratio2) + ratio2;
				yield return null;
			}
		}
		else
		{
			int i = 0;
			for (int max = GlobalInit.Instance.citiesScenename.Length; i < max; i++)
			{
				ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.citiesScenename[i], LoadSceneMode.Additive);
				while (!ao.isDone)
				{
					progress = 0f;
					if (ao != null)
					{
						progress = ao.progress;
					}
					slider.value = (progress + (float)i) * ratio2 / (float)max + (1f - ratio2);
					yield return null;
				}
			}
		}
		finished = true;
		yield return null;
		Object.Destroy(base.gameObject);
		Resources.UnloadAsset(background.mainTexture);
		Resources.UnloadUnusedAssets();
	}
}
