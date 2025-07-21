using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float meshDir;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool allowHelmet;

	[SerializeField]
	private bool endTutorial;

	private GameObject _mesh;

	private Character character;

	private Game game;

	private Helmet helmet;

	private bool hasInited;

	private TrackController trackController;

	private GameObject Mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.transform.Find("Arrow").gameObject;
			}
			return _mesh;
		}
	}

	private void Start()
	{
		game = Game.Instance;
		helmet = Helmet.Instance;
		if (game != null && !hasInited)
		{
			character = game.character;
			trackController = game.trackController;
			hasInited = true;
		}
		if (PlayerInfo.Instance.tutorialStep > 0)
		{
			base.enabled = false;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!character.stopColliding && collider.gameObject.layer == Layers.Instance.Character)
		{
			if (displayText)
			{
				UISliderInController.Instance.QueueMessage(Strings.Get(text));
			}
			if (displayMesh)
			{
				StartCoroutine(ShowMesh());
			}
			helmet.isAllowed = allowHelmet;
			if (endTutorial)
			{
				PlayerPrefs.SetInt("IsNewPlayerStatus", 1);
				PlayerPrefs.SetInt("NewPlayerFirstSaveStatus", 1);
				trackController.IsRunningOnTutorialTrack = false;
				PlayerInfo.Instance.tutorialStep = 1;
				Game.Instance.show20sAd = false;
			}
			GetComponent<Collider>().enabled = false;
		}
	}

	private IEnumerator ShowMesh()
	{
		Mesh.transform.rotation = Quaternion.AngleAxis(meshDir, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		Mesh.SetActive(true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(myTween.To(time, delegate(float t)
		{
			Mesh.transform.localPosition = Vector3.Lerp(pos - Mesh.transform.up * 5f, pos + Mesh.transform.up * 5f, t);
			Mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!game.IsInGame.Value)
			{
				Mesh.transform.localPosition = new Vector3(1000f, 1000f, 0f);
			}
		}));
		Mesh.SetActive(false);
	}

	private void Update()
	{
		if (PlayerInfo.Instance.tutorialStep > 0)
		{
			base.enabled = false;
		}
	}
}
