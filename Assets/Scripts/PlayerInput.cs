using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour
{
	Player player;

	// Use this for initialization
	void Start ()
	{
		player = GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (player.inputEnable)
        {
            InputMap();
        }

	}

    /// <summary>
    /// Set input map
    /// </summary>
    private void InputMap()
    {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);


		if (Input.GetButtonDown ("Jump")) {
			player.OnJumpInputDown ();
		}

		if (Input.GetButtonUp ("Jump")) {
			player.OnJumpInputUp ();
		}

		if (Input.GetButtonDown ("Run") || Input.GetAxis ("Run") != 0) {
			player.OnRunInputDown ();
		}

		if (Input.GetButtonUp ("Run") || Input.GetAxis ("Run") == 0) {
			player.OnRunInputUp ();
		}
    }
}
