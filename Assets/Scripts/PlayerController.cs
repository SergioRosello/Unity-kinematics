using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController {

	public GameObject DaggerPrefab;
	public float daggerSpeed = 6;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		// Saltar
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

		// Disparar
		if (Input.GetMouseButtonDown(0)) {
			
			var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			direction.Normalize();

			var dagger = GameObject.Instantiate(DaggerPrefab, transform.position + direction, Quaternion.identity);
			dagger.GetComponent<Rigidbody2D>().velocity = direction * daggerSpeed;
			//TODO: Falta hacer que los machetes estén dirigidos hacia el cursor
			// dagger.GetComponent<Transform>().rotation.SetLookRotation(direction);
		}

		// Sprint
		if(Input.GetKey(KeyCode.LeftShift)) sprinting = true;
		else sprinting = false;

		// Agacharse
		if(Input.GetKey(KeyCode.LeftAlt)) crouching = true;
		else crouching = false;
	}

	protected override void DetermineDirection() {
		h = Input.GetAxisRaw ("Horizontal");
	}
}