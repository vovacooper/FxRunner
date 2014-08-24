using UnityEngine;
using System.Collections;

public class timeCapsule : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		
		Debug.Log("From Capsule " + other.name);
		//FXRunner.fxRunnerManager.gameTime -= 10.0;

		Destroy(this.gameObject);
	}
}
