using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FXRuunerMultiplayerManager : MonoBehaviour {

	public FXRunnerMQTT comunicationManager;
	public Object PlayerPrefab;
	public Dictionary<string , GameObject> PlayersDict;



	// Use this for initialization
	void Start () {
		if(PlayerPrefab == null){
			Debug.LogError("PlayerPrefab == null");
		}
		if( comunicationManager == null ){
			Debug.LogError("comunicationManager == null");
			comunicationManager = this.transform.GetComponent<FXRunnerMQTT>();
		}
		PlayersDict = new Dictionary<string, GameObject>();
	}

	// Update is called once per frame
	void OnGUI () {
		//Debug.Log("--------------------");
		List<string> keyList = new List<string>( comunicationManager.PlayersDataMQTT.Keys );

		foreach( string key in keyList){
			//Debug.Log( "value = " +  comunicationManager.PlayersDataMQTT[key] ); //DEBUG
			if( !PlayersDict.ContainsKey( key) ){
				GameObject player = (GameObject)Instantiate( PlayerPrefab );
				FXRunnerPlayer playerManager = player.GetComponent<FXRunnerPlayer>();
				playerManager.FBID = key;
				playerManager.isNetworkPlayer = true;
				player.transform.parent = this.transform;

				PlayersDict[key] = player; //Create player(Network)

			}
		}
	}
}
