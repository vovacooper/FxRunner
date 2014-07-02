using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class RoadFunction : IRoadFunction{

	float rad = 200;
	//The position ant this point
	public Vector3 Pos(float t){
		Vector3 lastValue = new Vector3();
		lastValue.x = rad * Mathf.Cos(t * 0.2f);
		lastValue.y = 2 * rad * Mathf.Sin(t * 0.1f );
		lastValue.z = t * 40;
		
		return lastValue;
	}


	//The rotation in this point
	public float Ang(float t){
		return 0;// -t*20f;
	}


	public Vector3 Dir( float t ){
		return this.Pos(t + .1f) - this.Pos(t) ;
	}

	public Vector3 Right( float t ){
		
		Vector3 dir = this.Dir( t );
		Vector3 sqr = new Vector3(20,0,0);

		Vector3.OrthoNormalize(ref dir ,ref sqr);
		sqr *= 20;

		Quaternion q = Quaternion.AngleAxis( this.Ang( t ) ,dir );
		sqr = q * sqr;
		
		return sqr.normalized; 
	}

	public Vector3 Norm( float t ){

		Vector3 dir = this.Dir( t );
		Vector3 sqr = new Vector3(20,0,0);

		Vector3.OrthoNormalize(ref dir ,ref sqr);
		sqr *= 20;
		
		Quaternion q = Quaternion.AngleAxis( this.Ang( t ) ,dir );
		sqr = q * sqr;

		return Vector3.Cross( dir , sqr ).normalized; 
	}

	//return val from 0 to 1 that tepresent the posibility of an object on a frame
	public float posibilityForObject(){
		return Random.value * 0.5f;
	}

	//return val from 0 to 1 that tepresent the posibility of an object in a frame
	public float posibilityForObjectInFrame(){
		return Random.value * 0.3f;
	}

}