using UnityEngine;

public class PromotionManager : MonoBehaviour
{
	[SerializeField]
	private UITable table;

	[SerializeField]
	private PayHelper payHelper1;

	[SerializeField]
	private PayHelper payHelper2;

	[SerializeField]
	private Vector3 one;

	[SerializeField]
	private Vector3 two;

	public UITable.OnReposition onReposition;

	public void RefreshPayHelps()
	{
		bool flag = payHelper1.Check();
		payHelper1.gameObject.SetActive(flag);
		if (flag)
		{
			payHelper1.transform.localPosition = one;
		}
		bool flag2 = payHelper2.Check();
		payHelper2.gameObject.SetActive(flag2);
		if (flag2)
		{
			payHelper2.transform.localPosition = ((!flag) ? one : two);
		}
		table.Reposition();
	}

	private void OnEnable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
		RiseSdkListener.OnPaymentEvent += PayResult;
	}

	private void OnDisable()
	{
		RiseSdkListener.OnPaymentEvent -= PayResult;
	}

	public void PayResult(RiseSdk.PaymentResult result, int billId)
	{
		if ((billId == payHelper1.BillId || billId == payHelper2.BillId) && result == RiseSdk.PaymentResult.Success)
		{
			PlayerPrefs.SetInt("PaymentResult_" + billId, 1);
			RefreshPayHelps();
		}
	}
}
