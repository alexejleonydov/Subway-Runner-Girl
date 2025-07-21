using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TestAnimation : MonoBehaviour
{
	public AnimationClip[] Animations;

	private Animation _Animation;

	private void Start()
	{
		_Animation = GetComponent<Animation>();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("1"))
		{
			_Animation.CrossFadeQueued(Animations[0].name);
		}
		if (GUILayout.Button("2"))
		{
			_Animation.Stop();
		}
		if (GUILayout.Button("3"))
		{
			_Animation["run2"].enabled = true;
		}
		if (GUILayout.Button("4"))
		{
			foreach (AnimationState item in _Animation)
			{
				Debug.Log(item.clip.name);
				Debug.Log(item.name);
				Debug.Log(item.blendMode);
			}
		}
		if (GUILayout.Button("5"))
		{
			_Animation[Animations[0].name].wrapMode = WrapMode.PingPong;
			_Animation.Play(Animations[0].name);
		}
	}
}
