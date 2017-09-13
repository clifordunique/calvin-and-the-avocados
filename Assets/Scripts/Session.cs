using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour
{
	public static Session Instance;

	public string playerName;

	// @TODO: for speedrun mode
	public string globalTime;

	/// <summary>
	/// Awake this instance. Create singleton.
	/// </summary>
	public void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		}

		if (Instance != this) {
			Destroy (gameObject);
		}
	}

}
