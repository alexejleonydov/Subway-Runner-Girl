using UnityEngine;

public class JetPackCloud : MonoBehaviour
{
	private Material material;

	public float scrollSpeed = 0.5f;

	public float startOffset;

	private void Awake()
	{
		material = base.gameObject.GetComponent<Renderer>().material;
		material.mainTextureOffset = new Vector2(startOffset, 0f);
	}

	private void Update()
	{
		float x = (material.mainTextureOffset.x + Time.deltaTime * scrollSpeed) % 1f;
		material.mainTextureOffset = new Vector2(x, 0f);
	}
}
