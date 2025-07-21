using UnityEngine;

namespace InAppPurchase.Internal
{
	public class AndroidPurchaseProviderCallbackListener : MonoBehaviour
	{
		public int[] pass = new int[33]
		{
			3, 1, 4, 1, 5, 9, 2, 6, 5, 3,
			5, 3, 1, 4, 1, 5, 9, 2, 6, 5,
			3, 5, 3, 1, 4, 1, 5, 9, 2, 6,
			5, 3, 5
		};

		public string data;

		public string result;

		[ContextMenu("From")]
		public void From()
		{
			result = From(data);
		}

		[ContextMenu("To")]
		public void To()
		{
			result = To(data);
		}

		private string To(string data)
		{
			char[] array = data.ToCharArray();
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int num2 = array[i] + pass[i];
				if (num2 > 122)
				{
					num2 = num2 - 122 - 1 + 97;
				}
				else if (num2 > 90)
				{
					num2 = num2 - 90 - 1 + 65;
				}
				array[i] = (char)num2;
			}
			return new string(array);
		}

		private string From(string data)
		{
			char[] array = data.ToCharArray();
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int num2 = array[i] - pass[i];
				if (array[i] <= 'Z' && num2 < 65)
				{
					num2 = 90 - (65 - num2 - 1);
				}
				if (array[i] >= 'a' && num2 < 97)
				{
					num2 = 122 - (97 - num2 - 1);
				}
				array[i] = (char)num2;
			}
			return new string(array);
		}
	}
}
