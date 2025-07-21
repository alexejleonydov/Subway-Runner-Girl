using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{
	private struct GameObjectWrapper
	{
		public GameObject GameObject;

		public float z;

		public float Z
		{
			get
			{
				return z;
			}
		}

		public GameObjectWrapper(GameObject gameObject)
		{
			GameObject = gameObject;
			z = gameObject.transform.position.z;
		}
	}

	private struct SelectorWrapper
	{
		public GameObject GameObject;

		public float z;

		public Randomizer Selector;

		public float Z
		{
			get
			{
				return z;
			}
		}

		public SelectorWrapper(Randomizer selector, GameObject gameObject)
		{
			Selector = selector;
			GameObject = gameObject;
			z = gameObject.transform.position.z;
		}
	}

	public Dictionary<string, Track> trackDict;

	private Track currentTrack;

	private List<TrackPiece> activeTrackPieces = new List<TrackPiece>(5);

	private List<TrackPiece> trackPiecesForDeactivation = new List<TrackPiece>(5);

	[SerializeField]
	private int[] chestZ = new int[5] { 3500, 11000, 25000, 48000, 110000 };

	private bool firstTrackPiece = true;

	[HideInInspector]
	public int nextChestIndex;

	private float lastCityZ;

	private float lastSubsceneZ;

	private float lastPortalZ;

	private float trackPieceZ;

	private float lastFlypackSpawnZ;

	private bool willChangeMainMusic;

	private TrackPieceController trackPieces;

	private Flypack flypack;

	[HideInInspector]
	public int nextTrackOrder;

	[HideInInspector]
	public int forceNextCityOrder;

	[HideInInspector]
	public int taskSet;

	private static TrackController instance;

	public static TrackController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<TrackController>();
			}
			return instance;
		}
	}

	public float LastFlypackSpawnZ
	{
		set
		{
			lastFlypackSpawnZ = value;
		}
	}

	public int NumberOfTracks
	{
		get
		{
			return currentTrack.numberOfTracks;
		}
	}

	public bool IsRunningOnTutorialTrack { get; set; }

	private void Awake()
	{
		trackPieces = new TrackPieceController();
		flypack = Flypack.Instance;
		trackDict = new Dictionary<string, Track>();
		lastPortalZ = (lastCityZ = (lastSubsceneZ = 0f));
		lastFlypackSpawnZ = 0f;
		nextChestIndex = 0;
		willChangeMainMusic = false;
	}

	public void AddTrack(string city, Track track)
	{
		if (!trackDict.ContainsKey(city))
		{
			trackDict.Add(city, track);
		}
	}

	public void RemoveTrack(string city)
	{
		if (trackDict.ContainsKey(city))
		{
			trackDict.Remove(city);
		}
	}

	public City GetCity(int order)
	{
		City result = null;
		foreach (KeyValuePair<string, Track> item in trackDict)
		{
			if (item.Value.City.order == order)
			{
				result = item.Value.City;
				break;
			}
		}
		return result;
	}

	public int CurrentTaskSetChange(int taskSet)
	{
		foreach (KeyValuePair<string, Track> item in trackDict)
		{
			if (item.Value.City.lockedTaskSet == taskSet)
			{
				return item.Value.City.order;
			}
		}
		return -1;
	}

	public City NextTaskSetWithUnlockCity(int taskSet)
	{
		City city = null;
		City city2 = null;
		foreach (KeyValuePair<string, Track> item in trackDict)
		{
			city2 = item.Value.City;
			if (city == null)
			{
				if (city2.lockedTaskSet > taskSet)
				{
					city = city2;
				}
			}
			else if (city2.lockedTaskSet < city.lockedTaskSet)
			{
				city = city2;
			}
		}
		if (city == null)
		{
			foreach (KeyValuePair<string, Track> item2 in trackDict)
			{
				city2 = item2.Value.City;
				if (city == null)
				{
					city = city2;
				}
				else if (city2.lockedTaskSet > city.lockedTaskSet)
				{
					city = city2;
				}
			}
		}
		return city;
	}

	public City LastTaskSetWithUnlockCity(int taskSet)
	{
		City city = null;
		City city2 = null;
		foreach (KeyValuePair<string, Track> item in trackDict)
		{
			city2 = item.Value.City;
			if (city2.lockedTaskSet < 0)
			{
				continue;
			}
			if (city == null)
			{
				if (city2.lockedTaskSet <= taskSet)
				{
					city = city2;
				}
			}
			else if (city2.lockedTaskSet <= taskSet && city2.lockedTaskSet > city.lockedTaskSet)
			{
				city = city2;
			}
		}
		return city;
	}

	public void SetStartCity()
	{
		Track track = null;
		if (GlobalInit.Instance.debug)
		{
			track = trackDict[GlobalInit.Instance.cityScenename];
		}
		else
		{
			foreach (KeyValuePair<string, Track> item in trackDict)
			{
				if (item.Value.City.order == 0)
				{
					track = item.Value;
					break;
				}
			}
		}
		if (track != null)
		{
			currentTrack = track;
			Shader.SetGlobalVector("_Distort", currentTrack.City.distort);
			trackPieces.SetTrackPieces(currentTrack.DefaultSubscene);
			willChangeMainMusic = false;
			currentTrack.ChangeBackgroundMusic();
		}
		else
		{
			Debug.LogError("The Cities's orders do not have 0!!!");
			Debug.Break();
		}
	}

	public void ChangeToNextCity(bool isContinue)
	{
		Track track = null;
		foreach (KeyValuePair<string, Track> item in trackDict)
		{
			if (item.Value.City.order == nextTrackOrder)
			{
				track = item.Value;
				break;
			}
		}
		if (track != null)
		{
			ChangeCity(track, isContinue);
			return;
		}
		Debug.LogError(nextTrackOrder + " has not exit!!!");
		Debug.Break();
	}

	public void ChangeCity(Track track, bool isContinue)
	{
		RandomizerHold.Initialize();
		if (currentTrack != null)
		{
			currentTrack.Restart();
		}
		currentTrack = track;
		Shader.SetGlobalVector("_Distort", currentTrack.City.distort);
		if (PlayerInfo.Instance.forceNextCityOrder == nextTrackOrder)
		{
			PlayerInfo.Instance.forceNextCityOrder = 0;
			Instance.forceNextCityOrder = 0;
		}
		if (!isContinue)
		{
			trackPieceZ = 180f;
			lastPortalZ = 180f;
			lastCityZ = 180f;
			lastSubsceneZ = 180f;
			currentTrack.SetTrackPosition();
		}
		else
		{
			lastPortalZ = (lastSubsceneZ = (lastCityZ = trackPieceZ));
			trackPieceZ += 180f;
		}
		StopAllCoroutines();
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			activeTrackPieces[i].Deactivate();
		}
		activeTrackPieces.Clear();
		trackPieces.SetTrackPieces(currentTrack.DefaultSubscene);
		trackPieces.Initialize(0f);
		currentTrack.ChangeBackgroundMusic();
	}

	private void ChangeSubscene(int subId)
	{
		SubScene subSceneByName = currentTrack.GetSubSceneByName(subId);
		lastSubsceneZ = trackPieceZ;
		trackPieces.SetTrackPieces(subSceneByName);
		trackPieces.Initialize(0f);
		willChangeMainMusic = true;
	}

	public SubScene GetCurrentSubScene()
	{
		if (currentTrack != null)
		{
			return currentTrack.CurrentSubScene;
		}
		return null;
	}

	public City GetCurrentCity()
	{
		if (currentTrack != null)
		{
			return currentTrack.City;
		}
		return null;
	}

	public void ChangeBackgroundMusic(float characterZ)
	{
		if (willChangeMainMusic && characterZ >= lastSubsceneZ - 360f)
		{
			willChangeMainMusic = false;
			currentTrack.ChangeBackgroundMusic();
		}
	}

	public float LayJetpackPieces(float characterZ, float flyLength)
	{
		LayTracksUpTo(characterZ, flyLength, true, false);
		float result = trackPieceZ - characterZ;
		LayTrackPiece(trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.Jetpacklanding));
		return result;
	}

	public void LayTransitionPieces(float characterZ)
	{
		LayTracksUpTo(characterZ, currentTrack.aheadDistance, false, true);
	}

	public void LayTransitionEndPeice()
	{
		LayTrackPiece(trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.TransitionEnd));
	}

	public void LayTrackPieces(float characterZ)
	{
		LayTracksUpTo(characterZ, currentTrack.aheadDistance, false, false);
	}

	public void LayTracksUpTo(float characterZ, float trackAheadDistance, bool isJetpack, bool isTransition)
	{
		if (!trackPieces.CanDeliver())
		{
			return;
		}
		float num = characterZ + trackAheadDistance;
		if (trackPieceZ < num)
		{
			CleanupTrackPieces(characterZ);
		}
		int num2 = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		SubScene subScene = null;
		TrackPiece trackPiece = null;
		while (trackPieceZ < num)
		{
			if (firstTrackPiece && PlayerInfo.Instance.tutorialStep == 0)
			{
				trackPiece = trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.Tutorial);
				firstTrackPiece = false;
				if (trackPiece.CheckPoints.Count > 0)
				{
					IsRunningOnTutorialTrack = true;
				}
			}
			else if (isJetpack)
			{
				trackPiece = trackPieces.GetJetPakPiece(num2);
				num2++;
			}
			else if (isTransition)
			{
				trackPiece = trackPieces.GetTransitionPiece(num2);
				num2++;
			}
			else
			{
				flag = false;
				flag2 = false;
				flag3 = false;
				flag4 = false;
				flag5 = false;
				subScene = GetCurrentSubScene();
				if (trackPieceZ - lastSubsceneZ > (float)subScene.minLength)
				{
					flag = true;
				}
				if (trackPieceZ - lastSubsceneZ > (float)subScene.maxLength)
				{
					flag4 = true;
					flag = true;
				}
				if (trackPieceZ - lastFlypackSpawnZ < currentTrack.aheadDistance * 1.1f)
				{
					flag4 = false;
					flag = false;
				}
				if (currentTrack.CurrentSubScene.allowCityEnd && trackPieceZ - lastCityZ > (float)currentTrack.City.minLength)
				{
					if (trackPieceZ - lastPortalZ > (float)currentTrack.City.minIntervalLength)
					{
						flag2 = true;
					}
					if (trackPieceZ - lastPortalZ > (float)currentTrack.City.maxIntervalLength)
					{
						flag2 = true;
						flag5 = true;
						flag4 = false;
						flag = false;
					}
				}
				if (nextChestIndex < chestZ.Length && trackPieceZ >= (float)chestZ[nextChestIndex])
				{
					flag3 = true;
					flag2 = false;
					flag5 = false;
					flag4 = false;
					flag = false;
				}
				trackPieces.MoveForward(subScene.LastZ, flag3, flag, flag4, flag2, flag5);
				trackPiece = trackPieces.GetRandomActive();
				int num3 = 0;
				while (activeTrackPieces.Contains(trackPiece) && num3 < 500)
				{
					trackPiece = trackPieces.GetRandomActive();
					num3++;
				}
			}
			LayTrackPiece(trackPiece);
		}
	}

	private void LayTrackPiece(TrackPiece TrackPiece)
	{
		StartCoroutine(LayTrackPieceAsync(TrackPiece));
	}

	private IEnumerator LayTrackPieceAsync(TrackPiece trackPiece)
	{
		trackPiece.gameObject.transform.position = Vector3.forward * trackPieceZ;
		trackPieceZ += trackPiece.zSize;
		GetCurrentSubScene().LastZ += trackPiece.zSize;
		activeTrackPieces.Add(trackPiece);
		if (trackPiece.trackPieceType == TrackPieceType.SubsceneTransition)
		{
			ChangeSubscene(currentTrack.ChangeToNextSubScene(trackPiece.subscene));
		}
		if (trackPiece.trackPieceType == TrackPieceType.CityTransition)
		{
			lastPortalZ = trackPieceZ;
		}
		if (trackPiece.trackPieceType == TrackPieceType.Chest)
		{
			nextChestIndex++;
		}
		if (trackPiece.trackPieceType == TrackPieceType.Jetpacklanding)
		{
			flypack.PlacePickups(trackPiece.transform.position.z);
		}
		trackPiece.RestoreHiddenObstacles();
		yield return StartCoroutine(PerformRecursiveRandomizer(trackPiece.gameObject));
		if (!trackPiece.hasSorted)
		{
			Array.Sort(trackPiece.objects, (TrackObject to1, TrackObject to2) => to1.transform.position.z.CompareTo(to2.transform.position.z));
			trackPiece.hasSorted = true;
		}
		TrackObject[] toes = trackPiece.objects;
		foreach (TrackObject o in toes)
		{
			if (o.gameObject.activeInHierarchy)
			{
				o.Activate();
				yield return null;
			}
		}
	}

	private IEnumerator PerformRecursiveRandomizer(GameObject parent, bool sortSpawnUpgrades = true)
	{
		List<GameObjectWrapper> objectsToActivate = new List<GameObjectWrapper>();
		List<SelectorWrapper> spawnPoints = new List<SelectorWrapper>();
		List<GameObject> objectsToVisit = new List<GameObject> { parent };
		GameObject gameObject2 = null;
		Transform gameObjectTransform2 = null;
		while (objectsToVisit.Count > 0)
		{
			gameObject2 = objectsToVisit[0];
			objectsToVisit.RemoveAt(0);
			if (sortSpawnUpgrades)
			{
				SpawnUpgrade component = gameObject2.GetComponent<SpawnUpgrade>();
				if (component != null)
				{
					spawnPoints.Add(new SelectorWrapper(component, gameObject2));
					continue;
				}
			}
			SelectorOffset component2 = gameObject2.GetComponent<SelectorOffset>();
			if (component2 != null)
			{
				component2.ChooseRandomOffset();
			}
			objectsToActivate.Add(new GameObjectWrapper(gameObject2));
			Randomizer component3 = gameObject2.GetComponent<Randomizer>();
			if (component3 != null)
			{
				component3.PerformRandomizer(objectsToVisit);
				continue;
			}
			int j = 0;
			for (gameObjectTransform2 = gameObject2.transform; j < gameObjectTransform2.childCount; j++)
			{
				objectsToVisit.Add(gameObjectTransform2.GetChild(j).gameObject);
			}
		}
		int layer1;
		int layer2;
		objectsToActivate.Sort(delegate(GameObjectWrapper x, GameObjectWrapper y)
		{
			layer1 = x.GameObject.layer;
			layer2 = y.GameObject.layer;
			if ((layer1 != 16 && layer2 != 16) || (layer1 == 16 && layer2 == 16))
			{
				return x.Z.CompareTo(y.Z);
			}
			return (layer1 == 16) ? 1 : (-1);
		});
		int i = 0;
		List<GameObjectWrapper>.Enumerator enumrator = objectsToActivate.GetEnumerator();
		try
		{
			while (enumrator.MoveNext())
			{
				GameObjectWrapper gow = enumrator.Current;
				if (gow.GameObject != null)
				{
					gow.GameObject.SetActive(true);
				}
				i++;
				if (i == 4)
				{
					yield return null;
					i = 0;
				}
			}
		}
		finally
		{
			enumrator.Dispose();
		}
		if (spawnPoints.Count <= 0)
		{
			yield break;
		}
		spawnPoints.Sort((SelectorWrapper x, SelectorWrapper y) => x.Z.CompareTo(y.Z));
		objectsToVisit.Clear();
		List<SelectorWrapper>.Enumerator enumerator = spawnPoints.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.Selector.PerformRandomizer(objectsToVisit);
			}
		}
		finally
		{
			enumerator.Dispose();
		}
		List<GameObject>.Enumerator enumerator2 = objectsToVisit.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				enumerator2.Current.SetActive(true);
			}
		}
		finally
		{
			enumerator2.Dispose();
		}
	}

	public Vector3 GetPosition(float x, float z)
	{
		return currentTrack.GetPosition(x, z);
	}

	public float GetTrackX(int trackIndex)
	{
		return currentTrack.GetTrackX(trackIndex);
	}

	public void CleanupTrackPieces(float characterZ)
	{
		float num = characterZ - currentTrack.cleanUpDistance;
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			if (activeTrackPieces[i].transform.position.z + activeTrackPieces[i].zSize < num)
			{
				trackPiecesForDeactivation.Add(activeTrackPieces[i]);
			}
		}
		int j = 0;
		for (int count2 = trackPiecesForDeactivation.Count; j < count2; j++)
		{
			if (trackPiecesForDeactivation[j].trackPieceType != TrackPieceType.Tutorial)
			{
				trackPiecesForDeactivation[j].Deactivate();
			}
			activeTrackPieces.Remove(trackPiecesForDeactivation[j]);
		}
		trackPiecesForDeactivation.Clear();
	}

	public void DeactivateTrackPieces()
	{
		StopAllCoroutines();
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			activeTrackPieces[i].Deactivate();
		}
	}

	public void LayEmptyPieces(float characterZ, float removeDistance)
	{
		RemovePieceObstacles(characterZ + removeDistance);
	}

	public void RemovePieceObstacles(float removeDistance)
	{
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			activeTrackPieces[i].DeactivateObstacles(removeDistance);
		}
	}

	public float GetLastCheckPoint(float characterZ)
	{
		TrackPiece trackPiece = null;
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			trackPiece = activeTrackPieces[i];
			if (IsRunningOnTutorialTrack && trackPiece.trackPieceType == TrackPieceType.Tutorial)
			{
				return trackPiece.GetLastCheckPoint(characterZ);
			}
		}
		Debug.Log("No checkpoints in track");
		return 0f;
	}

	public TrackPiece.TrackCheckPoint GetNextCheckPoint(float characterZ)
	{
		TrackPiece.TrackCheckPoint trackCheckPoint = null;
		TrackPiece trackPiece = null;
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			trackPiece = activeTrackPieces[i];
			if (trackPiece.CheckPoints != null && trackPiece.CheckPoints.Count != 0)
			{
				trackCheckPoint = trackPiece.GetNextCheckPoint(characterZ);
				if (trackCheckPoint != null)
				{
					break;
				}
			}
		}
		return trackCheckPoint;
	}

	public void SetCharacterPosition(Character character)
	{
		character.z = trackPieceZ + 250f;
		character.transform.position = GetPosition(character.x, character.z);
	}

	public void Restart()
	{
		RandomizerHold.Initialize();
		if (currentTrack != null)
		{
			currentTrack.Restart();
		}
		nextChestIndex = 0;
		trackPieceZ = 0f;
		lastPortalZ = 0f;
		lastCityZ = 0f;
		lastSubsceneZ = 0f;
		lastFlypackSpawnZ = 0f;
		willChangeMainMusic = false;
		SetStartCity();
		forceNextCityOrder = PlayerInfo.Instance.forceNextCityOrder;
		taskSet = PlayerInfo.Instance.amountOfLevel;
		trackPieces.Initialize(0f);
		int i = 0;
		for (int count = activeTrackPieces.Count; i < count; i++)
		{
			activeTrackPieces[i].Deactivate();
		}
		activeTrackPieces.Clear();
		firstTrackPiece = true;
	}

	public bool AllowFlypack()
	{
		if (nextChestIndex < chestZ.Length && trackPieceZ >= (float)(chestZ[nextChestIndex] - 300))
		{
			return false;
		}
		return currentTrack.AllowFlypack();
	}
}
