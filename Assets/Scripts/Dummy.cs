using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour {
	private Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		//rb.angularVelocity = -45;
		//rb.velocity = Vector2.right + Vector2.up * .5f;

	}
	
	// Update is called once per frame
	void Update () {
		//transform.position += new Vector3(0, 1, 0) * Time.deltaTime;
		//rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + new Vector2(1, 0) * Time.deltaTime);

		//transform.position = new Vector3(0, 1, 0);
		//rb.MovePosition(new Vector2(0, 1));

		var hit = Utils.Raycast2D (new Vector2 (transform.position.x, transform.position.y), Vector2.right, 100, 1 << Layers.Enemies);

		if (hit.collider == null) {
			Debug.Log ("Null");
		} else {
			Debug.Log (hit.collider.name);
		}

		var hits = Utils.RaycastAll2D (new Vector2 (transform.position.x, transform.position.y + .05f), Vector2.right, 100, 1 << Layers.Enemies);
		Debug.Log ("Touching " + hits.Length + " colliders");
	}
}
