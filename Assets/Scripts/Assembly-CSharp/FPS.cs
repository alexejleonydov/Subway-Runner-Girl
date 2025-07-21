using UnityEngine;

public class FPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private float fps;

	private void Start()
	{
		timeleft = updateInterval;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			fps = accum / (float)frames;
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}

	private void OnGUI()
	{
		if (fps < 20f)
		{
			GUI.color = Color.red;
		}
		else if (fps < 30f)
		{
			GUI.color = Color.yellow;
		}
		else
		{
			GUI.color = Color.green;
		}
		GUI.Label(new Rect(100f, 100f, 300f, 300f), string.Format("{0:F2} FPS", fps));
	}
}
