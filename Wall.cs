using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
	public AudioClip hitOne;
	public AudioClip hitTwo;
	public Sprite dmgsprite;

	public int hp= 4;

	private SpriteRenderer spriteRenderer;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void DamageWall (int loss){
		SoundManager.instance.RandomizeSfx(hitOne, hitTwo);
		spriteRenderer.sprite = dmgsprite;
		hp -= loss;

		if (hp < 1){
			gameObject.SetActive(false);
		}
	}
}
