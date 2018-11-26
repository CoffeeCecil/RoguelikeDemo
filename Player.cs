using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public float restartLevelDelay = 1f;
	public int pointsPerFood=10;
	public int pointsPerSoda = 20;
	public int wallDamage=1;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	public int food;
	public Text foodText;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	private Vector2 touchOrigin = -Vector2.one;
#endif

	protected override void Start(){
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;
		Debug.Log(food.ToString());
		base.Start();//call the base class.
	}
	
	private void OnDisable(){
		GameManager.instance.playerFoodPoints = food;
	}

	// Update is called once per frame
	private void Update () {
		if(!GameManager.instance.playersTurn){
			return;
		}

		int horizontal =0, vertical = 0;

		horizontal = (int)(Input.GetAxisRaw("Horizontal"));
		vertical = (int)(Input.GetAxisRaw("Vertical"));
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if(horizontal != 0){
			vertical = 0;
		}
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	
		if(Input.touchCount > 0){
			Touch myTouch = Input.touches[0]
		}

		if(myTouch.phase == TouchPhase.Began){
			touchOrigin = myTouch.position;
		}

		else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >=0){
			Vector2 touchEnd = myTouch.position;
			float x = touchEnd.x - touchOrigin.x;
			touchOrigin.x =-1;
			if(Mathf.Abs(x) > Mathf.Abs(y)){
				horizontal = x > 0 ? 1 : -1;
			}else{
				vertical = y > 0 ? 1 : -1;
			}
		}
#endif
		if(horizontal !=0 || vertical !=0){
			AttemptMove<Wall>(horizontal, vertical);
		}
	}

	protected override void AttemptMove<T>(int xdir, int ydir){
		--food;
		foodText.text = string.Format("Food: {0}", food);
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;
		//play some noise
		if(Move (xdir, ydir, out hit)){
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
	}

	private void pickupFood(int amt){
		Debug.Log("f: " + food);
		Debug.Log(string.Format("f: {0}", food));
		foodText.text = string.Format("+{0} Food: {0}", amt, food);  
	}

	protected override void OnCantMove<T>(T component){
		Wall hitwall = component as Wall;

		hitwall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");

	}

	private void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Exit"){
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}

		else if (other.tag == "Food"){
			food += pointsPerFood;
			pickupFood(pointsPerFood);
			other.gameObject.SetActive(false);
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
		}

		else if (other.tag == "Soda"){
			food += pointsPerSoda;
			pickupFood(pointsPerSoda);
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive(false);
		}

	}
	private void Restart(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}

	public void LoseFood(int loss){
		animator.SetTrigger("playerHit");
		food-=loss;
		foodText.text = string.Format("-{0} Food: {0}", loss, food);
		CheckIfGameOver();
	}

	private void CheckIfGameOver(){
		if (food < 1){
			
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}
}
