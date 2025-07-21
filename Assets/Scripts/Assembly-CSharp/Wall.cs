using System.Collections;
using UnityEngine;

public class Wall : BaseO
{
	[SerializeField]
	private int trackIndex;

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
	private float maxDistance = 100f;

	public SwipeDir swipeDir;

	[SerializeField]
	private BoxCollider bCollider;

	[SerializeField]
	private float height;

	private GameObject _mesh;

	private Game game;

	private Character character;

	private float distance;

	private bool willShowMesh;

	private bool hasShowMesh;

	private string cityName;

	public BoxCollider Collider
	{
		get
		{
			return bCollider;
		}
	}

	public float Height
	{
		get
		{
			return height;
		}
	}

	public Bounds Bounds { get; private set; }

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

	protected override void Awake()
	{
		base.Awake();
		game = Game.Instance;
		if (game != null)
		{
			character = game.character;
		}
		cityName = base.transform.root.GetComponent<Track>().cityName;
	}

	public override void OnActivate()
	{
		bCollider.enabled = true;
		Bounds = bCollider.bounds;
		willShowMesh = false;
		hasShowMesh = false;
		base.enabled = true;
	}

	public override void OnDeactivate()
	{
		bCollider.enabled = false;
	}

	public void OnTrigger()
	{
		if (PlayerInfo.Instance.tutorialStep > 0)
		{
			bCollider.enabled = false;
		}
		if (PlayerInfo.Instance.CheckWallWalkingTutorial(cityName) && !willShowMesh && trackIndex == character.TrackIndexTarget)
		{
			distance = character.z - base.transform.position.z;
			if (!(distance > maxDistance))
			{
				willShowMesh = true;
			}
		}
	}

	private void Update()
	{
		if (!willShowMesh)
		{
			return;
		}
		if (willShowMesh && !hasShowMesh && CanSwipe())
		{
			if (displayText)
			{
				UISliderInController.Instance.QueueMessage(Strings.Get(text));
			}
			if (displayMesh)
			{
				StartCoroutine(ShowMesh());
			}
			PlayerInfo.Instance.AddWallWalkingTutorial(cityName);
			hasShowMesh = true;
		}
		if (!CanSwipe())
		{
			StopAllCoroutines();
			Mesh.SetActive(false);
			base.enabled = false;
		}
	}

	private bool CanSwipe()
	{
		return character.characterController.isGrounded && trackIndex == character.TrackIndexTarget;
	}

	private IEnumerator ShowMesh()
	{
		Mesh.transform.rotation = Quaternion.AngleAxis(meshDir, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		Mesh.SetActive(true);
		float time = this.time / game.NormalizedGameSpeed;
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
		base.enabled = false;
	}
}
