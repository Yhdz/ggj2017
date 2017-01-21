using UnityEngine;
using System.Collections;

public class Kraken : MonoBehaviour {

	public float minSpeed;
	public float maxSpeed;
	public float rotSpeed;
	public float strokeFrequency;
	public float sinkingSpeed;

	public string RotAxisName;
	public string ForwardAxisName;

	public Sprite[] swimSpites;

	private float speed;
	private float momentum;
	private float strokePhase;

	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rigidBody;

	void Start () {
		strokePhase = 0;
		momentum = 0;
		speed = minSpeed;
		if (maxSpeed < minSpeed) {
			maxSpeed = minSpeed;
		}
		if (ForwardAxisName == null) {
			ForwardAxisName = "Vertical";
		}
		if (RotAxisName == null) {
			RotAxisName = "Horizontal";
		}
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		float turn = Input.GetAxis (RotAxisName);
		transform.Rotate (-turn * rotSpeed * Vector3.forward * Time.deltaTime);

		float swim = Input.GetAxis (ForwardAxisName);
		Vector2 deltaForward = new Vector2 ();
		if (swim > 0) {
			momentum = swim;
			speed += (maxSpeed - minSpeed) / .1f * Time.deltaTime;
			deltaForward = momentum * speed * Vector2.up * Time.deltaTime * Mathf.Abs (Mathf.Sin (strokePhase));
			strokePhase += strokeFrequency * Time.deltaTime;

			if (Mathf.Abs (Mathf.Sin (strokePhase)) < .5f) {
				spriteRenderer.sprite = swimSpites [0];
			} else {
				spriteRenderer.sprite = swimSpites [1];
			}
		} else if (momentum > 0) {
			momentum -= Time.deltaTime;
			speed = minSpeed;
			deltaForward = momentum * speed * Vector2.up * Time.deltaTime;
		}
		transform.Translate (deltaForward);

		Vector2 sinkingVec = transform.InverseTransformDirection (Vector2.down);
		transform.Translate (sinkingSpeed * Time.deltaTime * sinkingVec);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log ("Hit!");
	}
}
