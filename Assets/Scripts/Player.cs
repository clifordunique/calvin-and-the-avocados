using UnityEngine;
using System.Collections;

// dependencies
[RequireComponent (typeof(Controller2D))]

/// <summary>
/// Player
/// </summary>
public class Player : MonoBehaviour
{

	float gravity = -20;
	float moveSpeed = 6;
	Vector3 velocity;


	Controller2D controller;

	void Start ()
	{
		controller = GetComponent<Controller2D> ();
	}

	void Update ()
	{

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		velocity.x = input.x * moveSpeed;

		// gravity * time in deconds it tool to complete the last frame
		velocity.y += gravity * Time.deltaTime;

		// move the player
		controller.Move (velocity * Time.deltaTime);
	}
}