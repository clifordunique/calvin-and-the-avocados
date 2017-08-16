using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trap controller.
/// </summary>
public class TrapController : MonoBehaviour
{

	public bool isRotating;


	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{

		if (isRotating) {
			RotateTrap ();
		}

	}

	/// <summary>
	/// Rotates the trap.
	/// </summary>
	/// <returns>The trap.</returns>
	private void RotateTrap ()
	{
		transform.Rotate (Vector3.forward * -20);
	}
}
