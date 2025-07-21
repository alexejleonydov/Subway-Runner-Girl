using System;
using System.Collections;
using UnityEngine;

public class LongMagnet : MonoBehaviour
{
	private Character character;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private OnTriggerObject coinMagnetCollider;

	private Game game;

	private VariableBool longMagnetSuction = new VariableBool();

	public float pullSpeed = 200f;

	public void Activate()
	{
		longMagnetSuction.Add(this);
	}

	public void CoinHit(Collider collider)
	{
		if (game.IsInFlypackMode)
		{
			return;
		}
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.GetComponent<Collider>().enabled = false;
			if (component.meshRenderer != null)
			{
				component.meshRenderer.enabled = false;
			}
			StartCoroutine(Pull(component));
		}
		else
		{
			IPickup componentInChildren = collider.GetComponentInChildren<IPickup>();
			if (componentInChildren != null && !(componentInChildren is BoundJumpPickup))
			{
				componentInChildren.NotifyPickup(character.CharacterPickupParticleSystem);
			}
		}
	}

	public void Deactivate()
	{
		longMagnetSuction.Remove(this);
	}

	private IEnumerator Pull(Coin coin)
	{
		Vector3 coinPosition = coin.PivotTransform.position;
		Vector3 vector = coinPosition - characterController.transform.position;
		Vector3 offsetCoinHitPosition = new Vector3(0f, -6f, 0f);
		yield return StartCoroutine(myTween.To(vector.magnitude / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
		{
			coin.PivotTransform.position = Vector3.Lerp(coinPosition, characterModel.meshSuperShoes.transform.position + offsetCoinHitPosition, t * t);
		}));
		IPickup pickup = coin.GetComponent<IPickup>();
		if (pickup != null)
		{
			character.NotifyPickup(pickup);
		}
	}

	public void DelegteInGameOne(bool _value)
	{
		if (_value)
		{
			longMagnetSuction.Clear();
		}
	}

	public void DelegteInGameTwo(bool _value)
	{
		if (_value)
		{
			coinMagnetCollider.OnEnter = CoinHit;
			coinMagnetCollider.GetComponent<Collider>().enabled = true;
		}
		else
		{
			coinMagnetCollider.GetComponent<Collider>().enabled = false;
			coinMagnetCollider.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Remove(coinMagnetCollider.OnEnter, new OnTriggerObject.OnEnterDelegate(CoinHit));
		}
	}

	public void Start()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		coinMagnetCollider = character.coinMagnetLongCollider;
		characterController = character.characterController;
		game = Game.Instance;
		game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(DelegteInGameOne));
		longMagnetSuction.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(longMagnetSuction.OnChange, new VariableBool.OnChangeDelegate(DelegteInGameTwo));
	}
}
