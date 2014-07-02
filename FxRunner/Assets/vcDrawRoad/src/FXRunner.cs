using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

//Game manager
public class FXRunner : MonoBehaviour {
	
	/*******************************************************
	 * PUBLIC
	 ********************************************************/
	/// The player prefab.
	public GameObject Player;

	public Material lineMaterial;

	public GameObject[] obsiclesPrefabs;
	//GUI style
	public GUIStyle labelStyle;

	public static FXRunnerManager fxRunnerManager;

	/*******************************************************
	 * MonoBehaviour
	 ********************************************************/
	//ParseObject testObject;
	void Start () {
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.white;

		fxRunnerManager = new FXRunnerManager( this.transform , new RoadFunction(),lineMaterial,obsiclesPrefabs, 10 , 1 , .8f); //TODO

		//testObject = new ParseObject("TestObject");
		//testObject["foo"] = "bar";
		//testObject["a"] = "2";
		//testObject["b"] = "2";
		//testObject.SaveAsync();
		
	}
	
	public bool inputMethod = true;
	void Update () {
		fxRunnerManager.speed += Input.GetAxisRaw("Vertical") * Time.deltaTime ;
		//Debug.Log("Speed = " + fxRunnerManager.speed);

		////////CLEAR
		if(Input.GetKeyUp(KeyCode.C)) {
			fxRunnerManager.Restart();
		}
	
		//Camera speed update
		fxRunnerManager.MakeStep( Time.deltaTime );

		fxRunnerManager.setCam( Camera.main );

		if( Input.GetKeyUp(KeyCode.K) ){
			inputMethod = !inputMethod;
		}
		if(inputMethod){
			fxRunnerManager.setPlayer( Player , Input.GetAxisRaw("Horizontal") , Time.deltaTime ); //Keyboard
		}else{
			fxRunnerManager.setPlayer( Player ,2.5f * (Input.mousePosition.x/(Screen.width) - 0.5f) ); //Mouse
		}
	}
	
	

	
	/*******************************************************
	 * GUI debug
	 ********************************************************/

	void OnGUI() {
		int vc = fxRunnerManager.numOfLines;//
		GUI.Label (new Rect (10, 5, 300, 24), "Drawin " + vc + " lines. 'C' to clear", labelStyle);
		
		GUI.Label (new Rect (10, 20, 300, 24), "Speed: " + fxRunnerManager.speed , labelStyle);
		GUI.Label (new Rect (10, 35, 300, 24), "Max speed: " + fxRunnerManager.maxSpeed , labelStyle);
		
		GUI.Label (new Rect (10, 50, 300, 24), "Collision Count: " + FXRunnerPlayer.CollisionCount , labelStyle);
		
		GUI.Label (new Rect (10, 65, 300, 24), "Score: " + fxRunnerManager.gameScore , labelStyle);

		FXRunnerGUI.Instance.Score = FXRunner.fxRunnerManager.gameScore;
	}

}
