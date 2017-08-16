using UnityEngine;
using UnityEngine.SceneManagement;

// dependencies
[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Player behaviour and actions
/// </summary>
public class Player : MonoBehaviour
{

	// sounds
	public AudioClip jumpAudio;
	public AudioClip gameOverAudio;
	private AudioSource sourceAudio;

	// settings
	public float maxJumpHeight = 4;
	public float timeToJumpApex = .4f;
	private readonly float accelerationTimeAirborne = .2f;
	private readonly float accelerationTimeGrounded = .1f;
	private const float SPEED = 6;
	private float moveSpeed;
	private float runSpeed;

	// e.g. x: 7.5 y: 16
	public Vector2 wallJumpClimb;

	// e.g. x: 8.5 y: 7
	public Vector2 wallJumpOff;

	// e.g. x: 18 y: 17
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	private float timeToWallUnstick;

	// velocity and gravity
	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private float velocityXSmoothing;
	public Vector3 velocity;

	private Controller2D controller;

	// input
	private Vector2 directionalInput;
	[HideInInspector]
	public bool inputEnable;

	// wall jumping
	private bool wallSliding;
	private int wallDirX;

	// animation
	private Animator anim;
	private bool isFacingLeft;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
		sourceAudio = GetComponent<AudioSource> ();

		inputEnable = true;

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
	private void Update ()
	{

		CalculateVelocity ();

		HandleWallSliding ();

		if (!controller.collisions.mortal) {
			controller.Move (velocity * Time.deltaTime, directionalInput);
		}

		if (controller.collisions.above || controller.collisions.below) {
			anim.SetBool ("jumping", false);
			velocity.y = 0;
		}

		if ((velocity.x > 0 && isFacingLeft) || (velocity.x < 0 && !isFacingLeft)) {
			Flip ();
		} 
	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	/// <returns>The update.</returns>
	private void FixedUpdate ()
	{
		anim.SetFloat ("speed", Mathf.Abs (velocity.x));
		anim.SetFloat ("vspeed", velocity.y);
		HandleDeathAndRespawn ();
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


	/// <summary>
	/// Make run on input down
	/// </summary>
	public void OnRunInputDown ()
	{
		moveSpeed = runSpeed;
		anim.SetBool ("running", true);
	}

	/// <summary>
	/// Stop running on input up
	/// </summary>
	public void OnRunInputUp ()
	{
		moveSpeed = SPEED;
		anim.SetBool ("running", false);
	}

	/// <summary>
	/// Raises the jump input down event.
	/// </summary>
	public void OnJumpInputDown ()
	{

		if (!anim.GetBool ("jumping") && !anim.GetBool ("death")) {
			sourceAudio.PlayOneShot (jumpAudio);
			anim.SetBool ("jumping", true);
		}

		if (wallSliding) {
			// trying to move on the same direction of the input
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (directionalInput.x == 0) {
				// ain't touching the direction controller
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			} else {
				// input is opposite direction
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
	/// Raises the pause input down event
	/// </summary>
	public void OnPauseInputDown ()
	{
		inputEnable = !inputEnable;
		Debug.Log ("pause menu inputenable : " + inputEnable);
	}

	/// <summary>
	/// Flip this instance.
	/// </summary>
	private void Flip ()
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
	private void HandleWallSliding ()
	{
		// 1 wall left -1 wall right
		wallDirX = (controller.collisions.left) ? -1 : 1;

		// wall jump
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) &&
		    !controller.collisions.below
		    && velocity.y < 0
		    && !controller.collisions.mortal) {

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
	/// Kill the player and make it respawn
	/// </summary>
	private void HandleDeathAndRespawn ()
	{

		if (controller.collisions.mortal && !anim.GetBool ("death")) {
			sourceAudio.Stop ();
			sourceAudio.PlayOneShot (gameOverAudio);
			anim.SetBool ("death", true);
			Invoke ("Respawn", 1.25f);
		}

	}

	/// <summary>
	/// Respawn meen restart level
	/// </summary>
	private void Respawn ()
	{
		string scene = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (scene);
	}

	/// <summary>
	/// Calculates the velocity.
	/// </summary>
	/// <returns>The velocity.</returns>
	private void CalculateVelocity ()
	{
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (
			velocity.x, targetVelocityX, 
			ref velocityXSmoothing, 
			(controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne
		);
		velocity.y += gravity * Time.deltaTime;
	}
}
