﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using System;


public class FXRunnerFacebook : MonoBehaviour {

#region Unity
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void Awake(){
		// Initialize FB SDK              
		enabled = false;                  
		FB.Init(OnInitComplete, OnHideUnity);  
	}
#endregion


	/***************************************************
	 * 				Login
	 **************************************************/
	void LoginCallback(FBResult result)                                                        
	{                                                                                          
		FbDebug.Log("LoginCallback");                                                          
  
		if(FB.IsLoggedIn) {
			Debug.Log(FB.UserId);
			OnLoggedIn();
		} else {
			Debug.Log("User cancelled login");
		}
	}                                                                                          
	
	void OnLoggedIn()                                                                          
	{                                                                                          
		FbDebug.Log("Logged in. ID: " + FB.UserId);     

		// Reqest player info and profile picture                                                                           
		FB.API("/me?fields=id,first_name,picture", Facebook.HttpMethod.GET, APICallback);  
		//FB.API(Util.GetPictureURL("me", 128, 128), Facebook.HttpMethod.GET, MyPictureCallback);
		//FB.API(Util.GetPictureURL("777349191", 128, 128), Facebook.HttpMethod.GET, MyPictureCallback);   //ilya = 777349191
	}  
	public Dictionary <string,string> profile;
	void APICallback(FBResult result)                                                                                              
	{                                                                                                                              
		FbDebug.Log("APICallback");                                                                                                
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			FbDebug.Error(result.Error);                                                                                           
			// Let's just try again                                                                                                
			FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);     
			return;                                                                                                                
		}                                                                                                                          
		Debug.Log("result.Text = " + result.Text);
		profile = Util.DeserializeJSONProfile(result.Text);                                                                        
		//FXRunnerGUI.Instance.TextGUIText.text  = profile["first_name"];    
		//friends = Util.DeserializeJSONFriends(result.Text);                                                                        
	}                                                                                                                              
	
	void MyPictureCallback(FBResult result)                                                                                        
	{                                                                                                                              
		FbDebug.Log("MyPictureCallback");                                                                                          
		
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			FbDebug.Error(result.Error);                                                                                           
			// Let's just try again                                                                                                
			FB.API(Util.GetPictureURL("me", 128, 128), Facebook.HttpMethod.GET, MyPictureCallback);                                
			return;                                                                                                                
		}  


		UnityEngine.UI.RawImage fbPicture = GameObject.Find("FBPicture").GetComponent<UnityEngine.UI.RawImage>();
		fbPicture.enabled = true;
		fbPicture.texture = result.Texture;                                                                          
	}      
	
	/***************************************************
	 * 				INIT
	 **************************************************/
	//After facebook init callback
	private void OnInitComplete()                                                                       
	{                                                                                            
		FbDebug.Log("SetInit");                                                                  
		enabled = true; // "enabled" is a property inherited from MonoBehaviour                  
		if (FB.IsLoggedIn)                                                                       
		{                                                                                        
			FbDebug.Log("Already logged in");                                                    
			OnLoggedIn();                                                                        
		}else{
			FB.Login("email,publish_actions", LoginCallback);   
		}
	}       
	
	private void OnHideUnity(bool isGameShown)                                                   
	{                                                                                            
		FbDebug.Log("OnHideUnity");                                                              
		if (!isGameShown)                                                                        
		{                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		}                                                                                        
		else                                                                                     
		{                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}                                                                                        
	}    
}
