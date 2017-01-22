using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenTentacleTrigger : MonoBehaviour {

	void OnTriggerStay2D(Collider2D collider) {
		Kraken squidy = collider.gameObject.GetComponent<Kraken> ();
		if (squidy != null) {
			squidy.AddDamage (.1f);
		}
	}
}
