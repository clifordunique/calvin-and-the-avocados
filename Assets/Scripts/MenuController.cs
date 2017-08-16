using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu controller.
/// </summary>
public class MenuController : MonoBehaviour
{

	// audio
	public AudioClip selectAudio;
	protected AudioSource sourceAudio;

	protected int current;
	protected List<Button> buttons;
	protected bool checkAxes;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	public virtual void Awake ()
	{
		checkAxes = false;

	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	public virtual void Start ()
	{
		buttons = new List<Button> ();
		sourceAudio = GetComponent<AudioSource> ();
		Time.timeScale = 1f;
		current = 0;
	}

	/// <summary>
	/// Input when in pause menu mode
	/// </summary>
	public void InputMap ()
	{

		if (Input.GetAxisRaw ("Vertical") == 1 && checkAxes) {
			sourceAudio.PlayOneShot (selectAudio);
			Debug.Log ("input up " + current);
			current = (current <= 0) ? 0 : --current;
			buttons [current].Select ();
			checkAxes = false;
		}

		if (Input.GetAxisRaw ("Vertical") == -1 && checkAxes) {
			sourceAudio.PlayOneShot (selectAudio);
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
}
