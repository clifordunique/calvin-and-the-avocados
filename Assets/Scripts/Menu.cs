using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Menu.
/// </summary>
public class Menu : MenuController
{

	public Button start;
	public Button quit;

	// Use this for initialization
	public override void Start ()
	{

		base.Start ();

		// restart button
		start = start.GetComponent<Button> ();
		start.onClick.AddListener (StartManager);
		start.Select ();
		buttons.Add (start);

		// quit button
		quit = quit.GetComponent<Button> ();
		quit.onClick.AddListener (QuitManager);
		buttons.Add (quit);
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputMap ();
	}

	/// <summary>
	/// Starts the manager.
	/// </summary>
	/// <returns>The manager.</returns>
	static void StartManager ()
	{

		SceneManager.LoadSceneAsync ("playerInfo");
		
	}

	/// <summary>
	/// Quit application
	/// </summary>
	static void QuitManager ()
	{
		Application.Quit ();
	}
}
