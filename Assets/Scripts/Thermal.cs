using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermal : MonoBehaviour {
	public FlyingModel FlyingModel;
	protected float tempSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Bird"){
			//get in game speed
			tempSpeed = FlyingModel.flyingSpeed;
			//new max speed
			FlyingModel.maxSpeed = FlyingModel.originalMaxSpeed * 2.0f;
			//set to max speed
			FlyingModel.flyingSpeed = FlyingModel.maxSpeed;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Bird"){
			//set max to original max
			FlyingModel.maxSpeed = FlyingModel.originalMaxSpeed;
			//set speed to temp speed
			FlyingModel.flyingSpeed = tempSpeed;
		}
	}
}
