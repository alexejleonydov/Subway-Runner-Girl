using UnityEngine;

public class CoinScreen : UIBaseScreen
{
	public GameObject coinPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private GameObject listTitleComponent;

	private GameObject go;

	private bool _hasInited;

	private void FillTable()
	{
		if (!_hasInited)
		{
			int num = 0;
			GameObject gameObject = NGUITools.AddChild(_table.gameObject, listTitleComponent);
			ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
			gameObject.name = string.Format("{0:000}", num);
			component.Setup(Strings.Get(LanguageKey.COIN_SCREEN_STORE), Strings.Get(LanguageKey.COIN_SCREEN_STORE_DESCRIPTION));
			gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
			num++;
			for (int i = 0; i < InAppData.inAppData.Count; i++)
			{
				go = NGUITools.AddChild(_table.gameObject, coinPrefab);
				go.name = string.Format("{0:000}", num);
				go.GetComponent<CoinButtonHelper>().Init();
				go.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
				NGUITools.AddWidgetCollider(go);
				num++;
			}
			_hasInited = true;
			_table.Reposition();
		}
	}

	public override void Init()
	{
		base.Init();
		base.gameObject.SetActive(false);
		if (UIBaseScreen.IsOutOfProportion())
		{
			Vector3 localPosition = _parentDragPanel.transform.parent.localPosition;
			localPosition.y = 0f;
			_parentDragPanel.transform.parent.localPosition = localPosition;
		}
		InitializeCoinbox(true, true, true, true);
	}

	public override void Show()
	{
		base.Show();
		FillTable();
	}
}
