using UnityEngine;

[ExecuteInEditMode]
public class TwoGradientToTexture : MonoBehaviour
{
	public Gradient firstGradient = new Gradient();

	public Gradient secondGradient = new Gradient();

	public Texture2D gradientTexture;

	public int pixelWidth = 64;

	private void BuildGradient()
	{
		gradientTexture.Reinitialize(pixelWidth, 1);
		for (int i = 0; i < pixelWidth; i++)
		{
			if ((float)i / (float)pixelWidth < 0.5f)
			{
				gradientTexture.SetPixel(i, 0, firstGradient.Evaluate((float)i / ((float)(pixelWidth - 1) * 0.5f)));
			}
			else
			{
				gradientTexture.SetPixel(i, 0, secondGradient.Evaluate(((float)i - (float)(pixelWidth - 1) * 0.5f) / ((float)(pixelWidth - 1) * 0.5f)));
			}
		}
		gradientTexture.Apply();
	}

	public Texture2D Initialize()
	{
		gradientTexture = new Texture2D(pixelWidth, 2);
		BuildGradient();
		return gradientTexture;
	}
}
