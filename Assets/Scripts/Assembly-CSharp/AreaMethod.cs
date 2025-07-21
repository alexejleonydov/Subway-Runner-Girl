using System;

public class AreaMethod
{
	private double[] probability;

	private int length;

	private Random rand;

	private int index;

	public AreaMethod(double[] prob, Random random)
	{
		if (prob == null || rand == null)
		{
			throw new NullReferenceException();
		}
		if (prob.Length == 0)
		{
			throw new ArgumentOutOfRangeException("Probability vector must be nonempty.");
		}
		length = prob.Length;
		probability = new double[length];
		probability[0] = prob[0];
		for (int i = 1; i < length; i++)
		{
			probability[i] = probability[i - 1] + prob[i];
		}
	}

	public int next()
	{
		double num = rand.NextDouble();
		int result = 0;
		int num2 = 0;
		int num3 = length - 1;
		int num4 = 0;
		while (num2 < num3)
		{
			num4 = (num2 + num3) / 2;
			if (probability[num4] < num)
			{
				num2 = num4 + 1;
				if (probability[num2] > num)
				{
					result = num2;
					break;
				}
				continue;
			}
			if (probability[num4] > num)
			{
				num3 = num4 - 1;
				if (!(probability[num3] <= num))
				{
					continue;
				}
				result = num4;
				break;
			}
			result = num4 + 1;
			break;
		}
		return result;
	}
}
