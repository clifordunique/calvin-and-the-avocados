using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfoMenu  : MenuController
{

	public Text warning;
	public InputField playerName;

	/// <summary>
	/// Start this instance.
	/// Get audiosource
	/// Let the time flow
	/// store first selected object
	/// Set player name if already set
	/// </summary>
	public override void Start ()
	{
		base.Start ();

		if (Session.playerName != "") {
			playerName.text = Session.playerName;
		}
	}

	/// <summary>
	/// Load worldmap and set player name in session data
	/// </summary>
	/// <returns>The manager.</returns>
	public void ContinueManager ()
	{
		string text = playerName.text;
		if (text.Length > 0) {
			Session.playerName = playerName.text;
			SceneManager.LoadSceneAsync ("worldmap");
		} else {
			warning.text = "name should not be empty";
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
