using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyingModel : MonoBehaviour {
    /* Speeds */
    public float flyingSpeed;
    public float originalFlyingSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float decelerationSpeedMultipler;
    public float accelerationSpeedMultipler;

    /* bools for actions */
    public bool bTurnUp;
    public bool bTurnDown;
    public bool bTurnLeft;
    public bool bTurnRight;
    public bool bFlappingWings;
    public bool bFlappingUp;
    public bool bFlappingDown;

    /* action haptics */
    public bool bTurnUpHaptic;
    public bool bTurnDownHaptic;
    public bool bTurnLeftHaptic;
    public bool bTurnRightHaptic;
    public bool bFlappingWingsHaptic;

    /* pitch, yaw, roll */
    public float pitchValue;
	public float yawValue;
	public float rollValue;

    /* Thresholds */
    public float pitchThreshold;
    public float yawThreshold;
    public float rollThreshold;
    public float wingFlapThreshold;

    /* Flapping */
    public int currentFlapType;
}