using System.Collections.Generic;
using UnityEngine;

public static class InAppData
{
	public enum DataType
	{
		None = 0,
		Coin = 1,
		Coin_Key = 2,
		Key = 3
	}

	private static bool _hasInited;

	private static Dictionary<int, InAppProfile> _inAppData;

	public static Dictionary<int, InAppProfile> inAppData
	{
		get
		{
			if (!_hasInited)
			{
				Init();
			}
			return _inAppData;
		}
	}

	static InAppData()
	{
		_inAppData = new Dictionary<int, InAppProfile>
		{
			{
				0,
				new InAppProfile
				{
					amountOfCoins = 7500,
					title = "com.ivyios.subwayrunner1",
					iconName = "jinbi1",
					type = DataType.Coin,
					price = "$0.99",
					isConsumable = true
				}
			},
			{
				1,
				new InAppProfile
				{
					amountOfCoins = 18000,
					title = "com.ivyios.subwayrunner2",
					iconName = "jinbi2",
					type = DataType.Coin,
					price = "$1.99",
					isConsumable = true
				}
			},
			{
				2,
				new InAppProfile
				{
					amountOfCoins = 30000,
					title = "com.ivyios.subwayrunner3",
					iconName = "jinbi3",
					type = DataType.Coin,
					price = "$2.99",
					isConsumable = true
				}
			},
			{
				3,
				new InAppProfile
				{
					amountOfCoins = 45000,
					title = "com.ivyios.subwayrunner4",
					iconName = "jinbi4",
					type = DataType.Coin,
					price = "$4.99",
					isConsumable = true
				}
			},
			{
				4,
				new InAppProfile
				{
					amountOfCoins = 100000,
					title = "com.ivyios.subwayrunner5",
					iconName = "jinbi5",
					type = DataType.Coin,
					price = "$9.99",
					isConsumable = true
				}
			},
			{
				5,
				new InAppProfile
				{
					amountOfKeys = 10,
					title = "com.ivyios.subwayrunner6",
					iconName = "diamond1",
					type = DataType.Key,
					price = "$0.99",
					isConsumable = true
				}
			},
			{
				6,
				new InAppProfile
				{
					amountOfKeys = 25,
					title = "com.ivyios.subwayrunner7",
					iconName = "diamond2",
					type = DataType.Key,
					price = "$1.99",
					isConsumable = true
				}
			},
			{
				7,
				new InAppProfile
				{
					amountOfKeys = 80,
					title = "com.ivyios.subwayrunner8",
					iconName = "diamond3",
					type = DataType.Key,
					price = "$5.99",
					isConsumable = true
				}
			},
			{
				8,
				new InAppProfile
				{
					amountOfKeys = 300,
					title = "com.ivyios.subwayrunner9",
					iconName = "diamond4",
					type = DataType.Key,
					price = "$9.99",
					isConsumable = true
				}
			},
			{
				14,
				new InAppProfile
				{
					amountOfCoins = 1000,
					amountOfKeys = 20,
					amountOfHeadstarts = 5,
					title = "com.ivyios.subwayrunner11",
					iconName = "diamond4",
					type = DataType.Coin_Key,
					price = "$0.99",
					priceAmount = 0.99f,
					isConsumable = true
				}
			},
			{
				15,
				new InAppProfile
				{
					amountOfCoins = 10000,
					amountOfKeys = 200,
					amountOfHeadstarts = 20,
					removeAd = true,
					title = "com.ivyios.subwayrunner12",
					iconName = "diamond4",
					type = DataType.Coin_Key,
					price = "$4.99",
					priceAmount = 4.99f,
					isConsumable = true
				}
			}
		};
	}

	private static void Init()
	{
		string paymentDatas = RiseSdk.Instance.GetPaymentDatas();
		Debug.Log("GetPaymentDatas: " + paymentDatas);
		IDictionary<string, object> dictionary = RiseJson.Deserialize(paymentDatas) as IDictionary<string, object>;
		InAppProfile inAppProfile = new InAppProfile();
		if (dictionary.ContainsKey("0"))
		{
			IDictionary<string, object> dictionary2 = dictionary["0"] as IDictionary<string, object>;
			inAppProfile = _inAppData[0];
			if (dictionary2.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary2["desc"];
			}
			if (dictionary2.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary2["price"];
			}
		}
		if (dictionary.ContainsKey("1"))
		{
			IDictionary<string, object> dictionary3 = dictionary["1"] as IDictionary<string, object>;
			inAppProfile = _inAppData[1];
			if (dictionary3.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary3["desc"];
			}
			if (dictionary3.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary3["price"];
			}
		}
		if (dictionary.ContainsKey("2"))
		{
			IDictionary<string, object> dictionary4 = dictionary["2"] as IDictionary<string, object>;
			inAppProfile = _inAppData[2];
			if (dictionary4.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary4["desc"];
			}
			if (dictionary4.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary4["price"];
			}
		}
		if (dictionary.ContainsKey("3"))
		{
			IDictionary<string, object> dictionary5 = dictionary["3"] as IDictionary<string, object>;
			inAppProfile = _inAppData[3];
			if (dictionary5.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary5["desc"];
			}
			if (dictionary5.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary5["price"];
			}
		}
		if (dictionary.ContainsKey("4"))
		{
			IDictionary<string, object> dictionary6 = dictionary["4"] as IDictionary<string, object>;
			inAppProfile = _inAppData[4];
			if (dictionary6.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary6["desc"];
			}
			if (dictionary6.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary6["price"];
			}
		}
		if (dictionary.ContainsKey("5"))
		{
			IDictionary<string, object> dictionary7 = dictionary["5"] as IDictionary<string, object>;
			inAppProfile = _inAppData[5];
			if (dictionary7.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary7["desc"];
			}
			if (dictionary7.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary7["price"];
			}
		}
		if (dictionary.ContainsKey("6"))
		{
			IDictionary<string, object> dictionary8 = dictionary["6"] as IDictionary<string, object>;
			inAppProfile = _inAppData[6];
			if (dictionary8.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary8["desc"];
			}
			if (dictionary8.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary8["price"];
			}
		}
		if (dictionary.ContainsKey("7"))
		{
			IDictionary<string, object> dictionary9 = dictionary["7"] as IDictionary<string, object>;
			inAppProfile = _inAppData[7];
			if (dictionary9.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary9["desc"];
			}
			if (dictionary9.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary9["price"];
			}
		}
		if (dictionary.ContainsKey("8"))
		{
			Debug.Log("GetPaymentDatas: 8");
			IDictionary<string, object> dictionary10 = dictionary["8"] as IDictionary<string, object>;
			inAppProfile = _inAppData[8];
			if (dictionary10.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary10["desc"];
			}
			if (dictionary10.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary10["price"];
			}
		}
		if (dictionary.ContainsKey("14"))
		{
			Debug.Log("GetPaymentDatas: 14");
			IDictionary<string, object> dictionary11 = dictionary["14"] as IDictionary<string, object>;
			inAppProfile = _inAppData[14];
			if (dictionary11.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary11["desc"];
			}
			if (dictionary11.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary11["price"];
			}
			if (dictionary11.ContainsKey("price_amount"))
			{
				inAppProfile.priceAmount = (float)(double)dictionary11["price_amount"];
			}
		}
		if (dictionary.ContainsKey("15"))
		{
			Debug.Log("GetPaymentDatas: 15");
			IDictionary<string, object> dictionary12 = dictionary["15"] as IDictionary<string, object>;
			inAppProfile = _inAppData[15];
			if (dictionary12.ContainsKey("title"))
			{
				inAppProfile.description = (string)dictionary12["desc"];
			}
			if (dictionary12.ContainsKey("price"))
			{
				inAppProfile.price = (string)dictionary12["price"];
			}
			if (dictionary12.ContainsKey("price_amount"))
			{
				inAppProfile.priceAmount = (float)(double)dictionary12["price_amount"];
			}
		}
		_hasInited = true;
	}
}
