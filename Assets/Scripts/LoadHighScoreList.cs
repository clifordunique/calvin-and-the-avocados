using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadHighScoreList : MonoBehaviour, ISelectHandler
{

	public Text highScoreList;
	public Text level;
	public Sprite completed;
	public GameSerialization localData = new GameSerialization ();

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		SaveLoadController.Load (gameObject.name);
		localData.set (SaveLoadController.savedGames);
		if (localData.score.Count > 0 && localData.name.Contains (Session.playerName)) {
			Image image = gameObject.GetComponent<Image> ();
			image.sprite = completed;
		}

	}

	/// <summary>
	/// Raises the select event.
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnSelect (BaseEventData data)
	{
		Debug.Log (gameObject.name + " Selected");
		LoadLevelName ();
		LoadHighScore ();
	}

	/// <summary>
	/// Load level name
	/// </summary>
	private void LoadLevelName ()
	{
		level.text = "-" + gameObject.name + "-";
	}

	/// <summary>
	/// Loads the high score.
	/// </summary>
	/// <returns>The high score.</returns>
	private void LoadHighScore ()
	{
		SaveLoadController.Load (gameObject.name);
		localData.set (SaveLoadController.savedGames);

		if (localData.score.Count > 0) {
			highScoreList.text = "";
			for (int i = 0; i < localData.score.Count; i++) {
				highScoreList.text += FormatScore (i, localData.name [i], localData.score [i]);
			}
		} else {
			highScoreList.text = "no high score";
		}
	}

	/// <summary>
	/// Formats the score.
	/// </summary>
	/// <returns>The score.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="name">Name.</param>
	/// <param name="score">Score.</param>
	private static string FormatScore (int pos, string name, float score)
	{
		return string.Format ("{0} - {1} : {2:N2}\n", (++pos), name, score);
	}
}
