using UnityEngine;

public class AchievementScreen : UIBaseScreen
{
	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private AchievementScrollView scrollView;

	[SerializeField]
	private GameObject achievementCellPrefab;

	public override void Init()
	{
		base.Init();
		InitCell();
	}

	private void InitCell()
	{
		int num = 0;
		int maxCellCount = scrollView.MaxCellCount;
		for (int i = 0; i < maxCellCount; i++)
		{
			GameObject gameObject = NGUITools.AddChild(scrollView.gameObject, achievementCellPrefab);
			gameObject.name = string.Format("{0:D3}", num);
			AchievementCell component = gameObject.GetComponent<AchievementCell>();
			if (component == null)
			{
				Debug.LogError("The achievementCellPrefab has no class - AchievementCell.");
			}
			scrollView.SetCell(component, i);
			if (animatorCtrl != null)
			{
				Animator component2 = gameObject.GetComponent<Animator>();
				if (component2 != null)
				{
					animatorCtrl.AddAnimator(component2);
				}
			}
			num++;
		}
	}

	private void RefreshOrder()
	{
		int num = 0;
		int num2 = 0;
		int[] orders = scrollView.orders;
		for (int i = 0; i < orders.Length; i++)
		{
			num = GetState(i);
			if (num == 1)
			{
				orders[num2++] = i;
			}
		}
		for (int j = 0; j < orders.Length; j++)
		{
			num = GetState(j);
			if (num == 2)
			{
				orders[num2++] = j;
			}
		}
		for (int k = 0; k < orders.Length; k++)
		{
			num = GetState(k);
			if (num == 3)
			{
				orders[num2++] = k;
			}
		}
		scrollView.ResetCell();
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(scrollView.contentPivot);
		scrollView.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, false);
	}

	public int GetState(int index)
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(index + 3);
		if (PlayerInfo.Instance.GetCurrentAchievementAward(index))
		{
			return 3;
		}
		if (taskInfo.complete)
		{
			return 1;
		}
		return 2;
	}

	public override void Show()
	{
		base.Show();
		RefreshOrder();
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_TITLE);
	}
}
