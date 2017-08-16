using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dependencies
[RequireComponent (typeof(Player))]

/// <summary>
/// Player's input for controller and keyboard
/// </summary>
public class PlayerInput : MonoBehaviour
{
	Player player;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		player = GetComponent<Player> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{
		if (player.inputEnable) {
			InputMap ();
		}
	}

	/// <summary>
	/// Map controller/keyboard key to player action
	/// </summary>
	/// <returns>The map.</returns>
	private void InputMap ()
	{
		SetPlayerDirectionalInput (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (Input.GetButtonDown ("Jump")) {
			player.OnJumpInputDown ();
		}

		if (Input.GetButtonUp ("Jump")) {
			player.OnJumpInputUp ();
		}

		if (GetRunInputDownConditions ()) {
			player.OnRunInputDown ();
		}

		if (GetRunInputUpConditions ()) {
			player.OnRunInputUp ();
		}
	}

	/// <summary>
	/// Sets the player directional input.
	/// </summary>
	/// <returns>The player directional input.</returns>
	/// <param name="horizontal">Horizontal.</param>
	/// <param name="vertical">Vertical.</param>
	private void SetPlayerDirectionalInput (float horizontal, float vertical)
	{
		player.SetDirectionalInput (new Vector2 (horizontal, vertical));
	}

	/// <summary>
	/// Gets the run input down conditions.
	/// </summary>
	/// <returns>The run input down conditions.</returns>
	private static bool GetRunInputDownConditions ()
	{
		return (Input.GetButtonDown ("Run") ||
		Input.GetAxis ("Run") != 0) &&
		Input.GetAxisRaw ("Horizontal") != 0;
	}

	/// <summary>
	/// Gets the run input up conditions.
	/// </summary>
	/// <returns>The run input up conditions.</returns>
	private static bool GetRunInputUpConditions ()
	{
		return Input.GetButtonUp ("Run") ||
		Input.GetAxis ("Run") == 0 ||
		Input.GetAxisRaw ("Horizontal") == 0;
	}
}
