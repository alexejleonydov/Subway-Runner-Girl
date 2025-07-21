public class RankCellScrollView : ScrollViewRecycle<RankCell, TopRun>
{
	protected override void RefreshUI(int cellIndex, TopRun index)
	{
		monos[cellIndex].RefreshUI(index);
	}
}
