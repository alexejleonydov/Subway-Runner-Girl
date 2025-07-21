using UnityEngine;

public class RenderersActivator : BaseO
{
	private Renderer[] renderers;

	protected override void Awake()
	{
		renderers = GetComponentsInChildren<Renderer>();
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		int i = 0;
		for (int num = renderers.Length; i < num; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = true;
			}
		}
	}

	public override void OnDeactivate()
	{
		int i = 0;
		for (int num = renderers.Length; i < num; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = false;
			}
		}
	}
}
