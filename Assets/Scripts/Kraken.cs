using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Kraken : MonoBehaviour {
	public int playerID;

	public float minSpeed;
	public float maxSpeed;
	public float rotSpeed;
	public float strokeFrequency;
	public float sinkingSpeed;

	[Range(0.0f, 1.0f)]
	public float splashPower;

	public Sprite[] swimSpites;
	public Sprite[] attackSprites;

	public Image playerPressurePanel;
	public GameObject explosionParticleSystem;
	public GameObject impactPrefab;

	public ParticleSystem waterDropParticleSystem;
	public ParticleSystem bubbleParticleSystem;

	private float speed;
	private float momentum;
	private float strokePhase;
	private float pressure;

	private float strokeTimer;
	private float noStrokeTimer;

	private string RotAxisName;
	private string ForwardAxisName;
	private string SwimButtonName;
	private string AttackButtonName;

	private SpriteRenderer spriteRenderer;
	private AudioSource swimmingSound;
	private AudioSource dyingSound;
	private AudioSource beukenSound;
	private AudioSource splashSound;
	private AudioSource[] AllAudio;
		

	private Sea sea = null;

	private int attackSpriteIndex;

	void Start () {
		strokePhase = 0;
		momentum = 0;
		pressure = 0;
		speed = minSpeed;
		if (maxSpeed < minSpeed) {
			maxSpeed = minSpeed;
		}
		ForwardAxisName = "Vertical" + playerID;
		RotAxisName = "Horizontal" + playerID;
		AttackButtonName = "MakeWaves" + playerID;
		SwimButtonName = "Swim" + playerID;

		sea = FindObjectOfType<Sea> ();

		spriteRenderer = GetComponent<SpriteRenderer> ();
		AllAudio = GetComponents<AudioSource> ();
		swimmingSound = AllAudio[0];
		beukenSound     = AllAudio[1];
		dyingSound     = AllAudio[2];
		splashSound     = AllAudio[3];

		if (playerID == 0) {
			GetComponent<Rigidbody2D> ().gravityScale = 0.0f;
			GetComponent<Rigidbody2D> ().freezeRotation = false;
		} else {
		}
	}

	void FixedUpdate()
	{
		if (playerID == 0) {
		} else {
			if (strokeTimer > 0.0f) {
				float timeScalar = Mathf.Clamp01 (strokeTimer / 0.5f);
				GetComponent<Rigidbody2D> ().AddForce (transform.up * 2000.0f * timeScalar * Time.fixedDeltaTime);
			} else if (noStrokeTimer > 0.0f) {
				float timeScalar = Mathf.Clamp01 (noStrokeTimer / 0.5f);
				GetComponent<Rigidbody2D> ().AddForce (-transform.up * 1000.0f * timeScalar * Time.fixedDeltaTime);
			}
		}
	}

	void Update () {
		if (pressure >= 1.0f) {
			return;
		}

		float turn = Input.GetAxis (RotAxisName);
		transform.Rotate (-turn * rotSpeed * Vector3.forward * Time.deltaTime);

		if (playerID == 0) {
			//float swim = Input.GetAxis (ForwardAxisName);
			float swim = Input.GetButton(SwimButtonName) ? 1.0f : 0.0f;
			Vector2 deltaForward = new Vector2 ();
			if (swim > 0) {
				momentum = swim;
				speed += (maxSpeed - speed) / .1f * Time.deltaTime;
				deltaForward = momentum * speed * Vector2.up * Time.deltaTime * Mathf.Abs (Mathf.Sin (strokePhase));
				strokePhase += strokeFrequency * Time.deltaTime;

				if (Mathf.Abs (Mathf.Sin (strokePhase)) < .5f) {
					spriteRenderer.sprite = swimSpites [0];
				} else {
					spriteRenderer.sprite = swimSpites [1];
				}

				if (!swimmingSound.isPlaying) {
					swimmingSound.Play ();
				}
			} else if (momentum > 0) {
				momentum -= Time.deltaTime;
				speed = minSpeed;
				deltaForward = momentum * speed * Vector2.up * Time.deltaTime;
			}
			transform.Translate (deltaForward);

			Vector2 sinkingVec = transform.InverseTransformDirection (Vector2.down);
			transform.Translate (sinkingSpeed * Time.deltaTime * sinkingVec);
		} else {
			strokeTimer -= Time.deltaTime;
			noStrokeTimer -= Time.deltaTime;
			if (Input.GetButtonDown (SwimButtonName)) {
				strokeTimer = 0.5f;
				noStrokeTimer = 0.0f;
				spriteRenderer.sprite = swimSpites [1];
				if (!swimmingSound.isPlaying) {
					swimmingSound.Play ();
				}
			}
			if (Input.GetButtonUp (SwimButtonName)) {
				strokeTimer = 0.0f;
				noStrokeTimer = 0.2f;
				spriteRenderer.sprite = swimSpites [0];
			}
		}


		// Make waves (only when under water)
		if (sea != null) {
			float depth = sea.GetHeightVelocity (transform.position.x).x - transform.position.y;

			if (depth > 0.0f && Input.GetButtonDown (AttackButtonName)) {
				float finalSplashPower = splashPower * Mathf.Clamp01 (2.0f - depth) * 5.0f;
				sea.Splash (Random.Range (transform.position.x - 0.2f, transform.position.x + 0.2f), finalSplashPower);

				spriteRenderer.sprite = attackSprites [attackSpriteIndex];
				attackSpriteIndex = (attackSpriteIndex + 1) % attackSprites.Length;

				GameObject impact = Instantiate (impactPrefab, transform.position, transform.rotation);
				Destroy (impact, 0.7f);

				if (depth < 1.0f) {
					splashSound.Play ();
					splashSound.pitch = Random.Range (0.9f, 1.1f);
					ParticleSystem p = Instantiate<ParticleSystem> (waterDropParticleSystem, new Vector3(transform.position.x, 0.0f, 0.0f), waterDropParticleSystem.transform.rotation);
					Destroy (p.gameObject, 1.0f);
				}
			}

			if (Input.GetKeyDown(KeyCode.I)){
				float finalSplashPower = splashPower * Mathf.Clamp01 (2.0f - 0.3f) * 5.0f;
				sea.Splash (Random.Range (transform.position.x - 0.2f, transform.position.x + 0.2f), finalSplashPower);
			}
		}

		if (playerPressurePanel != null) {
			float depth = sea.GetHeightVelocity (transform.position.x).x - transform.position.y;
			if (depth < 2.0f) {
				pressure += 0.1f * Time.deltaTime;
			} else {
				pressure -= 0.2f * Time.deltaTime;
			}

			if (pressure >= 1.0f) {
				StartCoroutine (StartDeadSequence ());
			}

			pressure = Mathf.Clamp01 (pressure);
			playerPressurePanel.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, pressure * 225.0f);
		}
	}
	
	IEnumerator StartDeadSequence() {
		GetComponent<SpriteRenderer>().enabled = false;
		Instantiate(explosionParticleSystem, transform.position, transform.rotation);
		dyingSound.Play ();
		GetComponent<CircleCollider2D> ().enabled = false;
		GetComponent<Rigidbody2D> ().simulated = false;

		yield return new WaitForSeconds(2.0f);
		if (playerID == 0) {
			SceneManager.LoadScene ("EndScreen_Orange");
		} else {
			SceneManager.LoadScene ("EndScreen_Purple");
		}
	}

	public void AddDamage(float d) {
		pressure += d;
		Debug.Log (pressure);
		if (pressure > 1.0f) {
			StartCoroutine (StartDeadSequence ());
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (!beukenSound.isPlaying & momentum > 0) {
			beukenSound.Play ();
		}
		//Debug.Log ("Hit!");
	}
}
