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
	/// Move the specified moveAmount and standingOnPlatform.
	/// Use by platform
	/// </summary>
	/// <param name="moveAmount">moveAmount.</param>
	/// <param name="standingOnPlatform">Standing on platform.</param>
	public void Move (Vector2 moveAmount, bool standingOnPlatform = false)
	{
		Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	/// <summary>
	/// Move the player by moving his moveAmount
	/// </summary>
	/// <param name="moveAmount">moveAmount.</param>
	/// <param name="standingOnPlatform">Standing on platform.</param>
	public void Move (Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;

		playerInput = input;

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (moveAmount.x);
		}

		if (moveAmount.y < 0) {
			DescendSlope (ref moveAmount);
		}

		HorizontalCollisions (ref moveAmount);

		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		transform.Translate (moveAmount);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	/// <summary>
	/// Detect horizontals collisions.
	/// </summary>
	/// <param name="moveAmount">moveAmount.</param>
	void HorizontalCollisions (ref Vector2 moveAmount)
	{
		// get direction of Y moveAmount ( up positive, down negative)
		float directionX = collisions.faceDir;

		// force positive with ABS to stock value
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs (moveAmount.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionX, Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				// ascending slope
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				// collisions and not climbing slope
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x);
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
	/// <param name="moveAmount">moveAmount.</param>
	void VerticalCollisions (ref Vector2 moveAmount)
	{
		// get direction of Y moveAmount ( up positive, down negative)
		float directionY = Mathf.Sign (moveAmount.y);

		// force positive with ABS to stock value
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionY, Color.red);

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

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
				}

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}

		// new slope at Y axe
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (moveAmount.x);
			rayLength = Mathf.Abs (moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	/// <summary>
	/// Climbs the slope. 
	/// </summary>
	/// <param name="moveAmount">moveAmount.</param>
	/// <param name="slopeAngle">Slope angle.</param>
	void ClimbSlope (ref Vector2 moveAmount, float slopeAngle)
	{
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;

		}
	}

	/// <summary>
	/// Descends the slope.
	/// </summary>
	/// <param name="moveAmount">moveAmount.</param>
	void DescendSlope (ref Vector2 moveAmount)
	{
		float directionX = Mathf.Sign (moveAmount.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign (hit.normal.x) == directionX) {

					// how far we are from the slope
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) {
						float moveDistance = Mathf.Abs (moveAmount.x);
						float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
						moveAmount.y -= descendmoveAmountY;

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
		public Vector2 moveAmountOld;
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