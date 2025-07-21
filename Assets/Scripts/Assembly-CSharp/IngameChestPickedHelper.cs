using System.Collections;
using UnityEngine;

public class IngameChestPickedHelper : MonoBehaviour
{
	public delegate void OnPickedChestDelegate();

	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private Animation iconAnim;

	[SerializeField]
	private ParticleSystem followPs;

	[SerializeField]
	private Animation bgAnim;

	[SerializeField]
	private UILabel numberLbl;

	[SerializeField]
	private float delay;

	private int count;

	private float frame;

	private bool hasPlayAction;

	public static event OnPickedChestDelegate OnPickedChest;

	private void Awake()
	{
		Hide();
	}

	public void Hide()
	{
		parent.SetActive(false);
	}

	public void Show()
	{
		parent.SetActive(true);
		int num = GameStats.Instance.chestPickups;
		count = 0;
		while (num > 0)
		{
			if (num % 2 == 1)
			{
				count++;
			}
			num >>= 1;
		}
		numberLbl.text = "x" + (count - 1);
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		frame = 0f;
		hasPlayAction = false;
		iconAnim.Play();
		Check();
		yield return null;
		followPs.Play();
		float length2 = iconAnim.clip.length;
		float time2 = 0f;
		while (time2 < length2)
		{
			time2 += Time.deltaTime;
			followPs.transform.localPosition = iconAnim.transform.localPosition;
			Check();
			yield return null;
		}
		followPs.transform.localPosition = iconAnim.transform.localPosition;
		bgAnim.Play();
		numberLbl.text = "x" + count;
		length2 = bgAnim.clip.length;
		time2 = 0f;
		while (time2 < length2)
		{
			time2 += Time.deltaTime;
			Check();
			yield return null;
		}
		parent.SetActive(false);
		while (frame < delay)
		{
			frame += Time.deltaTime;
			yield return null;
		}
		if (!hasPlayAction && IngameChestPickedHelper.OnPickedChest != null)
		{
			IngameChestPickedHelper.OnPickedChest();
		}
	}

	private void Check()
	{
		if (hasPlayAction)
		{
			return;
		}
		frame += Time.deltaTime;
		if (frame >= delay)
		{
			if (IngameChestPickedHelper.OnPickedChest != null)
			{
				IngameChestPickedHelper.OnPickedChest();
			}
			hasPlayAction = true;
		}
	}
}
