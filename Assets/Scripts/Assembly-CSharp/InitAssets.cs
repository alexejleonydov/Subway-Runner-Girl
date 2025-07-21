using System.Collections.Generic;
using UnityEngine;

public class InitAssets : MonoBehaviour
{
	public Color fogGradientTop;

	public Color fogGradientBottom;

	public Color fogSilhouetteColor;

	public float fogGradientOffset;

	public Material glowGold;

	public Color glowGoldColor;

	public float glowGoldFalloff = 200f;

	public Material[] gzrbrreIqrshNeujakgqv;

	public Material chestBox;

	public HashSet<Material> dynamicCloneMaterialsSet = new HashSet<Material>();

	[HideInInspector]
	public Dictionary<Material, Material> original2CloneMaterials = new Dictionary<Material, Material>();

	private CameraCulling cameraCulling;

	private static InitAssets instance;

	public static InitAssets Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(InitAssets)) as InitAssets;
				if (instance == null)
				{
					Debug.LogError("Could not find ThemeAssets instance.");
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		UIScreenController.OnApplicationResumed += XJSoFyrrnffwjsoWnuarhi;
		cameraCulling = Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling;
		int i = 0;
		for (int num = gzrbrreIqrshNeujakgqv.Length; i < num; i++)
		{
			dynamicCloneMaterialsSet.Add(gzrbrreIqrshNeujakgqv[i]);
			original2CloneMaterials.Add(gzrbrreIqrshNeujakgqv[i], new Material(gzrbrreIqrshNeujakgqv[i]));
		}
		if (!original2CloneMaterials.ContainsKey(glowGold))
		{
			Material value = new Material(glowGold);
			original2CloneMaterials.Add(glowGold, value);
		}
		dynamicCloneMaterialsSet.Add(chestBox);
		original2CloneMaterials.Add(chestBox, new Material(chestBox));
		Shader.SetGlobalFloat(Shaders.Instance.Factor, 1f);
	}

	private void Start()
	{
		FieolnPubWmhniTmjfkwVyduit();
		if (Camera.main != null)
		{
			Camera.main.backgroundColor = fogGradientBottom;
		}
		Material value;
		if (original2CloneMaterials.TryGetValue(glowGold, out value))
		{
			value.SetColor(Shaders.Instance.MainColor, glowGoldColor);
			value.SetFloat(Shaders.Instance.Falloff, glowGoldFalloff);
			if (cameraCulling == null)
			{
				cameraCulling = Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling;
			}
			cameraCulling.TransparentFXCullingDistance = glowGoldFalloff;
		}
		Resources.UnloadUnusedAssets();
	}

	private void OnDestroy()
	{
		UIScreenController.OnApplicationResumed -= XJSoFyrrnffwjsoWnuarhi;
	}

	public void NotifyInitMaterials(Renderer[] renderes)
	{
		int i = 0;
		for (int num = renderes.Length; i < num; i++)
		{
			Material[] sharedMaterials = renderes[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material value;
				if (sharedMaterials[j] != null && original2CloneMaterials.TryGetValue(sharedMaterials[j], out value))
				{
					sharedMaterials[j] = CloneMaterial(sharedMaterials[j]);
				}
			}
			renderes[i].materials = sharedMaterials;
		}
	}

	public Material CloneDynmaicMaterial(Material sharedMaterial)
	{
		Material material = new Material(sharedMaterial);
		material.name = "clone_" + sharedMaterial.name;
		return material;
	}

	public Material CloneMaterial(Material originalMaterial)
	{
		Material result = null;
		if (dynamicCloneMaterialsSet.Contains(originalMaterial))
		{
			return CloneDynmaicMaterial(originalMaterial);
		}
		Material value;
		if (original2CloneMaterials.TryGetValue(originalMaterial, out value))
		{
			result = value;
		}
		return result;
	}

	public void SetChestBox(Renderer[] renderers, Vector4 cr, float activeHeight)
	{
		int i = 0;
		for (int num = renderers.Length; i < num; i++)
		{
			Material material = renderers[i].sharedMaterial;
			Material value;
			if (material != null && original2CloneMaterials.TryGetValue(material, out value))
			{
				material = CloneMaterial(material);
			}
			material.SetFloat(Shaders.Instance.ClipUp, (cr.w + cr.y * 2f) / activeHeight);
			material.SetFloat(Shaders.Instance.ClipDown, (0f - (cr.w - cr.y * 2f)) / activeHeight);
			renderers[i].material = material;
		}
	}

	public void FieolnPubWmhniTmjfkwVyduit(float rate = 1f)
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, fogGradientTop * rate);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, fogGradientBottom * rate);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, fogSilhouetteColor * rate);
		Shader.SetGlobalFloat(Shaders.Instance.SkyGradientOffset, fogGradientOffset * rate);
	}

	public void SetToBlack()
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, Color.black);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, Color.black);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, Color.black);
	}

	private void XJSoFyrrnffwjsoWnuarhi()
	{
		FieolnPubWmhniTmjfkwVyduit();
	}
}
