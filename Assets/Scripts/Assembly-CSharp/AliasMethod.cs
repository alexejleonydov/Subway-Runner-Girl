using System;
using System.Collections.Generic;

public class AliasMethod
{
	private double[] probability;

	private int[] alias;

	private int length;

	private Random rand;

	public AliasMethod(double[] prob, Random rand)
	{
		if (prob == null || rand == null)
		{
			throw new NullReferenceException();
		}
		if (prob.Length == 0)
		{
			throw new ArgumentOutOfRangeException("Probability vector must be nonempty.");
		}
		this.rand = rand;
		length = prob.Length;
		probability = new double[length];
		alias = new int[length];
		double[] array = new double[length];
		Stack<int> stack = new Stack<int>();
		Stack<int> stack2 = new Stack<int>();
		for (int i = 0; i < length; i++)
		{
			array[i] = prob[i] * (double)length;
			if (array[i] < 1.0)
			{
				stack.Push(i);
			}
			else
			{
				stack2.Push(i);
			}
		}
		while (stack.Count > 0 && stack2.Count > 0)
		{
			int num = stack.Pop();
			int num2 = stack2.Pop();
			probability[num] = array[num];
			alias[num] = num2;
			array[num2] -= 1.0 - probability[num];
			if (array[num2] < 1.0)
			{
				stack.Push(num2);
			}
			else
			{
				stack2.Push(num2);
			}
		}
		while (stack.Count > 0)
		{
			probability[stack.Pop()] = 1.0;
		}
		while (stack2.Count > 0)
		{
			probability[stack2.Pop()] = 1.0;
		}
	}

	public int next()
	{
		int num = rand.Next(length);
		return (!(rand.NextDouble() < probability[num])) ? alias[num] : num;
	}
}
