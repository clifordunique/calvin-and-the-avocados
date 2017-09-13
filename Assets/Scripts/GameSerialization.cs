using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable] 
public class GameSerialization
{

	public List<string> name;
	public List<float> score;

	/// <summary>
	/// Initializes a new instance of the <see cref="GameSerialization"/> class.
	/// </summary>
	public GameSerialization ()
	{
		name = new List<string> ();
		score = new List<float> ();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GameSerialization"/> class.
	/// </summary>
	/// <param name="_name">Name.</param>
	/// <param name="_score">Score.</param>
	public void Add (string _name, float _score)
	{

		// are we in the top 10 high score

		score.Sort ();

		int index = -1;
		int i = 0;

		// get new high score position
		while (i < score.Count && index == -1) {
			if (_score < score [i]) {
				index = i;
				Debug.Log (i);
				Debug.Log (index);
			}

			i++;
		}

		if (index == -1) {
			index = score.Count;
		}

		// add score
		score.Insert (index, _score);
		name.Insert (index, _name);

		// if we had too much high score
		if (score.Count >= 20) {
			score = score.GetRange (0, 20);
		} 

	}

	/// <summary>
	/// Sets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="_name">Name.</param>
	/// <param name="_score">Score.</param>
	public void set (GameSerialization _game)
	{
		score = _game.score;
		name = _game.name;
	}
}
