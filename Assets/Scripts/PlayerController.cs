using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController {

	public GameObject DaggerPrefab;

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
			dagger.GetComponent<Rigidbody2D> ().velocity = facing * 15;
		}

		//Checks if player is sprinting
		if(Input.GetKey(KeyCode.LeftShift)) sprinting = true;
		else sprinting = false;
	}

	protected override void DetermineDirection() {
		h = Input.GetAxisRaw ("Horizontal");
	}
}