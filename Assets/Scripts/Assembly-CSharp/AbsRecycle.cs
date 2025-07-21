using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsRecycle<TRecycleItem> : MonoBehaviour where TRecycleItem : AbsRecycleItem
{
	public GameObject origGo;

	public List<TRecycleItem> recycleItems;

	public int max;

	protected AbsRecycle()
	{
		max = 20;
	}

	public virtual void Awake()
	{
		if (origGo == null)
		{
			origGo = base.gameObject;
		}
		recycleItems = new List<TRecycleItem>();
	}

	public void Instantiate()
	{
		try
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(origGo);
			gameObject.transform.parent = base.transform;
			AbsRecycleItem absRecycleItem = NewRecycleItem(gameObject);
			recycleItems.Add((TRecycleItem)absRecycleItem);
			gameObject.SetActive(false);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public abstract TRecycleItem NewRecycleItem(GameObject newGo);

	public virtual TRecycleItem Retain()
	{
		if (recycleItems.Count < max && origGo != null)
		{
			GameObject newGo = UnityEngine.Object.Instantiate(origGo);
			AbsRecycleItem absRecycleItem = NewRecycleItem(newGo);
			absRecycleItem.Retain();
			recycleItems.Add((TRecycleItem)absRecycleItem);
			return absRecycleItem as TRecycleItem;
		}
		for (int i = 0; i < recycleItems.Count; i++)
		{
			TRecycleItem result = recycleItems[i];
			if (!result.IsUsing())
			{
				result.Retain();
				return result;
			}
		}
		return (TRecycleItem)null;
	}

	public virtual void WarmUp()
	{
		StartCoroutine(WarmUpC());
	}

	public virtual IEnumerator WarmUpC()
	{
		while (recycleItems.Count < max)
		{
			if (origGo != null)
			{
				Instantiate();
				Instantiate();
				Instantiate();
				Instantiate();
				yield return null;
			}
		}
	}
}
