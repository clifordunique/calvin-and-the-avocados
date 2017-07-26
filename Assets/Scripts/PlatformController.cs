using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

	public LayerMask passengerMask;
	public Vector3 move;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateRaycastOrigins ();
		Vector3 velocity = move * Time.deltaTime;

		MovePassengers (velocity);
		transform.Translate (velocity);
	}

	/// <summary>
	/// Moves the passengers (any controller2d).
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	void MovePassengers (Vector3 velocity)
	{

		HashSet<Transform> movedPassengers = new HashSet<Transform> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// vertically moving platform
		if (velocity.y != 0) {

			// force positive with ABS to stock value
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				// witch direction are we moving ?
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				// we found the passenger and see how far we gonna move him
				if (hit) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						hit.transform.Translate (new Vector3 (pushX, pushY));
						
					}
				}
			}
		}

		// Horizontally moving platform
		if (velocity.x != 0) {

			// force positive with ABS to stock value
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				// witch direction are we moving ?
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				// we found the passenger and see how far we gonna move him
				if (hit) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = 0;

						hit.transform.Translate (new Vector3 (pushX, pushY));
						
					}
				}
			}
		}

		// Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {

			float rayLength = skinWidth * 2;

			for (int i = 0; i < verticalRayCount; i++) {
				// witch direction are we moving ?
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, rayLength, passengerMask);

				// we found the passenger and see how far we gonna move him
				if (hit) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = velocity.x;
						float pushY = velocity.y;

						hit.transform.Translate (new Vector3 (pushX, pushY));
						
					}
				}
			}
		}
		
	}
}
