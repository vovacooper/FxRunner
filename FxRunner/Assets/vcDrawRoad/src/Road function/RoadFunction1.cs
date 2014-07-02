using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class RoadFunction1 : IRoadFunction {

	float baseRad = 200;
	//The position ant this point
	public Vector3 Pos(float t){
		t = t + 2000;
		float radX = baseRad/2f + Mathf.Cos(t*0.1f+0.05f*Mathf.Cos(t*0.32f))/4f;
		float radY = baseRad/2f + Mathf.Cos(t*0.12f+0.05f*Mathf.Cos(t*0.321f))/4f;
		float fCos = 0.1f+Mathf.Sin(t*0.013f)*0.02f;
		float fSin = 0.1f+Mathf.Cos(t*0.0134f)*0.02f;
		Vector3 lastValue = new Vector3();
		lastValue.x = radX * Mathf.Cos(t * fCos);
		lastValue.y = radY * Mathf.Sin(t * fSin);
		lastValue.z = t * 40;
		
		return lastValue;
	}


	//The rotation in this point
	public float Ang(float t){
		//return Mathf.Sin (t) * 10 ;
		return  360*Mathf.Sin(t*0.037f)*Mathf.Sin(t*0.0037f)*2+360*Mathf.Sin(-t*0.02832f)*Mathf.Sin(t*0.0031f)*2;
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