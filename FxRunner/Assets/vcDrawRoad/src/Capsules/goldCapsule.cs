using UnityEngine;
using System.Collections;

public class goldCapsule : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		
		Debug.Log("From Capsule " + other.name);
		//FXRunner.fxRunnerManager.gameScore += 100;
		FXRunner.fxRunnerManager.gameScore += FXRunner.fxRunnerManager.speed ;
		Destroy(this.gameObject);
	}

}
