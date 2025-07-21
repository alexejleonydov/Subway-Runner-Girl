using System;

[Serializable]
public class TopRun
{
	public string userId;

	public int highestScore;

	public int rank;

	public TopRun()
	{
		rank = -1;
		highestScore = 0;
	}
}
