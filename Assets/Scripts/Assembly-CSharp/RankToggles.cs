using UnityEngine;

public class RankToggles : MonoBehaviour
{
	[SerializeField]
	private TopRunToggle global;

	[SerializeField]
	private TopRunToggle vip;

	[SerializeField]
	private TopRunToggle friend;

	public void Global()
	{
		global.Toggle(true);
		vip.Toggle(false);
		friend.Toggle(false);
	}

	public void Vip()
	{
		global.Toggle(false);
		vip.Toggle(true);
		friend.Toggle(false);
	}

	public void Friend()
	{
		global.Toggle(false);
		vip.Toggle(false);
		friend.Toggle(true);
	}
}
