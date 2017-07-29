using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;

	[Range (0, 2)]
	public float easeAmount;

	int fromWaypointIndex;
	float percentBetweenWaypoints;
	// pourcentage between 0 and 1
	float nextMoveTime;

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D> ();

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();

		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints [i] = localWaypoints [i] + transform.position;
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateRaycastOrigins ();
		Vector3 velocity = CalculatePlatformMovement ();

		CalculatePassengerMovement (velocity);

		MovePassengers (true);
		transform.Translate (velocity);
		MovePassengers (false);
	}

	/// <summary>
	/// Easing
	/// </summary>
	/// <param name="x"></param>
	/// <returns>Easing value</returns>
	float Ease (float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow (x, a) / Mathf.Pow (1 - x, a);
	}

	/// <summary>
	/// Calculate platform movement
	/// </summary>
	/// <returns>New Position</returns>
	Vector3 CalculatePlatformMovement ()
	{

		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		// reach next waypoint
		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;

			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length - 1) {
					// start again at the beginning by reversing waypoint
					fromWaypointIndex = 0;
					System.Array.Reverse (globalWaypoints);
				}
			}

			nextMoveTime = Time.time + waitTime;

		}

		return newPos - transform.position;

	}

	/// <summary>
	/// Moves the passengers.
	/// </summary>
	/// <param name="beforeMovePlatform">Before move platform.</param>
	/// <returns></returns>
	void MovePassengers (bool beforeMovePlatform)
	{
		foreach (PassengerMovement passenger in passengerMovement) {
			if (!passengerDictionary.ContainsKey (passenger.transform)) {
				passengerDictionary.Add (passenger.transform, passenger.transform.GetComponent<Controller2D> ());
			}

			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passengerDictionary [passenger.transform].Move (passenger.velocity, passenger.standingOnPlatform);
			}
		}
		
	}

	/// <summary>
	/// Moves the passengers (any controller2d).
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	/// <returns></returns>
	void CalculatePassengerMovement (Vector3 velocity)
	{

		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();

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
				if (hit && hit.distance != 0) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add (new PassengerMovement (hit.transform, new Vector3 (pushX, pushY), directionY == 1, true));

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
				if (hit && hit.distance != 0) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add (new PassengerMovement (hit.transform, new Vector3 (pushX, pushY), false, true));
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
				if (hit && hit.distance != 0) {

					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);

						// close the gap between platform and passenger
						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add (new PassengerMovement (hit.transform, new Vector3 (pushX, pushY), true, false));

					}
				}
			}
		}
		
	}

	/// <summary>
	/// Passenger movement.
	/// </summary>
	struct PassengerMovement
	{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement (
			Transform _transform, 
			Vector3 _velocity, 
			bool _standingOnPlatform, 
			bool _moveBeforePlatform
		)
		{
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
			
		}

	}

	/// <summary>
	/// Ons the draw gizmos.
	/// </summary>
	void OnDrawGizmos ()
	{
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints [i] : localWaypoints [i] + transform.position;
				Gizmos.DrawLine (globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine (globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		} 
	}
}

