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

	// UI
	public Button resume;
	public Button restart;
	public Button quit;

	// variable
	private Player player;

	// On awake
	public override void Awake ()
	{
		base.Awake ();
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

		// resume button
		resume = resume.GetComponent<Button> ();
		resume.onClick.AddListener (PauseManager);
		buttons.Add (resume);

		// restart button
		restart = restart.GetComponent<Button> ();
		restart.onClick.AddListener (RestartManager);
		buttons.Add (restart);

		// quit button
		quit = quit.GetComponent<Button> ();
		quit.onClick.AddListener (QuitManager);
		buttons.Add (quit);

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{

		if (Input.GetButtonDown ("Pause")) {
			PauseManager ();
		}

		if (!player.inputEnable && !player.hasVictory) {
			InputMap ();
		}
	}

	/// <summary>
	/// Pause/unpause game
	/// </summary>
	private void PauseManager ()
	{
		isPaused = !isPaused;
		Time.timeScale = (isPaused) ? 0f : 1f;
		pauseMenu.SetActive (isPaused);
		player.inputEnable = !isPaused;
	}

	/// <summary>
	/// Restart level
	/// </summary>
	static void RestartManager ()
	{
		string scene = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (scene);
	}

	/// <summary>
	/// Load main menu
	/// </summary>
	static void QuitManager ()
	{
		SceneManager.LoadScene ("menu");
	}
}
