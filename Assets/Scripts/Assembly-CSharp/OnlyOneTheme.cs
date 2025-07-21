using UnityEngine;

public class OnlyOneTheme : MonoBehaviour
{
	public Color fogColor;

	public Color fogGradientTop;

	public Color fogGradientBottom;

	public Color fogSilhouetteColor;

	public float fogGradientOffset;

	public Material glowGold;

	public Color glowGoldColor;

	public float glowGoldFalloff = 200f;

	private CameraCulling cameraCulling;

	private static OnlyOneTheme _instance;

	private void Awake()
	{
		UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
		glowGold.SetColor(Shaders.Instance.MainColor, glowGoldColor);
		glowGold.SetFloat(Shaders.Instance.Falloff, glowGoldFalloff);
		if (cameraCulling == null)
		{
			cameraCulling = Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling;
		}
		cameraCulling.TransparentFXCullingDistance = glowGoldFalloff;
		RenderSettings.fogColor = fogColor;
		SetShaderStates();
		if (Camera.main != null)
		{
			Camera.main.backgroundColor = fogGradientBottom;
		}
		Resources.UnloadUnusedAssets();
	}

	private void SetShaderStates()
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, fogGradientTop);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, fogGradientBottom);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, fogSilhouetteColor);
		Shader.SetGlobalFloat(Shaders.Instance.SkyGradientOffset, fogGradientOffset);
	}

	private void UIScreenController_OnApplicationResumed()
	{
		SetShaderStates();
	}
}
