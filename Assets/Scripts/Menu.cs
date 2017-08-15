﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class Menu : MonoBehaviour
{

    // audio
	public AudioClip selectAudio;
	private AudioSource sourceAudio;

	public string scene;
	public Button start;
	public Button quit;

	List<Button> buttons;
	int current;
	bool checkAxes = false;

	// Use this for initialization
	void Start ()
	{

		buttons = new List<Button> ();

		Time.timeScale = 1f;
		sourceAudio = GetComponent<AudioSource> ();

		// restart button
		start = start.GetComponent<Button> ();
		buttons.Add (start);

		// quit button
		quit = quit.GetComponent<Button> ();
		quit.onClick.AddListener (QuitManager);
		buttons.Add (quit);

		current = -1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputMap ();
	}

	/// <summary>
	/// Input when in pause menu mode
	/// </summary>
	private void InputMap ()
	{

		if (Input.GetAxisRaw ("Vertical") == 1 && checkAxes) {
            sourceAudio.PlayOneShot(selectAudio);
			Debug.Log ("input up " + current);
			current = (current <= 0) ? 0 : --current;
			buttons [current].Select ();
			checkAxes = false;
		}

		if (Input.GetAxisRaw ("Vertical") == -1 && checkAxes) {
            sourceAudio.PlayOneShot(selectAudio);
			Debug.Log ("input down before " + current);
			int count = buttons.Count - 1;
			current = (current == count) ? count : ++current;
			buttons [current].Select ();
			Debug.Log ("input down after " + current);
			checkAxes = false;
		}

		if (Input.GetAxisRaw ("Vertical") == 0 && !checkAxes) {
			checkAxes = true;
		}

	}

	/// <summary>
	/// Quit application
	/// </summary>
	private void QuitManager ()
	{
		Application.Quit ();
	}
}