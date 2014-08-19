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
		Debug.Log("Subscribe " + topic);
		client.Subscribe( new string[] { "FxRunner/" + topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		//client.Subscribe(new string[] { "FxRunner/world" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
	}
	public void UnSubscribe( string topic ){
		client.Unsubscribe( new string[] { "FxRunner/" + topic } );
	}
	public void Publish( string topic , byte[] message ){
		Debug.Log("Publish " + topic);
		client.Publish( "FxRunner/" + topic , message , MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//client.Publish("FxRunner/world", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}
#endregion

#region Unity
	void Start () {
		// create client instance 
		client = new MqttClient( "broker.mqttdashboard.com", 1883, false, null );  //198.41.30.241:1883 Public MQTT broker , IPAddress.Parse("198.41.30.241")

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
		Subscribe( "world" );
	}
	
	void OnGUI(){
		if ( GUI.Button (new Rect (20,40,80,20), "Level 1")) {
			Debug.Log("sending...");
			Publish("world" , System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!") );
			//client.Publish("FxRunner/world", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			Debug.Log("sent");
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
#endregion


#region MQTT callbacks
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
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

}
