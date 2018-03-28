using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	private Transform target;
	public Vector3 offset = new Vector3(0, 1, -10);

	// Use this for initialization
	void Awake () {
		target = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			transform.position = target.transform.position + offset;
		}
	}
}
