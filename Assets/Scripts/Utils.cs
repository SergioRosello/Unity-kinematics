using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
	public static RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask) {
		Debug.DrawLine (new Vector3(origin.x, origin.y, 0), new Vector3(origin.x, origin.y, 0) + new Vector3(direction.x, direction.y, 0).normalized * distance, Color.red);
		return Physics2D.Raycast (origin, direction, distance, layerMask);
	}

	public static RaycastHit2D[] RaycastAll2D(Vector2 origin, Vector2 direction, float distance, int layerMask) {
		Debug.DrawLine (new Vector3(origin.x, origin.y, 0), new Vector3(origin.x, origin.y, 0) + new Vector3(direction.x, direction.y, 0).normalized * distance, Color.green);
		return Physics2D.RaycastAll (origin, direction, distance, layerMask);
	}
}