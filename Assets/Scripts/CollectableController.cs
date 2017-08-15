using UnityEngine;

// dependencies
[RequireComponent (typeof(AudioSource))]
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
	bool collected;
    bool isLoading = false;

    // sprite
	SpriteRenderer sprite;

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
	void FixedUpdate ()
	{
		UpdateRaycastOrigins ();
		OnCollisionWithPlayer ();

		if (collected && !isLoading) {
            sourceAudio.PlayOneShot(finishAudio);
            sprite.enabled = false;
            player.inputEnable = false;
            isLoading = true;
            Invoke("LoadNewLevel", 3f);
		}

	}

    /// <summary>
    /// Load new level
    /// </summary>
    void LoadNewLevel()
    {
        levelLoader.GetComponent<LevelLoader> ().LoadLevel (level);
    }

	/// <summary>
	/// Raises the collision with player event.
	/// </summary>
	void OnCollisionWithPlayer ()
	{

		float rayLength = 1 + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			// witch direction are we moving ?
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigins.center, Vector2.up, rayLength, playerMask);

			// we found the passenger and see how far we gonna move him
			if (hit) {
				collected = true;
			}
		}
	}

}
