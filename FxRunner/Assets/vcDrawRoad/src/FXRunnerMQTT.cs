using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;


using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using System.Text;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using Facebook;
using Facebook.MiniJSON;

public class FXRunnerMQTT : MonoBehaviour {
#region PUBLIC
	/// <summary>
	/// The distance for each section.
	/// </summary>
	public int distanceForSubscription = 100;
	/// <summary>
	/// The distance for intersection.
	/// </summary>
	public float distanceForintersection = 10f;
	/// <summary>
	/// The number of section to be subscribed to.
	/// will be subscribed to this number of section before and after the corrent section
	/// </summary>
	public int numberOfSectionToBeSubscribedTo = 2;
	/// <summary>
	/// The players.
	/// key - Facebook ID
	/// value - all the data (playerData) syncronyzed by MQTT or other servers. 
	/// </summary>
	public Dictionary<string , playerData> PlayersDataMQTT;
#endregion

#region PRIVATE
	private float oldPosition = 0;
	private int oldSection = 0;

	private static MqttClient mqttClient;

#endregion

#region Communication
	public void Subscribe( string topic ){
		//Debug.Log("Subscribe " + topic);
		mqttClient.Subscribe( new string[] { "FxRunner/" + topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
		//client.Subscribe(new string[] { "FxRunner/world" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
	}
	public void UnSubscribe( string topic ){
		//Debug.Log("UnSubscribe " + topic);
		mqttClient.Unsubscribe( new string[] { "FxRunner/" + topic } );
	}
	public void Publish( string topic , byte[] message ){
		//Debug.Log("Publish " + topic);
		mqttClient.Publish( "FxRunner/" + topic , message , MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
		//client.Publish("FxRunner/world", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}
#endregion

#region Unity
	void Start () {
		PlayersDataMQTT = new Dictionary<string, playerData>();
		// create client instance 
		mqttClient = new MqttClient( "www.iziking.com", 1883, false, null );  //198.41.30.241:1883 Public MQTT broker , IPAddress.Parse("192.168.56.101 ")//TODO SSL
		// register to message received 
		mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		mqttClient.MqttMsgPublished += client_MqttMsgPublished;
		mqttClient.MqttMsgSubscribed += client_MqttMsgSubscribed;
		mqttClient.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
		mqttClient.MqttMsgDisconnected += client_MqttMsgDisconnected;
	}

	/// <summary>
	/// The mqtt inited.
	/// </summary>
	private bool mqttInited = false;
	/// <summary>
	/// Raises the init event after FB has been initialized
	/// </summary>
	private void OnInit(){
		//string clientId = Guid.NewGuid().ToString(); 
		mqttClient.Connect(FB.UserId); //TODO UserName Pass
		
		//PerSubscribe to the first 3 sections
		Subscribe( "0" );
		Subscribe( "1" );
		Subscribe( "2" );
		
		mqttInited = true;
	}


	// Update is called once per frame
	void Update () {
		if(!mqttInited){ //check if need to be initialized
			if( FB.IsLoggedIn ){ //check for FB connectivity
				OnInit();
				mqttInited = true;
			}else{ //do nothing without FB connected
				return;
			}
		}

		int currentSection = (int)((((int)FXRunner.fxRunnerManager.x) - (((int)FXRunner.fxRunnerManager.x) % distanceForSubscription)) / distanceForSubscription);

		/////////////////////////////////
		//Publishing to the right section
		/////////////////////////////////
		Publish( currentSection.ToString() , dataToSend() );

		/////////////////////////////////
		//manage subscriptions 
		//	s	s	s	s	s	s	s
		//|___|___|___|___|___|___|___|
		//			    ME
		//		<--------------->
		//Subscribes to numberOfSectionToBeSubscribedTo befor and after my position
		/////////////////////////////////
		//new section
		if(  oldPosition < (oldSection * distanceForSubscription + distanceForSubscription - distanceForintersection)
		   	&& FXRunner.fxRunnerManager.x > (oldSection * distanceForSubscription + distanceForSubscription - distanceForintersection) ){
			OnEnterNewSection( oldSection + 1 );
		}else if( oldPosition < (oldSection * distanceForSubscription + distanceForintersection)
		         && FXRunner.fxRunnerManager.x > (oldSection * distanceForSubscription + distanceForintersection)){
			OnExitOldSection( oldSection - 1 );
		}

		//Update old position
		oldPosition = FXRunner.fxRunnerManager.x;
		oldSection = currentSection;
	}

	///////////////////////////////////////////Section Events
	void OnEnterNewSection( int newSection ){
		Debug.Log("+++++++++++++Entering new section " + (newSection + numberOfSectionToBeSubscribedTo).ToString() );
		Subscribe( (newSection + numberOfSectionToBeSubscribedTo).ToString() );
	}
	void OnExitOldSection( int oldSection ){
		Debug.Log("+++++++++++++OnExitOldSection new section " + (oldSection - numberOfSectionToBeSubscribedTo).ToString() );
		UnSubscribe( (oldSection - numberOfSectionToBeSubscribedTo).ToString() );
	}

	//////////////////////////////////////////DATA
	byte[] dataToSend(){
		playerData pd = new playerData(FB.UserId , FXRunner.fxRunnerManager.x , FXRunner.fxRunnerManager.y );
		return  pd.playerDataSerialize();
	}

	playerData byteToData(byte[] serializedPlayerData){
		return playerData.playerDataDeserialize( serializedPlayerData );
	}
#endregion
	

#region MQTT callbacks
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		//Debug.Log("--------------");
		//Debug.Log(sender.ToString());
		//Debug.Log(e.ToString());
		//Debug.Log(System.BitConverter.ToString(e.Message));
		//Debug.Log(Encoding.UTF8.GetString(e.Message));
		//ASCIIEncoding ascii = new ASCIIEncoding();
		//String decoded = ascii.GetString(e.Message);
		//Debug.Log("ascii = " + decoded);
		playerData pd = byteToData(e.Message);
		//Debug.Log( pd.ToString() );
		if(pd.FBID != ""){
			PlayersDataMQTT[pd.FBID] = pd;
        }
	} 
	void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e) 
	{ 
		Debug.Log("Published: " + e.MessageId  );
	}
	void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e) 
	{ 
		Debug.Log("Subscribed: " + e.MessageId  );
	}
	void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e) 
	{ 
		Debug.Log("Unsubscribed: " + e.MessageId  );
	}
	void client_MqttMsgDisconnected(object sender, EventArgs e) 
	{ 
		Debug.Log("Disconnected: " + e.ToString()  );
	}
#endregion	

	public class playerData {
		//DATA to save
		public float x; //[0,infinity)
		public float y; //[-1,1]
		public string FBID;
			
		private int _byteLength;
		public playerData( string fBID, float xPos , float yPos ){
			if(fBID == null){
				FBID = "";
			}else{
				FBID = fBID;
			}

			x = xPos;
			y = yPos;

			_byteLength = sizeof( int ) + System.Text.Encoding.UTF8.GetBytes(FBID).Length + sizeof( float ) + sizeof( float );
		}
		public override string ToString ()
		{
			return string.Format ("( x:{0} , y:{1} , FBID:{2})",x,y,FBID);
		}

		public static playerData playerDataDeserialize( byte[] serializedData ){
			playerData deserializedperson = new playerData("a",1,1);
			deserializedperson.x = System.BitConverter.ToSingle(serializedData , 4);
			deserializedperson.y = System.BitConverter.ToSingle(serializedData , 8);
			if(serializedData.Length > 12 ){
				deserializedperson.FBID = System.Text.Encoding.UTF8.GetString(serializedData ,12 , 10);// System.BitConverter.ToString(serializedData , 12);

			}else{
				deserializedperson.FBID = "";
			}

			return deserializedperson;
		}
		public byte[] playerDataSerialize( ){
			//byte[] b = new byte[_byteLength];
			byte[] _len = BitConverter.GetBytes(10);
			byte[] _x = BitConverter.GetBytes(x);
			byte[] _y = BitConverter.GetBytes(y);
			byte[] _fbid = System.Text.Encoding.UTF8.GetBytes(FBID);



			List<byte> list1 = new List<byte>(_len);
			List<byte> list2 = new List<byte>(_x);
			List<byte> list3 = new List<byte>(_y);
			List<byte> list4 = new List<byte>(_fbid);
			list1.AddRange(list2);
			list1.AddRange(list3);
			list1.AddRange(list4);

			return list1.ToArray();
		}


	}
}
