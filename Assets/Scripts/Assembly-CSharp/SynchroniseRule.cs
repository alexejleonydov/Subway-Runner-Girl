public class SynchroniseRule
{
	private bool _waitingForRespond;

	private float _time;

	private float _interval;

	public bool hasGotInitValue { get; set; }

	public SynchroniseRule(float interval)
	{
		_interval = interval;
		hasGotInitValue = false;
		_waitingForRespond = false;
		_time = -1f;
	}

	public bool Check()
	{
		if (hasGotInitValue)
		{
			return false;
		}
		return !_waitingForRespond || RealTimeTracker.time > _time;
	}

	public void Request()
	{
		_waitingForRespond = true;
		_time = RealTimeTracker.time + _interval;
	}

	public void Respond()
	{
		_waitingForRespond = false;
	}

	public void Reset()
	{
		hasGotInitValue = false;
		_waitingForRespond = false;
		_time = -1f;
	}
}
