using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Menu.
/// </summary>
public class AdventureMenu : MenuController
{

	/// <summary>
	/// Quit application
	/// </summary>
	public void QuitManager ()
	{
		SceneManager.LoadSceneAsync ("menu");
	}
}
