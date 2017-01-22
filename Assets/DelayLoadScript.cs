using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayLoadScript : MonoBehaviour {
	public float delayTime;
	public string sceneName;

	// Use this for initialization
	void Start () {
		Invoke ("LoadScene", delayTime);
	}

	void LoadScene(){
		SceneManager.LoadScene (sceneName);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
