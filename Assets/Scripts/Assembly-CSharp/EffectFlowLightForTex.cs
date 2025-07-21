using System;
using UnityEngine;

public class EffectFlowLightForTex : MonoBehaviour
{
	public float mUvStart;

	public float mUvSpeed = 0.02f;

	public float mUvXMax = 0.9f;

	public float mTimeInteval = 3f;

	public Material mCurMaterial;

	private float mUvAdd;

	private bool mIsPlaying;

	private void Awake()
	{
		UITexture component = base.gameObject.GetComponent<UITexture>();
		if (component != null)
		{
			component.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(component.onRender, new UIDrawCall.OnRenderCallback(UpdateMaterial));
			component.material = mCurMaterial;
		}
		else
		{
			UISprite component2 = base.gameObject.GetComponent<UISprite>();
			if (component2 != null)
			{
				component2.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(component2.onRender, new UIDrawCall.OnRenderCallback(UpdateMaterial));
			}
			component2.mGlitterMaterial = new Material(mCurMaterial);
		}
		mUvAdd = 0f;
		mIsPlaying = true;
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
				DelayInvoke.delayDo(delegate
				{
					PlayOnceAgain();
				}, mTimeInteval);
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
