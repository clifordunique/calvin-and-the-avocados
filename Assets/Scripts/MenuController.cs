using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Menu controller.
/// </summary>
public class MenuController : MonoBehaviour
{

	// audio
	public AudioClip selectAudio;
	protected AudioSource sourceAudio;

	public EventSystem ES;
	protected GameObject storeSelected;

	/// <summary>
	/// Start this instance.
	/// Get audiosource
	/// Let the time flow
	/// store first selected object
	/// </summary>
	public virtual void Start ()
	{
		sourceAudio = GetComponent<AudioSource> ();
		Time.timeScale = 1f;
		storeSelected = ES.firstSelectedGameObject;
	}

	/// <summary>
	/// Update this instance.
	/// Apply mouse conflict fix
	/// </summary>
	public virtual void Update ()
	{
		FixMouseConflictController ();
	}

	/// <summary>
	/// If we use the mouse, we loose focus
	/// on the button and the navigation become impossible
	/// with a controller. This fix assure that a button is
	/// always selected.
	/// </summary>
	public void FixMouseConflictController ()
	{
		if (ES.currentSelectedGameObject != storeSelected) {

			sourceAudio.PlayOneShot (selectAudio);

			if (ES.currentSelectedGameObject == null) {
				ES.SetSelectedGameObject (storeSelected);
			} else {
				storeSelected = ES.currentSelectedGameObject;
			}
		}	
	}
 
}
