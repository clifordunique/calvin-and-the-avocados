using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfoMenu  : MenuController
{

	public Button continueBtn;
	public Text warning;
	public InputField playerName;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start ()
	{

		base.Start ();

		// player name text field
		playerName = playerName.GetComponent<InputField> ();

		// continue button
		continueBtn = continueBtn.GetComponent<Button> ();
		continueBtn.onClick.AddListener (ContinueManager);
		continueBtn.Select ();
		buttons.Add (continueBtn);

	}

	// Update is called once per frame
	void Update ()
	{
		InputMap ();
	}

	/// <summary>
	/// Load worldmap and set player name in session data
	/// </summary>
	/// <returns>The manager.</returns>
	private void ContinueManager ()
	{
		if (playerName.text.Length > 0) {
			Session.playerName = playerName.text;
			SceneManager.LoadSceneAsync ("worldmap");
		} else {
			warning.text = "name should not be empty";
			
		}
	}
}
