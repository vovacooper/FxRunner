using UnityEngine;
using System.Collections;

public class FXRunnerPlayer : MonoBehaviour {


	static public int CollisionCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
		//this.transform.RotateAround( Vector3.forward , Input.GetAxisRaw("Horizontal") * 20 );
		this.transform.Rotate( Vector3.forward , -Input.GetAxisRaw("Horizontal") * 15 ,Space.Self );
	}

	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
		Debug.Log(other.name);

		CollisionCount++;

		//FXRunner.fxRunnerManager.gameScore += FXRunner.fxRunnerManager.speed ;
		//FXRunnerGUI.Instance.Score = FXRunner.fxRunnerManager.gameScore;
	}
}
