using UnityEngine;

public class SuperChestEffect : MonoBehaviour
{
	public ParticleSystem[] particleSystems = new ParticleSystem[0];

	public Renderer[] glowRenderers = new Renderer[0];

	private float _glowAniFactor;

	private Color[] _glowBaseColors;

	private Material[] _glowMaterials;

	private bool _glowShouldBeOn;

	public bool isRendering { get; set; }

	private void Awake()
	{
		isRendering = false;
	}

	private void Start()
	{
		int i = 0;
		for (int num = particleSystems.Length; i < num; i++)
		{
			ParticleSystem.MainModule main = particleSystems[i].main;
			main.playOnAwake = false;
		}
		_glowMaterials = new Material[glowRenderers.Length];
		_glowBaseColors = new Color[glowRenderers.Length];
		for (int j = 0; j < glowRenderers.Length; j++)
		{
			_glowMaterials[j] = glowRenderers[j].material;
			_glowBaseColors[j] = _glowMaterials[j].GetColor(Shaders.Instance.TintColor);
		}
		ApplyGlowAniFactor();
	}

	private void ApplyGlowAniFactor()
	{
		float num = 0.5f + 0.5f * Mathf.Cos(3.141593f + 3.141593f * _glowAniFactor);
		for (int i = 0; i < _glowMaterials.Length; i++)
		{
			_glowMaterials[i].SetColor(Shaders.Instance.TintColor, _glowBaseColors[i] * num);
		}
	}

	public void FastForwardEffect(int time)
	{
		int i = 0;
		for (int num = particleSystems.Length; i < num; i++)
		{
			particleSystems[i].Simulate(time);
		}
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public void StartEffect()
	{
		if (!isRendering)
		{
			isRendering = true;
			_glowShouldBeOn = true;
			int i = 0;
			for (int num = particleSystems.Length; i < num; i++)
			{
				particleSystems[i].Play();
			}
		}
	}

	public void StopEffect()
	{
		isRendering = false;
		_glowShouldBeOn = false;
		int i = 0;
		for (int num = particleSystems.Length; i < num; i++)
		{
			particleSystems[i].Stop();
		}
	}

	private void Update()
	{
		if (_glowShouldBeOn && _glowAniFactor < 1f)
		{
			_glowAniFactor = Mathf.Clamp01(_glowAniFactor + 0.5f * Time.deltaTime);
			ApplyGlowAniFactor();
		}
		else if (!_glowShouldBeOn && _glowAniFactor > 0f)
		{
			_glowAniFactor = Mathf.Clamp01(_glowAniFactor - 0.5f * Time.deltaTime);
			ApplyGlowAniFactor();
		}
	}
}
