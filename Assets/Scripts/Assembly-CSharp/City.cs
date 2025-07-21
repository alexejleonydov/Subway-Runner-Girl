using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "City")]
public class City : ScriptableObject
{
	public LanguageKey cityName;

	public Keepsake[] keepsakes;

	public string sceneName;

	public Vector4 distort;

	public SubScene[] subScenes;

	public int minLength;

	public int minIntervalLength;

	public int maxIntervalLength;

	public bool allowSnow;

	public int order;

	public int lockedTaskSet;

	public string iconName;

	public string texturePath;
}
