using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {
	public float speed;
	public int playerID;
	public Sprite[] sprites;

	public BoatSpawner boatSpawner;

	private Sea sea;
	private Rigidbody2D rigidBody;
	private float randomTimeOffset;
	private int state = 0;			// 0: floating, 1: sinking
	private float damage;

	// Use this for initialization
	void Start () {
		damage = 0.0f;
		sea = FindObjectOfType<Sea> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		randomTimeOffset = Random.Range (0.0f, Mathf.PI * 2.0f);

		GetComponent<SpriteRenderer> ().sprite = sprites [playerID];
	}

	public void SetColor(Color color)
	{
		GetComponent<SpriteRenderer> ().color = color;
	}

	void FixedUpdateFloatPhysics(float waterHeight, float waterAngle)
	{
		Vector3 heightVelocityAngle = sea.GetHeightVelocity (transform.position.x);

		float heightDiff = heightVelocityAngle.x - transform.position.y;
		rigidBody.drag = 0.1f;
		if (heightDiff > 0.0f) {
			float force = heightDiff * 100.0f;
			if (force > 50.0f) {
				force = 50.0f;
			}
			rigidBody.AddForce (Vector2.up * force);
			rigidBody.drag = 0.8f;
		}
	}

	void FixedUpdateStickToWater(float waterHeight, float waterAngle)
	{
		float slowBounce = Mathf.Sin (Time.time * 8.0f + randomTimeOffset) * 0.02f;

		transform.position = new Vector3 (transform.position.x, waterHeight + slowBounce, transform.position.z);
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.time * 3.0f + randomTimeOffset) * 3.0f + waterAngle);
	}

	void FixedUpdate()
	{
		if (state == 0) {
			Vector3 heightVelocityAngle = sea.GetHeightVelocity (transform.position.x);

			// Update bouncing on water
			//FixedUpdateFloatPhysics();
			FixedUpdateStickToWater (heightVelocityAngle.x, heightVelocityAngle.z);

			// update x position
			transform.Translate (new Vector3 (speed * Time.fixedDeltaTime, 0.0f, 0.0f));
			if (Mathf.Abs (transform.position.x) > 7.0f) {
				speed *= -1.0f;
			}

//			if (Mathf.Abs (heightVelocityAngle.y) > 0.01f) {
//				state = 1;
//			}

		} else if (state == 1) {	// sinking ship
			transform.Translate (new Vector3 (0.0f, -0.5f * Time.fixedDeltaTime, 0.0f), Space.World);
			if (transform.position.y < -7.0f) {
				state = 2;
			}

			boatSpawner.BoatSunk (playerID);

		} else if (state == 2) {
			// sunken ship
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		Impact impact = other.GetComponent<Impact> ();
		if (impact != null) {
			float distance = Vector2.Distance (other.transform.position, transform.position) / impact.radius;
			AddDamage ((1.0f - distance) * 0.5f);
		}
	}

	public void AddDamage(float d)
	{
		damage += d;
		if (damage > 1.0f) {
			state = 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		damage = Mathf.Clamp01(damage - Time.deltaTime);
	}
}
