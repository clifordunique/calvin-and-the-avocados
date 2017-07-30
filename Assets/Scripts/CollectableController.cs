using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : RaycastController
{

	public LayerMask playerMask;
	public string level;
	bool collected;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		base.Start ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void FixedUpdate ()
	{
		UpdateRaycastOrigins ();
		OnCollisionWithPlayer ();

		transform.Rotate (new Vector3 (0, 0, .45f) * Time.deltaTime);

		if (collected) {
			Application.LoadLevel (level);
		}

	}

	/// <summary>
	/// Raises the collision with player event.
	/// </summary>
	void OnCollisionWithPlayer ()
	{

		float rayLength = 1 + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigins.center, Vector2.up, rayLength, playerMask);

			// we found the passenger and see how far we gonna move him
			if (hit) {
				collected = true;
			}
		}
	}

}
