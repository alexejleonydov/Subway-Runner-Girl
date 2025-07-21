using System;
using System.Collections.Generic;
using UnityEngine;

public class RecodeManager
{
	[Serializable]
	public class RecodeData
	{
		public Good[] data;
	}

	[Serializable]
	public class Good
	{
		public string _type;

		public string _id;

		public string _num;
	}

	public enum RecodeResult
	{
		Success = 0,
		OutTime = 1,
		Invalid = 2,
		HadUsed = 3,
		None = 4
	}

	private static RecodeManager _instance;

	private RecodeResult result = RecodeResult.None;

	private Good[] goods;

	public static RecodeManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RecodeManager();
			}
			return _instance;
		}
	}

	public void SendRecode(string recode)
	{
		result = RecodeResult.None;
		goods = null;
		IvyApp.Instance.Recode(recode, RecodeListener);
	}

	private void RecodeListener(string text)
	{
		Debug.Log("RecodeListener:::" + text);
		result = RecodeResult.None;
		if (string.IsNullOrEmpty(text))
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			return;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0 || !dictionary.ContainsKey("status") || !dictionary.ContainsKey("data"))
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			return;
		}
		switch ((int)(long)dictionary["status"])
		{
		case 200:
		{
			IList<object> list = dictionary["data"] as IList<object>;
			goods = new Good[list.Count];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				goods[i] = new Good();
				dictionary = list[i] as IDictionary<string, object>;
				if (dictionary.ContainsKey("goods_type"))
				{
					goods[i]._type = (string)dictionary["goods_type"];
				}
				if (dictionary.ContainsKey("goods_id"))
				{
					goods[i]._id = (string)dictionary["goods_id"];
				}
				if (dictionary.ContainsKey("goods_num"))
				{
					goods[i]._num = (string)dictionary["goods_num"];
				}
			}
			result = RecodeResult.Success;
			UISliderInController.Instance.OnRecodeStatusPickedUp(true);
			UIScreenController.Instance.PushPopup("RedeemPopup");
			break;
		}
		case 201:
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			result = RecodeResult.OutTime;
			break;
		case 202:
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			result = RecodeResult.Invalid;
			break;
		case 203:
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			result = RecodeResult.HadUsed;
			break;
		}
	}

	public Good[] GetRecodes()
	{
		if (result == RecodeResult.Success)
		{
			return goods;
		}
		return null;
	}

	public void GetRecodeGoods()
	{
		if (result != 0)
		{
			return;
		}
		Good good = null;
		int i = 0;
		for (int num = goods.Length; i < num; i++)
		{
			good = goods[i];
			int num2;
			if (int.TryParse(good._num, out num2))
			{
				switch (good._id)
				{
				case "1":
					PlayerInfo.Instance.amountOfCoins += num2;
					TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, num2);
					break;
				case "2":
					PlayerInfo.Instance.amountOfKeys += num2;
					break;
				}
			}
		}
	}
}
