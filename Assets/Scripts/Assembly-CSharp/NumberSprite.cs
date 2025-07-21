using UnityEngine;

public class NumberSprite : MonoBehaviour
{
	[SerializeField]
	private UISprite x;

	[SerializeField]
	private UISprite num1;

	[SerializeField]
	private UISprite num2;

	[SerializeField]
	private UISprite num3;

	public void SetLevelNumber(int number)
	{
		int num = number / 100;
		int num2 = number % 100 / 10;
		int num3 = number % 10;
		if (num > 0)
		{
			num1.enabled = true;
			this.num2.enabled = true;
			this.num3.enabled = true;
			this.num3.spriteName = "A_sz" + num;
			this.num3.transform.localPosition = new Vector3(x.width - 10, 0f, 0f);
			this.num3.MakePixelPerfect();
			this.num2.spriteName = "A_sz" + num2;
			this.num2.transform.localPosition = new Vector3(x.width + this.num3.width - 20, 0f, 0f);
			this.num2.MakePixelPerfect();
			num1.spriteName = "A_sz" + num3;
			num1.transform.localPosition = new Vector3(x.width + this.num3.width + this.num2.width - 30, 0f, 0f);
			num1.MakePixelPerfect();
		}
		else if (num2 > 0)
		{
			num1.enabled = true;
			this.num2.enabled = true;
			this.num3.enabled = false;
			this.num2.spriteName = "A_sz" + num2;
			this.num2.transform.localPosition = new Vector3(x.width, 0f, 0f);
			this.num2.MakePixelPerfect();
			num1.spriteName = "A_sz" + num3;
			num1.transform.localPosition = new Vector3(x.width + this.num2.width, 0f, 0f);
			num1.MakePixelPerfect();
		}
		else
		{
			num1.enabled = true;
			this.num2.enabled = false;
			this.num3.enabled = false;
			num1.spriteName = "A_sz" + num3;
			num1.transform.localPosition = new Vector3(x.width, 0f, 0f);
			num1.MakePixelPerfect();
		}
	}
}
