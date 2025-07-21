using UnityEngine;

public class TaskPopup : UIBaseScreen
{
	[SerializeField]
	private TaskList taskList;

	[SerializeField]
	private UILabel titleLabel;

	private void OnEnable()
	{
		titleLabel.text = Strings.Get(LanguageKey.TASK_POPUP_MAIN_TITLE);
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		taskList.Show();
	}
}
