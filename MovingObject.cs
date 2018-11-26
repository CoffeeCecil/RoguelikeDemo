using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

	public float movetime = 0.1f;//movetime in seconds.

	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2d;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f/ movetime;
	}
	
	//Coroutine for moving units, takes the end position as an argument
	protected IEnumerator SmoothMovement( Vector3 end){
		float sqrRemainingDistance = (transform.position-end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon){
			Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
			rb2d.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;//wait for a frame.
		}

	}

	protected abstract void OnCantMove<T> (T component) where T:Component;

	protected bool Move(int xdir, int ydir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xdir, ydir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled=true;
		
		if (hit.transform == null){
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	protected virtual void AttemptMove<T>(int xdir, int ydir) where T: Component{
		RaycastHit2D hit;
		bool canMove = Move(xdir, ydir, out hit);

		if (hit.transform == null){
			return;
		}

		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null){
			OnCantMove(hitComponent);
		}
	}
}
