using UnityEngine;

public class RendererActivator : BaseO
{
	private Renderer render;

	protected override void Awake()
	{
		render = GetComponent<Renderer>();
		base.Awake();
		OnDeactivate();
	}

	public override void OnActivate()
	{
		render.enabled = true;
	}

	public override void OnDeactivate()
	{
		render.enabled = false;
	}
}
