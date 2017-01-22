using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenTentacle : MonoBehaviour {

	public bool ShowTentacle;
	public bool DoAttack;
	public bool ShowHead;

	private Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
		StartCoroutine(AnimationCoroutine());
	}

	IEnumerator AnimationCoroutine()
	{
		while (true) {

			ShowTentacle = false;
			ShowHead = false;
			DoAttack = false;

			// wait a while...
			yield return new WaitForSeconds (Random.Range(2, 4));

			ShowTentacle = true;
			animator.SetBool ("ShowTentacle", ShowTentacle);

			yield return new WaitForSeconds (Random.Range (2, 4));

			ShowHead = true;
			animator.SetBool ("ShowHead", ShowHead);

			yield return new WaitForSeconds (Random.Range (2, 4));

			DoAttack = true;
			animator.SetBool ("DoAttack", DoAttack);

		}
	}

}
