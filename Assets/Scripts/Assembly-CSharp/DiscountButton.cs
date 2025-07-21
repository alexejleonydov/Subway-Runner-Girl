using UnityEngine;

public class DiscountButton : MonoBehaviour
{
	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel description;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected GameObject vipTip;

	protected Vector3 originalContentPosition;

	protected Color32 originalDescriptionColor;

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalFillGrayScale;

	protected string originalFillGraySpriteName;

	protected Vector3 originalPricePosition;

	public virtual void Awake()
	{
		originalFillGraySpriteName = fillGraySprite.spriteName;
		originalFillGrayScale = new Vector3(fillGraySprite.width, fillGraySprite.height, 1f);
		originalContentPosition = content.localPosition;
		originalDescriptionPosition = description.cachedTransform.localPosition;
		originalPricePosition = price.cachedTransform.localPosition;
		originalDescriptionColor = description.color;
		vipTip.SetActive(false);
	}

	private void Common(int productIndex)
	{
		icon.spriteName = InAppData.inAppData[productIndex].iconName;
		price.text = InAppData.inAppData[productIndex].price;
		if (PlayerInfo.Instance.hasSubscribed && vipTip != null && InAppData.inAppData[productIndex].type == InAppData.DataType.Key)
		{
			vipTip.SetActive(true);
		}
	}

	protected virtual void ShowNoDiscount(int productIndex, string backupDescription = "")
	{
		fillGraySprite.width = Mathf.RoundToInt(originalFillGrayScale.x);
		fillGraySprite.height = Mathf.RoundToInt(originalFillGrayScale.y);
		content.localPosition = originalContentPosition;
		fillGraySprite.spriteName = originalFillGraySpriteName;
		price.transform.localPosition = originalPricePosition;
		if (string.IsNullOrEmpty(InAppData.inAppData[productIndex].description))
		{
			description.text = backupDescription;
		}
		else
		{
			description.text = InAppData.inAppData[productIndex].description;
		}
		description.color = Color.white;
		description.cachedTransform.localPosition = originalDescriptionPosition;
		description.color = originalDescriptionColor;
		Common(productIndex);
	}
}
