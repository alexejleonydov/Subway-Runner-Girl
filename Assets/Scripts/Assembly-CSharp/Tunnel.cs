using System.Collections;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
	public enum TunnelType
	{
		Tunnel = 0,
		Upstair = 1,
		Downstair = 2,
		None = 3
	}

	public TunnelType type;

	private Game game;

	private float tunnelLength;

	public float slope = 1f;

	public Transform start;

	public Transform end;

	public Transform fadeInOut;

	private float fadeDistance;

	private float startZ;

	private bool inTunnel;

	private void Awake()
	{
		game = Game.Instance;
		if (start == null || end == null)
		{
			tunnelLength = GetComponent<Collider>().bounds.size.z;
		}
		else if ((bool)start && (bool)end)
		{
			tunnelLength = end.position.z - start.position.z;
			slope = (end.position.y - start.position.y) / tunnelLength;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			inTunnel = true;
			switch (type)
			{
			case TunnelType.Tunnel:
				game.Running.StartTunnel(tunnelLength);
				break;
			case TunnelType.Upstair:
				game.Running.StartUpstair(tunnelLength, slope);
				fadeDistance = end.position.z - fadeInOut.position.z;
				startZ = fadeInOut.position.z;
				StartCoroutine(FadeInOut(0.5f, 1f, fadeDistance));
				break;
			case TunnelType.Downstair:
				game.Running.StartDownstair(tunnelLength, slope);
				fadeDistance = fadeInOut.position.z - start.position.z;
				startZ = start.position.z;
				StartCoroutine(FadeInOut(1f, 0.5f, fadeDistance));
				break;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			switch (type)
			{
			case TunnelType.Upstair:
				game.Running.EndUpstair();
				break;
			case TunnelType.Downstair:
				game.Running.EndDownstair();
				break;
			}
			inTunnel = false;
		}
	}

	private IEnumerator FadeInOut(float start, float end, float distance)
	{
		while (game.character.z < startZ)
		{
			yield return null;
		}
		float factor = 0f;
		float rate2 = start;
		while (factor < 1f)
		{
			factor = (inTunnel ? ((game.character.z - startZ) / distance) : 1f);
			rate2 = Mathf.SmoothStep(start, end, factor);
			InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit(rate2);
			for (int i = 0; i < 5; i++)
			{
				yield return null;
			}
		}
	}

	public float GetYFromZ(float z)
	{
		if (z < tunnelLength)
		{
			return z * slope + start.position.y;
		}
		return end.position.y;
	}

	public float GetDeltaY(float deltaZ)
	{
		return deltaZ * slope;
	}

	public float GetEndY()
	{
		if (end == null)
		{
			return 0f;
		}
		return end.position.y;
	}
}
