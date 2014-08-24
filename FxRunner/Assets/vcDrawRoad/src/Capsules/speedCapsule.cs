using UnityEngine;
using System.Collections;

public class speedCapsule : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {

		Debug.Log("From Capsule " + other.name);
		FXRunner.fxRunnerManager.speed += 1;

		Destroy(this.gameObject);
	}
}
