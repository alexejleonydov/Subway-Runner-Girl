using UnityEngine;

public class CameraCulling : MonoBehaviour
{
	private enum FadeState
	{
		FadeIn = 0,
		FadeOut = 1,
		White = 2,
		None = 3
	}

	private float[] distances = new float[32];

	private Material mat;

	[SerializeField]
	private Shader fadeShader;

	[SerializeField]
	private FadeData defaultFadeData;

	private FadeState state;

	private float fadeup = 0.3f;

	private float ing = 0.3f;

	private float fadedown = 0.3f;

	private float factor;

	public float TransparentFXCullingDistance
	{
		set
		{
			distances[1] = value;
			GetComponent<Camera>().layerCullDistances = distances;
		}
	}

	private void Start()
	{
		if (mat == null)
		{
			mat = new Material(fadeShader);
		}
		base.enabled = false;
	}

	private void Update()
	{
		if (state == FadeState.FadeIn && factor < 1f)
		{
			factor += Time.deltaTime / fadeup;
			mat.SetFloat(Shaders.Instance.TintValue, (!(factor > 1f)) ? (1f - factor) : 0f);
			if (factor >= 1f)
			{
				factor = 0f;
				state = FadeState.White;
			}
		}
		if (state == FadeState.White)
		{
			if (factor < 1f)
			{
				factor += Time.deltaTime / ing;
			}
			else
			{
				factor = 0f;
				state = FadeState.FadeOut;
			}
		}
		if (state == FadeState.FadeOut)
		{
			if (factor < 1f)
			{
				factor += Time.deltaTime / fadedown;
				factor = ((!(factor > 1f)) ? factor : 1f);
				mat.SetFloat(Shaders.Instance.TintValue, factor);
			}
			else
			{
				factor = 0f;
				state = FadeState.None;
			}
		}
		if (state == FadeState.None)
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, mat);
	}

	private void SetToAndThenBack(Color color, float fadeup, float ing, float fadedown)
	{
		base.enabled = true;
		this.fadeup = fadeup;
		this.ing = ing;
		this.fadedown = fadedown;
		mat.SetColor(Shaders.Instance.Color, color);
		mat.SetFloat(Shaders.Instance.TintValue, 1f);
		factor = 0f;
		state = FadeState.FadeIn;
		Update();
	}

	public FadeData SetToAndThenBack()
	{
		SetToAndThenBack(defaultFadeData.color, defaultFadeData.fadeInDuration, defaultFadeData.onDuration, defaultFadeData.fadeOutDuration);
		return defaultFadeData;
	}

	public void SetToAndThenBack(FadeData fadeData)
	{
		SetToAndThenBack(fadeData.color, fadeData.fadeInDuration, fadeData.onDuration, fadeData.fadeOutDuration);
	}
}
