using UnityEngine;

/// <summary>
/// Control 2D collisions
/// </summary>
public class Controller2D : RaycastController
{

	// top, bottom, left, right
	public CollisionInfo collisions;

	// need to had playerInput
	[HideInInspector]
	public Vector2 playerInput;


	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start ()
	{
		base.Start ();
		collisions.faceDir = 1;
	}

	/// <summary>
	/// Move the player when he does stand on a platform this time
	/// </summary>
	/// <param name="moveAmount">Move amount.</param>
	/// <param name="standingOnPlatform">Standing on platform.</param>
	public void Move (Vector2 moveAmount, bool standingOnPlatform = false)
	{
		Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	/// <summary>
	/// Move the player or whatever
	/// </summary>
	/// <param name="moveAmount">Move amount.</param>
	/// <param name="input">Input.</param>
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
	/// Detect horizontal collision
	/// </summary>
	/// <returns>The collisions.</returns>
	/// <param name="moveAmount">Move amount.</param>
	private void HorizontalCollisions (ref Vector2 moveAmount)
	{

		// where are we going ? (left or right)
		float directionX = collisions.faceDir;
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

			if (hit && hit.distance != 0) {

				if (hit.collider.tag == "Mortal") {
					collisions.mortal = true;
					continue;
				}

				// collectable
				if (hit.collider.tag == "Collectable") {
					collisions.collectable = true;
					continue;
				}

				// collisions
				moveAmount.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	/// <summary>
	/// Detect vertical collisions or ignore them if a layer
	/// has the "through" tag.
	/// </summary>
	/// <returns>The collisions.</returns>
	/// <param name="moveAmount">Move amount.</param>
	private void VerticalCollisions (ref Vector2 moveAmount)
	{
		// where are we going ? (up or down)
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionY, Color.red);

			if (hit) {

				// goes through platform with the tag "through"
				if (hit.collider.tag == "Through") {
					if (directionY == 1 || hit.distance == 0 || collisions.fallingThroughPlatform) {
						continue;
					}

					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke ("ResetFallingThroughPlatform", .5f);
						continue;
					}
				}

				// collide with mortal
				if (hit.collider.tag == "Mortal") {
					collisions.mortal = true;
					continue;
				}

				// collectable
				if (hit.collider.tag == "Collectable") {
					collisions.collectable = true;
					continue;
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}
	}

	/// <summary>
	/// Resets collisions.fallingThroughPlatform to false
	/// </summary>
	/// <returns>The falling through platform.</returns>
	private void ResetFallingThroughPlatform ()
	{
		collisions.fallingThroughPlatform = false;
	}

	/// <summary>
	/// Collision info. Where the element collide ?
	/// </summary>
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;
		public bool mortal;
		public bool collectable;

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset ()
		{
			above = below = false;
			left = right = false;
			mortal = false;
			fallingThroughPlatform = false;
			collectable = false;
		}
	}

}