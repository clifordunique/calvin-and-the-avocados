using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dependencies
[RequireComponent (typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

	public LayerMask collisionMask;

	public const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	public virtual void Awake ()
	{
		collider = GetComponent<BoxCollider2D> ();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	public virtual void Start ()
	{
		CalculateRaySpacing ();
	}

	/// <summary>
	/// Updates the raycast origins.
	/// </summary>
	public void UpdateRaycastOrigins ()
	{
		// get collider bounds
		Bounds bounds = collider.bounds;

		// take skin width in account
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
		raycastOrigins.center = new Vector2 (bounds.center.x, bounds.center.y);
	}

	/// <summary>
	/// Calculates the ray spacing.
	/// </summary>
	public void CalculateRaySpacing ()
	{
		// get collider bounds
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		// does value is between horizontalRaycount and 2 ?
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	/// <summary>
	/// Keep raycast origins
	/// </summary>
	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
		public Vector2 center;
	}

}
