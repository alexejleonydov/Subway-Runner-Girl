using UnityEngine;

public class CharacterPopup : MonoBehaviour
{
	public Animation anim;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			anim.Stop();
		}
	}
}
