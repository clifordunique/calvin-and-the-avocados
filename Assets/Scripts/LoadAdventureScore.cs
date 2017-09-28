using UnityEngine;
using UnityEngine.UI;

public class LoadAdventureScore : MonoBehaviour
{

	public Text highScoreList;
	public GameSerialization localData = new GameSerialization ();

	// Use this for initialization
	void Start ()
	{

		SaveLoadController.Load ("adventure");
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
