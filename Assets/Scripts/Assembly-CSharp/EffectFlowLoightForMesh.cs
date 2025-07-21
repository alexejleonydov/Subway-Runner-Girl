using UnityEngine;

public class EffectFlowLoightForMesh : MonoBehaviour
{
	public float mUvStart;

	public float mUvSpeed = 0.02f;

	public float mUvXMax = 0.9f;

	public float mTimeInteval = 3f;

	public MeshRenderer render;

	public Material mCurMaterial;

	private float mUvAdd;

	private bool mIsPlaying;

	private void Awake()
	{
		if (render != null)
		{
			render.material = new Material(mCurMaterial);
		}
		mUvAdd = 0f;
		mIsPlaying = true;
	}

	private void Update()
	{
		UpdateMaterial(render.material);
	}

	private void UpdateMaterial(Material mat)
	{
		if (mIsPlaying)
		{
			mUvAdd += mUvSpeed;
			mat.SetFloat(Shaders.Instance.FlowLightOffset, mUvStart + mUvAdd);
			mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 1f);
			if (mUvAdd >= mUvXMax)
			{
				mIsPlaying = false;
				mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 0f);
				StartCoroutine(DelayInvoke.start(delegate
				{
					PlayOnceAgain();
				}, mTimeInteval));
			}
		}
		else
		{
			mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 0f);
		}
	}

	private void PlayOnceAgain()
	{
		mUvAdd = 0f;
		mIsPlaying = true;
	}
}
