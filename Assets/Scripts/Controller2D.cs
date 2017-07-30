using UnityEngine;
using System.Collections;

/// <summary>
/// Will move and collide the player
/// </summary>
public class Controller2D : RaycastController
{

	float maxClimbAngle = 80;
	float maxDescendAngle = 75;

	public CollisionInfo collisions;

	[HideInInspector]
	public Vector2 playerInput;

	public override void Start ()
	{
		base.Start ();
		collisions.faceDir = 1;
	}

	/// <summary>
	/// Move the specified velocity and standingOnPlatform.
	/// Use by platform
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	/// <param name="standingOnPlatform">Standing on platform.</param>
	public void Move (Vector3 velocity, bool standingOnPlatform = false)
	{
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	/// <summary>
	/// Move the player by moving his velocity
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	/// <param name="standingOnPlatform">Standing on platform.</param>
	public void Move (Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;

		playerInput = input;

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope (ref velocity);
		}

		HorizontalCollisions (ref velocity);

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	/// <summary>
	/// Detect horizontals collisions.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	void HorizontalCollisions (ref Vector3 velocity)
	{
		// get direction of Y velocity ( up positive, down negative)
		float directionX = collisions.faceDir;

		// force positive with ABS to stock value
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		if (Mathf.Abs (velocity.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionX * rayLength, Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				// ascending slope
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				// collisions and not climbing slope
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}


			}
		}
	}

	/// <summary>
	/// Detect vertical collisions.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	void VerticalCollisions (ref Vector3 velocity)
	{
		// get direction of Y velocity ( up positive, down negative)
		float directionY = Mathf.Sign (velocity.y);

		// force positive with ABS to stock value
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionY * rayLength, Color.red);

			if (hit) {

				if (hit.collider.tag == "Through") {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}

					if (collisions.fallingThroughPlatform) {
						continue;
					}

					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke ("ResetFallingThroughPlatform", .5f);
						continue;
					}
				}					

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}

		// new slope at Y axe
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	/// <summary>
	/// Climbs the slope. 
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	/// <param name="slopeAngle">Slope angle.</param>
	void ClimbSlope (ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;

		}
	}

	/// <summary>
	/// Descends the slope.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	void DescendSlope (ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign (hit.normal.x) == directionX) {

					// how far we are from the slope
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x)) {
						float moveDistance = Mathf.Abs (velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Resets the falling though platform.
	/// </summary>
	/// <returns>The falling though platform.</returns>
	void ResetFallingThroughPlatform ()
	{
		collisions.fallingThroughPlatform = false;
	}

	/// <summary>
	/// Collision info.
	/// Where exactly collision happen ?
	/// </summary>
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset ()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}

	}

}