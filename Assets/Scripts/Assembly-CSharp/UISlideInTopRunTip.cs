using UnityEngine;

public class UISlideInTopRunTip : UISlideIn
{
	[SerializeField]
	private UILabel _tipLabel;

	public void SetupSlideTopRunTip(string tip)
	{
		base.gameObject.SetActive(true);
		_tipLabel.text = tip;
		SlideIn(null);
	}
}
