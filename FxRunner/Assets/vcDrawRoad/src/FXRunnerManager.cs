

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FXRunnerManager
{
	/***********************************************************
	**	PROPERTIES
	***********************************************************/

	//Time

	private float _startGameTime;
	public float gameTime{
		get{
			return (Time.time - _startGameTime);
		}
	}

	private void resetTime(){
		_startGameTime = Time.time;
	}

	//SPEED

	/// The max speed in current game.
	public float maxSpeed = 0;
	/// The max speed in current game.
	public float maxSpeedAllowed = 0;
	///The speed of the camera and the player
	private float _speed;
	public float speed{
		get{
			return _speed;
		}set{
			_speed = value;
			if( _speed > maxSpeed ){
				maxSpeed = _speed;
			}
		}
	}

	//PLAYER

	///the speed of the player going horizontally - available for keyboard only
	public float horizontalPlayerSpeed = .7f;
	///goes ftom [-1 , 1], represent the location of the player on the road. 
	///the left most is -1 
	///the right most is 1
	///allways normlized
	private float _playerRelativePosition;
	private float playerRelativePosition{
		get{
			return _playerRelativePosition;
		}set{
			_playerRelativePosition = value;
			//Normlize player relative position position
			if( Mathf.Abs( _playerRelativePosition ) >= 1){
				_playerRelativePosition /= Mathf.Abs( _playerRelativePosition );
			}

		}
	}

	///The distance of the object from the camera. measured by the given function
	///if the camera is in f(t) then the object will be at f(t + distanceFromCamera)
	private float _playerDistanceFromCamera;
	

	


	//SCORE

	/// The game score.
	private float _gameScore;
	public float gameScore{
		get{
			return _gameScore;
		}set{
			_gameScore = value;
			if( _gameScore > maxScore ) {
				maxScore = _gameScore;
			}
		}
	}
	/// The max score in current game.
	public float maxScore;

	

	
	// POSITION

	//this values will move the camera and the road according to the RoadFunction Class
	private float _x; //posOfCamera
	public float x {
		set{
			_x = value;
			posOfRoad = _x + _roadDistanceFromCamera;
		}
		get{
			return _x;
		}
	}
	
	//The distance of the road beeng rendered from the camera. 
	private float _roadDistanceFromCamera;

	//the value represent the position of the road on the function. should be posOfCamera + distanceFromCamera
	private float _posOfRoad;
	private float posOfRoad{
		get{
			return _posOfRoad;
		}set{
			float t = value;

			for(   ; _posOfRoad < (int)t ; _posOfRoad += fxRoad.step ){
				fxRoad.addPointFrame( _posOfRoad );
			}
		}
	}

	public int numOfLines{
		get{
			return fxRoad.numOfLiveLines;
		}
	}
	public int numOfObjects{
		get{
			return fxRoad.numOfLiveObjects;
		}
	}

	public void setCam( Camera camera ){
		if(camera == null){
			Debug.LogError("camera cant be null!!!");
		}
		if(Fx == null){
			Debug.LogError("Fx cant be null!!!");
		}
		//Move camera to position
		camera.transform.position = Fx.Pos( x );
		camera.transform.LookAt(  Fx.Pos( x ) + Fx.Dir( x ) , Fx.Norm( x ) * 20 );
		camera.transform.position =  Fx.Pos( x ) + Fx.Dir( x ) + Fx.Norm( x ) * 20;
		camera.transform.Rotate( 25 , 0 , 0 );


	}

	//for mouse input
	//fet pos [-1 , 1] if not the function will normalize it.
	public void setPlayer( GameObject player , float pos ){
		playerRelativePosition = pos;
		Vector3 positionOfPlayer = Fx.Pos( x + _playerDistanceFromCamera );

		player.transform.position = positionOfPlayer + Fx.Right( x + _playerDistanceFromCamera ) * ( fxRoad.roadWidth / 2 ) * playerRelativePosition;
		
		player.transform.LookAt( positionOfPlayer + Fx.Dir( x + _playerDistanceFromCamera ) + 
		                        Fx.Right( x + _playerDistanceFromCamera ) *  
		                        ( fxRoad.roadWidth / 2 ) * playerRelativePosition 
		                        , Fx.Norm( x + _playerDistanceFromCamera + 0.1f) );
	}
	//For keyboard input
	public void setPlayer( GameObject player , float hs , float dt ){

		//Move player position
		if( speed >= 1f ){
			playerRelativePosition +=  (1f + 5f *   (1 - (1 / ( Mathf.Abs( speed ) ) ) ) ) * horizontalPlayerSpeed * hs * dt;
		}else{
			playerRelativePosition +=  horizontalPlayerSpeed * hs * dt;
		}


		Vector3 positionOfPlayer = Fx.Pos( x + _playerDistanceFromCamera );
		player.transform.position = positionOfPlayer + Fx.Right( x + _playerDistanceFromCamera ) * ( fxRoad.roadWidth / 2 ) * playerRelativePosition;

		player.transform.LookAt( positionOfPlayer + Fx.Dir( x + _playerDistanceFromCamera ) + 
		                         Fx.Right( x + _playerDistanceFromCamera ) *  
		                         ( fxRoad.roadWidth / 2 ) * playerRelativePosition 
		                         , Fx.Norm( x + _playerDistanceFromCamera + 0.1f) );
	}
	

	//MANAGERS

	public IRoadFunction Fx;
	private FXRoad fxRoad;

	private Transform _parent;

	/***********************************************************
	**	Ctor's
	***********************************************************/
	public FXRunnerManager ( Transform parent , 
	                        IRoadFunction fx , 
	                        Material lineMaterial , 
	                        GameObject[] obsticlePrefabList ,
	                        float roadDistanceFromCamera ,
	                        float startSpeed ,
	                        float playerDistanceFromCamera)
	{
		resetTime();
		_playerDistanceFromCamera = playerDistanceFromCamera;
		Fx = fx;
		speed = startSpeed;
		_parent = parent;
		_roadDistanceFromCamera = roadDistanceFromCamera;
		fxRoad = new FXRoad( fx , 60 , .3f , lineMaterial ,obsticlePrefabList , 20 , 8 , parent , .5f );
	}



	public void Restart(){

	}

	//
	public void MakeStep ( float deltaTime ){
		x += speed * deltaTime;

	}

	/*******************************************************
	 * GAMEPLAY
	 ********************************************************/
	/// <summary>
	/// Jump the specified strength.
	/// </summary>
	/// <param name="strength">Strength is in [-1,1].</param>
	private bool canjump = false;
	private void jump( float strength ){
		if( !canjump ){
			return;
		}
		
	}
	
	/// <summary>
	/// Speeds the player.
	/// </summary>
	/// <param name="boosStrength">Boos strength by boosStrength factor.</param>
	private void speedBust( float boosStrength ){
		
	}
	
	/// <summary>
	/// slows down the player.
	/// </summary>
	/// <param name="boosStrength">reduce speed by strength factor.</param>
	private void speedBump( float strength ){
		
	}



}


