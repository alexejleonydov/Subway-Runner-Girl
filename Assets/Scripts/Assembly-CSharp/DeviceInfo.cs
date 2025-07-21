using UnityEngine;

public class DeviceInfo
{
	public enum FormFactor
	{
		iPhone = 0,
		iPad = 1,
		small = 2,
		medium = 3,
		large = 4,
		iPhone5 = 5,
		iPhone6 = 6,
		iPhone6Plus = 7
	}

	public enum PerformanceLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public PerformanceLevel performanceLevel = PerformanceLevel.High;

	public string deviceModel = SystemInfo.deviceModel;

	public readonly float dpi;

	public readonly FormFactor formFactor;

	public readonly bool isHighres;

	private static DeviceInfo _instance;

	public static DeviceInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DeviceInfo();
			}
			return _instance;
		}
	}

	private DeviceInfo()
	{
		if (Screen.height < 500)
		{
			formFactor = FormFactor.small;
		}
		else if (Screen.height < 900)
		{
			formFactor = FormFactor.medium;
		}
		else
		{
			formFactor = FormFactor.large;
		}
		if (isTablet())
		{
			formFactor = FormFactor.iPad;
		}
		if (Screen.height >= 500 && Screen.width > 320)
		{
			isHighres = true;
		}
		else
		{
			isHighres = false;
		}
		formFactor = FormFactor.small;
		isHighres = false;
		dpi = Screen.dpi;
		if (dpi <= 0f)
		{
			dpi = 300f;
		}
		if (isDeviceLowPerformance())
		{
			performanceLevel = PerformanceLevel.Low;
		}
	}

	private bool isDeviceLowPerformance()
	{
		int processorCount = SystemInfo.processorCount;
		string processorType = SystemInfo.processorType;
		int systemMemorySize = SystemInfo.systemMemorySize;
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		if (processorCount >= 4)
		{
			return false;
		}
		if (processorType.Contains("rev"))
		{
			int num = processorType.IndexOf("rev");
			string text = processorType.Substring(num + 3).Trim();
			if (text.Contains(" "))
			{
				int length = text.IndexOf(" ");
				int result;
				if (int.TryParse(text.Substring(0, length).Trim(), out result))
				{
					bool flag = processorCount >= 2;
					bool flag2 = result >= 6;
					bool flag3 = systemMemorySize >= 512;
					bool flag4 = graphicsMemorySize >= 250;
					if (flag && flag2 && flag3 && flag4)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private bool isTablet()
	{
		float f = ((!(Screen.dpi <= 0f)) ? ((float)Screen.width / Screen.dpi) : ((float)Screen.width));
		float f2 = ((!(Screen.dpi <= 0f)) ? ((float)Screen.height / Screen.dpi) : ((float)Screen.height));
		double num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		return num >= 6.0;
	}
}
