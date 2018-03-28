using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovementController {

	protected override void Awake() {
		base.Awake ();
		h = Random.value < .5f ? -1 : 1;
	}

	protected override void DetermineDirection () {
		// Detección de paredes y suelo
		var hitWall = Utils.Raycast2D(new Vector2(transform.position.x, transform.position.y), new Vector2(h, 0), .01f + box.size.x / 2, ObstacleMask);
		var hitGround = Utils.Raycast2D(new Vector2(transform.position.x + h * (box.size.x/2), transform.position.y), Vector2.down, .5f + box.size.y / 2, ObstacleMask);

		if (hitWall.collider != null || hitGround.collider == null) {
			h = -h;
		}

	}
}