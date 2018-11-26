using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = 0.1f;
	public int playerFoodPoints = 100;
	public static GameManager instance= null;
	[HideInInspector] public bool playersTurn = true;
	private Text levelText;
	private GameObject levelImage;
	private BoardManager boardScript;
	private int level = 1;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup = true;

	// Use this for initialization
	void Awake () {
		if(instance == null){
			instance = this;
		} else if(instance != this){
			Destroy(gameObject);
		}
		//will not be destroyed when reloading scene.
		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();

		boardScript = GetComponent<BoardManager>();
		instance.InitGame();
	}
	

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
			instance.level++;
			instance.InitGame();
		}



	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode){
		level++;
		instance.InitGame();
	}

	void InitGame(){

		doingSetup = true;
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level;
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);
		enemies.Clear();
		Debug.Log(playerFoodPoints.ToString() + " At InitGame1: " + level.ToString());
		boardScript.SetupScene(level);
		Debug.Log(playerFoodPoints.ToString() + " At InitGame2: " + level.ToString());
	}

	void HideLevelImage(){
		levelImage.SetActive(false);
		doingSetup = false;
	}

	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving){
			return;
		}
		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy badguy){
		enemies.Add(badguy);
	}

	public void GameOver(){
		levelText.text = string.Format("After {0} days you starved.", level); 
		levelImage.SetActive(true);
		enabled = false;
	}
	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);

		if (enemies.Count==0){
			yield return new WaitForSeconds(turnDelay);
		}

		for(int i = 0; i < enemies.Count; i++){
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].movetime);
		}

		playersTurn = true;

		enemiesMoving = false;
	}




}
