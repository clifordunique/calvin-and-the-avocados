using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

	public Controller2D target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 bottomLeftLimit;
	public Vector2 topRightLimit;
	public Vector2 focusAreaSize;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
	}

	/// <summary>
	/// Lates the update.
	/// </summary>
	/// <returns>The update.</returns>
	void LateUpdate ()
	{
		focusArea.Update (target.collider.bounds);

		if (KeepFollowing ()) {
			Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

			if (focusArea.velocity.x != 0) {
				lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
				if (Mathf.Sign (target.playerInput.x) == Mathf.Sign (focusArea.velocity.x) && target.playerInput.x != 0) {
					targetLookAheadX = lookAheadDirX * lookAheadDstX;
				} else {

					if (!lookAheadStopped) {
						lookAheadStopped = true;
						targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
					}
				}
			}

			currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

			focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
			focusPosition += Vector2.right * currentLookAheadX;

			transform.position = (Vector3)focusPosition + Vector3.forward * -10;
		}
	}

	/// <summary>
	/// Keeps the following.
	/// </summary>
	/// <returns>The following.</returns>
	bool KeepFollowing ()
	{

		return focusArea.bottom >= bottomLeftLimit.y &&
		focusArea.top <= topRightLimit.y &&
		focusArea.left >= bottomLeftLimit.x &&
		focusArea.right <= topRightLimit.x;

	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	void OnDrawGizmos ()
	{
		Gizmos.color = new Color (1, 0, 0, .5f);
		Gizmos.DrawCube (focusArea.center, focusAreaSize);

		// camera offscreen limit
		Vector2 bottomLeft = (Vector3)bottomLeftLimit;
		Vector2 topLeft = new Vector3 (bottomLeftLimit.x, topRightLimit.y, 1);
		Vector2 bottomRight = new Vector3 (topRightLimit.x, bottomLeftLimit.y, 1);
		Vector2 topRight = (Vector3)topRightLimit;

		Gizmos.DrawLine (bottomLeft, topLeft);
		Gizmos.DrawLine (topLeft, topRight);
		Gizmos.DrawLine (topRight, bottomRight);
		Gizmos.DrawLine (bottomRight, bottomLeft);
	}

	/// <summary>
	/// Focus area.
	/// </summary>
	struct FocusArea
	{
		public Vector2 center;
		public Vector2 velocity;
		public float left;
		public float right;
		public float top;
		public float bottom;

		/// <summary>
		/// Initializes a new instance of the <see cref="CamaraFollow+FocusArea"/> struct.
		/// </summary>
		/// <param name="targetBounds">Target bounds.</param>
		/// <param name="size">Size.</param>
		public FocusArea (Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
		}

		/// <summary>
		/// Update the specified targetBounds.
		/// </summary>
		/// <param name="targetBounds">Target bounds.</param>
		public void Update (Bounds targetBounds)
		{

			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2 (shiftX, shiftY);
		}

	}

}
