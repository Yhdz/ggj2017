using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour {
	public float radius;

	// Use this for initialization
	void Start () {
		radius = GetComponent<CircleCollider2D> ().radius;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
