using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player"){
			other.GetComponent<PlayerController>().ObstacleMask ^= 1 << Layers.OneWayPlatform;
		}
    }

	void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player"){
			other.GetComponent<PlayerController>().ObstacleMask |= 1 << Layers.OneWayPlatform;
		}
    }
}

