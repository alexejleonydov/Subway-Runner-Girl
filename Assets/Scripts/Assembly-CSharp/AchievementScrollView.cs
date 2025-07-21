public class AchievementScrollView : ScrollViewRecycle<AchievementCell, int>
{
	protected override void RefreshUI(int cellIndex, int index)
	{
		monos[cellIndex].RefreshUI(index);
	}
}
