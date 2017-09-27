using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMapMenu : MenuController
{

	public GameObject worlds;
	private List<Button> buttons;
	private int current;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start ()
	{
		base.Start ();

		Button[] worldButtons = worlds.GetComponentsInChildren<Button> ();

		buttons = new List<Button> (worldButtons);
		current = GetNextLevel (Session.lastPlayedScene);
		ES.SetSelectedGameObject (buttons [current].gameObject);

	}

	/// <summary>
	/// Gets the next level.
	/// </summary>
	/// <returns>The next level.</returns>
	/// <param name="scene">Scene.</param>
	private int GetNextLevel (string scene)
	{

		int selected = 0;
		for (int i = 0; i < buttons.Count; i++) {
			if (buttons [i].gameObject.name == scene) {
				selected = i;
			}
		}

		return selected;
	}

	/// <summary>
	/// Go back to player info.
	/// </summary>
	/// <returns>The back manager.</returns>
	public void GoBackManager ()
	{
		SceneManager.LoadSceneAsync ("playerInfo");
	}

}
