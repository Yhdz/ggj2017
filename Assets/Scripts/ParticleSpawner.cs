using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour {

	public ParticleSystem waterDropParticleSystem;
	public ParticleSystem bubbleParticleSystem;

	private float t;
	private float bubbleOnsetTime;

	void Start () {
		
	}


	void Update () {
		if (Input.GetKeyDown ("p")) {
			t = Time.time;
			if (!waterDropParticleSystem.isPlaying) {
				waterDropParticleSystem.Play ();
			}
		}
		if (Time.time - t > .5f) {
			waterDropParticleSystem.Stop ();
		}

		if (Input.GetKeyDown ("b")) {
			if (!bubbleParticleSystem.isPlaying) {
				bubbleOnsetTime = Time.time;
				bubbleParticleSystem.Play ();
			}
		}
		if (Time.time - bubbleOnsetTime > 1f) {
			bubbleParticleSystem.Stop ();
		}
	}
}
