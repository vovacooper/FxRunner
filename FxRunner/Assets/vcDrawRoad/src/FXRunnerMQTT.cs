using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public class FXRunnerMQTT : MonoBehaviour {
#region PUBLIC
	public static List<GameObject> players;
#endregion

#region PRIVATE
	private static MqttClient client;
#endregion

#region Communication
	public void Subscribe( string topic ){
		//Debug.Log("Subscribe " + topic);
		client.Subscribe( new string[] { "FxRunner/" + topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		//client.Subscribe(new string[] { "FxRunner/world" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
	}
	public void UnSubscribe( string topic ){
		//Debug.Log("UnSubscribe " + topic);
		client.Unsubscribe( new string[] { "FxRunner/" + topic } );
	}
	public void Publish( string topic , byte[] message ){
		//Debug.Log("Publish " + topic);
		client.Publish( "FxRunner/" + topic , message , MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//client.Publish("FxRunner/world", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}
#endregion

#region Unity
	void Start () {
		// create client instance 
		client = new MqttClient( "test.mosquitto.org", 1883, false, null );  //198.41.30.241:1883 Public MQTT broker , IPAddress.Parse("192.168.56.101 ")

		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		client.MqttMsgPublished += client_MqttMsgPublished;
		client.MqttMsgSubscribed += client_MqttMsgSubscribed;
		client.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
		client.MqttMsgDisconnected += client_MqttMsgDisconnected;

		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId);
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		//client.Subscribe(new string[] { "FxRunner/world" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
		Subscribe( "0" );
		Subscribe( "1" );
		Subscribe( "2" );

	}
	
	void OnGUI(){
		if ( GUI.Button (new Rect (20,40,80,20), "Level 1")) {
			Debug.Log("sending...");
			Publish("world" , System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!") );
			//client.Publish("FxRunner/world", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			Debug.Log("sent");
		}
	}
	public int distanceForSubscription = 100;
	public float distanceForintersection = 10f;
	public int numberOfSectionToBeSubscribedTo = 2;

	private float oldPosition = 0;
	private int oldSection = 0;
	// Update is called once per frame
	void Update () {
		int currentSection = (int)((((int)FXRunner.fxRunnerManager.x) - (((int)FXRunner.fxRunnerManager.x) % distanceForSubscription)) / distanceForSubscription);

		/////////////////////////////////
		//Publishing to the right section
		/////////////////////////////////
		Publish( currentSection.ToString() , dataToBynary() );

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

		oldPosition = FXRunner.fxRunnerManager.x;
		oldSection = currentSection;

		FXRunnerGUI.Instance.TextGUIText.text  = tempRecieved.ToString();  
	}

	//Sections
	void OnEnterNewSection( int newSection ){
		Debug.Log("+++++++++++++Entering new section " + (newSection + numberOfSectionToBeSubscribedTo).ToString() );
		Subscribe( (newSection + numberOfSectionToBeSubscribedTo).ToString() );
	}
	void OnExitOldSection( int oldSection ){
		Debug.Log("+++++++++++++OnExitOldSection new section " + (oldSection - numberOfSectionToBeSubscribedTo).ToString() );
		UnSubscribe( (oldSection - numberOfSectionToBeSubscribedTo).ToString() );
	}

	//DATA
	byte[] dataToBynary(){
		return BitConverter.GetBytes( FXRunner.fxRunnerManager.x) ;
	}

	void byteToData(){

	}
#endregion

	private float tempRecieved = 0;
#region MQTT callbacks
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		tempRecieved = System.BitConverter.ToSingle(e.Message, 0);
		//Debug.Log("Received: " + System.BitConverter.ToSingle(e.Message, 0));// System.Text.Encoding.UTF8.GetString(e.Message)  );
	} 
	void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e) 
	{ 
		//Debug.Log("Published: " + e.MessageId  );
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

}
