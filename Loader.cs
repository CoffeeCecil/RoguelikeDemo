using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject gameManager;
	public GameObject soundManager;

	// Use this for initialization
	void Awake () {
		if (gameManager == null){
			throw new System.NullReferenceException("Game Manager Must be initialized in editor.");
		}
		if(GameManager.instance == null ){
			//check if static instance exists. If not, initiate the singleton.
			Instantiate(gameManager);

		}
		if (soundManager == null){
			throw new System.NullReferenceException("Sound Manager Must be initialized in editor.");
		}
		if(SoundManager.instance == null){
			Instantiate(soundManager);
		}
	}
	
}
