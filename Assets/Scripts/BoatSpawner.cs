using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoatSpawner : MonoBehaviour {
	public GameObject BoatPrefab;

	int numBoatsAlive0 = 10;
	int numBoatsAlive1 = 10;

	// Use this for initialization
	void Start () {
		// spawn boats for every player
		for (int player = 0; player < 2; player++) {
			for (int boatNumber = 0; boatNumber < 10; boatNumber++) {
				float randomPosition = Random.Range (-7.0f, 7.0f);
				Boat boat = Instantiate (BoatPrefab, new Vector3 (randomPosition, 0.0f, 0.0f), Quaternion.identity).GetComponent<Boat> ();;
				boat.playerID = player;
				boat.speed = (Random.Range (0, 2) * 2 - 1) * boat.speed;
				boat.boatSpawner = this;
			}
		}

	}


	public void BoatSunk(int playerID)
	{
		if (playerID == 0)
		{
			numBoatsAlive0--;
		}
		if (playerID == 1)
		{
			numBoatsAlive1--;
		}

		if (numBoatsAlive0 <= 0 || numBoatsAlive1 <= 0){
			StartCoroutine(StartEndSequence());
		}
	}

	IEnumerator StartEndSequence()
	{
		// TODO: play end sound
		Debug.Log ("Game over by boat count");
		yield return new WaitForSeconds(3.0f);
		if (numBoatsAlive0 <= 0) {
			SceneManager.LoadScene ("EndScreen_Orange");
		} else {
			SceneManager.LoadScene ("EndScreen_Purple");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
