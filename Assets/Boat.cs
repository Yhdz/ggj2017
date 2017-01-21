using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {
	private StringSimulation sea;
	private Rigidbody2D rigidBody;
	private float randomTimeOffset;

	// Use this for initialization
	void Start () {
		sea = FindObjectOfType<StringSimulation> ();
		rigidBody = GetComponent<Rigidbody2D> ();
		randomTimeOffset = Random.Range (0.0f, Mathf.PI * 2);
	}

	void FixedUpdateFloatPhysics()
	{
		Vector2 heightVelocity = sea.GetHeightVelocity (transform.position.x);

		float heightDiff = heightVelocity.x - transform.position.y;
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

	void FixedUpdateStickToWater()
	{
		Vector2 heightVelocity = sea.GetHeightVelocity (transform.position.x);
		float slowBounce = Mathf.Sin (Time.time * 8.0f + randomTimeOffset) * 0.02f;

		transform.position = new Vector3 (transform.position.x, heightVelocity.x + slowBounce, transform.position.z);
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.time * 3.0f + randomTimeOffset) * 5.0f);
	}

	void FixedUpdate()
	{
		//FixedUpdateFloatPhysics();
		FixedUpdateStickToWater();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
