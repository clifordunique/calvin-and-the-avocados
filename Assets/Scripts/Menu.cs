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

	/// <summary>
	/// Starts the manager.
	/// </summary>
	/// <returns>The manager.</returns>
	public void StartManager ()
	{
		SceneManager.LoadSceneAsync ("playerInfo");
	}

	/// <summary>
	/// Adventures the manager.
	/// </summary>
	/// <returns>The manager.</returns>
	public void AdventureManager ()
	{
		Session.isSpeedrunMode = true;
		SceneManager.LoadSceneAsync ("playerInfo");
	}

	/// <summary>
	/// Quit application
	/// </summary>
	public void QuitManager ()
	{
		Application.Quit ();
	}
}
