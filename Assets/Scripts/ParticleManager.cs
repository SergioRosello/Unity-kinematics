using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager> {
	public GameObject BloodPrefab;
	public GameObject GroundHitPrefab;

	void Start() {
		PoolManager.Load (BloodPrefab, 5);
		PoolManager.Load (GroundHitPrefab, 5);
	}

	public void SpawnParticles(GameObject prefab, Vector3 position) {
		var go = PoolManager.Spawn (prefab);
		go.transform.position = position;
	}
}
