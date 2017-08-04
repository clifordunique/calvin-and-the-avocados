using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalDanger : RaycastController
{

	public LayerMask playerMask;

	// Use this for initialization
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
				print ("hit");
			}
		}
	}

}
