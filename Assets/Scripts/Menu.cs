﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Menu.
/// </summary>
public class Menu : MenuController
{

	// audio
	public AudioClip selectAudio;

	public string scene;
	public Button start;
	public Button quit;

	// Use this for initialization
	public override void Start ()
	{

		base.Start ();

		// restart button
		start = start.GetComponent<Button> ();
		buttons.Add (start);

		// quit button
		quit = quit.GetComponent<Button> ();
		quit.onClick.AddListener (QuitManager);
		buttons.Add (quit);
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputMap ();
	}

	/// <summary>
	/// Quit application
	/// </summary>
	static void QuitManager ()
	{
		Application.Quit ();
	}
}
