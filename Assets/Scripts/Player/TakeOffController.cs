using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOffController : MonoBehaviour {
	protected TakeOffModel takeOffModel;
	protected BirdMovementController birdMovementController;

	// Use this for initialization
	void Start () {
		takeOffModel = GetComponent<TakeOffModel>();
		birdMovementController = GetComponent<BirdMovementController>();
	}
	
	// Update is called once per frame
	void Update () {
		//detect event changes in the hand triggers
		HandTriggerEventListener();

		//If both controller hand grips are pressed at the same time
		if (takeOffModel.leftGripPressed && takeOffModel.rightGripPressed) {

			//Listen for flapping
			armMovementListener();

			//If the hand positions aren't yet set then set them
			if (! takeOffModel.handPositionSet) {
				setHandStartPositions();
			}
		
		//If either controller isn't pressed
		} else if (! takeOffModel.leftGripPressed || ! takeOffModel.rightGripPressed) {

			//If the hand positions are set then un-set them
			if (takeOffModel.handPositionSet) {
				unsetHandPositions();
			}

		}
	}

	protected void HandTriggerEventListener()
	{
		// returns a float of the Hand Trigger’s current state on the Left Oculus Touch controller.
		float leftHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch);
		// returns a float of the Hand Trigger’s current state on the Right Oculus Touch controller.
		float rightHandTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch);

		takeOffModel.leftGripPressed = (leftHandTrigger > 0.0f) ? true : false;
		takeOffModel.rightGripPressed = (rightHandTrigger > 0.0f) ? true : false;
	}

	protected void setHandStartPositions()
	{
		takeOffModel.handPositionSet = true;
		takeOffModel.leftHandStart = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		takeOffModel.rightHandStart = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
	}

	public void unsetHandPositions()
	{
		takeOffModel.handPositionSet = false;
		takeOffModel.armsMovingDown = false;
		takeOffModel.armsMovingUp = false;
	}

	protected void armMovementListener()
	{
		if (takeOffModel.armsMovingDown) {

			waitForUpwardMovement();

		} else if (takeOffModel.armsMovingUp) {

			waitForDownwardMovement();

		} else {

			waitForDownwardMovement();

		}
	}

	protected void waitForUpwardMovement()
	{
		Vector3 leftHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		Vector3 rightHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

		//Player flapped wings downward
		if ((leftHandCurPos.y - takeOffModel.movementThreshHold) > takeOffModel.leftHandStart.y && 
		   ((rightHandCurPos.y - takeOffModel.movementThreshHold)  > takeOffModel.rightHandStart.y)) {

			   takeOffModel.armsMovingDown = false;
			   takeOffModel.armsMovingUp = true;

			   setHandStartPositions();

		}
	}

	protected void waitForDownwardMovement()
	{
		Vector3 leftHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		Vector3 rightHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

		//Player flapped wings downward
		if ((leftHandCurPos.y + takeOffModel.movementThreshHold) < takeOffModel.leftHandStart.y && 
		   ((rightHandCurPos.y + takeOffModel.movementThreshHold)  < takeOffModel.rightHandStart.y)) {

			   takeOffModel.armsMovingUp = false;
			   takeOffModel.armsMovingDown = true;

			   setHandStartPositions();

				//Play wing flapping noise
				takeOffModel.flapSound.Play();
				//move the player up
				birdMovementController.moveUp();
		}
	}
}