using UnityEngine;

public class PayHelper : MonoBehaviour
{
	[SerializeField]
	private int billId;

	[SerializeField]
	private GameObject button;

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel discountLbl;

	[SerializeField]
	private UILabel tipLbl;

	[SerializeField]
	private UILabel coinsLbl;

	[SerializeField]
	private UILabel gemsLbl;

	[SerializeField]
	private UILabel propsLbl;

	[SerializeField]
	private UILabel remove_big_Lbl;

	[SerializeField]
	private UILabel remove_small_Lbl;

	[SerializeField]
	private UILabel priceLbl;

	[SerializeField]
	private UILabel invalidLbl;

	[SerializeField]
	private float off;

	public int BillId
	{
		get
		{
			return billId;
		}
	}

	public bool Check()
	{
		bool flag = PlayerPrefs.HasKey("PaymentResult_" + billId);
		bool flag2 = PlayerPrefs.GetInt("CheckResult_" + billId, 0) == 1;
		return !flag && !flag2;
	}

	public void OnButtonClick()
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			RiseSdk.Instance.Pay(billId);
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
		UIScreenController.Instance.ClosePopup(null);
	}

	private void OnEnable()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_BEGINNERS_PACK_CAPITAL);
		discountLbl.text = Strings.Get(LanguageKey.UI_SHOP_DISCOUNT_LABEL);
		tipLbl.text = Strings.Get(LanguageKey.UI_SHOP_BUY_TIMES_LABEL);
		coinsLbl.text = Strings.Get(LanguageKey.UI_LAB_COINS_CAPITAL);
		gemsLbl.text = Strings.Get(LanguageKey.UI_LAB_GEMS_CAPITAL);
		propsLbl.text = Strings.Get(LanguageKey.UI_LAB_PROPS_CAPITAL);
		remove_big_Lbl.text = Strings.Get(LanguageKey.UI_LAB_REMOVE_CAPITAL);
		remove_small_Lbl.text = Strings.Get(LanguageKey.UI_LAB_PRIVILEGE_CAPITAL);
		priceLbl.text = InAppData.inAppData[billId].price;
		if (invalidLbl != null)
		{
			float num = InAppData.inAppData[billId].priceAmount / (1f - off);
			num = (float)Mathf.RoundToInt(num * 100f) * 0.01f;
			invalidLbl.text = InAppData.inAppData[billId].price.Replace(InAppData.inAppData[billId].priceAmount.ToString(), num.ToString());
		}
	}
}
