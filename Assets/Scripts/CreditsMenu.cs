using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Menu.
/// </summary>
public class CreditsMenu : MenuController
{

	public Text text;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		if (Session.playerName.ToLower () != "margot") {
			text.text = "Congratulations !! Your quest is now over !!";
		}
	}

	/// <summary>
	/// Go back to main menu.
	/// </summary>
	/// <returns>The back manager.</returns>
	public void GoBackManager ()
	{
		SceneManager.LoadSceneAsync ("menu");
	}
}
