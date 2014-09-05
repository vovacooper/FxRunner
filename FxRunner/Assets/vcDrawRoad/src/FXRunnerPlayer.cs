using UnityEngine;
using System.Collections;

public class FXRunnerPlayer : MonoBehaviour {

	public bool isNetworkPlayer = true;
	public string FBID;
	public Texture FBImage;

	public FXRunnerMQTT MQTTManager;

	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;
	static public int CollisionCount = 0;
	public GameObject ship;
	// Use this for initialization
	void Start () {
		MQTTManager = GameObject.Find("MQTTManager").GetComponent<FXRunnerMQTT>();
	}

	void SetPictureCallback(FBResult result)                                                                                        
	{                                                                                                                              
		Debug.Log("MyPictureCallback");                                                                                          
		
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			Debug.LogError(result.Error);                                                                                           
			// Let's just try again                                                                                                
			//FB.API(Util.GetPictureURL("me", 128, 128), Facebook.HttpMethod.GET, MyPictureCallback);                                
			return;                                                                                                                
		}  
		FBImage = result.Texture;
		UnityEngine.UI.RawImage fbPicture = this.transform.Find("Canvas/FBPicture").GetComponent<UnityEngine.UI.RawImage>();
		fbPicture.enabled = true;
		fbPicture.texture = result.Texture;                                                                          
	}  


	// Update is called once per frame
	void Update () {
		if( FBID == "" ){//Main Player
			if( !isNetworkPlayer ){ //Main Player
				if( FB.IsLoggedIn ){
					FBID = FB.UserId;
				}
			}else{//Network Player

			}
		}
		if(FBImage == null){ //Load image
			if( FB.IsLoggedIn && FBImage == null ){ //TODO check that entare here onec avery sec...
				FB.API(Util.GetPictureURL(FBID, 128, 128), Facebook.HttpMethod.GET, SetPictureCallback);
			}
		}

		if( !isNetworkPlayer ){ //Main Player
			if( FB.IsLoggedIn ){
				FBID = FB.UserId;
			}

			FXRunner.fxRunnerManager.setTransform( this.transform );

			//Tilt smoothly the ship when going left or right
			var tiltAroundZ = -1 * Input.GetAxis("Horizontal") * tiltAngle;
			Quaternion target = Quaternion.Euler (0, 0,tiltAroundZ);
			ship.transform.localRotation = Quaternion.Slerp(ship.transform.localRotation, target, Time.deltaTime * smooth);
		}else{ //Network Player
			if(FBID == null){
				Debug.LogError("FBID == null");
			}
			FXRunner.fxRunnerManager.setTransform( this.transform , MQTTManager.PlayersDataMQTT[FBID].x ,MQTTManager.PlayersDataMQTT[FBID].y  );
			//Debug.Log(MQTTManager.PlayersDataMQTT[FBID].ToString());
		}
	}




	void OnTriggerEnter(Collider other) {
		Debug.Log(other.name);
		CollisionCount++;
	}
}
