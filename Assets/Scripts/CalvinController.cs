using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalvinController : MonoBehaviour {

	// body 
	public float MAX_SPEED = 1.5f;
	public Rigidbody2D rigidbody;


	// ground
	public Transform groundCheck;
	public LayerMask whatIsGround;
	float GROUND_RADIUS = 0.2f;
	bool isGrounded = false; // since our character does not begin on the ground

	// animation
	bool isGoingRight = true;
	Animator anim;

	// jump
	public float JUMP_FORCE = 350f;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

		// TODO: create own key for jump
		if (isGrounded && Input.GetKeyDown (KeyCode.Space)) {
			anim.SetBool ("ground", false);
			rigidbody.AddForce (new Vector2 (0, JUMP_FORCE));

		}

	}

	// Update is called once per frame
	void FixedUpdate () {

		// ground check
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, GROUND_RADIUS, whatIsGround);
		anim.SetBool ("ground", isGrounded);
		anim.SetFloat ("vspeed", rigidbody.velocity.y);


		// movement
		// get axis
		float move = Input.GetAxis("Horizontal");

		// TODO: make transition for walk and run
		// set animation
		anim.SetFloat("speed", Mathf.Abs(move));

		// move body
		rigidbody.velocity = new Vector2 (move * MAX_SPEED, GetComponent<Rigidbody2D>().velocity.y);

		// flip
		if (move > 0 && !isGoingRight)
			Flip ();
		else if (move < 0 && isGoingRight)
			Flip ();
	}

	// change sprite direction
	void Flip() {
		isGoingRight = !isGoingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
