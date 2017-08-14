using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

	public GameObject loadingScreen;
	public Slider slider;
	public Text pressKey;

	/// <summary>
	/// Ons the awake.
	/// </summary>
	/// <returns>The awake.</returns>
	public void Awake ()
	{
		loadingScreen.SetActive (false);
	}


	/// <summary>
	/// Loads the level.
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="scene">Scene.</param>
	public void LoadLevel (string scene)
	{
		StartCoroutine (LoadAsync (scene));
	}

	/// <summary>
	/// Loads the async.
	/// </summary>
	/// <returns>The async.</returns>
	/// <param name="scene">Scene.</param>
	IEnumerator LoadAsync (string scene)
	{
		yield return null;

		AsyncOperation operation = SceneManager.LoadSceneAsync (scene);

		pressKey.text = "";
		loadingScreen.SetActive (true);
		operation.allowSceneActivation = false;

		while (!operation.isDone) {
			// convert value to [0-1]
			float progress = Mathf.Clamp01 (operation.progress / .9f);

			slider.value = progress;

			if (operation.progress == .9f) {
				pressKey.text = "Press start to continue";

				if (Input.GetButtonDown ("Pause")) {
					operation.allowSceneActivation = true;
				}
			}
			yield return null;
		}
	}
}
