//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;


public interface IRoadFunction
{
	///The position ant this point
	Vector3 Pos(float t);

	///The rotation in this point
	float Ang(float t);

	///return the rirection of the road at t point (the gradient of the function in that point)
	Vector3 Dir( float t );
	///return the vectror that points to the right of the road
	///will be user to calculate the normal of the road
	Vector3 Right( float t );
	///the normal of the function an t point
	///should be normalaized
	Vector3 Norm( float t );

	///the posibility for an object to be at a point on a function.
	float posibilityForObject();

	///the posibility for an object to be in a frame. 
	float posibilityForObjectInFrame();

}


