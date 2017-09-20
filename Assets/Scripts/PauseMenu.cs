using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// In-game pause menu
/// </summary>
public class PauseMenu : MenuController
{

	private bool isPaused = false;
	public GameObject pauseMenu;

	// variable
	private Player player;

	// On awake
	public void Awake ()
	{
		isPaused = false;
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start ()
	{
		base.Start ();
		player = GetComponent<Player> ();
		player.inputEnable = true;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public override void Update ()
	{
		if (isPaused) {
			base.Update ();
		}

		if (Input.GetButtonDown ("Pause")) {
			PauseManager ();
		}
	}

	/// <summary>
	/// Pause/unpause game
	/// </summary>
	public void PauseManager ()
	{
		isPaused = !isPaused;
		Time.timeScale = (isPaused) ? 0f : 1f;
		pauseMenu.SetActive (isPaused);
		player.inputEnable = !isPaused;
	}

	/// <summary>
	/// Restart level
	/// </summary>
	public void RestartManager ()
	{
		string scene = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (scene);
	}

	/// <summary>
	/// Load main menu
	/// </summary>
	public void QuitManager ()
	{
		SceneManager.LoadScene ("worldmap");
	}
}
