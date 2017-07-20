using UnityEngine;
using System.Collections;

// dependencies
[RequireComponent (typeof(Controller2D))]

/// <summary>
/// Player
/// </summary>
public class Player : MonoBehaviour
{

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;


	Controller2D controller;

	void Start ()
	{
		controller = GetComponent<Controller2D> ();

		// since gravity must be negative
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);

		// must be positive
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;

		// info
		print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}

	void Update ()
	{

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}

		// gravity * time in deconds it tool to complete the last frame
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (
			velocity.x, 
			targetVelocityX, 
			ref velocityXSmoothing, 
			(controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne)
		);
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
}