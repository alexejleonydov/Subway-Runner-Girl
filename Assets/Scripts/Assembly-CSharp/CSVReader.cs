using System.Text;

public class CSVReader
{
	private BetterList<string> mTemp = new BetterList<string>();

	private byte[] mBuffers;

	private int mOffset;

	private bool canRead
	{
		get
		{
			return mBuffers != null && mOffset < mBuffers.Length;
		}
	}

	public CSVReader(byte[] buffers)
	{
		mBuffers = buffers;
		mOffset = 0;
	}

	public BetterList<string> ReadLine()
	{
		mTemp.Clear();
		int num = mBuffers.Length;
		string text = string.Empty;
		bool flag = false;
		int num2 = 0;
		while (canRead)
		{
			if (flag)
			{
				string text2 = ReadLine(false);
				if (text2 == null)
				{
					return null;
				}
				text2 = text2.Replace("\\n", "\n");
				text = text + "\n" + text2;
			}
			else
			{
				string text3 = ReadLine(true);
				if (text3 == null)
				{
					return null;
				}
				text = text3.Replace("\\n", "\n");
				num2 = 0;
			}
			int i = num2;
			for (int length = text.Length; i < length; i++)
			{
				int num3 = text[i];
				if (num3 == 44 && !flag)
				{
					mTemp.Add(text.Substring(num2, i - num2));
					num2 = i + 1;
				}
				if (num3 != 34)
				{
					continue;
				}
				if (flag)
				{
					if (i + 1 >= length)
					{
						mTemp.Add(text.Substring(num2, i - num2).Replace("\"\"", "\""));
						return mTemp;
					}
					if (text[i + 1] != '"')
					{
						flag = false;
						mTemp.Add(text.Substring(num2, i - num2).Replace("\"\"", "\""));
						if (text[i + 1] == ',')
						{
							i++;
							num2 = i + 1;
						}
					}
					else
					{
						i++;
					}
				}
				else
				{
					num2 = i + 1;
					flag = true;
				}
			}
			if (num2 >= text.Length || flag)
			{
				continue;
			}
			mTemp.Add(text.Substring(num2, text.Length - num2));
			return mTemp;
		}
		return null;
	}

	private string ReadLine(int index, int count)
	{
		return Encoding.UTF8.GetString(mBuffers, index, count);
	}

	private string ReadLine(bool skipEmptyLines)
	{
		int num = mBuffers.Length;
		if (skipEmptyLines)
		{
			while (mOffset < num && mBuffers[mOffset] < 32)
			{
				mOffset++;
			}
		}
		int num2 = mOffset;
		if (num2 < num)
		{
			int num3;
			do
			{
				if (num2 < num)
				{
					num3 = mBuffers[num2++];
					continue;
				}
				num2++;
				break;
			}
			while (num3 != 10 && num3 != 13);
			string result = ReadLine(mOffset, num2 - mOffset - 1);
			mOffset = num2;
			return result;
		}
		mOffset = num;
		return null;
	}
}
