using UnityEngine;

public class ListTitleComponentHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel bigLabel;

	[SerializeField]
	private UILabel smallLabel;

	public void Setup(string text, string descripe)
	{
		bigLabel.text = text;
		smallLabel.text = descripe;
	}
}
