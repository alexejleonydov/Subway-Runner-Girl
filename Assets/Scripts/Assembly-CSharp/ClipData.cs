using System;

[Serializable]
public class ClipData
{
	public float startValue;

	public string path;

	public string propertyName;

	public ClipData(float value, string path, string propertyName)
	{
		startValue = value;
		this.path = path;
		this.propertyName = propertyName;
	}
}
