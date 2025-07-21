using UnityEngine;

public class UIChestReward : MonoBehaviour
{
	[SerializeField]
	private Animation anim;

	[SerializeField]
	private UISprite fillSpr;

	[SerializeField]
	private UISprite iconSpr;

	[SerializeField]
	private UILabel numberLbl;

	[SerializeField]
	private UILabel descripeLbl;

	[SerializeField]
	private GameObject sliderGo;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UILabel progressLbl;

	[SerializeField]
	private float iconSize = 0.6f;

	private PrizeEntryTemplate _template;

	private int _have;

	private float _total;

	public void Show(PrizeEntryType type, int amount)
	{
		sliderGo.SetActive(false);
		_template = ChestsData.GetPrizeEntryTemplate(type);
		if (_template == null)
		{
			Debug.LogError("There is not a PrizeEntryTemplate with the type of " + type);
			return;
		}
		fillSpr.spriteName = _template.bg_large;
		iconSpr.spriteName = _template.icon;
		iconSpr.MakePixelPerfect();
		float aspectRatio = iconSpr.aspectRatio;
		iconSpr.width = (int)((float)fillSpr.width * iconSize);
		iconSpr.height = (int)((float)iconSpr.width / aspectRatio);
		numberLbl.text = amount.ToString();
		descripeLbl.text = Strings.Get(_template.description);
		_have = _template.Have() - amount;
		_total = _template.Total();
	}

	public bool InitProgress(bool counting)
	{
		if (_template == null || !_template.UseSlider())
		{
			return false;
		}
		sliderGo.SetActive(true);
		if (!counting)
		{
			_have = _template.Have();
		}
		RefreshProgress(0);
		return true;
	}

	public void RefreshProgress(int amount)
	{
		if (_template != null && _template.UseSlider())
		{
			slider.value = (float)(_have + amount) / _total;
			progressLbl.text = _have + amount + "/" + _total;
		}
	}

	public float Appear()
	{
		if (anim == null)
		{
			return 0f;
		}
		anim.Play("Appear");
		return anim["Appear"].length;
	}

	public void Stop()
	{
		if (!(anim == null))
		{
			anim.Stop();
		}
	}
}
