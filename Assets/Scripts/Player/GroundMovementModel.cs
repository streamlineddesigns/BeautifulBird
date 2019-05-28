using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovementModel : MonoBehaviour {
	public bool armPositionSet;
	public bool rightArmHigher;
	public bool leftArmHigher;
	public bool initialArmMovement;

	[System.NonSerialized]
	public float movementThreshHold = 0.15f;

	[System.NonSerialized]
	public float timeForMoveThreshHold = 1.0f;
	public float timer;
}
