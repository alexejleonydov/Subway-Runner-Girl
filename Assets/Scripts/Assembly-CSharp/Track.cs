using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
	public string cityName;

	public Transform trackLeft;

	public Transform trackRight;

	public int numberOfTracks = 3;

	public float cleanUpDistance = 2000f;

	public float aheadDistance = 700f;

	private const float deltaX = 500f;

	private City city;

	private float trackSpacing;

	public Dictionary<int, SubScene> subScenes = new Dictionary<int, SubScene>();

	private int currentSubSceneId;

	public SubScene CurrentSubScene
	{
		get
		{
			return GetSubSceneByName(currentSubSceneId);
		}
	}

	public SubScene DefaultSubscene
	{
		get
		{
			return GetSubSceneByName(0);
		}
	}

	public City City
	{
		get
		{
			return city;
		}
	}

	public void Awake()
	{
		trackSpacing = (trackRight.position - trackLeft.position).magnitude / (float)(numberOfTracks - 1);
		city = Resources.Load<City>("City/" + cityName);
		if (city == null)
		{
			Debug.LogError(cityName + " is not exit in Resources/City fold.");
			Debug.Break();
		}
		TrackController.Instance.AddTrack(cityName, this);
		int i = 0;
		for (int num = city.subScenes.Length; i < num; i++)
		{
			SubScene subScene = city.subScenes[i];
			if (subScene.minLength == -1)
			{
				subScene.minLength = int.MaxValue;
			}
			if (subScene.maxLength == -1)
			{
				subScene.maxLength = int.MaxValue;
			}
			subScenes.Add(subScene.order, subScene);
		}
		Restart();
	}

	public void AddToChunks(TrackPiece newPiece)
	{
		if (!subScenes.ContainsKey(newPiece.subscene))
		{
			Debug.LogError(newPiece.subscene + " is not exit in " + cityName + "'s subScenes.");
			Debug.Break();
		}
		SubScene subScene = subScenes[newPiece.subscene];
		subScene.AddToDict(newPiece);
	}

	public void Restart()
	{
		foreach (KeyValuePair<int, SubScene> subScene in subScenes)
		{
			subScene.Value.Restart();
		}
		currentSubSceneId = 0;
	}

	public void SetTrackPosition()
	{
		base.transform.position = new Vector3(500f * (float)city.order, 0f, 0f);
	}

	public int GetSubsceneId(bool force)
	{
		if (force)
		{
			int num = (currentSubSceneId + 1) % subScenes.Count;
			if (subScenes.ContainsKey(num))
			{
				return num;
			}
		}
		return currentSubSceneId;
	}

	public bool AllowFlypack()
	{
		return CurrentSubScene.allowFlypack;
	}

	public void ChangeBackgroundMusic()
	{
		AudioPlayer.Instance.PlayMusic(CurrentSubScene.mainMusic, 0.5f, 0.5f, 0.5f);
	}

	public int ChangeToNextSubScene(int subOrder)
	{
		if (subScenes.ContainsKey(subOrder))
		{
			currentSubSceneId = (subOrder + 1) % subScenes.Count;
		}
		return currentSubSceneId;
	}

	public SubScene GetSubSceneByName(int subOrder)
	{
		if (!subScenes.ContainsKey(subOrder))
		{
			Debug.LogError(cityName + " does not contain " + subOrder);
			Debug.Break();
			return subScenes[currentSubSceneId];
		}
		return subScenes[subOrder];
	}

	public Vector3 GetPosition(float x, float z)
	{
		return Vector3.forward * z + trackLeft.position + x * Vector3.right;
	}

	public float GetTrackX(int trackIndex)
	{
		return (trackLeft.position + trackRight.position).x * 0.5f + trackSpacing * (float)trackIndex;
	}
}
