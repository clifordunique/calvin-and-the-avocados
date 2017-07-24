using UnityEngine;
using System.Collections;

// dependencies
[RequireComponent (typeof(BoxCollider2D))]

/// <summary>
/// Will move and collide the player
/// </summary>
public class Controller2D : MonoBehaviour
{
	public LayerMask collisionMask; 
	const float SKIN_WIDTH = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	float maxClimbAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start ()
	{
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

	/// <summary>
	/// Move the player by moving his velocity
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	public void Move (Vector3 velocity)
	{
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}

	/// <summary>
	/// Detect horizontals collisions.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	void HorizontalCollisions (ref Vector3 velocity)
	{
		// get direction of Y velocity ( up positive, down negative)
		float directionX = Mathf.Sign (velocity.x);

		// force positive with ABS to stock value
		float rayLength = Mathf.Abs (velocity.x) + SKIN_WIDTH;

		for (int i = 0; i < horizontalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionX * rayLength, Color.red);

			if (hit) {

				// ascending slope
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					ClimbSlope (ref velocity, slopeAngle);
					print (slopeAngle);
				}


				// collisions
				velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
				rayLength = hit.distance;

				collisions.left = (directionX == -1);
				collisions.right = (directionX == 1);
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
		float rayLength = Mathf.Abs (velocity.y) + SKIN_WIDTH;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionY * rayLength, Color.red);

			if (hit) {
				velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
				rayLength = hit.distance;

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}
	}

	/// <summary>
	/// Climbs the slope. 
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	/// <param name="slopeAngle">Slope angle.</param>
	void ClimbSlope (ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		velocity.y = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);


	}

	/// <summary>
	/// Updates the raycast origins.
	/// </summary>
	void UpdateRaycastOrigins ()
	{
		// get collider bounds
		Bounds bounds = collider.bounds;

		// take skin width in account
		bounds.Expand (SKIN_WIDTH * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	/// <summary>
	/// Calculates the ray spacing.
	/// </summary>
	void CalculateRaySpacing ()
	{
		// get collider bounds
		Bounds bounds = collider.bounds;
		bounds.Expand (SKIN_WIDTH * -2);

		// does value is between horizontalRaycount and 2 ?
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	/// <summary>
	/// Keep raycast origins
	/// </summary>
	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	/// <summary>
	/// Collision info.
	/// Where exactly collision happen ?
	/// </summary>
	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset() {
			above = below = false;
			left = right = false;
		}
	}

}