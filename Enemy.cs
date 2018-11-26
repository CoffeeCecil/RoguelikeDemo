using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public int playerDamage;
	public AudioClip attackSound1;
	public AudioClip attackSound2;
	private Animator animator;
	private Transform target;
	private bool skipMove;


	// Use this for initialization
	protected override void Start () {
		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
	}
	
	protected override void AttemptMove<T>(int xdir, int ydir){
		if(skipMove){
			skipMove = false;
			return;
		}
		base.AttemptMove<T>(xdir, ydir);
	}

	public void MoveEnemy(){
		int xdir=0,ydir = 0;
		if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon){
			ydir = target.position.y > transform.position.y ? 1 : -1;
		}
		else{
			xdir = target.position.x > transform.position.x ? 1 : -1;
		}
		AttemptMove<Player>(xdir, ydir);
	}

	protected override void OnCantMove<T>(T component){
		Player hitPlayer = component as Player;
		hitPlayer.LoseFood(playerDamage);
		animator.SetTrigger("enemyAttack");
		SoundManager.instance.RandomizeSfx(attackSound1,attackSound2);
	}

}
