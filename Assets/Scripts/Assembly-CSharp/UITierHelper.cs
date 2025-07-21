using UnityEngine;

public class UITierHelper : MonoBehaviour
{
	private PropType _type;

	public UIAtlas usedAtlas;

	private UISprite[] _slots;

	private UISprite[] _activeSlots;

	private bool _hasInited;

	public void ResetTiers()
	{
		if (_hasInited)
		{
			int currentTier = PlayerInfo.Instance.GetCurrentTier(_type);
			for (int i = 0; i < _activeSlots.Length; i++)
			{
				_activeSlots[i].enabled = i < currentTier;
			}
		}
	}

	public void SetupTiers(PropType type)
	{
		if (_hasInited)
		{
			return;
		}
		_type = type;
		Upgrade upgrade = Upgrades.upgrades[type];
		int numberOfTiers = upgrade.numberOfTiers;
		int currentTier = PlayerInfo.Instance.GetCurrentTier(type);
		_slots = new UISprite[numberOfTiers - 1];
		_activeSlots = new UISprite[numberOfTiers - 1];
		UISprite uISprite = null;
		for (int i = 0; i < numberOfTiers - 1; i++)
		{
			uISprite = NGUITools.AddSprite(base.gameObject, usedAtlas, string.Format(UIPosScalesAndNGUIAtlas.Instance.slotFormat, 1));
			uISprite.name = "slot" + (i + 1);
			uISprite.pivot = UIWidget.Pivot.BottomLeft;
			uISprite.cachedTransform.localPosition = new Vector3(71 * i, 0f, 0f);
			uISprite.depth = 12;
			uISprite.MakePixelPerfect();
			_slots[i] = uISprite;
			uISprite = NGUITools.AddSprite(base.gameObject, usedAtlas, string.Format(UIPosScalesAndNGUIAtlas.Instance.slotFormat, 2));
			uISprite.name = "ActiveSlot" + (i + 1);
			uISprite.pivot = UIWidget.Pivot.BottomLeft;
			uISprite.transform.localPosition = new Vector3(71 * i, 0f, 0f);
			uISprite.depth = 12;
			uISprite.MakePixelPerfect();
			if (i >= currentTier)
			{
				uISprite.enabled = false;
			}
			_activeSlots[i] = uISprite;
		}
		_hasInited = true;
	}
}
