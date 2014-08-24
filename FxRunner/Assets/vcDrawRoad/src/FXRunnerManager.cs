

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FXRunnerManager
{
	/***********************************************************
	**	PROPERTIES
	***********************************************************/

	//////////////////////////Time
	private float _startGameTime;
	public float gameTime{
		get{
			return (Time.time - _startGameTime);
		}
	}
	
	//////////////////////////SPEED

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



	//////////////////////////SCORE

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


	////////////////////////// POSITION
	
	///the speed of the player going horizontally - available for keyboard only
	public float horizontalPlayerSpeed = .7f;
	///goes ftom [-1 , 1], represent the location of the player on the road. 
	///the left most is -1 
	///the right most is 1
	///allways normlized
	private float _y;
	public float y{
		get{
			return _y;
		}set{
			_y = value;
			//Normlize player relative position position
			if( Mathf.Abs( _y ) >= 1){
				_y /= Mathf.Abs( _y );
			}
		}
	}

	//this values will move the camera and the road according to the RoadFunction Class
	private float _x; //posOfCamera
	public float x {
		set{
			_x = value;
			posOfRoad = _x + _roadDistanceFromXPosition;
		}get{
			return _x;
		}
	}
	
	//The distance of the road beeng rendered from the player. 
	private float _roadDistanceFromXPosition;

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


	/// <summary>
	/// Sets the transform of the Trans to the local (x,y) position.
	/// </summary>
	/// <param name="trans">Trans.</param>
	public void setTransform( Transform trans ){
		Vector3 positionOfPlayer = Fx.Pos( x );
		trans.position = positionOfPlayer + Fx.Right( x ) * ( fxRoad.roadWidth / 2 ) * y;
		
		trans.LookAt( positionOfPlayer + Fx.Dir( x ) + 
		                        Fx.Right( x ) *  
		                        ( fxRoad.roadWidth / 2 ) * y 
		                        , Fx.Norm( x + 0.1f) );
	}

	/// <summary>
	/// Sets the transform to xPos and yPos positions on the road.
	/// </summary>
	/// <param name="trans">Trans.</param>
	/// <param name="xPos">X position.</param>
	/// <param name="yPos">Y position.</param>
	public void setTransform( Transform trans , float xPos , float yPos ){
		Vector3 positionOfPlayer = Fx.Pos( xPos );
		trans.position = positionOfPlayer + Fx.Right( xPos ) * ( fxRoad.roadWidth / 2 ) * yPos;
		
		trans.LookAt( positionOfPlayer + Fx.Dir( xPos ) + 
		             Fx.Right( xPos ) *  
		             ( fxRoad.roadWidth / 2 ) * yPos 
		             , Fx.Norm( xPos + 0.1f) );
	}

	//////////////////////////MANAGERS

	//The function
	public IRoadFunction Fx;
	//The road builder
	private FXRoad fxRoad;

	/***********************************************************
	**	Ctor's
	***********************************************************/
	/// <summary>
	/// Initializes a new instance of the <see cref="FXRunnerManager"/> class.
	/// </summary>
	/// <param name="parent">Parent.</param>
	/// <param name="fx">Fx.</param>
	/// <param name="lineMaterial">Line material.</param>
	/// <param name="obsticlePrefabList">Obsticle prefab list.</param>
	/// <param name="roadDistanceFromXPosition">Road distance from camera.</param>
	/// <param name="startSpeed">Start speed.</param>
	public FXRunnerManager ( Transform parent , 
	                        IRoadFunction fx , 
	                        Material lineMaterial , 
	                        GameObject[] obsticlePrefabList ,
	                        float roadDistanceFromXPosition ,
	                        float startSpeed )
	{
		resetTime();
		Fx = fx;
		speed = startSpeed;
		_roadDistanceFromXPosition = roadDistanceFromXPosition;
		fxRoad = new FXRoad( fx , 60 , .3f , lineMaterial ,obsticlePrefabList , 20 , 8 , parent , .5f );
	}
	
	/// <summary>
	/// Makes the step.
	/// </summary>
	/// <param name="deltaTime">Delta time.</param>
	/// <param name="verticalInput">Vertical input.</param>
	/// <param name="horizontalInput">Horizontal input.</param>
	/// <param name="isAbsolute">If set to <c>true</c> is absolute.</param>
	public void MakeStep ( float deltaTime , float verticalInput , float horizontalInput , bool isAbsolute ){
		speed += verticalInput * deltaTime ;
		x += speed * deltaTime;
		if(isAbsolute == true){
			y = horizontalInput;
		}else{
			//Move player position
			if( speed >= 1f ){
				y +=  (1f + 5f *   (1 - (1 / ( Mathf.Abs( speed ) ) ) ) ) * horizontalPlayerSpeed * horizontalInput * deltaTime;
			}else{
				y +=  horizontalPlayerSpeed * horizontalInput * deltaTime;
			}
		}
	}

	/*******************************************************
	 * GAMEPLAY
	 ********************************************************/
	/// <summary>
	/// Resets the time.
	/// </summary>
	private void resetTime(){
		_startGameTime = Time.time;
	}
}






















/// <summary>
/// Sets the camera position.
/// </summary>
/// <param name="camera">Camera.</param>
/*public void setCam( Camera camera ){
		if(camera == null){
			Debug.LogError("camera cant be null!!!");
		}
		if(Fx == null){
			Debug.LogError("Fx cant be null!!!");
		}
		//Move camera to position
		camera.transform.position = Fx.Pos( x - _playerDistanceFromCamera);
		camera.transform.LookAt(  Fx.Pos( x - _playerDistanceFromCamera) + Fx.Dir( x ) , Fx.Norm( x ) * 20 );
		camera.transform.position =  Fx.Pos( x - _playerDistanceFromCamera ) + Fx.Dir( x ) + Fx.Norm( x ) * 20;
		camera.transform.Rotate( 25 , 0 , 0 );
	}*/

