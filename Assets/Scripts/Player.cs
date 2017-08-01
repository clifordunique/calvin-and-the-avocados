using UnityEngine;
using System.Collections;

// dependencies
[RequireComponent (typeof(Controller2D))]

/// <summary>
/// Player
/// </summary>
public class Player : MonoBehaviour
{

	public float maxJumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	const float SPEED = 6;
	float moveSpeed;
	float runSpeed;

	// e.g. x: 7.5 y: 16
	public Vector2 wallJumpClimb;

	// e.g. x: 8.5 y: 7
	public Vector2 wallJumpOff;

	// e.g. x: 18 y: 17
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

	Animator anim;
	bool isFacingLeft;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();

		// since gravity must be negative
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);

		moveSpeed = SPEED;
		runSpeed = SPEED * 2;
	
		isFacingLeft = false;

		// jump
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpVelocity);

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{

		CalculateVelocity ();

		HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) {
			anim.SetBool ("jumping", false);
			velocity.y = 0;
		}

		if (velocity.x > 0 && isFacingLeft) {
			Flip ();
		} else if (velocity.x < 0 && !isFacingLeft) {
			Flip ();
		}
	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	/// <returns>The update.</returns>
	void FixedUpdate ()
	{
		anim.SetFloat ("speed", Mathf.Abs (velocity.x));
		anim.SetFloat ("vspeed", velocity.y);
	}


	/// <summary>
	/// Sets the directional input.
	/// </summary>
	/// <returns>The directional input.</returns>
	/// <param name="Input">Input.</param>
	public void SetDirectionalInput (Vector2 Input)
	{
		directionalInput = Input;
	}


	public void onRunInputDown ()
	{
		moveSpeed = runSpeed;
		anim.SetBool ("running", true);
	}

	public void onRunInputUp ()
	{
		moveSpeed = SPEED;
		anim.SetBool ("running", false);
	}

	/// <summary>
	/// Raises the jump input down event.
	/// </summary>
	public void OnJumpInputDown ()
	{

		anim.SetBool ("jumping", true);
		if (wallSliding) {
			// trying to move on the same direction of the input
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
				// ain't touching the direction controller
				else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
				// input is opposite direction
				else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}

		if (controller.collisions.below) {
			velocity.y = maxJumpVelocity;
		}
		
	}

	/// <summary>
	/// Raises the jump input up event.
	/// </summary>
	public void OnJumpInputUp ()
	{
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

	/// <summary>
	/// Flip this instance.
	/// </summary>
	void Flip ()
	{
		isFacingLeft = !isFacingLeft;

		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

	}

	/// <summary>
	/// Handles the wall sliding.
	/// </summary>
	/// <returns>The wall sliding.</returns>
	void HandleWallSliding ()
	{
		// 1 wall left -1 wall right
		wallDirX = (controller.collisions.left) ? -1 : 1;

		// wall jump
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;


			// don't pass max sliding speed
			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;

			}
		}
			
		anim.SetBool ("jumping", !wallSliding);
		anim.SetBool ("sliding", wallSliding);
		
	}

	/// <summary>
	/// Calculates the velocity.
	/// </summary>
	/// <returns>The velocity.</returns>
	void CalculateVelocity ()
	{
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}
}
