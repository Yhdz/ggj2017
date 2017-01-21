using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSpawner : MonoBehaviour {
	public GameObject BoatPrefab;

	// Use this for initialization
	void Start () {
		// spawn boats for every player
		for (int player = 0; player < 2; player++) {
			for (int boatNumber = 0; boatNumber < 10; boatNumber++) {
				float randomPosition = Random.Range (-7.0f, 7.0f);
				Boat boat = Instantiate (BoatPrefab, new Vector3 (randomPosition, 0.0f, 0.0f), Quaternion.identity).GetComponent<Boat> ();;
				boat.party = player;
				boat.speed = (Random.Range (0, 2) * 2 - 1) * boat.speed;
				if (boat.party == 0) {
					boat.SetColor (Color.red);
				} else {
					boat.SetColor (Color.blue);
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
