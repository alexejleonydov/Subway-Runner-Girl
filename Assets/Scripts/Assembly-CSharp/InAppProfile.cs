public class InAppProfile
{
	public int amountOfCoins;

	public int amountOfKeys;

	public int amountOfHelmets;

	public int amountOfHeadstarts;

	public int amountOfScoreboosters;

	public bool removeAd;

	public string title;

	public string description;

	public string iconName = string.Empty;

	public InAppData.DataType type;

	public bool isConsumable;

	public float priceAmount;

	public string price;

	public bool validInApp = true;

	public static InAppProfile getInAppProfileClone(InAppProfile objectToClone)
	{
		InAppProfile inAppProfile = new InAppProfile();
		inAppProfile.amountOfCoins = objectToClone.amountOfCoins;
		inAppProfile.amountOfKeys = objectToClone.amountOfKeys;
		inAppProfile.amountOfHelmets = objectToClone.amountOfHelmets;
		inAppProfile.amountOfHeadstarts = objectToClone.amountOfHeadstarts;
		inAppProfile.amountOfScoreboosters = objectToClone.amountOfScoreboosters;
		inAppProfile.removeAd = objectToClone.removeAd;
		inAppProfile.title = objectToClone.title;
		inAppProfile.description = objectToClone.description;
		inAppProfile.iconName = objectToClone.iconName;
		inAppProfile.type = objectToClone.type;
		inAppProfile.price = objectToClone.price;
		inAppProfile.validInApp = objectToClone.validInApp;
		return inAppProfile;
	}
}
