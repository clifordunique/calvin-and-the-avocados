using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Collectable controller.
/// </summary>
public class CollectableController : MonoBehaviour
{

	// layer
	public LayerMask playerMask;

	// controller
	public Player player;

	// loader
	public string level;
	public GameObject levelLoader;

	// local
	private bool collected;
	private bool isLoading = false;

	// sprite
	private SpriteRenderer sprite;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		sprite = GetComponent<SpriteRenderer> ();
		player = player.GetComponent<Player> ();

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void FixedUpdate ()
	{
		OnCollisionWithPlayer ();

		if (collected && !isLoading) {
			FinishLevel ();
		}

	}

	/// <summary>
	/// Execute actions when a level is complete 
	/// by the player
	/// </summary>
	/// <returns>The level.</returns>
	private void FinishLevel ()
	{
		Debug.Log ("-- finish level ---");
		sprite.enabled = false;
		isLoading = true;
		Invoke ("LoadNewLevel", 3f);
		
	}

	/// <summary>
	/// Load new level
	/// </summary>
	private void LoadNewLevel ()
	{

		if (Session.isSpeedrunMode) {
			levelLoader.GetComponent<LevelLoader> ().LoadLevel (level);
		} else {
			Session.lastPlayedScene = SceneManager.GetActiveScene ().name;
			SceneManager.LoadSceneAsync ("worldmap");
		}
	}

	/// <summary>
	/// Raises the collision with player event.
	/// </summary>
	private void OnCollisionWithPlayer ()
	{
		if (player.hasVictory) {
			collected = true;
		}
	}

}
