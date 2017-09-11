using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

/// <summary>
/// Save load controller.
/// </summary>
public static class SaveLoadController
{
	public const string FOLDER = "Saves";
	public const string SAVE_EXT = ".savegame";
	public static GameSerialization savedGames = new GameSerialization ();


	/// <summary>
	/// Create a savefile
	/// </summary>
	public static void Save (string scene)
	{
		if (!Directory.Exists (FOLDER)) {
			Directory.CreateDirectory (FOLDER);
		}

		BinaryFormatter bf = new BinaryFormatter ();

		FileStream file = File.Create (FOLDER + "/" + scene + SAVE_EXT);

		SaveLoadController.savedGames = Score.Instance.localPlayerData;

		bf.Serialize (file, SaveLoadController.savedGames);

		file.Close ();	

	}

	/// <summary>
	/// Load the last save file
	/// </summary>
	public static void Load (string scene)
	{
		if (File.Exists (FOLDER + "/" + scene + SAVE_EXT)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (FOLDER + "/" + scene + SAVE_EXT, FileMode.Open);
			SaveLoadController.savedGames = (GameSerialization)bf.Deserialize (file);
			Debug.Log (SaveLoadController.savedGames.name);
			Debug.Log (SaveLoadController.savedGames.score);
			file.Close ();
		}
	}
}
