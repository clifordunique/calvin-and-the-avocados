using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dependencies
[RequireComponent (typeof(BoxCollider2D))]

/// <summary>
/// Raycast controller.
/// </summary>
public class RaycastController : MonoBehaviour
{

	// const
	private const float dstBetweenRays = .25f;
	public const float skinWidth = .015f;

	public LayerMask collisionMask;

	// v/h raycast and spacing
	[HideInInspector]
	public int horizontalRayCount;
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public new BoxCollider2D collider;
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
	/// <returns>The raycast origins.</returns>
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
	/// <returns>The ray spacing.</returns>
	public void CalculateRaySpacing ()
	{
		// get collider bounds
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		float boundWidth = bounds.size.x;
		float boundHeight = bounds.size.y;

		// does value is between horizontalRaycount and 2 ?
		horizontalRayCount = Mathf.RoundToInt (boundHeight / dstBetweenRays); 
		verticalRayCount = Mathf.RoundToInt (boundWidth / dstBetweenRays); 

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	/// <summary>
	/// Raycast origins.
	/// </summary>
	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
		public Vector2 center;
	}

}
