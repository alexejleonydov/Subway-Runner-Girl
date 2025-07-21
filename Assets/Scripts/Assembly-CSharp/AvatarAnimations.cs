using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	public Animation Target;

	public bool PlayIdleAnimations;

	public AnimationClip Celebrate;

	public AnimationClip Breath;

	public AnimationClip startClip;

	public List<AnimationClip> Idles;

	public List<AnimationClip> HighscorePopup;

	public List<AnimationClip> UnlockPopup;

	public bool Paused;

	private IEnumerator routine;

	private float nextTime;

	private bool useHighscoreIdles;

	private bool useStartClip;

	private bool useUnlockIdles;

	private AvatarEyeAnimation eyeAnimation;

	private string celebrateName;

	private string breathName;

	private string startClipName;

	private List<string> IdlesName;

	private List<string> HighscoreName;

	private List<string> UnlockPopupName;

	private bool _hasInited;

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private IEnumerator Play()
	{
		CharacterModel model = base.transform.parent.parent.parent.parent.GetComponent<CharacterModel>();
		if (model.PlayBall.gameObject.activeSelf)
		{
			model.PlayBall.gameObject.SetActive(false);
		}
		if (model.Flower.gameObject.activeSelf)
		{
			model.Flower.gameObject.SetActive(false);
		}
		if (model.Heart.gameObject.activeSelf)
		{
			model.Heart.gameObject.SetActive(false);
		}
		if (!useHighscoreIdles && !useUnlockIdles)
		{
			if (useStartClip)
			{
				Target.Play(startClipName);
				if (startClipName.Equals("SW_B_01_00_alert") || startClipName.Equals("SW_B_01_00_idle") || startClipName.Equals("SW_B_01_00_idle_2"))
				{
					model.PlayBall.gameObject.SetActive(true);
					model.PlayBall[startClipName + "_q"].time = 0f;
					model.PlayBall.Play(startClipName + "_q");
				}
				if (startClipName.Equals("SW_B_03_00_idle2"))
				{
					model.Flower.gameObject.SetActive(true);
					model.Flower[startClipName + "_hua"].time = 0f;
					model.Flower.Play(startClipName + "_hua");
				}
				if (startClipName.Equals("SW_B_03_00_idle"))
				{
					model.Heart.gameObject.SetActive(true);
					model.Heart[startClipName + "_heart"].time = 0f;
					model.Heart.Play(startClipName + "_heart");
				}
				nextTime = startClip.length;
				while (nextTime > 0f)
				{
					nextTime -= Time.deltaTime;
					yield return null;
				}
				useStartClip = false;
				if (model.PlayBall.gameObject.activeSelf)
				{
					model.PlayBall.gameObject.SetActive(false);
				}
				if (model.Flower.gameObject.activeSelf)
				{
					model.Flower.gameObject.SetActive(false);
				}
				if (model.Heart.gameObject.activeSelf)
				{
					model.Heart.gameObject.SetActive(false);
				}
			}
		}
		else if (useHighscoreIdles && useStartClip)
		{
			Target.Play(celebrateName);
			if (celebrateName.Equals("SW_B_01_00_alert") || celebrateName.Equals("SW_B_01_00_idle") || celebrateName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[celebrateName + "_q"].time = 0f;
				model.PlayBall.Play(celebrateName + "_q");
			}
			if (celebrateName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[celebrateName + "_hua"].time = 0f;
				model.Flower.Play(celebrateName + "_hua");
			}
			if (celebrateName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[celebrateName + "_heart"].time = 0f;
				model.Heart.Play(celebrateName + "_heart");
			}
			nextTime = Celebrate.length;
			while (nextTime > 0f)
			{
				nextTime -= Time.deltaTime;
				yield return null;
			}
			useStartClip = false;
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
		}
		if ((bool)eyeAnimation && !eyeAnimation.IsAnimating())
		{
			eyeAnimation.StartAnimatingEyes();
		}
		if (!useHighscoreIdles && !useUnlockIdles)
		{
			Target.Play(breathName);
			if (breathName.Equals("SW_B_01_00_alert") || breathName.Equals("SW_B_01_00_idle") || breathName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[breathName + "_q"].time = 0f;
				model.PlayBall.Play(breathName + "_q");
			}
			if (breathName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[breathName + "_hua"].time = 0f;
				model.Flower.Play(breathName + "_hua");
			}
			if (breathName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[breathName + "_heart"].time = 0f;
				model.Heart.Play(breathName + "_heart");
			}
			nextTime = Breath.length;
			while (nextTime > 0f)
			{
				nextTime -= Time.deltaTime;
				yield return null;
			}
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
		}
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = (useHighscoreIdles ? HighscorePopup.FindAll(IsNotNull) : ((!useUnlockIdles) ? Idles.FindAll(IsNotNull) : UnlockPopup.FindAll(IsNotNull)));
			AnimationClip selectedClip = null;
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[Random.Range(0, possibleClips.Count)];
			}
			string aniName = selectedClip.name;
			Target.Play(aniName);
			nextTime = selectedClip.length;
			if (aniName.Equals("SW_B_01_00_alert") || aniName.Equals("SW_B_01_00_idle") || aniName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[aniName + "_q"].time = 0f;
				model.PlayBall.Play(aniName + "_q");
			}
			if (aniName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[aniName + "_hua"].time = 0f;
				model.Flower.Play(aniName + "_hua");
			}
			if (aniName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[aniName + "_heart"].time = 0f;
				model.Heart.Play(aniName + "_heart");
			}
			while (nextTime > 0f)
			{
				nextTime -= Time.deltaTime;
				yield return null;
			}
			yield return null;
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
			if (useHighscoreIdles || useUnlockIdles)
			{
				continue;
			}
			int count = 0;
			Target.Play(breathName);
			if (breathName.Equals("SW_B_01_00_alert") || breathName.Equals("SW_B_01_00_idle") || breathName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[breathName + "_q"].time = 0f;
				model.PlayBall.Play(breathName + "_q");
			}
			if (breathName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[breathName + "_hua"].time = 0f;
				model.Flower.Play(breathName + "_hua");
			}
			if (breathName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[breathName + "_heart"].time = 0f;
				model.Heart.Play(breathName + "_heart");
			}
			int max = Random.Range(2, 5);
			while (count < max)
			{
				count++;
				Target[breathName].time = 0f;
				if (breathName.Equals("SW_B_01_00_alert") || breathName.Equals("SW_B_01_00_idle") || breathName.Equals("SW_B_01_00_idle_2"))
				{
					model.PlayBall[breathName + "_q"].time = 0f;
				}
				if (breathName.Equals("SW_B_03_00_idle2"))
				{
					model.Flower[breathName + "_hua"].time = 0f;
				}
				if (breathName.Equals("SW_B_03_00_idle"))
				{
					model.Heart[breathName + "_heart"].time = 0f;
				}
				nextTime = Breath.length;
				while (nextTime > 0f)
				{
					nextTime -= Time.deltaTime;
					yield return null;
				}
			}
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
		}
		routine = null;
	}

	private void Init()
	{
		if (!_hasInited)
		{
			eyeAnimation = GetComponent<AvatarEyeAnimation>();
			Target = FindAnimationInParent(base.gameObject);
			celebrateName = ((!(Celebrate == null)) ? Celebrate.name : string.Empty);
			breathName = ((!(Breath == null)) ? Breath.name : string.Empty);
			startClipName = ((!(startClip == null)) ? startClip.name : string.Empty);
			HighscoreName = new List<string>(HighscorePopup.Count);
			int i = 0;
			for (int count = HighscorePopup.Count; i < count; i++)
			{
				HighscoreName.Add((!(HighscorePopup[i] == null)) ? HighscorePopup[i].name : string.Empty);
			}
			IdlesName = new List<string>(Idles.Count);
			int j = 0;
			for (int count2 = Idles.Count; j < count2; j++)
			{
				IdlesName.Add((!(Idles[j] == null)) ? Idles[j].name : string.Empty);
			}
			UnlockPopupName = new List<string>(UnlockPopup.Count);
			int k = 0;
			for (int count3 = UnlockPopup.Count; k < count3; k++)
			{
				UnlockPopupName.Add((!(UnlockPopup[k] == null)) ? UnlockPopup[k].name : string.Empty);
			}
			_hasInited = true;
			if (Target == null)
			{
				Debug.LogError("AvatarAnimations: No animation component for avatar animations", this);
			}
		}
	}

	public void StartIdleAnimations()
	{
		Init();
		PlayIdleAnimations = true;
		Paused = false;
		useHighscoreIdles = false;
		useUnlockIdles = false;
		Target.AddClip(Breath, breathName);
		if (startClip != null)
		{
			useStartClip = true;
			Target.AddClip(startClip, startClipName);
		}
		else
		{
			useStartClip = false;
		}
		int i = 0;
		for (int count = Idles.Count; i < count; i++)
		{
			Target.AddClip(Idles[i], IdlesName[i]);
		}
		if ((bool)eyeAnimation && !eyeAnimation.IsAnimating())
		{
			eyeAnimation.StartAnimatingEyes();
		}
		routine = Play();
		routine.MoveNext();
	}

	public void StartHighscoreAnimations()
	{
		Init();
		PlayIdleAnimations = true;
		Paused = false;
		useHighscoreIdles = true;
		useUnlockIdles = false;
		if (startClip != null)
		{
			useStartClip = true;
			Target.AddClip(Celebrate, celebrateName);
		}
		else
		{
			useStartClip = false;
		}
		int i = 0;
		for (int count = HighscorePopup.Count; i < count; i++)
		{
			Target.AddClip(HighscorePopup[i], HighscoreName[i]);
		}
		if ((bool)eyeAnimation && !eyeAnimation.IsAnimating())
		{
			eyeAnimation.StartAnimatingEyes();
		}
		routine = Play();
		routine.MoveNext();
	}

	public void StartUnlockAnimations()
	{
		Init();
		PlayIdleAnimations = true;
		Paused = false;
		useHighscoreIdles = false;
		useUnlockIdles = true;
		int i = 0;
		for (int count = UnlockPopup.Count; i < count; i++)
		{
			Target.AddClip(UnlockPopup[i], UnlockPopupName[i]);
		}
		if ((bool)eyeAnimation && !eyeAnimation.IsAnimating())
		{
			eyeAnimation.StartAnimatingEyes();
		}
		routine = Play();
		routine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		PlayIdleAnimations = false;
		if (Target[breathName] != null)
		{
			Target.RemoveClip(Breath);
		}
		if (useHighscoreIdles)
		{
			int i = 0;
			for (int count = HighscorePopup.Count; i < count; i++)
			{
				IEnumerator enumerator = Target.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					if (animationState.clip == HighscorePopup[i])
					{
						Target.RemoveClip(HighscorePopup[i]);
					}
				}
			}
		}
		else if (useUnlockIdles)
		{
			if (Celebrate != null && Target[celebrateName] != null)
			{
				Target.RemoveClip(Celebrate);
			}
			int j = 0;
			for (int count2 = UnlockPopup.Count; j < count2; j++)
			{
				IEnumerator enumerator2 = Target.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					AnimationState animationState2 = (AnimationState)enumerator2.Current;
					if (animationState2.clip == UnlockPopup[j])
					{
						Target.RemoveClip(UnlockPopup[j]);
					}
				}
			}
		}
		else
		{
			if (startClip != null && Target[startClipName] != null)
			{
				Target.RemoveClip(startClip);
			}
			int k = 0;
			for (int count3 = Idles.Count; k < count3; k++)
			{
				IEnumerator enumerator3 = Target.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					AnimationState animationState3 = (AnimationState)enumerator3.Current;
					if (animationState3.clip == Idles[k])
					{
						Target.RemoveClip(Idles[k]);
					}
				}
			}
		}
		if ((bool)eyeAnimation && eyeAnimation.IsAnimating())
		{
			eyeAnimation.StopAnimatingEyes();
		}
		routine = null;
	}

	private void Update()
	{
		if (PlayIdleAnimations && routine != null && !Paused)
		{
			routine.MoveNext();
			if (useUnlockIdles && Target != null)
			{
				Target.transform.Rotate(0f, 50f * Time.deltaTime, 0f);
			}
		}
	}

	private bool IsNotNull(AnimationClip a)
	{
		return a != null;
	}
}
