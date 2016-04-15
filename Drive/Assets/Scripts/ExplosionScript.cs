using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var exp = GetComponent<ParticleSystem>();
		Destroy(gameObject, exp.duration);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
