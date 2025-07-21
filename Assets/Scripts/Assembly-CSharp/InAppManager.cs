using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private static InAppManager _instance;

	private InAppManagerPopupData _popupData;

	public static InAppManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null)
		{
			Object.DestroyImmediate(this);
		}
		else
		{
			_instance = this;
		}
	}

	public InAppManagerPopupData GetPopupData()
	{
		return _popupData;
	}

	public static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "InAppManager";
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<InAppManager>();
		}
	}

	public void SetupNativePopup(int cost, bool isPopup, InAppData.DataType type)
	{
		int num = 0;
		string empty = string.Empty;
		bool isCoins = false;
		if (type == InAppData.DataType.Coin)
		{
			isCoins = true;
			num = cost - PlayerInfo.Instance.amountOfCoins;
			empty = Strings.Get(LanguageKey.NOT_ENOUGH_COINS);
		}
		else
		{
			num = cost - PlayerInfo.Instance.amountOfKeys;
			empty = Strings.Get(LanguageKey.NOT_ENOUGH_KEYS);
		}
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		if (type == InAppData.DataType.Coin)
		{
			empty2 = ((num >= 2) ? Strings.Get(LanguageKey.PURCHASE_COINS_MULTIPLE_NO_INTERNET) : Strings.Get(LanguageKey.PURCHASE_COINS_ONE_NO_INTERNET));
			empty3 = string.Format(empty2, num);
		}
		else
		{
			empty2 = ((num >= 2) ? Strings.Get(LanguageKey.PURCHASE_KEYS_MULTIPLE_NO_INTERNET) : Strings.Get(LanguageKey.PURCHASE_KEYS_ONE_NO_INTERNET));
			empty3 = string.Format(empty2, num);
		}
		if (UIScreenController.isInstanced)
		{
			InAppManagerPopupData inAppManagerPopupData = new InAppManagerPopupData();
			inAppManagerPopupData.popupTitle = empty;
			inAppManagerPopupData.popupDescription = empty3;
			inAppManagerPopupData.isCoins = isCoins;
			inAppManagerPopupData.lastIsPopup = isPopup;
			_popupData = inAppManagerPopupData;
			UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
		}
	}
}
