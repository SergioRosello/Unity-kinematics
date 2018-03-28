using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Dagger : MonoBehaviour {
	float spawnTime, timeToDespawn = 5;
	private SpriteRenderer sr;
	private Rigidbody2D rb;

	void Awake() {
		StartCoroutine (DespawnCoroutine());
		sr = GetComponentInChildren<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		sr.flipY = rb.velocity.x < 0;
	}

	private IEnumerator DespawnCoroutine() {
		yield return new WaitForSeconds(timeToDespawn);
		Destroy (gameObject);
		yield return null;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == Layers.Enemies) {
			var targetHealth = other.GetComponent<Health> ();
			if (targetHealth) {
				targetHealth.CurrentHealth--;
			}
			ParticleManager.Instance.SpawnParticles (ParticleManager.Instance.BloodPrefab, transform.position);
		} else if (other.gameObject.layer == Layers.Platforms) {
			ParticleManager.Instance.SpawnParticles (ParticleManager.Instance.GroundHitPrefab, transform.position);
		}

		if (other.gameObject.layer == Layers.Enemies ||  other.gameObject.layer == Layers.Platforms) {
			Destroy (gameObject);
		}
	}	
}
