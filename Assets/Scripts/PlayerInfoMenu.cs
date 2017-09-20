using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfoMenu  : MenuController
{

	public Text warning;
	public InputField playerName;

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
}
