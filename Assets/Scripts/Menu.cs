using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public GameObject titleScreenPanel;
	public GameObject gameNamePanel;
	public GameObject menuPanel;
	public GameObject creditsPanel;

	// Use this for initialization
	void Start () {
		titleScreenPanel.SetActive (true);
		gameNamePanel.SetActive (false);
		menuPanel.SetActive (false);
		creditsPanel.SetActive (false);

		StartCoroutine (GameTitleAnimationCoroutine());
	}

	IEnumerator GameTitleAnimationCoroutine()
	{
		yield return new WaitForSeconds (5f);
		titleScreenPanel.SetActive (false);
		gameNamePanel.SetActive (true);
		yield return new WaitForSeconds (5f);
		gameNamePanel.SetActive (false);
		menuPanel.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnStartGame()
	{
		SceneManager.LoadScene ("Introscreens");
	}

	public void OnCredits()
	{
		menuPanel.SetActive (false);
		creditsPanel.SetActive (true);
	}

	public void OnCreditsBack()
	{
		menuPanel.SetActive (true);
		creditsPanel.SetActive (false);
	}
}
