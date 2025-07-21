using UnityEngine;

public class UIButtonSendCount : MonoBehaviour
{
	public string[] IdString = new string[2];

	public string toPanelString;

	public void OnClick()
	{
		if (IdString.Length > 0)
		{
			for (int i = 0; i < IdString.Length; i++)
			{
				if (IdString != null)
				{
					string @event = IdString[i] + "_" + toPanelString;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, @event, 0);
				}
			}
		}
		else
		{
			Debug.LogError("Idstring is null!!");
		}
	}
}
