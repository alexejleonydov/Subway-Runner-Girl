using UnityEngine;

public class BoundJumpPickup : IPickup
{
	public float jumpHeight = 70f;

	public float jumpDistance = 400f;

	public float totalDistance = 400f;

	public override void NotifyPickup(PickupParticles particles)
	{
		if (canPickup && !Game.Instance.IsInFlypackMode)
		{
			Game.Instance.PickupBound(jumpHeight, jumpDistance, totalDistance, base.transform.position.y);
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 vector = base.transform.position;
		Vector3 vector2 = vector;
		float num = 0f;
		int i = 1;
		for (float num2 = ((!(jumpDistance > totalDistance)) ? totalDistance : jumpDistance); (float)i < num2; i++)
		{
			num = (float)i / num2;
			vector2 = new Vector3(vector2.x, ObliqueMotion.CalcHeight(num) * jumpHeight + base.transform.position.y, jumpDistance * num + base.transform.position.z);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}
}
