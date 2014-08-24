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
	private GUIStyle labelStyle; //TODO For debug
	#endregion

	/*******************************************************
	 * MonoBehaviour
	 ********************************************************/
	void Start () {
		labelStyle = new GUIStyle();//TODO For debug
		labelStyle.normal.textColor = Color.white;//TODO For debug

		fxRunnerManager = new FXRunnerManager( this.transform , new RoadFunction(), lineMaterial, obsiclesPrefabs, 10 , 1); 
	}
	
	void Update () {
		//players players Y position
		if( Input.GetKeyUp(KeyCode.K) ){
			inputMethod = !inputMethod;
		}
		if(inputMethod){
			fxRunnerManager.MakeStep( Time.deltaTime , Input.GetAxisRaw("Vertical") , Input.GetAxisRaw("Horizontal") , false);
		}else{
			fxRunnerManager.MakeStep( Time.deltaTime , Input.GetAxisRaw("Vertical") , 2.5f * (Input.mousePosition.x/(Screen.width) - 0.5f) , true);
		}
	}

	/*******************************************************
	 * GUI debug
	 ********************************************************/
	void OnGUI() {
		//TODO DEBUG
		GUI.Label (new Rect (10, 5, 300, 24), "Drawin " + fxRunnerManager.numOfLines + " lines. 'C' to clear", labelStyle);
		GUI.Label (new Rect (10, 20, 300, 24), "Speed: " + fxRunnerManager.speed , labelStyle);
		GUI.Label (new Rect (10, 35, 300, 24), "Max speed: " + fxRunnerManager.maxSpeed , labelStyle);
		GUI.Label (new Rect (10, 50, 300, 24), "Collision Count: " + FXRunnerPlayer.CollisionCount , labelStyle);
		GUI.Label (new Rect (10, 65, 300, 24), "Score: " + fxRunnerManager.gameScore , labelStyle);
		GUI.Label (new Rect (10, 80, 300, 24), "position: " + fxRunnerManager.x , labelStyle);
		GUI.Label (new Rect (10, 95, 300, 24), "Version: 8/22/2014 15:07" , labelStyle);
	}
}
