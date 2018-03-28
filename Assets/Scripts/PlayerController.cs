using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController {

	public GameObject DaggerPrefab;
	protected Vector2 mousePosition;
	public float daggerSpeed = 6;

	//https://pastebin.com/14H2X5j7

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (Input.GetKeyDown (KeyCode.Space)) {
			TryJump ();
		} else if (Input.GetKeyUp(KeyCode.Space)){
			// Calcular cuánto tiempo he apretado el botón
			var timePressed = Time.time - _jumpPressTime;
			// Calcular cuánto tengo que saltar en función a ese tiempo
			var percentHeight = Mathf.Clamp(timePressed/FullJumpTime, MinJumpPercent, 1);
			// Recalcular _targetHeight en función a esa "cantidad" de salto
			targetHeight = _jumpStartY + JumpHeight * percentHeight;
			Debug.Log ("Jumping " + (JumpHeight * percentHeight) +" meters");
		}

		if (Input.GetMouseButtonDown(0)) {
			// Lanzar daga
			var dagger = GameObject.Instantiate(DaggerPrefab, transform.position, Quaternion.identity);
			// El vector se tiene que convertir a coordenadas del personaje, por eso no va bien ;)
			
			// Parece que esta es otra forma de hacerlo, pero no va!
			// Vector3 target;
			// target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// dagger.GetComponent<Rigidbody2D>().velocity = Vector2.MoveTowards(transform.position, target, 20f);

			// Parece que esta es otra forma de hacerlo, pero no va!
			// mousePosition = transform.InverseTransformVector(Input.mousePosition).normalized;
			// Debug.Log("Mouse coordinates: " + mousePosition);
			// dagger.GetComponent<Rigidbody2D>().velocity = mousePosition * daggerSpeed;
		}

		//Checks if player is sprinting
		if(Input.GetKey(KeyCode.LeftShift)) sprinting = true;
		else sprinting = false;
	}

	protected override void DetermineDirection() {
		h = Input.GetAxisRaw ("Horizontal");
	}
}