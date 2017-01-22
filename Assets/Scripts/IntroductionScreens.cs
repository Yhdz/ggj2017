using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroductionScreens : MonoBehaviour {

	public GameObject[] screens;

	private int screenIndex = 0;

	private GameObject screenObj;

	// Use this for initialization
	void Start () {
		if (screens.Length > 0) {
			screenObj = Instantiate(screens[screenIndex++]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			if (screenIndex < screens.Length) {
				Destroy (screenObj);
				screenObj = Instantiate (screens [screenIndex++]);
			} else {
				SceneManager.LoadScene ("Game");
			}
		}
	}
}
