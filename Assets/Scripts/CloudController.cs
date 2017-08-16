using UnityEngine;
using System.Collections;

/// <summary>
/// Spawn random cloud
/// </summary>
public class CloudController : MonoBehaviour
{
	public float BufferLimitX;
	public float minSpeed;
	public float maxSpeed;
	public float minY;
	public float maxY;
	public float maxScale;
	public float minScale;

	private float speed;
	private float scale;

	private SpriteRenderer sprite;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{

		// set sprite default behavior
		sprite = GetComponent<SpriteRenderer> ();

		// init cloud mouvement
		RandomizeCloud ();

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		transform.Translate (speed * Time.deltaTime, 0, 0);
	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	/// <returns>The update.</returns>
	void FixedUpdate ()
	{

		// reset if offscreen
		if ((transform.position.x - scale) > BufferLimitX) {
			RandomizeCloud ();
		}
		
	}

	/// <summary>
	/// Randomizes the cloud.
	/// </summary>
	/// <returns>The cloud.</returns>
	void RandomizeCloud ()
	{

		//Set random speed
		speed = Random.Range (minSpeed, maxSpeed);

		// set random cloud size
		scale = Random.Range (minScale, maxScale);
		transform.localScale = new Vector3 (scale, scale, 1);

		// change sort ording if scale is to big
		sprite.sortingOrder = (scale <= 10) ? -2 : -1;

		// set cloud position
		float spawnX = -((Camera.main.orthographicSize * Camera.main.aspect) - Camera.main.gameObject.transform.position.x);
		transform.position = new Vector3 (spawnX, Random.Range (minY, maxY), transform.position.z);

	}
}