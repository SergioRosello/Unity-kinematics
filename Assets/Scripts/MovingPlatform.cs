using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public Vector2[] path;

	public float MovementSpeed = 4;

	private Rigidbody2D rb;
	private Vector2[] _absolutePositions;
	private int _nextNode;
	private bool _goingForward = true;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		_absolutePositions = new Vector2[path.Length + 1];
		_absolutePositions [0] = new Vector2(transform.position.x, transform.position.y);

		for (int i = 0; i < path.Length; i++) {
			_absolutePositions [i + 1] = _absolutePositions [i] + path[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Ver cuánto queda hasta el destino
		var distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), _absolutePositions[_nextNode]);
		// Si hemos llegado al siguiente nodo, elegimos nuevo nodo
		if (distanceToTarget < 0.1f) {
			PickNextPoint ();
		}
		// Vemos en qué dirección nos vamos a mover
		var targetDirection = (_absolutePositions[_nextNode] - new Vector2(transform.position.x, transform.position.y)).normalized;
		// Aplicamos movimiento
		rb.velocity = targetDirection * MovementSpeed;
	}

	private void PickNextPoint() {
		int nextIndex = _nextNode + (_goingForward ? 1 : -1);

		if (nextIndex < 0) {
			_goingForward = true;
			nextIndex = 1;
		} else if (nextIndex >= _absolutePositions.Length){
			_goingForward = false;
			nextIndex = _absolutePositions.Length - 2;
		}

		_nextNode = nextIndex;
	}

	void OnDrawGizmos() {

		var points = new Vector3[path.Length + 1];

		if (_absolutePositions != null && _absolutePositions.Length > 0) {
			points [0] = _absolutePositions [0];
		} else {
			points [0] = transform.position;
		}

		for (int i = 0; i < path.Length; i++) {
			points[i+1] = points[i] + new Vector3(path[i].x, path[i].y, 0);
		}


		for (int i = 0; i < points.Length; i++) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (points[i], .1f);

			if (i + 1 < points.Length) {
				Gizmos.color = Color.white;
				Gizmos.DrawLine (points [i], points [i + 1]);
			}
		}
	}
}
