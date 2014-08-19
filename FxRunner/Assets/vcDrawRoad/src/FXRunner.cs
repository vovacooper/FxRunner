using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Game manager
public class FXRunner : MonoBehaviour {
	
	/*******************************************************
	 * PUBLIC
	 ********************************************************/
	#region PUBLIC
	/// <summary>
	/// The player gameobject.
	/// </summary>
	public GameObject Player;
	/// <summary>
	/// The player camera.
	/// </summary>
	public Camera PlayerCamera;

	/// <summary>
	/// The line material.
	/// </summary>
	public Material lineMaterial;
	/// <summary>
	/// The obsicles prefabs.
	/// </summary>
	public GameObject[] obsiclesPrefabs;

	/// <summary>
	/// The input method.
	/// true  => keyboard
	/// false => mouse
	/// </summary>
	public bool inputMethod = true;

	public static FXRunnerManager fxRunnerManager;
	#endregion
	
	#region PRIVATE
	/// <summary>
	/// The label style.
	/// for Debug porposes.
	/// </summary>
	private GUIStyle labelStyle; //For debug
	#endregion

	/*******************************************************
	 * MonoBehaviour
	 ********************************************************/
	void Start () {
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.white;

		fxRunnerManager = new FXRunnerManager( this.transform , new RoadFunction(),lineMaterial,obsiclesPrefabs, 10 , 1 , .8f); //TODO
	}
	
	void Update () {
		//Update speed
		fxRunnerManager.speed += Input.GetAxisRaw("Vertical") * Time.deltaTime ;

		//CLEAR
		if(Input.GetKeyUp(KeyCode.C)) {
			fxRunnerManager.Restart();
		}

		//Update players X position
		fxRunnerManager.MakeStep( Time.deltaTime );
	
		//Camera speed update
		if(PlayerCamera == null){
			fxRunnerManager.setCam( Camera.main );
		}else{
			fxRunnerManager.setCam( PlayerCamera );
		}

		//players players Y position
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
		GUI.Label (new Rect (10, 5, 300, 24), "Drawin " + fxRunnerManager.numOfLines + " lines. 'C' to clear", labelStyle);
		GUI.Label (new Rect (10, 20, 300, 24), "Speed: " + fxRunnerManager.speed , labelStyle);
		GUI.Label (new Rect (10, 35, 300, 24), "Max speed: " + fxRunnerManager.maxSpeed , labelStyle);
		GUI.Label (new Rect (10, 50, 300, 24), "Collision Count: " + FXRunnerPlayer.CollisionCount , labelStyle);
		GUI.Label (new Rect (10, 65, 300, 24), "Score: " + fxRunnerManager.gameScore , labelStyle);
		GUI.Label (new Rect (10, 80, 300, 24), "position: " + fxRunnerManager.x , labelStyle);

		FXRunnerGUI.Instance.Score = FXRunner.fxRunnerManager.gameScore;
	}
}
