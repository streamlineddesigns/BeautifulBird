using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOffModel : MonoBehaviour {

	public bool armsMovingDown;
	public bool armsMovingUp;

	[System.NonSerialized]
	public float movementThreshHold = 0.15f;
	public bool leftGripPressed;
	public bool rightGripPressed;
	public Vector3 leftHandStart;
	public Vector3 rightHandStart;
	public bool handPositionSet;
	public AudioSource flapSound;
}
