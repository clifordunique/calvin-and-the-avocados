using UnityEngine;

// dependencies
[RequireComponent (typeof(AudioSource))]

/// <summary>
/// Collectable controller.
/// </summary>
public class CollectableController : RaycastController
{

	// audio
	public AudioClip finishAudio;
	private AudioSource sourceAudio;
    
	// layer
	public LayerMask playerMask;

	// player
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
	public override void Start ()
	{

		base.Start ();

		player = player.GetComponent<Player> ();
		sprite = GetComponent<SpriteRenderer> ();
		sourceAudio = GetComponent<AudioSource> ();

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void FixedUpdate ()
	{
		UpdateRaycastOrigins ();
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
		sourceAudio.PlayOneShot (finishAudio);
		sprite.enabled = false;
		player.inputEnable = false;
		player.moveSpeed = 0;
		player.runSpeed = 0;
		isLoading = true;
		Invoke ("LoadNewLevel", 3f);
		
	}

	/// <summary>
	/// Load new level
	/// </summary>
	private void LoadNewLevel ()
	{
		levelLoader.GetComponent<LevelLoader> ().LoadLevel (level);
	}

	/// <summary>
	/// Raises the collision with player event.
	/// </summary>
	private void OnCollisionWithPlayer ()
	{

		float rayLength = 1 + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigins.center, Vector2.up, rayLength, playerMask);

			// got a collision with the player
			if (hit) {
				collected = true;
			}
		}
	}

}
